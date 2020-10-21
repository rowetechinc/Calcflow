using System;
using System.Collections.Generic;
using System.Text;
using EstimatingFlowMode;

namespace Calcflow
{
    internal class EnsembleFlow : CalculateFlow
    {
        #region  顶部和底部流量属性

        private double draft = 0;
        /// <summary>
        /// 换能器吃水深度(m)
        /// </summary>
        public double Draft
        {
            get { return draft; }
            set { draft = value; }
        }
        private TopFlowMode topMode = TopFlowMode.PowerFunction;
        /// <summary>
        /// 顶部流量计算方案
        /// </summary>
        public TopFlowMode TopMode
        {
            get { return topMode; }
            set { topMode = value; }
        }

        private BottomFlowMode bottomMode = BottomFlowMode.PowerFunction;
        /// <summary>
        /// 底部流量计算方案
        /// </summary>
        public BottomFlowMode BottomMode
        {
            get { return bottomMode; }
            set { bottomMode = value; }
        }

        private double exponent = 1 / 6.0;
        /// <summary>
        /// 幂函数指数
        /// </summary>
        public double Exponent
        {
            get { return exponent; }
            set { exponent = value; }
        }
        #endregion

        #region  计算流量的方法
        /// <summary>
        /// 计算实测单元流量
        /// </summary>
        /// <param name="src">参与计算的数据</param>
        /// <param name="boatVelocity">船速(地理坐标系下)</param>
        /// <param name="dTime">时间差</param>
        /// <returns>单元流量信息集合</returns>
        public EnsembleFlowInfo CalculateEnsembleFlow(ArrayClass src, EarthVelocity boatVelocity, double dTime)
        {
            EnsembleFlowInfo ensembleFlow = new EnsembleFlowInfo();
            ensembleFlow.Valid = false;
            if (Math.Abs(boatVelocity.EastVelocity) > 80
                || Math.Abs(boatVelocity.NorthVelocity)>80) return ensembleFlow;

            //单元大小
            double Da = src.A_CellSize;
            if (Da < 1e-6) return ensembleFlow;
            //底跟踪四个波束中的最小深度
            double Dmin = double.NaN;
            //计算四个波束的平均深度
            int num = 0;
            double depthSum = 0;
            foreach (double d in src.B_Range)
            {
                if (d < 1e-6) continue;

                if (double.IsNaN(Dmin))
                {
                    Dmin = d;
                }
                else
                {
                    if(d<Dmin)
                    {
                        Dmin = d;
                    }
                }

                num++;
                depthSum += d;
            }
            if (num == 0) return ensembleFlow;
            double Davg = depthSum / num;

            //有效单元的可能最大深度
            double DLGmax = Dmin * Math.Cos(beamAngle / 180.0 * Math.PI) + draft - Math.Max((pulseLength + pulseLag) / 2.0,Da/2.0) ;

            //水面到水底的距离
            double Dtotal = Davg + draft;

            //第一个有效单元
            MeasuredBinInfo firstValid = new MeasuredBinInfo();
            firstValid.Status = BinFlowStatus.Bad;
            //最后一个有效单元
            MeasuredBinInfo lastValid = new MeasuredBinInfo();
            //单元深度
            double binDepth = src.A_FirstCellDepth + draft;
            //实测累计流量
            double SumMeasuredFlow = 0;
            //累计东向水速
            double SumEastVelocity = 0;
            //累计北向水速
            double SumNorthVelocity = 0;
            //GoodBin个数
            int GoodBinCount = 0;
            List<MeasuredBinInfo> binsInfoArray = new List<MeasuredBinInfo>();

            // 保存有效单元的索引号
            List<int> validBinsIndex = new List<int>();
            for (int i = 0; binDepth < DLGmax && i < src.E_Cells && i * 4 < src.Earth.Length; i++, binDepth += Da)
            {
                //获取矢量交叉乘积
                double xi = CrossProduct(src.Earth[0, i], src.Earth[1, i], boatVelocity.EastVelocity, boatVelocity.NorthVelocity);
              
                MeasuredBinInfo binInfo = new MeasuredBinInfo();
                binInfo.Depth = binDepth;
                binInfo.Flow = xi * dTime * Da;
                binInfo.Status = (IsGoodBin(src, i) ? BinFlowStatus.Good : BinFlowStatus.Bad);
                binsInfoArray.Add(binInfo);

                if (binInfo.Status == BinFlowStatus.Good)
                {
                    if (firstValid.Status == BinFlowStatus.Bad)
                    {
                        firstValid = binInfo;
                    }
                    lastValid = binInfo;

                    SumMeasuredFlow += binInfo.Flow;

                    SumEastVelocity  += src.Earth[0, i];
                    SumNorthVelocity += src.Earth[1, i];
                    GoodBinCount++;

                    //保存一个有效单元索引
                    validBinsIndex.Add(i);

                }

            }
            ensembleFlow.BinsInfo = binsInfoArray.ToArray();
            if(firstValid.Status == BinFlowStatus.Bad)
            {
                return ensembleFlow;
            }

            //第一个有效单元的深度
            double Dtop = firstValid.Depth;
            //最后一个有效单元的深度
            double Dlg = lastValid.Depth;

            double Z3 = Dtotal;
            double Z2 = Dtotal - Dtop + Da / 2.0;
            double Z1 = Dtotal - Dlg - Da / 2.0;

            //实测流量
            ensembleFlow.MeasuredFlow = dTime * (Z2 - Z1 )*CrossProduct(SumEastVelocity / GoodBinCount, SumNorthVelocity / GoodBinCount,
                boatVelocity.EastVelocity, boatVelocity.NorthVelocity);

            //顶部流量
            switch (topMode)
            {
                case TopFlowMode.Constants:

                    ensembleFlow.TopFlow = firstValid.Flow / Da * (Z3 - Z2);
                    break;
                case TopFlowMode.PowerFunction:
                    {
                        double a = exponent + 1;
                        ensembleFlow.TopFlow = SumMeasuredFlow * (Math.Pow(Z3, a) - Math.Pow(Z2, a)) / (Math.Pow(Z2, a) - Math.Pow(Z1, a));
                    }
                    break;
                case TopFlowMode.Slope:
                    {
                        //用于外延计算的 validbins 数量不足，则降为常数法计算
                        if (validBinsIndex.Count < 6)
                        {
                            ensembleFlow.TopFlow = firstValid.Flow / Da * (Z3 - Z2);
                            break;
                        }
                        else
                        {
                            // validbin 的信息，三个列表是索引对应同一个 validbin
                            // 每个 validbin 的深度信息
                            List<double> binsDepth = new List<double>();
                            // 每个 validbin 的东向速度
                            List<double> binsEastVelocity = new List<double>();
                            // 每个 validbin 的北向速度
                            List<double> binsNorthVelocity = new List<double>();

                            for (int i = 0; i < 3; i++)
                            {
                                // validbin 索引号
                                int bin_index = validBinsIndex[i];
                                // validbin 深度（从水面起）
                                double bin_depth = src.A_FirstCellDepth + draft + bin_index * Da;
                                // 东向和北向速度
                                double bin_east_vel = src.Earth[0, bin_index];
                                double bin_north_vel = src.Earth[1, bin_index];

                                //加入列表
                                binsDepth.Add(bin_depth);
                                binsEastVelocity.Add(bin_east_vel);
                                binsNorthVelocity.Add(bin_north_vel);
                            }

                            // 外延计算
                            // 外延的深度位置（从水面起算）
                            double dep = (Z3 - Z2 ) / 2;
                            // 外延计算东向速度
                            Slope east_slope = new Slope();
                            east_slope.setXYs(binsDepth.ToArray(), binsEastVelocity.ToArray());

                            double east_vel;
                            try
                            {
                                east_vel = east_slope.calY(dep);
                            } // 外延失败，斜角为90度，转而采用常数法
                            catch (DivideByZeroException)
                            {
                                ensembleFlow.TopFlow = firstValid.Flow / Da * (Z3 - Z2);
                                break;
                            }


                            //外延计算北向速度
                            Slope north_slope = new Slope();
                            north_slope.setXYs(binsDepth.ToArray(), binsNorthVelocity.ToArray());

                            double north_vel;
                            try
                            {
                                north_vel = north_slope.calY(dep);
                            }// 外延失败，斜角为90度，转而采用常数法
                            catch (DivideByZeroException)
                            {
                                ensembleFlow.TopFlow = firstValid.Flow / Da * (Z3 - Z2);
                                break;
                            }
                            // 计算流量
                            ensembleFlow.TopFlow = dTime * (Z3 - Z2) * CrossProduct(east_vel, north_vel,
                                                     boatVelocity.EastVelocity, boatVelocity.NorthVelocity);
                            break;

                        }

                    }
                
                default:
                    break;
            }

            //底部流量

            switch (bottomMode)
            {
                case BottomFlowMode.Constants:
                    ensembleFlow.BottomFlow = lastValid.Flow / Da * Z1;
                    break;
                case BottomFlowMode.PowerFunction:
                    {
                        double a = exponent + 1;
                        ensembleFlow.BottomFlow = SumMeasuredFlow * Math.Pow(Z1, a) / (Math.Pow(Z2, a) - Math.Pow(Z1, a));
                    }
                    break;
                default:
                    break;
            }

            ensembleFlow.Valid = true;

            return ensembleFlow;
        }


        /// <summary>
        /// 计算实测单元流量
        /// </summary>
        /// <param name="src">参与计算的数据</param>
        /// <param name="boatVelocity">船速(地理坐标系下)</param>
        /// <param name="dTime">时间差</param>
        /// <param name="dHeadingOffset">航向角修正值，单位：弧度</param>
        /// <returns>单元流量信息集合</returns>
        public EnsembleFlowInfo CalculateEnsembleFlow(ArrayClass src, EarthVelocity boatVelocity, double dTime, double dHeadingOffset)
        {
            EnsembleFlowInfo ensembleFlow = new EnsembleFlowInfo();
            ensembleFlow.Valid = false;
            if (Math.Abs(boatVelocity.EastVelocity) > 80
                || Math.Abs(boatVelocity.NorthVelocity) > 80) return ensembleFlow;

            //单元大小
            double Da = src.A_CellSize;
            if (Da < 1e-6) return ensembleFlow;
            //底跟踪四个波束中的最小深度
            double Dmin = double.NaN;
            //计算四个波束的平均深度
            int num = 0;
            double depthSum = 0;
            foreach (double d in src.B_Range)
            {
                if (d < 1e-6) continue;

                if (double.IsNaN(Dmin))
                {
                    Dmin = d;
                }
                else
                {
                    if (d < Dmin)
                    {
                        Dmin = d;
                    }
                }

                num++;
                depthSum += d;
            }
            if (num == 0) return ensembleFlow;
            double Davg = depthSum / num;

            //有效单元的可能最大深度
            double DLGmax = Dmin * Math.Cos(beamAngle / 180.0 * Math.PI) + draft - Math.Max((pulseLength + pulseLag) / 2.0, Da / 2.0);

            //水面到水底的距离
            double Dtotal = Davg + draft;

            //第一个有效单元
            MeasuredBinInfo firstValid = new MeasuredBinInfo();
            firstValid.Status = BinFlowStatus.Bad;
            //最后一个有效单元
            MeasuredBinInfo lastValid = new MeasuredBinInfo();
            //单元深度
            double binDepth = src.A_FirstCellDepth + draft;
            //实测累计流量
            double SumMeasuredFlow = 0;
            //累计东向水速
            double SumEastVelocity = 0;
            //累计北向水速
            double SumNorthVelocity = 0;
            //GoodBin个数
            int GoodBinCount = 0;
            List<MeasuredBinInfo> binsInfoArray = new List<MeasuredBinInfo>();

            // 保存有效单元的索引号
            List<int> validBinsIndex = new List<int>();
            for (int i = 0; binDepth < DLGmax && i < src.E_Cells && i * 4 < src.Earth.Length; i++, binDepth += Da)
            {
                //LPJ 2013-9-6 Debug --start
                //根据传递的headingOffset重新计算ENU
                //double alpha = dHeadingOffset / 180.0 * Math.PI;
                double Vn = src.Earth[0, i] * Math.Cos(dHeadingOffset) - src.Earth[1, i] * Math.Sin(dHeadingOffset);
                double Ve = src.Earth[0, i] * Math.Sin(dHeadingOffset) + src.Earth[1, i] * Math.Cos(dHeadingOffset);

                //LPJ 2013-9-6 Debug --------end

                //获取矢量交叉乘积
                //double xi = CrossProduct(src.Earth[0, i], src.Earth[1, i], boatVelocity.EastVelocity, boatVelocity.NorthVelocity);
                double xi = CrossProduct(Vn, Ve, boatVelocity.EastVelocity, boatVelocity.NorthVelocity);
                MeasuredBinInfo binInfo = new MeasuredBinInfo();
                binInfo.Depth = binDepth;
                binInfo.Flow = xi * dTime * Da;
                binInfo.Status = (IsGoodBin(src, i) ? BinFlowStatus.Good : BinFlowStatus.Bad);
                binsInfoArray.Add(binInfo);

                if (binInfo.Status == BinFlowStatus.Good)
                {
                    if (firstValid.Status == BinFlowStatus.Bad)
                    {
                        firstValid = binInfo;
                    }
                    lastValid = binInfo;

                    SumMeasuredFlow += binInfo.Flow;

                    //SumEastVelocity += src.Earth[0, i];
                    //SumNorthVelocity += src.Earth[1, i];

                    SumEastVelocity += Vn;  //LPJ 2013-9-11 根据传递的HeadingOffset重新计算ENU
                    SumNorthVelocity += Ve; //LPJ 2013-9-11

                    GoodBinCount++;

                    //保存一个有效单元索引
                    validBinsIndex.Add(i);

                }

            }
            ensembleFlow.BinsInfo = binsInfoArray.ToArray();
            if (firstValid.Status == BinFlowStatus.Bad)
            {
                return ensembleFlow;
            }

            //第一个有效单元的深度
            double Dtop = firstValid.Depth;
            //最后一个有效单元的深度
            double Dlg = lastValid.Depth;

            double Z3 = Dtotal;
            double Z2 = Dtotal - Dtop + Da / 2.0;
            double Z1 = Dtotal - Dlg - Da / 2.0;

            //实测流量
            ensembleFlow.MeasuredFlow = dTime * (Z2 - Z1) * CrossProduct(SumEastVelocity / GoodBinCount, SumNorthVelocity / GoodBinCount,
                boatVelocity.EastVelocity, boatVelocity.NorthVelocity);

            //顶部流量
            switch (topMode)
            {
                case TopFlowMode.Constants:

                    ensembleFlow.TopFlow = firstValid.Flow / Da * (Z3 - Z2);
                    break;
                case TopFlowMode.PowerFunction:
                    {
                        double a = exponent + 1;
                        ensembleFlow.TopFlow = SumMeasuredFlow * (Math.Pow(Z3, a) - Math.Pow(Z2, a)) / (Math.Pow(Z2, a) - Math.Pow(Z1, a));
                    }
                    break;
                case TopFlowMode.Slope:
                    {
                        //用于外延计算的 validbins 数量不足，则降为常数法计算
                        if (validBinsIndex.Count < 6)
                        {
                            ensembleFlow.TopFlow = firstValid.Flow / Da * (Z3 - Z2);
                            break;
                        }
                        else
                        {
                            // validbin 的信息，三个列表是索引对应同一个 validbin
                            // 每个 validbin 的深度信息
                            List<double> binsDepth = new List<double>();
                            // 每个 validbin 的东向速度
                            List<double> binsEastVelocity = new List<double>();
                            // 每个 validbin 的北向速度
                            List<double> binsNorthVelocity = new List<double>();

                            for (int i = 0; i < 3; i++)
                            {
                                // validbin 索引号
                                int bin_index = validBinsIndex[i];
                                // validbin 深度（从水面起）
                                double bin_depth = src.A_FirstCellDepth + draft + bin_index * Da;
                                // 东向和北向速度
                                double bin_east_vel = src.Earth[0, bin_index];
                                double bin_north_vel = src.Earth[1, bin_index];

                                //加入列表
                                binsDepth.Add(bin_depth);
                                binsEastVelocity.Add(bin_east_vel);
                                binsNorthVelocity.Add(bin_north_vel);
                            }

                            // 外延计算
                            // 外延的深度位置（从水面起算）
                            double dep = (Z3 - Z2) / 2;
                            // 外延计算东向速度
                            Slope east_slope = new Slope();
                            east_slope.setXYs(binsDepth.ToArray(), binsEastVelocity.ToArray());

                            double east_vel;
                            try
                            {
                                east_vel = east_slope.calY(dep);
                            } // 外延失败，斜角为90度，转而采用常数法
                            catch (DivideByZeroException)
                            {
                                ensembleFlow.TopFlow = firstValid.Flow / Da * (Z3 - Z2);
                                break;
                            }


                            //外延计算北向速度
                            Slope north_slope = new Slope();
                            north_slope.setXYs(binsDepth.ToArray(), binsNorthVelocity.ToArray());

                            double north_vel;
                            try
                            {
                                north_vel = north_slope.calY(dep);
                            }// 外延失败，斜角为90度，转而采用常数法
                            catch (DivideByZeroException)
                            {
                                ensembleFlow.TopFlow = firstValid.Flow / Da * (Z3 - Z2);
                                break;
                            }
                            // 计算流量
                            ensembleFlow.TopFlow = dTime * (Z3 - Z2) * CrossProduct(east_vel, north_vel,
                                                     boatVelocity.EastVelocity, boatVelocity.NorthVelocity);
                            break;

                        }

                    }

                default:
                    break;
            }

            //底部流量

            switch (bottomMode)
            {
                case BottomFlowMode.Constants:
                    ensembleFlow.BottomFlow = lastValid.Flow / Da * Z1;
                    break;
                case BottomFlowMode.PowerFunction:
                    {
                        double a = exponent + 1;
                        ensembleFlow.BottomFlow = SumMeasuredFlow * Math.Pow(Z1, a) / (Math.Pow(Z2, a) - Math.Pow(Z1, a));
                    }
                    break;
                default:
                    break;
            }

            ensembleFlow.Valid = true;

            return ensembleFlow;
        }

        /// <summary>
        /// 计算表层和底层平均流速
        /// </summary>
        /// <param name="src">参与计算的数据</param>
        public void CalculateAverageVelocity(ArrayClass src,ref double dTopVx,ref double dTopVy,ref double dBottomVx,ref double dBottomVy)
        {
            //单元大小
            double Da = src.A_CellSize;
            if (Da < 1e-6) return;
            //底跟踪四个波束中的最小深度
            double Dmin = double.NaN;
            //计算四个波束的平均深度
            int num = 0;
            double depthSum = 0;
            foreach (double d in src.B_Range)
            {
                if (d < 1e-6) continue;

                if (double.IsNaN(Dmin))
                {
                    Dmin = d;
                }
                else
                {
                    if (d < Dmin)
                    {
                        Dmin = d;
                    }
                }

                num++;
                depthSum += d;
            }
            if (num == 0) return;
            double Davg = depthSum / num;

            //有效单元的可能最大深度
            double DLGmax = Dmin * Math.Cos(beamAngle / 180.0 * Math.PI) + draft - Math.Max((pulseLength + pulseLag) / 2.0, Da / 2.0);

            //水面到水底的距离
            double Dtotal = Davg + draft;

            //第一个有效单元
            MeasuredBinInfo firstValid = new MeasuredBinInfo();
            firstValid.Status = BinFlowStatus.Bad;
            //最后一个有效单元
            MeasuredBinInfo lastValid = new MeasuredBinInfo();
            //单元深度
            double binDepth = src.A_FirstCellDepth + draft;
            //累计东向水速
            double SumEastVelocity = 0;
            //累计北向水速
            double SumNorthVelocity = 0;
          
            for (int i = 0; binDepth < DLGmax && i < src.E_Cells && i * 4 < src.Earth.Length; i++, binDepth += Da)
            {
                MeasuredBinInfo binInfo = new MeasuredBinInfo();
                binInfo.Depth = binDepth;
                binInfo.Status = (IsGoodBin(src, i) ? BinFlowStatus.Good : BinFlowStatus.Bad);
             
                if (binInfo.Status == BinFlowStatus.Good)
                {
                    if (firstValid.Status == BinFlowStatus.Bad)
                    {
                        firstValid = binInfo;
                    }
                    lastValid = binInfo;

                    SumEastVelocity += src.Earth[0, i];
                    SumNorthVelocity += src.Earth[1, i];
                
                }
            }
          
            if (firstValid.Status == BinFlowStatus.Bad)
            {
                return;
            }

            //第一个有效单元的深度
            double Dtop = firstValid.Depth;
            //最后一个有效单元的深度
            double Dlg = lastValid.Depth;

            double Z3 = Dtotal;
            double Z2 = Dtotal - Dtop + Da / 2.0;
            double Z1 = Dtotal - Dlg - Da / 2.0;

            //表层流速
            double a = exponent + 1;
            dTopVx = SumEastVelocity * Da / (Dtop - Da / 2.0) * (Math.Pow(Z3, a) - Math.Pow(Z2, a)) / (Math.Pow(Z2, a) - Math.Pow(Z1, a));
            dTopVy = SumNorthVelocity * Da / (Dtop - Da / 2.0) * (Math.Pow(Z3, a) - Math.Pow(Z2, a)) / (Math.Pow(Z2, a) - Math.Pow(Z1, a));
          

            //底部流速
            dBottomVx = SumEastVelocity * Da / Z1 * Math.Pow(Z1, a) / (Math.Pow(Z2, a) - Math.Pow(Z1, a));
            dBottomVy = SumNorthVelocity * Da / Z1 * Math.Pow(Z1, a) / (Math.Pow(Z2, a) - Math.Pow(Z1, a));
           
        }

        #endregion

    }
}
