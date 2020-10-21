using System;
using System.Collections.Generic;
using System.Text;

namespace Calcflow
{
   
    /// <summary>
    /// 地理坐标系下的速度结构
    /// </summary>
    internal struct EarthVelocity 
    {
        /// <summary>
        /// 地理东向速度
        /// </summary>
        public double EastVelocity;
        /// <summary>
        /// 地理北向速度
        /// </summary>
        public double NorthVelocity;
        /// <summary>
        /// 垂直速度
        /// </summary>
        public double UpVelocity;
        /// <summary>
        /// 质量速度
        /// </summary>
        public double qVelocity;
    }

    /// <summary>
    /// 顶部模型
    /// </summary>
    public enum TopFlowMode
    {
        /// <summary>
        /// 常量
        /// </summary>
        Constants,
        /// <summary>
        /// 指数
        /// </summary>
        PowerFunction,
        /// <summary>
        /// 三点外延
        /// </summary>
        Slope

    }
    /// <summary>
    /// 底部模型
    /// </summary>
    public enum BottomFlowMode
    {
        /// <summary>
        /// 幂函数
        /// </summary>
        PowerFunction,
        /// <summary>
        /// 常量
        /// </summary>
        Constants,
    }
    /// <summary>
    /// Bin的状态
    /// </summary>
    public enum BinFlowStatus { 
        /// <summary>
        /// 好的
        /// </summary>
        Good,
        /// <summary>
        /// 坏的
        /// </summary>
        Bad };

    /// <summary>
    /// 原始数据
    /// </summary>
    public class ArrayClass
    {
        private const int MaxArray = 16;
        private const int MaxBins = 200;
        // Fields
        /// <summary>
        /// nArray
        /// </summary>
        public int nArray;
        /// <summary>
        /// 类型
        /// </summary>
        public int[] Type = new int[MaxArray];
        /// <summary>
        /// Bins
        /// </summary>
        public int[] Bins = new int[MaxArray];
        /// <summary>
        /// Beams
        /// </summary>
        public int[] Beams = new int[MaxArray];
        /// <summary>
        /// Imag
        /// </summary>
        public int[] Imag = new int[MaxArray];
        /// <summary>
        /// NameLen
        /// </summary>
        public int[] NameLen = new int[MaxArray];
        /// <summary>
        /// Name
        /// </summary>
        public string[] Name = new string[MaxArray];

        /// <summary>
        /// Beam方向速度有效性
        /// </summary>
        public bool VelocityAvailable;
        /// <summary>
        /// 仪器坐标系速度有效性
        /// </summary>
        public bool InstrumentAvailable;
        /// <summary>
        /// 地理坐标系下速度有效性
        /// </summary>
        public bool EarthAvailable;
        /// <summary>
        /// 回波强度有效性
        /// </summary>
        public bool AmplitudeAvailable;
        /// <summary>
        /// 相关性有效性
        /// </summary>
        public bool CorrelationAvailable;
        /// <summary>
        /// BeamN有效性
        /// </summary>
        public bool BeamNAvailable;
        /// <summary>
        /// XfrmN有效性
        /// </summary>
        public bool XfrmNAvailable;
        /// <summary>
        /// EnsembleData有效性
        /// </summary>
        public bool EnsembleDataAvailable;
        /// <summary>
        /// Ancillary有效性
        /// </summary>
        public bool AncillaryAvailable;
        /// <summary>
        /// 水底跟踪有效性
        /// </summary>
        public bool BottomTrackAvailable;
        /// <summary>
        /// Nmea数据有效性
        /// </summary>
        public bool NmeaAvailable;

        /// <summary>
        /// 每个Beam的Bin速度
        /// </summary>
        public float[,] Velocity = new float[4, MaxBins];
        /// <summary>
        /// 仪器坐标系下的每个Bin的速度
        /// </summary>
        public float[,] Instrument = new float[4, MaxBins];
        /// <summary>
        /// 地理坐标系下的每个Bin的速度
        /// </summary>
        public float[,] Earth = new float[4, MaxBins];
        /// <summary>
        /// 每个Beam的每个Bin的强度
        /// </summary>
        public float[,] Amplitude = new float[4, MaxBins];
        /// <summary>
        /// 每个Beam的每个Bin的相关性
        /// </summary>
        public float[,] Correlation = new float[4, MaxBins];
        /// <summary>
        /// 每个Beam的Bin的有效Ping个数
        /// </summary>
        public int[,] BeamN = new int[4, MaxBins];
        /// <summary>
        /// NG3，NG3,NG3,NG4
        /// </summary>
        public int[,] XfrmN = new int[4, MaxBins];

        //EnsembleData
        /// <summary>
        /// EnsembleNumber
        /// </summary>
        public long E_EnsembleNumber;
        /// <summary>
        /// Bin的个数
        /// </summary>
        public long E_Cells;
        /// <summary>
        /// beam的个数
        /// </summary>
        public long E_Beams;
        /// <summary>
        /// Ensemble的Pings的个数
        /// </summary>
        public long E_PingsInEnsemble;
        /// <summary>
        /// 总Ping数
        /// </summary>
        public long E_PingCount;
        /// <summary>
        /// 状态
        /// </summary>
        public long E_Status;

        /////////////////2013-11-04
        ///<summary>
        ///SN信息
        ///</summary>
        public byte[] E_SN_Buffer = new byte[33];

        ///<summary>
        ///固件版本号
        ///</summary>
        public byte[] E_FW_Vers = new byte[4];

        ////////////////2010-01-14 ///////////////
        /// <summary>
        /// 年
        /// </summary>
        public long YYYY;
        /// <summary>
        /// 月
        /// </summary>
        public long MM;
        /// <summary>
        /// 日
        /// </summary>
        public long DD;
        /// <summary>
        /// 小时
        /// </summary>
        public long HH;
        /// <summary>
        /// 分钟
        /// </summary>
        public long mm;
        /// <summary>
        /// 秒
        /// </summary>
        public long SS;
        /// <summary>
        /// 0.01秒
        /// </summary>
        public long hsec = 0;
        ///////////////////////////////////////////////////
        /// <summary>
        /// NMEA数据
        /// </summary>
        public byte[] NMEA_Buffer = new byte[8192];

        //Ancillary
        /// <summary>
        /// 第一个Bin到ADCP的距离
        /// </summary>
        public float A_FirstCellDepth;
        /// <summary>
        /// Bin的大小
        /// </summary>
        public float A_CellSize;
        /// <summary>
        /// FirstPingSeconds
        /// </summary>
        public float A_FirstPingSeconds;
        /// <summary>
        /// LastPingSeconds
        /// </summary>
        public float A_LastPingSeconds;
        /// <summary>
        /// 船首向
        /// </summary>
        public float A_Heading;
        /// <summary>
        /// 俯仰
        /// </summary>
        public float A_Pitch;
        /// <summary>
        /// 摇摆
        /// </summary>
        public float A_Roll;
        /// <summary>
        /// 水温
        /// </summary>
        public float A_WaterTemperature;
        /// <summary>
        /// BoardTemperature
        /// </summary>
        public float A_BoardTemperature;
        /// <summary>
        /// 盐度
        /// </summary>
        public float A_Salinity;
        /// <summary>
        /// 压强
        /// </summary>
        public float A_Pressure;
        /// <summary>
        /// 深度
        /// </summary>
        public float A_Depth;
        /// <summary>
        /// 声速
        /// </summary>
        public float A_SpeedOfSound;

        //Bottom Track
        /// <summary>
        /// FirstPingSeconds
        /// </summary>
        public float B_FirstPingSeconds;
        /// <summary>
        /// LastPingSeconds
        /// </summary>
        public float B_LastPingSeconds;
        /// <summary>
        /// 船首向
        /// </summary>
        public float B_Heading;
        /// <summary>
        /// 俯仰
        /// </summary>
        public float B_Pitch;
        /// <summary>
        /// 摇摆
        /// </summary>
        public float B_Roll;
        /// <summary>
        /// 水温
        /// </summary>
        public float B_WaterTemperature;
        /// <summary>
        /// BoardTemperature
        /// </summary>
        public float B_BoardTemperature;
        /// <summary>
        /// 盐度
        /// </summary>
        public float B_Salinity;
        /// <summary>
        /// 压强
        /// </summary>
        public float B_Pressure;
        /// <summary>
        /// 深度
        /// </summary>
        public float B_Depth;
        /// <summary>
        /// 声速
        /// </summary>
        public float B_SpeedOfSound;
        /// <summary>
        /// 状态
        /// </summary>
        public float B_Status;//12
        /// <summary>
        /// Beams
        /// </summary>
        public float B_Beams;//13
        /// <summary>
        /// PingCount
        /// </summary>
        public float B_PingCount;//14
        /// <summary>
        /// 4个Beam的水深
        /// </summary>
        public float[] B_Range = new float[4];//15-18
        /// <summary>
        /// SNR
        /// </summary>
        public float[] B_SNR = new float[4];//19-21
        /// <summary>
        /// Amplitude
        /// </summary>
        public float[] B_Amplitude = new float[4];//23-26
        /// <summary>
        /// Correlation
        /// </summary>
        public float[] B_Correlation = new float[4];//27-30
        /// <summary>
        /// Velocity
        /// </summary>
        public float[] B_Velocity = new float[4];//31-34
        /// <summary>
        /// BeamN 
        /// </summary>
        public float[] B_BeamN = new float[4];//35-38
        /// <summary>
        /// Instrument
        /// </summary>
        public float[] B_Instrument = new float[4];//39-42
        /// <summary>
        /// XfrmN
        /// </summary>
        public float[] B_XfrmN = new float[4];//43-46
        /// <summary>
        /// Earth
        /// </summary>
        public float[] B_Earth = new float[4];//47-50
        /// <summary>
        /// EarthN
        /// </summary>
        public float[] B_EarthN = new float[4];//51-54

        #region Profile Engineering data contains data that describes the profile ping setup
        public float[] PrePingVel = new float[4];
        public float[] PrePingCor = new float[4];
        public float[] PrePingAmp = new float[4];
        public float ProfileSamplesPerSecond;
        public float ProfileSystemFreqHz;
        public float ProfileLagSamples;
        public float ProfileCPCE;
        public float ProfileNCE;
        public float ProfileRepeatN;
        public float PrePingGap;
        public float PrePingNCE;
        public float PrePingRepeatN;
        public float PrePingLagSamples;
        public float TRhighGain;
        #endregion

        #region Bottom track Engineering data  contains data that describes the bottom track
        public float BottomTrackSamplesPerSecond;
        public float BottomTrackSystemFreqHz;
        public float BottomTrackLagSamples;
        public float BottomTrackCPCE;
        public float BottomTrackNCE;
        public float BottomTrackRepeatN;
        public float[] AmbHz = new float[4];
        public float[] AmbVel = new float[4];
        public float[] AmbAmp = new float[4];
        public float[] AmbCor = new float[4];
        public float[] AmbSNR = new float[4];
        public float[] LagUsed = new float[4];
        #endregion

        #region system settings
        /// <summary>
        /// Gage Height Samples Per Second
        /// </summary>
        public float BT_SamplesPerSecond;

        /// <summary>
        /// Gage Height SystemFreqHz
        /// </summary>
        public float BT_SystemFreqHz;

        /// <summary>
        /// Gage Height CyclesPerCodeElement
        /// </summary>
        public float BT_CyclesPerCodeElement;

        /// <summary>
        /// Gage Height NumberOfCodeElementsIncode
        /// </summary>
        public float BT_NumberOfCodeElementsIncode;

        /// <summary>
        /// Gage Height NumberOfRepeatedCodes
        /// </summary>
        public float BT_NumberOfRepeatedCodes;

        /// <summary>
        /// Water profile SamplesPerSecond
        /// </summary>
        public float WP_SamplesPerSecond;

        /// <summary>
        /// Water profile SystemFreqHz
        /// </summary>
        public float WP_SystemFreqHz;

        /// <summary>
        /// Water profile CyclesPerCodeElement
        /// </summary>
        public float WP_CyclesPerCodeElement;

        /// <summary>
        ///Water profile NumberOfCodeElementsIncode
        /// </summary>
        public float WP_NumberOfCodeElementsIncode;

        /// <summary>
        /// Water profile NumberOfRepeatedCodes
        /// </summary>
        public float WP_NumberOfRepeatedCodes;

        /// <summary>
        /// Water profile NumberOfSamplesInLag
        /// </summary>
        public float WP_NumberOfSamplesInLag;

        /// <summary>
        /// SystemInputVoltage
        /// </summary>
        public float SystemInputVoltage;

        /// <summary>
        /// TransmitterBoostVoltage
        /// </summary>
        public float TransmitterBoostVoltage;
        public float BT_Mode;
        public float BT_PulseToPulseLag;
        public float BT_LongRangeSwitchDepth;
        public float BT_BeamMultiplex;

        /// <summary>
        /// water profile mode
        /// </summary>
        public float WP_Mode;

        /// <summary>
        /// water profile Lag
        /// </summary>
        public float WP_Lag;
        public float WP_TransmitBandwith;
        public float WP_ReceiveBandwidth;

        #endregion

        #region Profile Range Tracking
        public float RT_NumberOfBeams;
        public float[] RT_SNR = new float[4];
        public float[] RT_Range = new float[4];
        public float[] RT_NumberOfPings = new float[4];
        public float[] RT_Amp = new float[4];
        public float[] RT_Corr = new float[4];
        public float[] RT_Velocity = new float[4];
        public float[] RT_InsVelocity = new float[4];
        public float[] RT_EarthVelocity = new float[4];

        #endregion

        #region Gage Data
        public float Status;
        public float AverageRange;
        public float SinglePingStd_Range;
        public float AverageSN;
        public float No_Pings_SN_Threshold;
        public float Salinity;
        public float Pressure;
        public float Depth_PressureSensor;
        public float WaterTemp;
        public float BackPlaneTemp;
        public float SpeedSound_Water;
        public float Heading;
        public float Pitch;
        public float Roll;
        public float AverageSurface_EchoLevel;
        public float AverageNoiseLevelPriorToSurfaceEcho;
        public float AverageNoiseLevelAfterSurfaceEcho;
        public float HighGain;
        public float DesiredPingsParameter;
        public float SN_ThresholdParameter;
        public float SignalLevel;
        public float SinglePingRange_Std;
        public float DesiredNoTransmitCarrierCycles;
        #endregion
    }

    /// <summary>
    /// 实测单元的流量信息
    /// </summary>
    public struct MeasuredBinInfo
    {
        /// <summary>
        /// 在换能器以下的深度
        /// </summary>
        public double Depth;
        /// <summary>
        /// 流量
        /// </summary>
        public double Flow;
        /// <summary>
        /// 状态
        /// </summary>
        public BinFlowStatus Status;
    }
    /// <summary>
    /// Ensemble流量信息
    /// </summary>
    public class EnsembleFlowInfo
    {
        /// <summary>
        /// 顶部流量
        /// </summary>
        public double TopFlow;
        /// <summary>
        /// 底部流量
        /// </summary>
        public double BottomFlow;
        /// <summary>
        /// 实测流量
        /// </summary>
        public double MeasuredFlow;
        /// <summary>
        /// 当前计算结果是否有效
        /// </summary>
        public bool Valid;
        /// <summary>
        /// 每个Bin的流量信息
        /// </summary>
        public MeasuredBinInfo[] BinsInfo;
    }
    /// <summary>
    /// GoodBin的质量控制条件
    /// </summary>
    public struct GoodBinConditions 
    {
        /// <summary>
        /// 最小回波强度
        /// </summary>
        public double RiverDischargeMinAmplitude;
        /// <summary>
        /// 最小相关性
        /// </summary>
        public double RiverDischargeMinCorrelation;
        /// <summary>
        /// 3个有效Beam解算的最小个数
        /// </summary>
        public int RiverDischargeMinNG3;
        /// <summary>
        /// 4个有效Beam解算的最小个数
        /// </summary>
        public int RiverDischargeMinNG4;
    }
    /// <summary>
    /// 仪器参数
    /// </summary>
    public struct InstrumentParams 
    {
        /// <summary>
        /// 波束角度(度)
        /// </summary>
        public double RiverDischargeBeamAngle;
        /// <summary>
        /// 脉冲间隔(m)
        /// </summary>
        public double RiverDischargePulseLag;
        /// <summary>
        /// 脉冲长度(m)
        /// </summary>
        public double RiverDischargePulseLength;

    }

}
