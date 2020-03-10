using SaspiesanasAlgoritmi.Huffman;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaspiesanasAlgoritmi
{
    class Program
    {
        static void Main(string[] args)
        {
            HuffmanCompressor oHuffmanCompressor = new HuffmanCompressor();
            oHuffmanCompressor.Compress("C:\\Users\\VS00537\\Desktop\\uni\\hufman\\test.txt");
            Console.WriteLine();
            //oHuffmanCompressor.Decompress("Huffman.txt");
            Console.WriteLine();
        }
    }
}
