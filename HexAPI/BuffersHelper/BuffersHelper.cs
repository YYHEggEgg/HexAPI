using YYHEggEgg.HexAPI.OutputHelper;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;

namespace YYHEggEgg.HexAPI
{
    internal partial class RandomReadBuffersHelper
    {
        /// <summary>
        /// Initializer
        /// </summary>
        /// <param name="function">A function that takes parameter (start, length), and returns a byte array. Please refer to the documents. </param>
        /// <param name="bufsize"></param>
        /// <param name="bufcount"></param>
        public RandomReadBuffersHelper(Int64 length, Func<Int64, Int64, byte[]> function, int bufsize = 4096, int maxbufcount = 16)
        {
            Length = length;
            GetBytes = function;
            this.bufsize = bufsize;
            this.maxbufcount = maxbufcount;
            buffers = new();
            buffer_limit = new();
            recent_used = null;
        }

        public Int64 Length { get; private set; }

        private int bufsize, bufcount;
        private readonly int maxbufcount;
        private readonly Func<Int64, Int64, byte[]> GetBytes;

        // Int64 refers to Buffer.offset, for binary search
        private SortedList<Int64, ReadOnlyBuffer> buffers;

        // If the count of buffers get out of the limitation, the oldest buffer should be disposed
        // Int64 refers to Buffer.offset, same as in [buffers]
        private List<ReadOnlyBuffer> buffer_limit;

        #region Global Timestamp
        private Int64 _global_buffer_timestamp;
        /// <summary>
        /// The timestamp instance. Notice that it will increase after each access.
        /// </summary>
        public Int64 Global_Timestamp
        {
            get
            {
                // return current timestamp then add it
                return _global_buffer_timestamp++;
            }
        }
        #endregion

        private ReadOnlyBuffer? recent_used;
        public byte this[Int64 index]
        {
            get
            {
                ReadOnlyBuffer demandedbuf;

                if (recent_used != null && 
                    recent_used.offset <= index && index < recent_used.offset + bufsize)
                {
                    demandedbuf = recent_used;
                }
                else demandedbuf = GetBuffer(index);

                Assert.ArgumentOutOfRange(demandedbuf.offset, demandedbuf.offset + bufsize - 1, index,
                    "offset", "offset + bufsize");

                return demandedbuf.GetByte(index, Global_Timestamp);
            }
        }

        #region Get / Allocate Buffer
        /// <summary>
        /// Get a buffer that contains needed byte. If there's no match, create it.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private ReadOnlyBuffer GetBuffer(Int64 index)
        {
            if (bufcount == 0)
            {
                return AllocateNewBuffer(index);
            }

            // Binary search isn't needed when the count <= 16 (need more performace test)
            if (bufcount <= 16)
            {
                foreach (var buf in buffer_limit)
                {
                    if (buf.offset <= index && index < buf.offset + bufsize)
                    {
                        return buf;
                    }
                }
            }
            else
            {
                int l = 0, r = bufcount;
                while (l < r)
                {
                    int mid = (l + r) / 2;
                    var buf = buffers.ElementAt(mid).Value;
                    if (buf.offset <= index && index < buf.offset + bufsize)
                    {
                        return buf;
                    }
                    else if (index < buf.offset) r = mid;
                    else l = mid + 1;
                }
                return buffers.ElementAt(l).Value;
            }

            return AllocateNewBuffer(index);
        }

        private ReadOnlyBuffer AllocateNewBuffer(Int64 index)
        {
            // Remove the oldest buffer
            if (buffer_limit.Count >= maxbufcount)
            {
                buffer_limit.Sort();

                var oldest = buffer_limit[0];
                buffer_limit.RemoveAt(0);
                buffers.Remove(oldest.offset);
                bufcount--;
            }

            /* We use the following rule to create a buffer:
             * request for [index], required buffer size = [bufsize],
             * so build a buffer from [index - bufsize / 2] to [index + bufsize / 2 - 1]
             * e.g. index = 8192, bufsize = 4096,
             *      Buffer.offset = index - bufsize / 2
             */

            Int64 newstart = index - bufsize / 2;
            #region Border Problem Handler
            if (newstart < 0) newstart = 0;

            if (newstart + bufsize >= Length)
                newstart = Length - bufsize;
            #endregion

            var newbuf = new ReadOnlyBuffer(newstart, GetBytes(newstart, bufsize));
            newbuf.last_access_timestamp = Global_Timestamp;
            
            buffer_limit.Add(newbuf);
            buffers.Add(newstart, newbuf);
            bufcount++;

            return newbuf;
        }
        #endregion
    }
}
