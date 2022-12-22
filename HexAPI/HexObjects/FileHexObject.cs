using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YYHEggEgg.HexAPI
{
    /// <summary>
    /// A HexObject from file.
    /// </summary>
    public class FileHexObject : HexObject
    {
        public FileHexObject(string filePath, bool ronly = false, 
            int bufsize = 4096, int bufcount = 16)
        {
            file = new FileInfo(filePath);
            ReadOnly = ronly;
            buffersHelper = new(Length, GetRange, bufsize, bufcount);
        }

        protected FileInfo file;

        #region Basic File Stream Handle
        private RandomReadBuffersHelper buffersHelper;

        public override byte this[Int64 index] { get => buffersHelper[index]; set => throw new NotImplementedException(); }
        #endregion

        public override long Length { get => file.Length; protected set => throw new NotImplementedException(); }

        public override byte[] GetRange(long start, long length)
        {
            if (length > int.MaxValue)
                throw new NotSupportedException(
                    "Get bytes >= 4GiB at a time is not currently supported.");

            Assert.ArgumentOutOfRange(start, Length, start + length);

            byte[] rtn = new byte[length];
            using (BinaryReader reader = new BinaryReader(new FileStream(file.FullName, FileMode.Open)))
            {
                reader.BaseStream.Seek(start, SeekOrigin.Begin);
                reader.Read(rtn, 0, (int)length);
            }
            return rtn;
        }

        public override void InsertRange(long start, byte[] bytes, long length)
        {
            throw new NotImplementedException();
        }

        public override void OverWriteRange(long start, byte[] bytes, long length)
        {
            throw new NotImplementedException();
        }

        public override void RemoveRange(long start, long length)
        {
            throw new NotImplementedException();
        }
    }


}
