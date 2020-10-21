using System;
using System.Collections.Generic;
using System.Text;

namespace RawDataParse
{
    class CRC16
    {
        private CRC16()
        {
        }

        internal static ushort Calculate(byte[] buffer, int index, int count)
        {
            ushort crc = 0;

            for (int i = index; i < index + count; i++)
            {
                crc = (ushort)((byte)(crc >> 8) | (crc << 8));
                crc ^= buffer[i];
                crc ^= (byte)((crc & 0xff) >> 4);
                crc ^= (ushort)((crc << 8) << 4);
                crc ^= (ushort)(((crc & 0xff) << 4) << 1);
            }

            ushort csum = crc;
            return csum;
        }
    }
}
