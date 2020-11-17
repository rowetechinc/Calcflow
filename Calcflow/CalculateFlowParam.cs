using System;
using System.Collections.Generic;
using System.Text;

#pragma warning disable IDE0017
#pragma warning disable CS1591
#pragma warning disable CS1573

namespace Calcflow
{
    /// <summary>
    /// 计算Ensemble流量需要的参数
    /// </summary>
    public class CalculateEnsembleFlowParam
    {
        /// <summary>
        /// 顶部流量模型
        /// </summary>
        public TopFlowMode RiverDischargeTopMode = TopFlowMode.PowerFunction;
        /// <summary>
        /// 底部流量模型
        /// </summary>
        public BottomFlowMode RiverDischargeBottomMode = BottomFlowMode.PowerFunction;
        /// <summary>
        /// ADCP吃水(m)
        /// </summary>
        public double RiverDischargeDraft = 0;
        /// <summary>
        /// 幂函数指数
        /// </summary>
        public double RiverDischargeExponent = 1/6.0;
        /// <summary>
        /// 判断GoodBin的条件
        /// </summary>
        public GoodBinConditions RiverDischargeConditions;
        /// <summary>
        /// ADCP仪器参数
        /// </summary>
        public InstrumentParams RiverDischargeInstrument;
        /// <summary>
        /// Ping的原始数据
        /// </summary>
        public ArrayClass RiverDischargeOrgData = null;
        /// <summary>
        /// 前一个有效数据组（Ensemble）与当前计算流量的有效数据组之间的时间间隔
        /// </summary>
        public double RiverDischarge_dTime = 0;
        /// <summary>
        /// 构造函数
        /// </summary>
        public CalculateEnsembleFlowParam()
        {
            RiverDischargeConditions = new GoodBinConditions();
            RiverDischargeConditions.RiverDischargeMinAmplitude = 0;
            RiverDischargeConditions.RiverDischargeMinCorrelation = 0;
            RiverDischargeConditions.RiverDischargeMinNG3 = 1;
            RiverDischargeConditions.RiverDischargeMinNG4 = 0;

            RiverDischargeInstrument = new InstrumentParams();
            RiverDischargeInstrument.RiverDischargeBeamAngle = 15;
            RiverDischargeInstrument.RiverDischargePulseLag = 0;
            RiverDischargeInstrument.RiverDischargePulseLength = 0;
        }

    }
    /// <summary>
    /// 计算岸边流量所需要的参数
    /// </summary>
    public class CalculateShoreFlowParam
    {
        /// <summary>
        /// ADCP吃水(m)
        /// </summary>
        public double RiverDischargeDraft = 0;
        /// <summary>
        /// 岸边系数
        /// </summary>
        public double RiverDischarge_A = 0.35;
        /// <summary>
        /// 岸边距(m)
        /// </summary>
        public double RiverDischargeDistance = 0;
        /// <summary>
        /// 判断GoodBin的条件
        /// </summary>
        public GoodBinConditions RiverDischargeConditions;
        /// <summary>
        /// ADCP仪器参数
        /// </summary>
        public InstrumentParams RiverDischargeInstrument;
        /// <summary>
        /// 参与计算的数据集合
        /// </summary>
        public ArrayClass[] RiverDischargeOrgData = null;
        /// <summary>
        /// 构造函数
        /// </summary>
        public CalculateShoreFlowParam()
        {
            RiverDischargeConditions = new GoodBinConditions();
            RiverDischargeConditions.RiverDischargeMinAmplitude = 0;
            RiverDischargeConditions.RiverDischargeMinCorrelation = 0;
            RiverDischargeConditions.RiverDischargeMinNG3 = 1;
            RiverDischargeConditions.RiverDischargeMinNG4 = 0;

            RiverDischargeInstrument = new InstrumentParams();
            RiverDischargeInstrument.RiverDischargeBeamAngle = 15;
            RiverDischargeInstrument.RiverDischargePulseLag = 0;
            RiverDischargeInstrument.RiverDischargePulseLength = 0;
        }

    }
    /// <summary>
    /// Ensemble设计参数
    /// </summary>
    public class EnsembleDataDesignParam
    {
        /// <summary>
        /// 数据模板
        /// </summary>
        public ArrayClass TemplateData;
        /// <summary>
        /// 地理坐标系下的参考流速(m/s)
        /// </summary>
        public float ReferEarthVelocity = 1;
        /// <summary>
        /// 参考回波强度
        /// </summary>
        public float ReferAmplitude = 800;
        /// <summary>
        /// 参考相关度
        /// </summary>
        public float ReferCorrelation = 0.5f;
        /// <summary>
        /// 参考水深
        /// </summary>
        public float ReferDepth = 30.0f;
        /// <summary>
        /// 噪音大小
        /// </summary>
        public float NoiseSize = 0.1f;
        /// <summary>
        /// 波束角(度)
        /// </summary>
        public double BeamAngle = 15;

    }

    /// <summary>
    /// 计算河流流量
    /// </summary>
    public class RiverDischargeCalculate
    {
        /// <summary>
        /// 计算岸边流量
        /// 如果数据组中不能得到有效的平均流速数据（比如10组数据中任何一组的有效bins小于2）,
        /// 此调用会抛出 Exception 异常，请注意捕捉
        /// </summary>
        /// <param name="param">参数</param>
        /// <returns>计算结果</returns>
        public static double CalculateShoreFlow(CalculateShoreFlowParam param)
        {
            ShoreFlow flow = new ShoreFlow();
            return flow.CalculateShoreFlow(param);
        }

        //JZH 2012-04-08 添加一个计算岸边平均流速的方法
        public static double CalculateShoreVelocity(CalculateShoreFlowParam param)
        {
            ShoreFlow flow = new ShoreFlow();
            EarthVelocity eVel;//=new EarthVelocity();
            eVel = flow.CalculateAvgVelocity(param.RiverDischargeOrgData, 0.0);
            double fVelDir = System.Math.Atan2(eVel.EastVelocity, eVel.NorthVelocity);
            if (fVelDir < 0)
                fVelDir = fVelDir / 3.14159 * 180 + 360;
            else
                fVelDir = fVelDir / 3.14159 * 180;
            return fVelDir;
        }
        /// <summary>
        /// 计算Ping流量
        /// </summary>
        /// <param name="param">参数</param>
        /// <returns>计算结果</returns>
        public static EnsembleFlowInfo CalculateEnsembleFlow(CalculateEnsembleFlowParam param, double EastVelocity, double NorthVelocity)
        {

            EnsembleFlow flow = new EnsembleFlow();
            flow.BottomMode = param.RiverDischargeBottomMode;
            flow.TopMode = param.RiverDischargeTopMode;
            flow.Draft = param.RiverDischargeDraft;
            flow.Exponent = param.RiverDischargeExponent;
            flow.MinAmplitude = param.RiverDischargeConditions.RiverDischargeMinAmplitude;
            flow.MinCorrelation = param.RiverDischargeConditions.RiverDischargeMinCorrelation;
            flow.MinNG3 = param.RiverDischargeConditions.RiverDischargeMinNG3;
            flow.MinNG4 = param.RiverDischargeConditions.RiverDischargeMinNG4;
            flow.PulseLag = param.RiverDischargeInstrument.RiverDischargePulseLag;
            flow.PulseLength = param.RiverDischargeInstrument.RiverDischargePulseLength;
            flow.BeamAngle = param.RiverDischargeInstrument.RiverDischargeBeamAngle;
            
             //LPJ 2013-5-21 将船速采用传递的参数---start
             EarthVelocity boat = new EarthVelocity();   
            ////boat.EastVelocity = - param.RiverDischargeOrgData.B_Earth[0];
            ////boat.NorthVelocity = - param.RiverDischargeOrgData.B_Earth[1];
            //boat.EastVelocity = param.RiverDischargeOrgData.B_Earth[0];  //JZH 2012-04-08
            //boat.NorthVelocity = param.RiverDischargeOrgData.B_Earth[1]; //JZH　2012-04-08
             boat.EastVelocity = EastVelocity;
             boat.NorthVelocity = NorthVelocity;

            //LPJ 2013-5-21  将船速采用传递的参数---end

            return flow.CalculateEnsembleFlow(param.RiverDischargeOrgData, boat, param.RiverDischarge_dTime);
        }

        /// <summary>
        /// 计算Ping流量
        /// </summary>
        /// <param name="param">参数</param>
        /// <param name="EastVelocity">船速东向分量</param>
        /// <param name="NorthVelocity">船速北向分量</param>
        /// <param name="HeadingOffset">航向角改正值，单位：弧度</param>
        /// <returns>计算结果</returns>
        public static EnsembleFlowInfo CalculateEnsembleFlow(CalculateEnsembleFlowParam param, double EastVelocity, double NorthVelocity,double HeadingOffset)
        {

            EnsembleFlow flow = new EnsembleFlow();
            flow.BottomMode = param.RiverDischargeBottomMode;
            flow.TopMode = param.RiverDischargeTopMode;
            flow.Draft = param.RiverDischargeDraft;
            flow.Exponent = param.RiverDischargeExponent;
            flow.MinAmplitude = param.RiverDischargeConditions.RiverDischargeMinAmplitude;
            flow.MinCorrelation = param.RiverDischargeConditions.RiverDischargeMinCorrelation;
            flow.MinNG3 = param.RiverDischargeConditions.RiverDischargeMinNG3;
            flow.MinNG4 = param.RiverDischargeConditions.RiverDischargeMinNG4;
            flow.PulseLag = param.RiverDischargeInstrument.RiverDischargePulseLag;
            flow.PulseLength = param.RiverDischargeInstrument.RiverDischargePulseLength;
            flow.BeamAngle = param.RiverDischargeInstrument.RiverDischargeBeamAngle;

            //LPJ 2013-5-21 将船速采用传递的参数---start
            EarthVelocity boat = new EarthVelocity();
            ////boat.EastVelocity = - param.RiverDischargeOrgData.B_Earth[0];
            ////boat.NorthVelocity = - param.RiverDischargeOrgData.B_Earth[1];
            //boat.EastVelocity = param.RiverDischargeOrgData.B_Earth[0];  //JZH 2012-04-08
            //boat.NorthVelocity = param.RiverDischargeOrgData.B_Earth[1]; //JZH　2012-04-08
            boat.EastVelocity = EastVelocity;
            boat.NorthVelocity = NorthVelocity;

            //LPJ 2013-5-21  将船速采用传递的参数---end

            return flow.CalculateEnsembleFlow(param.RiverDischargeOrgData, boat, param.RiverDischarge_dTime,HeadingOffset);
        }

        /// <summary>
        /// 读取raw数据
        /// </summary>
        /// <param name="filePath">raw数据文件名称</param>
        /// <returns>ArrayClass集合</returns>
        public static ArrayClass[] ReadEnsembleRawData(string filePath)
        {
            return ReadRTIData.ReadRawData(filePath);
        }
        /// <summary>
        /// 根据模板数据生成设计数据
        /// </summary>
        /// <param name="param">所需要的参数</param>
        /// <returns>设计数据</returns>
        public static ArrayClass FillEnsembleData(EnsembleDataDesignParam param)
        {
            return MakeEnsembleData.MakeData(param);
        }
  
        /// <summary>
        /// 计算表层和底部平均流速
        /// </summary>
        /// <param name="param">参数</param>
        /// <param name="dTopVx">表层平均流速X分量</param>
        /// <param name="dTopVy">表层平均流速Y分量</param>
        /// <param name="dBottomVx">底部平均流速X分量</param>
        /// <param name="dBottomVy">底部平均流速Y分量</param>
        public static void CalEnsembleAverageVelocity(CalculateEnsembleFlowParam param,ref double dTopVx,ref double dTopVy,ref double dBottomVx,ref double dBottomVy)
        {
            EnsembleFlow flow = new EnsembleFlow();
            flow.BottomMode = param.RiverDischargeBottomMode;
            flow.TopMode = param.RiverDischargeTopMode;
            flow.Draft = param.RiverDischargeDraft;
            flow.Exponent = param.RiverDischargeExponent;
            flow.MinAmplitude = param.RiverDischargeConditions.RiverDischargeMinAmplitude;
            flow.MinCorrelation = param.RiverDischargeConditions.RiverDischargeMinCorrelation;
            flow.MinNG3 = param.RiverDischargeConditions.RiverDischargeMinNG3;
            flow.MinNG4 = param.RiverDischargeConditions.RiverDischargeMinNG4;
            flow.PulseLag = param.RiverDischargeInstrument.RiverDischargePulseLag;
            flow.PulseLength = param.RiverDischargeInstrument.RiverDischargePulseLength;
            flow.BeamAngle = param.RiverDischargeInstrument.RiverDischargeBeamAngle;

            flow.CalculateAverageVelocity(param.RiverDischargeOrgData, ref dTopVx, ref dTopVy, ref dBottomVx, ref dBottomVy);
        }
       
    }

}
