using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Calcflow;

namespace RawDataParse
{
    class EnsembleBinaryProcess
    {
        private EnsembleBinaryProcess()
        {
        }

        internal static List<ArrayClass> Ensembles = new List<ArrayClass>();

        private static EnsembleParse parser = new EnsembleParse();

        internal static void Process(byte[] pack)
        {
            Ensembles.Clear();

            EnsemblePick.Process(pack);

            for (int i = 0; i < EnsemblePick.EnsemblePackets.Count; i++)
            {
                byte[] packet = EnsemblePick.EnsemblePackets[i];

                int payloadLen = EnsemblePick.GetPayloadLength(packet);
                ushort checkSum = CRC16.Calculate(packet, EnsemblePick.ENSEMBLE_HEADER_LENGTH, payloadLen);
                int copyCheckSum = BitConverter.ToInt32(packet, EnsemblePick.ENSEMBLE_HEADER_LENGTH + payloadLen);

                int number = BitConverter.ToInt32(packet, 16);

                if (checkSum == copyCheckSum)
                {
                    ArrayClass m = new ArrayClass();
                    try
                    {                        
                        parser.Parse(packet, m, packet.Length);
                    }
                    catch(Exception ex)
                    {
                        TextWriter output = Console.Out;
                        output.WriteLine("Warning: Ensemble Packet Number {0} parse failed.", number.ToString("D7"));
                        output.WriteLine("    {0}", ex.Message);
                        output.Flush();
                        continue;
                    }
                    Ensembles.Add(m);
                }
                else
                {
                    TextWriter output = Console.Out;
                    output.WriteLine("Warning: Ensemble Packet Number {0} CRC16 check failed.", number.ToString("D7"));
                    output.Flush();

                    // Save the raw data
                    //StreamWriter sw = new StreamWriter(number.ToString("D7") + " failed.bin", false);
                    //foreach (byte b in packet)
                    //{
                    //    sw.Write(b);
                    //}
                    //sw.Flush();
                    //sw.Close();
                }
            }

        }
    }
}
