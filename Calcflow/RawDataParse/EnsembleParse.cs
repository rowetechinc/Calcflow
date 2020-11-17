using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Calcflow;


namespace RawDataParse
{
    class EnsembleParse
    {
        public EnsembleParse()
        {
            HDRLEN = 0x20;
            PacketPointer = 0;
        }


        [StructLayout(LayoutKind.Explicit)]
        private struct TestUnion
        {
            // Fields
            [FieldOffset(0)]
            public byte A;
            [FieldOffset(1)]
            public byte B;
            [FieldOffset(2)]
            public byte C;
            [FieldOffset(3)]
            public byte D;
            [FieldOffset(0)]
            public float Float;
            [FieldOffset(0)]
            public int Int;
        }


        internal int PacketPointer;
        private TestUnion ByteArrayToNumber;
        internal int HDRLEN;

        internal const string VelocityID = "E000001\0";
        internal const string InstrumentID = "E000002\0";
        internal const string EarthID = "E000003\0";
        internal const string AmplitudeID = "E000004\0";
        internal const string CorrelationID = "E000005\0";
        internal const string BeamNID = "E000006\0";
        internal const string XfrmNID = "E000007\0";
        internal const string EnsembleDataID = "E000008\0";
        internal const string AncillaryID = "E000009\0";
        internal const string BottomTrackID = "E000010\0";
        internal const string NMEAID = "E000011\0";
        internal const string ProfileEngineeringdataID = "E000012\0";  //LPJ 2019-2-28
        internal const string BottomTrackEngineeringdataID = "E000013\0";  //LPJ 2019-2-28
        internal const string SystemSettingID = "E000014\0";  //LPJ 2015-8-14
        internal const string ProfileRangeTrackingID = "E000015\0";  //LPJ 2019-2-28
        internal const string GageDataID = "E000016\0";  //LPJ 2015-8-14


        internal const int MaxArray = 16;
        /*
        private byte ByteArrayToByte(byte[] packet)
        {
            return packet[PacketPointer++];
        }
        */
        private float ByteArrayToFloat(byte[] packet)
        {
            if (packet.Length > this.PacketPointer + 4)  //LPJ 2019-12-30
            {
                this.ByteArrayToNumber.A = packet[this.PacketPointer++];
                this.ByteArrayToNumber.B = packet[this.PacketPointer++];
                this.ByteArrayToNumber.C = packet[this.PacketPointer++];
                this.ByteArrayToNumber.D = packet[this.PacketPointer++];
            }
            return this.ByteArrayToNumber.Float;
        }
        private int ByteArrayToInt(byte[] packet)
        {
            if (packet.Length > this.PacketPointer + 4)  //LPJ 2019-12-30
            {
                this.ByteArrayToNumber.A = packet[this.PacketPointer++];
                this.ByteArrayToNumber.B = packet[this.PacketPointer++];
                this.ByteArrayToNumber.C = packet[this.PacketPointer++];
                this.ByteArrayToNumber.D = packet[this.PacketPointer++];
            }
            return this.ByteArrayToNumber.Int;
        }
        private string ByteArrayToString(byte[] packet, int len)
        {
            string s = "";
            for (int i = 0; i < len; i++)
            {
                s += ((char)packet[PacketPointer++]);
            }
            return s;
        }

        public void Parse(byte[] packet, ArrayClass m, int PacketSize)
        {
            int SizeCount = 0;
            int ArrayCount = 0;
            m.VelocityAvailable = false;
            m.InstrumentAvailable = false;
            m.EarthAvailable = false;
            m.AmplitudeAvailable = false;
            m.CorrelationAvailable = false;
            m.BeamNAvailable = false;
            m.XfrmNAvailable = false;
            m.EnsembleDataAvailable = false;
            m.AncillaryAvailable = false;
            m.BottomTrackAvailable = false;
            m.NmeaAvailable = false;
            int i = 0;
            PacketPointer = HDRLEN;
          
            while (i < MaxArray)
            {
                m.Type[i] = this.ByteArrayToInt(packet);
                m.Bins[i] = this.ByteArrayToInt(packet);
                m.Beams[i] = this.ByteArrayToInt(packet);
                m.Imag[i] = this.ByteArrayToInt(packet);
                m.NameLen[i] = this.ByteArrayToInt(packet);
                m.Name[i] = this.ByteArrayToString(packet, 8);
                ArrayCount = m.Bins[i] * m.Beams[i];
                SizeCount = this.PacketPointer;
              
                if (VelocityID.Equals(m.Name[i], StringComparison.Ordinal))
                {
                    m.VelocityAvailable = true;
                    for (int beam = 0; beam < m.Beams[i]; beam++)
                    {
                        for (int bin = 0; bin < m.Bins[i]; bin++)
                        {
                            m.Velocity[beam, bin] = this.ByteArrayToFloat(packet);
                        }
                    }

                }
                else if (InstrumentID.Equals(m.Name[i], StringComparison.Ordinal))
                {
                    m.InstrumentAvailable = true;
                    for (int beam = 0; beam < m.Beams[i]; beam++)
                    {
                        for (int bin = 0; bin < m.Bins[i]; bin++)
                        {
                            m.Instrument[beam, bin] = this.ByteArrayToFloat(packet);
                        }
                    }

                }
                else if (EarthID.Equals(m.Name[i], StringComparison.Ordinal))
                {
                    m.EarthAvailable = true;
                    for (int beam = 0; beam < m.Beams[i]; beam++)
                    {
                        for (int bin = 0; bin < m.Bins[i]; bin++)
                        {
                            m.Earth[beam, bin] = this.ByteArrayToFloat(packet);
                        }
                    }

                }
                else if (AmplitudeID.Equals(m.Name[i], StringComparison.Ordinal))
                {
                    m.AmplitudeAvailable = true;
                    for (int beam = 0; beam < m.Beams[i]; beam++)
                    {
                        for (int bin = 0; bin < m.Bins[i]; bin++)
                        {
                            m.Amplitude[beam, bin] = this.ByteArrayToFloat(packet);
                        }
                    }

                }
                else if (CorrelationID.Equals(m.Name[i], StringComparison.Ordinal))
                {
                    m.CorrelationAvailable = true;
                    for (int beam = 0; beam < m.Beams[i]; beam++)
                    {
                        for (int bin = 0; bin < m.Bins[i]; bin++)
                        {
                            m.Correlation[beam, bin] = this.ByteArrayToFloat(packet);
                        }
                    }

                }
                else if (BeamNID.Equals(m.Name[i], StringComparison.Ordinal))
                {
                    m.BeamNAvailable = true;
                    for (int beam = 0; beam < m.Beams[i]; beam++)
                    {
                        for (int bin = 0; bin < m.Bins[i]; bin++)
                        {
                            m.BeamN[beam, bin] = this.ByteArrayToInt(packet);
                        }
                    }

                }
                else if (XfrmNID.Equals(m.Name[i], StringComparison.Ordinal))
                {
                    m.XfrmNAvailable = true;
                    for (int beam = 0; beam < m.Beams[i]; beam++)
                    {
                        for (int bin = 0; bin < m.Bins[i]; bin++)
                        {
                            m.XfrmN[beam, bin] = this.ByteArrayToInt(packet);
                        }
                    }

                }
                else if (EnsembleDataID.Equals(m.Name[i], StringComparison.Ordinal))
                {
                    m.EnsembleDataAvailable = true;
                    m.E_EnsembleNumber = this.ByteArrayToInt(packet);
                    m.E_Cells = this.ByteArrayToInt(packet);
                    m.E_Beams = this.ByteArrayToInt(packet);
                    m.E_PingsInEnsemble = this.ByteArrayToInt(packet);
                    m.E_PingCount = this.ByteArrayToInt(packet);
                    m.E_Status = this.ByteArrayToInt(packet);
                    m.YYYY = this.ByteArrayToInt(packet);
                    m.MM = this.ByteArrayToInt(packet);
                    m.DD = this.ByteArrayToInt(packet);
                    m.HH = this.ByteArrayToInt(packet);
                    m.mm = this.ByteArrayToInt(packet);
                    m.SS = this.ByteArrayToInt(packet);
                    m.hsec = this.ByteArrayToInt(packet);

                    ///////////////2013-11-04  解析第22-23个字符以用于双频仪器中////////////////////
                    int j;
                    for (j = 0; j < 32; j++)
                    {
                        if (packet[PacketPointer] > 31 && packet[PacketPointer] < 127)
                            m.E_SN_Buffer[j] = packet[PacketPointer];
                        else
                            m.E_SN_Buffer[j] = 63;
                        PacketPointer++;
                    }
                    m.E_SN_Buffer[j] = 0;
                    for (j = 0; j < 4; j++)
                    {
                        m.E_FW_Vers[j] = packet[PacketPointer];
                        PacketPointer++;
                    }

                }
                else if (AncillaryID.Equals(m.Name[i], StringComparison.Ordinal))
                {
                    m.AncillaryAvailable = true;
                    m.A_FirstCellDepth = this.ByteArrayToFloat(packet);
                    m.A_CellSize = this.ByteArrayToFloat(packet);
                    m.A_FirstPingSeconds = this.ByteArrayToFloat(packet);
                    m.A_LastPingSeconds = this.ByteArrayToFloat(packet);
                    m.A_Heading = this.ByteArrayToFloat(packet);
                    m.A_Pitch = this.ByteArrayToFloat(packet);
                    m.A_Roll = this.ByteArrayToFloat(packet);
                    m.A_WaterTemperature = this.ByteArrayToFloat(packet);
                    m.A_BoardTemperature = this.ByteArrayToFloat(packet);
                    m.A_Salinity = this.ByteArrayToFloat(packet);
                    m.A_Pressure = this.ByteArrayToFloat(packet);
                    m.A_Depth = this.ByteArrayToFloat(packet);
                    m.A_SpeedOfSound = this.ByteArrayToFloat(packet);

                }
                else if (BottomTrackID.Equals(m.Name[i], StringComparison.Ordinal))
                {
                    m.BottomTrackAvailable = true;
                    m.B_FirstPingSeconds = this.ByteArrayToFloat(packet);
                    m.B_LastPingSeconds = this.ByteArrayToFloat(packet);
                    m.B_Heading = this.ByteArrayToFloat(packet);
                    m.B_Pitch = this.ByteArrayToFloat(packet);
                    m.B_Roll = this.ByteArrayToFloat(packet);
                    m.B_WaterTemperature = this.ByteArrayToFloat(packet);
                    m.B_BoardTemperature = this.ByteArrayToFloat(packet);
                    m.B_Salinity = this.ByteArrayToFloat(packet);
                    m.B_Pressure = this.ByteArrayToFloat(packet);
                    m.B_Depth = this.ByteArrayToFloat(packet);
                    m.B_SpeedOfSound = this.ByteArrayToFloat(packet);
                    m.B_Status = this.ByteArrayToFloat(packet);
                    m.B_Beams = this.ByteArrayToFloat(packet);
                    m.B_PingCount = this.ByteArrayToFloat(packet);
                    for (int beam = 0; beam < m.B_Beams; beam++)
                    {
                        m.B_Range[beam] = this.ByteArrayToFloat(packet);
                    }
                    for (int beam = 0; beam < m.B_Beams; beam++)
                    {
                        m.B_SNR[beam] = this.ByteArrayToFloat(packet);
                    }
                    for (int beam = 0; beam < m.B_Beams; beam++)
                    {
                        m.B_Amplitude[beam] = this.ByteArrayToFloat(packet);
                    }
                    for (int beam = 0; beam < m.B_Beams; beam++)
                    {
                        m.B_Correlation[beam] = this.ByteArrayToFloat(packet);
                    }
                    for (int beam = 0; beam < m.B_Beams; beam++)
                    {
                        m.B_Velocity[beam] = this.ByteArrayToFloat(packet);
                    }
                    for (int beam = 0; beam < m.B_Beams; beam++)
                    {
                        m.B_BeamN[beam] = this.ByteArrayToFloat(packet);
                    }
                    for (int beam = 0; beam < m.B_Beams; beam++)
                    {
                        m.B_Instrument[beam] = this.ByteArrayToFloat(packet);
                    }
                    for (int beam = 0; beam < m.B_Beams; beam++)
                    {
                        m.B_XfrmN[beam] = this.ByteArrayToFloat(packet);
                    }
                    for (int beam = 0; beam < m.B_Beams; beam++)
                    {
                        m.B_Earth[beam] = this.ByteArrayToFloat(packet);
                    }
                    for (int beam = 0; beam < m.B_Beams; beam++)
                    {
                        m.B_EarthN[beam] = this.ByteArrayToFloat(packet);
                    }

                }
                else if (NMEAID.Equals(m.Name[i], StringComparison.Ordinal))
                {
                    m.NmeaAvailable = true;
                    int j = 0;
                    while (packet[this.PacketPointer] != 0)
                    {
                        //m.NMEA_Buffer[j++] = packet[this.PacketPointer];
                        m.NMEA_Buffer[j++] = packet[this.PacketPointer++]; //LPJ 2013-7-8
                        if (j >= 0x2000)
                        {
                            break;
                        }
                    }
                    for (int end = j; end < 0x2000; end++)
                    {
                        m.NMEA_Buffer[end] = 0;
                    }

                }
                else if (ProfileEngineeringdataID.Equals(m.Name[i], StringComparison.Ordinal)) //LPJ 2019-2-28
                {
                    for (int k = 0; k < 4; k++)
                    {
                        m.PrePingVel[k] = this.ByteArrayToFloat(packet);
                    }
                    for (int k = 0; k < 4; k++)
                    {
                        m.PrePingCor[k] = this.ByteArrayToFloat(packet);
                    }
                    for (int k = 0; k < 4; k++)
                    {
                        m.PrePingAmp[k] = this.ByteArrayToFloat(packet);
                    }

                    m.ProfileSamplesPerSecond = this.ByteArrayToFloat(packet);
                    m.ProfileSystemFreqHz = this.ByteArrayToFloat(packet);
                    m.ProfileLagSamples = this.ByteArrayToFloat(packet);
                    m.ProfileCPCE = this.ByteArrayToFloat(packet);
                    m.ProfileNCE = this.ByteArrayToFloat(packet);
                    m.ProfileRepeatN = this.ByteArrayToFloat(packet);
                    m.PrePingGap = this.ByteArrayToFloat(packet);
                    m.PrePingNCE = this.ByteArrayToFloat(packet);
                    m.PrePingRepeatN = this.ByteArrayToFloat(packet);
                    m.PrePingLagSamples = this.ByteArrayToFloat(packet);
                    m.TRhighGain = this.ByteArrayToFloat(packet);

                }
                else if (BottomTrackEngineeringdataID.Equals(m.Name[i], StringComparison.Ordinal)) //LPJ 2019-2-28
                {
                    m.BottomTrackSamplesPerSecond = this.ByteArrayToFloat(packet);
                    m.BottomTrackSystemFreqHz = this.ByteArrayToFloat(packet);
                    m.BottomTrackLagSamples = this.ByteArrayToFloat(packet);
                    m.BottomTrackCPCE = this.ByteArrayToFloat(packet);
                    m.BottomTrackNCE = this.ByteArrayToFloat(packet);
                    m.BottomTrackRepeatN = this.ByteArrayToFloat(packet);
                    for (int k = 0; k < 4; k++)
                    {
                        m.AmbHz[k] = this.ByteArrayToFloat(packet);
                    }
                    for (int k = 0; k < 4; k++)
                    {
                        m.AmbVel[k] = this.ByteArrayToFloat(packet);
                    }
                    for (int k = 0; k < 4; k++)
                    {
                        m.AmbAmp[k] = this.ByteArrayToFloat(packet);
                    }
                    for (int k = 0; k < 4; k++)
                    {
                        m.AmbCor[k] = this.ByteArrayToFloat(packet);
                    }
                    for (int k = 0; k < 4; k++)
                    {
                        m.AmbSNR[k] = this.ByteArrayToFloat(packet);
                    }
                    for (int k = 0; k < 4; k++)
                    {
                        m.LagUsed[k] = this.ByteArrayToFloat(packet);
                    }

                }
                else if (SystemSettingID.Equals(m.Name[i], StringComparison.Ordinal))  //LPJ 2015-8-14 system Setting
                {
                    m.BT_SamplesPerSecond = this.ByteArrayToFloat(packet); //LPJ 2015-9-14 
                    m.BT_SystemFreqHz = this.ByteArrayToFloat(packet);
                    m.BT_CyclesPerCodeElement = this.ByteArrayToFloat(packet);
                    m.BT_NumberOfCodeElementsIncode = this.ByteArrayToFloat(packet);
                    m.BT_NumberOfRepeatedCodes = this.ByteArrayToFloat(packet);

                    m.WP_SamplesPerSecond = this.ByteArrayToFloat(packet);
                    m.WP_SystemFreqHz = this.ByteArrayToFloat(packet);
                    m.WP_CyclesPerCodeElement = this.ByteArrayToFloat(packet);
                    m.WP_NumberOfCodeElementsIncode = this.ByteArrayToFloat(packet);
                    m.WP_NumberOfRepeatedCodes = this.ByteArrayToFloat(packet);
                    m.WP_NumberOfSamplesInLag = this.ByteArrayToFloat(packet);

                    m.SystemInputVoltage = this.ByteArrayToFloat(packet);
                    m.TransmitterBoostVoltage = this.ByteArrayToFloat(packet);

                    m.BT_Mode = this.ByteArrayToFloat(packet);
                    m.BT_PulseToPulseLag = this.ByteArrayToFloat(packet);
                    m.BT_LongRangeSwitchDepth = this.ByteArrayToFloat(packet);
                    m.BT_BeamMultiplex = this.ByteArrayToFloat(packet);

                    m.WP_Mode = this.ByteArrayToFloat(packet);
                    m.WP_Lag = this.ByteArrayToFloat(packet);

                    m.WP_TransmitBandwith = this.ByteArrayToFloat(packet);
                    m.WP_ReceiveBandwidth = this.ByteArrayToFloat(packet);

                }
                else if (ProfileRangeTrackingID.Equals(m.Name[i], StringComparison.Ordinal)) //LPJ 2019-2-28
                {
                    m.RT_NumberOfBeams = this.ByteArrayToFloat(packet);

                    for (int beam = 0; beam < m.RT_NumberOfBeams; beam++)
                    {
                        m.RT_SNR[beam] = this.ByteArrayToFloat(packet);
                    }
                    for (int beam = 0; beam < m.RT_NumberOfBeams; beam++)
                    {
                        m.RT_Range[beam] = this.ByteArrayToFloat(packet);
                    }
                    for (int beam = 0; beam < m.RT_NumberOfBeams; beam++)
                    {
                        m.RT_NumberOfPings[beam] = this.ByteArrayToFloat(packet);
                    }
                    for (int beam = 0; beam < m.RT_NumberOfBeams; beam++)
                    {
                        m.RT_Amp[beam] = this.ByteArrayToFloat(packet);
                    }
                    for (int beam = 0; beam < m.RT_NumberOfBeams; beam++)
                    {
                        m.RT_Corr[beam] = this.ByteArrayToFloat(packet);
                    }
                    for (int beam = 0; beam < m.RT_NumberOfBeams; beam++)
                    {
                        m.RT_Velocity[beam] = this.ByteArrayToFloat(packet);
                    }
                    for (int beam = 0; beam < m.RT_NumberOfBeams; beam++)
                    {
                        m.RT_InsVelocity[beam] = this.ByteArrayToFloat(packet);
                    }
                    for (int beam = 0; beam < m.RT_NumberOfBeams; beam++)
                    {
                        m.RT_EarthVelocity[beam] = this.ByteArrayToFloat(packet);
                    }

                }
                else if (GageDataID.Equals(m.Name[i], StringComparison.Ordinal)) //LPJ 2015-8-14 gage data
                {
                    m.Status = this.ByteArrayToFloat(packet);
                    m.AverageRange = this.ByteArrayToFloat(packet);
                    m.SinglePingStd_Range = this.ByteArrayToFloat(packet);
                    m.AverageSN = this.ByteArrayToFloat(packet);
                    m.No_Pings_SN_Threshold = this.ByteArrayToFloat(packet);
                    m.Salinity = this.ByteArrayToFloat(packet);
                    m.Pressure = this.ByteArrayToFloat(packet);
                    m.Depth_PressureSensor = this.ByteArrayToFloat(packet);
                    m.WaterTemp = this.ByteArrayToFloat(packet);
                    m.BackPlaneTemp = this.ByteArrayToFloat(packet);
                    m.SpeedSound_Water = this.ByteArrayToFloat(packet);
                    m.Heading = this.ByteArrayToFloat(packet);
                    m.Pitch = this.ByteArrayToFloat(packet);
                    m.Roll = this.ByteArrayToFloat(packet);
                    m.AverageSurface_EchoLevel = this.ByteArrayToFloat(packet);
                    m.AverageNoiseLevelPriorToSurfaceEcho = this.ByteArrayToFloat(packet);
                    m.AverageNoiseLevelAfterSurfaceEcho = this.ByteArrayToFloat(packet);
                    m.HighGain = this.ByteArrayToFloat(packet);
                    m.DesiredPingsParameter = this.ByteArrayToFloat(packet);
                    m.SN_ThresholdParameter = this.ByteArrayToFloat(packet);
                    m.SignalLevel = this.ByteArrayToFloat(packet);
                    m.SinglePingRange_Std = this.ByteArrayToFloat(packet);
                    m.DesiredNoTransmitCarrierCycles = this.ByteArrayToFloat(packet);

                }

                SizeCount = (this.PacketPointer - SizeCount) / GetDataTypeSize(m.Type[i]); //LPJ 2019-4-8
                if (SizeCount != ArrayCount)
                {
                    this.PacketPointer += GetDataTypeSize(m.Type[i]) * (ArrayCount - SizeCount); //LPJ 2019-4-8
                }

                //SizeCount = (this.PacketPointer - SizeCount) / 4;
                //if (SizeCount != ArrayCount)
                //{
                //    this.PacketPointer += 4 * (ArrayCount - SizeCount);
                //}
                if ((this.PacketPointer + 4) >= PacketSize)
                {
                    break;
                }
                i++;
            }
            m.nArray = i + 1;
            if (i >= 16)
            {
                m.nArray = 16;
            }
            if (m.E_Cells < 1L)
            {
                m.E_Cells = 1L;
            }
            this.PacketPointer = 0;
        }

        private int GetDataTypeSize(int type)
        {
            int dataType;
            switch (type)
            {
                case 50:
                    dataType = 1;
                    break;
                case 20:
                    dataType = 4;
                    break;
                case 10:
                    dataType = 4;
                    break;
                default:
                    dataType = 4;
                    break;
            }
            return dataType;
        }

    }
}
