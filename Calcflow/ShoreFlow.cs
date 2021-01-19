using System;
using System.Collections.Generic;
using System.Text;

namespace Calcflow
{
    internal class ShoreFlow : CalculateFlow
    {
        //计算岸边流量
        public  double CalculateShoreFlow(CalculateShoreFlowParam param)
        {
            this.BeamAngle = param.RiverDischargeInstrument.RiverDischargeBeamAngle;
            this.PulseLength = param.RiverDischargeInstrument.RiverDischargePulseLength;
            this.PulseLag = param.RiverDischargeInstrument.RiverDischargePulseLag;

            this.MinAmplitude = param.RiverDischargeConditions.RiverDischargeMinAmplitude;
            this.MinCorrelation = param.RiverDischargeConditions.RiverDischargeMinCorrelation;
            this.MinNG3 = param.RiverDischargeConditions.RiverDischargeMinNG3;
            this.MinNG4 = param.RiverDischargeConditions.RiverDischargeMinNG4;

            return Calculate(param.RiverDischargeOrgData,
                            param.RiverDischargeDistance,
                            param.RiverDischarge_A,
                            param.RiverDischargeDraft);
        }


        /// <summary>
        /// 计算岸边流量
        /// </summary>
        /// <param name="src">参与岸边平均流速计算的数据</param>
        /// <param name="distance">岸边距(m)</param>
        /// <param name="a">岸边系数</param>
        /// <param name="_draft">吃水</param>
        /// <returns>岸流量</returns>
        private  double Calculate(ArrayClass[] src, double distance, double a, double _draft)  //JZH 
        {

            EarthVelocity avg_vel;
            avg_vel.EastVelocity = 0;
            avg_vel.NorthVelocity = 0;
            try
            {
                avg_vel = CalculateAvgVelocity(src, _draft);
            }
            catch //(Exception ex)
            {
                //throw new Exception("Average Velocity Error in ShoreFlow Calculation!" + "\r\n" + ex.Message, ex);
            }

            //求取船首方向角的平均值
            double[] headings = new double[src.Length];
            //求取深度
            double[,] d_range = new double[src.Length, 4];
            for (int i = 0; i < src.Length; i++)
            {
                headings[i] = src[i].A_Heading;
                d_range[i, 0] = src[i].B_Range[0];
                d_range[i, 1] = src[i].B_Range[1];
                d_range[i, 2] = src[i].B_Range[2];
                d_range[i, 3] = src[i].B_Range[3];
            }
            double avg_heading = 0.0;
            double avg_range = 0.0;

            foreach (double h in headings)
            {
                avg_heading += h;
            }
            avg_heading /= headings.Length;

            List<double> all_range = new List<double>();
            for (int i = 0; i < src.Length; i++)
            {
                List<double> ensemble_range = new List<double>();
                for (int j = 0; j < 4; j++)
                {
                    if (d_range[i, j] != 0.0)
                    {
                        ensemble_range.Add(d_range[i, j]);
                    }
                }
                if (ensemble_range.Count == 0)
                    continue;
                double ensemble_avg = 0.0;
                foreach (double _range in ensemble_range)
                {
                    ensemble_avg += _range;
                }
                ensemble_avg /= ensemble_range.Count;

                all_range.Add(ensemble_avg);
            }
            if(all_range.Count == 0)
                throw new Exception("Average Range Error in ShoreFlow Calculation!");

            foreach(double e_range in all_range)
            {
                avg_range += e_range;
            }
            avg_range /= all_range.Count;

            //计算流量 (Vn*sinθ - Ve*cosθ) * Distance * L *  a
            //double rad = (avg_heading / 180) * Math.PI;
            double ve = avg_vel.EastVelocity;
            double vn = avg_vel.NorthVelocity;

            //double flow = (vn * Math.Sin(rad) - ve * Math.Cos(rad)) * distance * a * (avg_range+_draft);
            double flow = Math.Sqrt(vn * vn + ve * ve) * distance * a * (avg_range + _draft);
        
            return flow;

        }

        /// <summary>
        /// 计算地理坐标系下的平均流速
        /// </summary>
        /// <param name="srcs">参与计算的数据</param>
        /// <param name="draft">吃水</param>
        /// <returns>地理坐标系下的平均流速</returns>
        public  EarthVelocity CalculateAvgVelocity(ArrayClass[] srcs, double draft)  //JZH 2012-04-08 提供公共方法
        {
            EarthVelocity avg = new EarthVelocity();
            {
                avg.EastVelocity = 0;
                avg.NorthVelocity = 0;
                avg.UpVelocity = 0;
                avg.qVelocity = 0;
            }
            List<EarthVelocity> vels = new List<EarthVelocity>();
            foreach (ArrayClass src in srcs)
            {
                //如果状态异常，则不用此数据
                //if (src.E_Status != 0x00)  //LPJ 2016-12-12 cancel 用户实际测量时发现其他数据正常，但状态异常，直接导致岸边流量不计算
                //    continue;
                if (src.B_Earth[0] > 80 || src.B_Earth[1] > 80 || src.B_Earth[2] > 80 || src.B_Earth[3] > 80)
                    continue;
                //单元大小
                double Da = src.A_CellSize;
                //if (Da < 1e-6)
                //    throw new Exception("Cell Size too small");

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
                if (num == 0) // 深度均无效则此此次测线数据无效
                {
                    continue;
                }

                double Davg = depthSum / num;

                //有效单元的可能最大深度
                double DLGmax = Dmin * Math.Cos(beamAngle / 180.0 * Math.PI) + draft - Math.Max((pulseLength + pulseLag) / 2.0,0.5*Da);

                //水面到水底的距离
                double Dtotal = Davg + draft;

                //单元深度
                double binDepth = src.A_FirstCellDepth + draft;

                List<EarthVelocity> cell_vels = new List<EarthVelocity>();
                for (int i = 0; binDepth < DLGmax && i < src.E_Cells && i * 4 < src.Earth.Length; i++)
                {
                    if (!IsGoodBin(src, i))
                        continue;

                    //if (src.B_Velocity[0] > 80 || src.B_Velocity[1] > 80 || src.B_Velocity[2] > 80 || src.B_Velocity[3] > 80)
                    //    continue;

                    ////获取速度信息
                    //double ve = src.Earth[0, i] - src.B_Earth[0];
                    //double vn = src.Earth[1, i] - src.B_Earth[1];
                    //double vu = src.Earth[2, i] - src.B_Earth[2];
                    //double vq = src.Earth[3, i] - src.B_Earth[3];

                    //获取绝对速度信息 JZH 2012-02-05
                    double ve = src.Earth[0, i] + src.B_Earth[0];
                    double vn = src.Earth[1, i] + src.B_Earth[1];
                    double vu = src.Earth[2, i] + src.B_Earth[2];
                    double vq = src.Earth[3, i] + src.B_Earth[3];

                    EarthVelocity ev = new EarthVelocity();
                    ev.EastVelocity = ve;
                    ev.NorthVelocity = vn;
                    ev.UpVelocity = vu;
                    ev.qVelocity = vq;
                    cell_vels.Add(ev);

                    binDepth += Da;
                }
                //如果有效单元少于1个，则这个测线数据不可信
                if (cell_vels.Count < 1)
                    continue;
                //求一条测线上平均流速
                double avg_ve = 0, avg_vn = 0, avg_vu = 0, avg_vq = 0;
                foreach (EarthVelocity earth_v in cell_vels)
                {
                    avg_ve += earth_v.EastVelocity;
                    avg_vn += earth_v.NorthVelocity;
                    avg_vu += earth_v.UpVelocity;
                    avg_vq += earth_v.qVelocity;
                }
                avg_ve /= cell_vels.Count;
                avg_vn /= cell_vels.Count;
                avg_vu /= cell_vels.Count;
                avg_vq /= cell_vels.Count;

                EarthVelocity cells_avg = new EarthVelocity();
                cells_avg.EastVelocity = avg_ve;
                cells_avg.NorthVelocity = avg_vn;
                cells_avg.UpVelocity = avg_vu;
                cells_avg.qVelocity = avg_vq;

                vels.Add(cells_avg);

            }
            //如果数据组全部不可信，则返回null
            //if (vels.Count == 0)
            //    throw new Exception("No Useful Ensembles");

            //求多次测量平均速度
            double avgall_ve = 0, avgall_vn = 0, avgall_vu = 0, avgall_vq = 0;
            foreach (EarthVelocity avg_v in vels)
            {
                avgall_ve += avg_v.EastVelocity;
                avgall_vn += avg_v.NorthVelocity;
                avgall_vu += avg_v.UpVelocity;
                avgall_vq += avg_v.qVelocity;
            }
            if (vels.Count > 0)
            {
                avgall_ve /= vels.Count;
                avgall_vn /= vels.Count;
                avgall_vu /= vels.Count;
                avgall_vq /= vels.Count;

                //EarthVelocity avg = new EarthVelocity();
                avg.EastVelocity = avgall_ve;
                avg.NorthVelocity = avgall_vn;
                avg.UpVelocity = avgall_vu;
                avg.qVelocity = avgall_vq;
            }

            return avg;
        }
    }
}
