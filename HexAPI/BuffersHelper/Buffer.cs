using YYHEggEgg.HexAPI.OutputHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YYHEggEgg.HexAPI
{
    internal partial class RandomReadBuffersHelper
    {
        protected class ReadOnlyBuffer : IComparable<ReadOnlyBuffer>
        {
            public ReadOnlyBuffer(Int64 offset, byte[] content)
            {
                this.offset = offset;
                this.content = content;
            }

            // this[index] = content[index - offset]
            public readonly Int64 offset;
            private byte[] content;

            // To record the access timestamp
            public Int64 last_access_timestamp;

            public byte GetByte(Int64 index, Int64 timestamp)
            {
                last_access_timestamp = timestamp;
                return content[index - offset];
            }

            public int CompareTo(ReadOnlyBuffer? other)
            {
                if (other == null) return 1;
                if (other == this) return 0;
                if (ReferenceEquals(this, other)) return 0;

                if (last_access_timestamp - other.last_access_timestamp > 0) return 1;
                else if (last_access_timestamp - other.last_access_timestamp < 0) return -1;
                else return 0;
            }

            public override string ToString()
            {
                return $"A bytes buffer: {HexHelper.GetHexFormat(offset)} -> " +
                    HexHelper.GetHexFormat(offset + content.Length - 1);
            }
        }
    }
}
