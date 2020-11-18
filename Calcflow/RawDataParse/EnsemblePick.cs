using System;
using System.Collections.Generic;
using System.Text;

namespace RawDataParse
{
    class EnsemblePick
    {

        private EnsemblePick()
        {
        }

        internal static List<byte[]> EnsemblePackets = new List<byte[]>(); 

        // Ensemble 标志头
        private static readonly byte[] ENSEMBLE_HEADER = new byte[] { 0x80, 0x80, 0x80, 0x80, 
                                                                      0x80, 0x80, 0x80, 0x80,
                                                                      0x80, 0x80, 0x80, 0x80, 
                                                                      0x80, 0x80, 0x80, 0x80};
        internal const int ENSEMBLE_HEADER_LENGTH = 0x20;
        private static List<byte> BytesArray = new List<byte>();
        private static int payloadLen = 0;
        private static int _payloadLen = 0;

        //子串查找
        private static int FindBytes(List<byte> source, byte[] pattern, int from_here)
        {
            // 保证子串长度不大于源字节串索引开始序列
            if (pattern.Length > (source.Count - from_here))
                return -1;
            int i = from_here;
            int j = 0;
            while ((i + j < source.Count) && (j < pattern.Length))
            {
                if (source[i + j] == pattern[j])
                    j++;
                else
                {
                    i++;
                    j = 0;
                }
            }
            if (j == pattern.Length)
                return i;
            else
                return -1;
        }

        //查找Ensemble数据头索引位置
        private static int FindEnsembleHeader(List<byte> source, int index)
        {
            return FindBytes(source, ENSEMBLE_HEADER, index);
        }
        //获取长度，返回有效性
        private static bool VerifyAndGetLength(List<byte> source)
        {
            byte[] Lng = new byte[4];
            byte[] _Lng = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                Lng[i] = (byte)source[0x18 + i];
                _Lng[i] = (byte)source[0x1c + i];
            }
            payloadLen = BitConverter.ToInt32(Lng, 0);
            _payloadLen = BitConverter.ToInt32(_Lng, 0);

            if ((payloadLen <= 0) || (((payloadLen + 1) + _payloadLen) != 0))
            {
                payloadLen = 0;
                _payloadLen = 0;
                return false;
            }
            else
            {
                return true;
            }
        }
        // 返回一个Ensemble Packet 的 payLoad 部分长度
        internal static int GetPayloadLength(byte[] packet)
        {
            byte[] Lng = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                Lng[i] = (byte)packet[0x18 + i];
            }
            return BitConverter.ToInt32(Lng, 0);
        }

        internal static void Process(byte[] pack)
        {
            EnsemblePackets.Clear();

            BytesArray.AddRange(pack);

            int index = 0;
            int header;// = -1;
            while ((header = FindEnsembleHeader(BytesArray, index)) != -1)
            {
                index = 0;
                if (header > 0)
                {
                    BytesArray.RemoveRange(0, header);
                }

                if (BytesArray.Count < ENSEMBLE_HEADER_LENGTH)
                {
                    return;
                }

                // 长度校验不正确，寻找下一头位置
                if (!VerifyAndGetLength(BytesArray))
                {
                    index = 1;
                    continue;
                }

                int end = ENSEMBLE_HEADER_LENGTH + payloadLen + 4;
                //数据区是否包含有完整的一个Ensemble数据包
                if (BytesArray.Count < end)
                {
                    return;
                }
                
                byte[] packet = new byte[end];
                BytesArray.CopyTo(0, packet, 0, end);
                BytesArray.RemoveRange(0, end);
             
                EnsemblePackets.Add(packet);
            }            

        }
    }


}
