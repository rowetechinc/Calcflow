using System;
using System.Collections.Generic;
using System.Text;
using RawDataParse;
using System.IO;

namespace Calcflow
{
    /// <summary>
    /// 读取RTI数据
    /// </summary>
    internal class ReadRTIData
    {
        /// <summary>
        /// 读取Raw数据
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>ArrayClass集合</returns>
        public static ArrayClass[] ReadRawData(string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            int length = (int)fs.Length;
            byte[] content = new byte[length];
            fs.Read(content, 0, length);
            EnsembleBinaryProcess.Process(content);
            return EnsembleBinaryProcess.Ensembles.ToArray();
        }
    }
}
