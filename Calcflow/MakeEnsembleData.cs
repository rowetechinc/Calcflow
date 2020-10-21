using System;
using System.Collections.Generic;
using System.Text;

namespace Calcflow
{
   internal class MakeEnsembleData
   {
       public static ArrayClass MakeData(EnsembleDataDesignParam param)
       {
           ArrayClass data = new ArrayClass();
          
           //E000008
           data.E_EnsembleNumber = param.TemplateData.E_EnsembleNumber;
           data.E_Cells = param.TemplateData.E_Cells;
           data.E_Beams = param.TemplateData.E_Beams;
           data.E_PingCount = param.TemplateData.E_PingCount;
           data.E_Status = 0;
           data.YYYY = DateTime.Now.Year;
           data.MM = DateTime.Now.Month;
           data.DD = DateTime.Now.Day;
           data.HH = DateTime.Now.Hour;
           data.mm = DateTime.Now.Minute;
           data.SS = DateTime.Now.Second;
           data.hsec = DateTime.Now.Millisecond / 10;
           //其他
           for (int i = 0; i<data.Bins.Length;i++ )
           {
               data.Bins[i] = (int)data.E_Cells;
           }
           for (int i = 0; i < data.Beams.Length;i++ )
           {
               data.Beams[i] = (int)data.E_Beams;
           }
           data.AmplitudeAvailable = true;
           data.AncillaryAvailable = true;
           data.BottomTrackAvailable = true;
           data.CorrelationAvailable = true;
           data.EarthAvailable = true;
           data.EnsembleDataAvailable = true;
           data.InstrumentAvailable = true;
           data.NmeaAvailable = true;
           data.VelocityAvailable = true;
           data.XfrmNAvailable = true;
          
           Random random = new Random();
           for (int i = 0;i<data.E_Cells;i++)
           {
             
               //E000002
               data.Instrument[0, i] = (float)(Math.Sin(Math.PI / 4) * param.ReferEarthVelocity * (1 + random.NextDouble() * param.NoiseSize));
               data.Instrument[1, i] = (float)(Math.Cos(Math.PI / 4) * param.ReferEarthVelocity * (1 + random.NextDouble() * param.NoiseSize));
               data.Instrument[2, i] = 0;
               data.Instrument[3, i] = 0;

               //E000001
               data.Velocity[0, i] = 0;
               data.Velocity[1, i] = data.Instrument[0, i] * 2 * Math.Sign(param.BeamAngle / 180.0 * Math.PI);
               data.Velocity[2, i] = 0;
               data.Velocity[3, i] = data.Instrument[1, i] * 2 * Math.Sign(param.BeamAngle / 180.0 * Math.PI); ;

               //E000003
               data.Earth[0, i] = data.Instrument[0, i];
               data.Earth[1, i] = data.Instrument[1, i];
               data.Earth[2, i] = 0;
               data.Earth[3, i] = 0;
               //E000004
               double a = Math.Sin(Math.PI / 4 + i / (double)data.E_Cells * Math.PI * 3 / 4);
               data.Amplitude[0, i] = (float)(a * param.ReferAmplitude * (1 + random.NextDouble() * param.NoiseSize));
               data.Amplitude[1, i] = (float)(a * param.ReferAmplitude * (1 + random.NextDouble() * param.NoiseSize));
               data.Amplitude[2, i] = (float)(a * param.ReferAmplitude * (1 + random.NextDouble() * param.NoiseSize));
               data.Amplitude[3, i] = (float)(a * param.ReferAmplitude * (1 + random.NextDouble() * param.NoiseSize));
               //E000005
               data.Correlation[0, i] = (float)(param.ReferCorrelation * (1 + random.NextDouble() * param.NoiseSize));
               data.Correlation[1, i] = (float)(param.ReferCorrelation * (1 + random.NextDouble() * param.NoiseSize));
               data.Correlation[2, i] = (float)(param.ReferCorrelation * (1 + random.NextDouble() * param.NoiseSize));
               data.Correlation[3, i] = (float)(param.ReferCorrelation * (1 + random.NextDouble() * param.NoiseSize));
               //E000006
               data.BeamN[0, i] = (int)param.TemplateData.E_EnsembleNumber;
               data.BeamN[1, i] = (int)param.TemplateData.E_EnsembleNumber;
               data.BeamN[2, i] = (int)param.TemplateData.E_EnsembleNumber;
               data.BeamN[3, i] = (int)param.TemplateData.E_EnsembleNumber;
               //E000007
               data.XfrmN[0, i] = (int)param.TemplateData.E_EnsembleNumber;
               data.XfrmN[1, i] = (int)param.TemplateData.E_EnsembleNumber;
               data.XfrmN[2, i] = (int)param.TemplateData.E_EnsembleNumber;
               data.XfrmN[3, i] = (int)param.TemplateData.E_EnsembleNumber;
           }
           //E000009
           data.A_FirstCellDepth = param.ReferDepth / data.E_Cells;
           data.A_CellSize = data.A_FirstCellDepth;
           data.A_FirstPingSeconds = param.TemplateData.A_FirstPingSeconds;
           data.A_LastPingSeconds = param.TemplateData.A_LastPingSeconds;
           data.A_Heading = 0;
           data.A_Pitch = 0;
           data.A_Roll = 0;
           data.A_WaterTemperature = param.TemplateData.A_WaterTemperature;
           data.A_BoardTemperature = param.TemplateData.A_BoardTemperature;
           data.A_Salinity = param.TemplateData.A_Salinity;
           data.A_Pressure = param.TemplateData.A_Pressure;
           data.A_Depth = param.TemplateData.A_Depth;
           data.A_SpeedOfSound = param.TemplateData.A_SpeedOfSound;

           //E000010
           data.B_FirstPingSeconds = param.TemplateData.B_FirstPingSeconds;
           data.B_LastPingSeconds = param.TemplateData.B_LastPingSeconds;
           data.B_Heading = 0;
           data.B_Pitch = 0;
           data.B_Roll = 0;
           data.B_WaterTemperature = param.TemplateData.B_WaterTemperature;
           data.B_BoardTemperature = param.TemplateData.B_BoardTemperature;
           data.B_Salinity = param.TemplateData.B_Salinity;
           data.B_Pressure = param.TemplateData.B_Pressure;
           data.B_Depth = data.A_CellSize * data.E_Cells;
           data.B_SpeedOfSound = param.TemplateData.B_SpeedOfSound;
           data.B_Status = 0;
           data.B_Beams = param.TemplateData.B_Beams;
           data.B_PingCount = param.TemplateData.B_PingCount;
           data.B_Range[0] = (float)(data.B_Depth * (1 + random.NextDouble() * param.NoiseSize));
           data.B_Range[1] = (float)(data.B_Depth * (1 + random.NextDouble() * param.NoiseSize));
           data.B_Range[2] = (float)(data.B_Depth * (1 + random.NextDouble() * param.NoiseSize));
           data.B_Range[3] = (float)(data.B_Depth * (1 + random.NextDouble() * param.NoiseSize));
           data.B_SNR[0] = 0;
           data.B_SNR[1] = 0;
           data.B_SNR[2] = 0;
           data.B_SNR[3] = 0;
           data.B_Amplitude[0] = param.ReferAmplitude;
           data.B_Amplitude[1] = param.ReferAmplitude;
           data.B_Amplitude[2] = param.ReferAmplitude;
           data.B_Amplitude[3] = param.ReferAmplitude;
           data.B_Correlation[0] = param.ReferCorrelation;
           data.B_Correlation[1] = param.ReferCorrelation;
           data.B_Correlation[2] = param.ReferCorrelation;
           data.B_Correlation[3] = param.ReferCorrelation;
           data.B_Instrument[0] =(float)(Math.Sin(Math.PI / 4) * param.ReferEarthVelocity * (1 + random.NextDouble() * param.NoiseSize));
           data.B_Instrument[1] = (float)(-Math.Cos(Math.PI / 4) * param.ReferEarthVelocity * (1 + random.NextDouble() * param.NoiseSize));
           data.B_Instrument[2] = 0;
           data.B_Instrument[3] = 0;
           data.B_Velocity[0] = 0;
           data.B_Velocity[1] = data.B_Instrument[0] * 2 * Math.Sign(param.BeamAngle / 180.0 * Math.PI);
           data.B_Velocity[2] = data.B_Instrument[1] * 2 * Math.Sign(param.BeamAngle / 180.0 * Math.PI); ;
           data.B_Velocity[3] = 0;
           data.B_Earth[0] = data.B_Instrument[0];
           data.B_Earth[1] = data.B_Instrument[1];
           data.B_Earth[2] = 0;
           data.B_Earth[3] = 0;
           data.B_BeamN[0] = data.B_PingCount;
           data.B_BeamN[1] = data.B_PingCount;
           data.B_BeamN[2] = data.B_PingCount;
           data.B_BeamN[3] = data.B_PingCount;
           data.B_XfrmN[0] = data.B_PingCount;
           data.B_XfrmN[1] = data.B_PingCount;
           data.B_XfrmN[2] = data.B_PingCount;
           data.B_XfrmN[3] = data.B_PingCount;
           data.B_EarthN[0] = data.B_PingCount;
           data.B_EarthN[1] = data.B_PingCount;
           data.B_EarthN[2] = data.B_PingCount;
           data.B_EarthN[3] = data.B_PingCount;

           return data;
       }
    
   }
}
