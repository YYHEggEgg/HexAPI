using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YYHEggEgg.HexAPI.OutputHelper
{
    public class HexHelper
    {
        public static string GetHexFormat(Int64 value)
        {
            return $"0x{value:X10}";
        }
    }
}
