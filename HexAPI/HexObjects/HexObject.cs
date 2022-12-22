using System.Runtime.CompilerServices;

namespace YYHEggEgg.HexAPI
{
    /// <summary>
    /// A universal standard for HEX readable & editable(?) instances.
    /// </summary>
    public abstract class HexObject
    {
        public bool ReadOnly { get; protected set; }
        public abstract Int64 Length { get; protected set; }

        #region Basic I/O
        public abstract byte this[Int64 index] { get; set; }

        /* Not Ready for using
        /// <summary>
        /// Insert a byte to the instance.
        /// </summary>
        public abstract void Insert(Int64 location, byte value);
        // Note: 
        //  assert start >= 0
        //  assert !ReadOnly

        /// <summary>
        /// Remove the byte at a certain location.
        /// </summary>
        public abstract void Remove(Int64 location);
        // Note:
        //  assert start >= 0
        //  assert !ReadOnly
        */
        #endregion

        #region Range Operation
        /// <summary>
        /// Get Bytes from start.
        /// </summary>
        /// <param name="start">The location start to read</param>
        /// <param name="length">The length of bytes</param>
        /// <returns>A byte[length] (HexObject[start + i] = rtn[i])</returns>
        public abstract byte[] GetRange(Int64 start, Int64 length);
        // Note: 
        //  assert start >= 0
        //  assert start + length < Length 

        /// <summary>
        /// Overwrite Bytes to the instance.
        /// </summary>
        /// <param name="start">The location start to overwrite</param>
        /// <param name="bytes">A byte[length] (HexObject[start + i] = bytes[i])</param>
        /// <param name="length">The length of bytes</param>
        public abstract void OverWriteRange(Int64 start, byte[] bytes, Int64 length);
        // Note: 
        //  assert start >= 0
        //  assert start + length < Length 
        //  assert bytes.Length == length
        //  assert !ReadOnly

        /// <summary>
        /// Insert Bytes to the instance.
        /// </summary>
        /// <param name="start">The location start to overwrite</param>
        /// <param name="bytes">A byte[length] (the new HexObject[start + i] = bytes[i])</param>
        /// <param name="length">The length of bytes</param>
        public abstract void InsertRange(Int64 start, byte[] bytes, Int64 length);
        // Note: 
        //  assert start >= 0
        //  assert bytes.Length == length
        //  assert !ReadOnly

        /// <summary>
        /// Remove Bytes from the location.
        /// </summary>
        /// <param name="start">The location start to overwrite</param>
        /// <param name="length">The length of deleting</param>
        public abstract void RemoveRange(Int64 start, Int64 length);
        // Note: 
        //  assert start >= 0
        //  assert start + length < Length 
        //  assert !ReadOnly
        #endregion

        /// <summary>
        /// Find all appear locations [content] have in the HexObject. Based on KMP Algorithm.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public List<Int64> Find(byte[] content)
        {
            int content_len = content.Length;
            Int64 obj_len = Length;

            #region nxt array init
            int[] nxt = new int[content.Length + 1];
            int i = 0, j = -1;
            nxt[0] = -1;

            while (i < content_len)
            {
                if (j == -1 || content[i] == content[j]) nxt[++i] = ++j;
                else j = nxt[j];
            }
            #endregion

            List<Int64> indexes = new();

            #region KMP
            int k = 0, m = 0;
            while (k < obj_len)
            {
                if (m == -1 || this[k] == content[m])
                {
                    k++; m++;
                }
                else m = nxt[m];

                if (m == content_len) 
                {
                    indexes.Add(k - content_len);
                    m = nxt[m];
                }
            }
            #endregion

            return indexes;
        }
    }
}