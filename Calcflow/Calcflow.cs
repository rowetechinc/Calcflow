using System;
using System.Collections.Generic;
using System.Text;

namespace Calcflow
{
    internal class CalculateFlow
    {
        #region  GoodBin的条件
        protected double minAmplitude = 0;
        /// <summary>
        /// 最小回波强度(20counts = 1dB)
        /// </summary>
        public double MinAmplitude
        {
            get { return minAmplitude; }
            set { minAmplitude = value; }
        }
        protected double minCorrelation = 0;
        /// <summary>
        /// 最小相关性
        /// </summary>
        public double MinCorrelation
        {
            get { return minCorrelation; }
            set { minCorrelation = value; }
        }
        protected int minNG3 = 1;
        /// <summary>
        /// 最小NG3
        /// </summary>
        public int MinNG3
        {
            get { return minNG3; }
            set { minNG3 = value; }
        }
        protected int minNG4 = 0;
        /// <summary>
        /// 最小NG4
        /// </summary>
        public int MinNG4
        {
            get { return minNG4; }
            set { minNG4 = value; }
        }
        #endregion

        #region  设备属性
        protected double beamAngle = 20;
        /// <summary>
        /// 波束角(度)
        /// </summary>
        public double BeamAngle
        {
            get { return beamAngle; }
            set { beamAngle = value; }
        }

        protected double pulseLength = 0;
        /// <summary>
        /// 脉冲长度(m)
        /// </summary>
        public double PulseLength
        {
            get { return pulseLength; }
            set { pulseLength = value; }
        }
        protected double pulseLag = 0;
        /// <summary>
        /// 脉冲间隔(m)
        /// </summary>
        public double PulseLag
        {
            get { return pulseLag; }
            set { pulseLag = value; }
        }
        #endregion

        /// <summary>
        /// 计算矢量交叉积
        /// </summary>
        /// <param name="Ve">水流相对于船的东向流速</param>
        /// <param name="Vn">水流相对于船的北向流速</param>
        /// <param name="Vbe">船的东向速度</param>
        /// <param name="Vbn">船的北向速度</param>
        /// <returns></returns>
        protected double CrossProduct(double Ve, double Vn, double Vbe, double Vbn)
        {
            return (Vn + Vbn) * Vbe - (Ve + Vbe) * Vbn;
        }


        /// <summary>
        /// 判断是否为一个GoodBin
        /// </summary>
        /// <param name="src">参与计算数据</param>
        /// <param name="binIndex">bin的索引号</param>
        /// <returns>是否为GoodBin</returns>
        protected bool IsGoodBin(ArrayClass src, int binIndex)
        {
            try
            {
                int count = 0;
                for (int i = 0; i < 4; i++)
                {
                    if (src.Amplitude[i, binIndex] < minAmplitude)
                        count++;
                }

                if (count > 1) return false;

                count = 0;
                for (int i = 0; i < 4; i++)
                {
                    if (src.Correlation[i, binIndex] < minCorrelation)
                        count++;
                }

                if (count > 1) return false;

                //JZH 2012-02-06 JZH   取消旧判断方法
                //if (src.BeamN[0, binIndex] < minNG3
                //    || src.BeamN[1, binIndex] < minNG3
                //    || src.BeamN[2, binIndex] < minNG3
                //    || src.BeamN[3, binIndex] < minNG4)
                //    return false;
                //if (src.Earth[0, binIndex] > 88
                //    || src.Earth[1, binIndex] > 88
                //    || src.Earth[2, binIndex] > 88
                //    || src.Earth[3, binIndex] > 88
                //    )
                //    return false;

                //JZH 2012-02-06 新判断方法
                if (src.Earth[0, binIndex] > 88
                   || src.Earth[1, binIndex] > 88
                   || src.Earth[2, binIndex] > 88
                //    || src.Earth[3, binIndex] > 88
                   )
                   return false;
            }
            catch 
            {
                return false;
            }
            

            return true;

        }


    }
}
