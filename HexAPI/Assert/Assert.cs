using YYHEggEgg.HexAPI.OutputHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YYHEggEgg.HexAPI
{
    internal class Assert
    {
        #region Argument Issue
        public static void ArgumentOutOfRange(Int64 start, Int64 end, Int64 index, 
            string? startdesc = null, string? enddesc = null)
        {
#if DEBUG
            bool assert = true;
            string assertReason = string.Empty;
            if (start < 0 || end < 0 || index < 0)
            {
                assert = false;
                assertReason += " (Request Arguments should be > 0) ";
            }

            if (start > end) 
            {
                assert = false;
                assertReason += " (Start > End) ";
            }

            if (start > index)
            {
                assert = false;
                assertReason += " (Underflow) ";
            }
            else if (end < index)
            {
                assert = false;
                assertReason += " (Overflow) ";
            }

            if (!assert)
            {
                var ex = new ArgumentException($"{HexHelper.GetHexFormat(index)} out of range" +
                    string.Format("[{0} -> {1}]",
                    startdesc ?? HexHelper.GetHexFormat(start), enddesc ?? HexHelper.GetHexFormat(end))
                    + assertReason);
                if (startdesc != null) ex.Data.Add(startdesc, HexHelper.GetHexFormat(start));
                if (enddesc != null) ex.Data.Add(enddesc, HexHelper.GetHexFormat(end));
                throw ex;
            }
#endif
        }
        #endregion
    }
}
