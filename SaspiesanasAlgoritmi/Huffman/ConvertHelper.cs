using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaspiesanasAlgoritmi.Huffman
{
    static class ConvertHelper
    {
        public static int BinaryStringToInt32(this string Value)
        {
            int iBinaryStringToInt32 = 0;

            for (int i = (Value.Length - 1), j = 0; i >= 0; i--, j++)
            {
                iBinaryStringToInt32 += ((Value[j] == '0' ? 0 : 1) * (int)(Math.Pow(2, i)));
            }

            return iBinaryStringToInt32;
        }
    }
}
