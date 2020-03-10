using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaspiesanasAlgoritmi.Huffman
{
    public class HuffmanCompressor
    {
        private HuffmanNode oRootHuffmanNode;
        private List<HuffmanNode> oValueHuffmanNodes;

        private List<HuffmanNode> BuildBinaryTree(string Value)
        {
            List<HuffmanNode> oHuffmanNodes = GetInitialNodeList();

            //  Update node weights
            Value.ToList().ForEach(m => oHuffmanNodes[m].Weight++);

            //  Select only nodes that have a weight
            oHuffmanNodes = oHuffmanNodes
                .Where(m => (m.Weight > 0))
                .OrderBy(m => (m.Weight))
                .ThenBy(m => (m.Value))
                .ToList();

            //  Assign parent nodes
            oHuffmanNodes = UpdateNodeParents(oHuffmanNodes);

            oRootHuffmanNode = oHuffmanNodes[0];
            oHuffmanNodes.Clear();

            //  Sort nodes into a tree
            SortNodes(oRootHuffmanNode, oHuffmanNodes);

            return oHuffmanNodes;
        }

        public void Compress(string FileName)
        {
            FileInfo oFileInfo = new FileInfo(FileName);

            if (oFileInfo.Exists == true)
            {
                string sFileContents = "";

                using (StreamReader oStreamReader = new StreamReader(File.OpenRead(FileName)))
                {
                    sFileContents = oStreamReader.ReadToEnd();
                }

                List<HuffmanNode> oHuffmanNodes = BuildBinaryTree(sFileContents);

                //  Filter to nodes we care about
                oValueHuffmanNodes = oHuffmanNodes
                    .Where(m => (m.Value.HasValue == true))
                    .OrderBy(m => (m.BinaryWord))   //  Not really required, for presentation purposes
                    .ToList();

                //  Construct char to binary word dictionary for quick value to binary word resolution
                Dictionary<char, string> oCharToBinaryWordDictionary = new Dictionary<char, string>();
                foreach (HuffmanNode oHuffmanNode in oValueHuffmanNodes)
                {
                    oCharToBinaryWordDictionary.Add(oHuffmanNode.Value.Value, oHuffmanNode.BinaryWord);
                }

                StringBuilder oStringBuilder = new StringBuilder();
                List<byte> oByteList = new List<byte>();
                for (int i = 0; i < sFileContents.Length; i++)
                {
                    string sWord = "";

                    //  Append the binary word value using the char located at the current file position
                    oStringBuilder.Append(oCharToBinaryWordDictionary[sFileContents[i]]);

                    //  Once we have at least 8 chars, we can construct a byte
                    while (oStringBuilder.Length >= 8)
                    {
                        sWord = oStringBuilder.ToString().Substring(0, 8);

                        //  Remove the word to be constructed from the buffer
                        oStringBuilder.Remove(0, sWord.Length);
                    }

                    //  Convert the word and add it onto the list
                    if (String.IsNullOrEmpty(sWord) == false)
                    {
                        oByteList.Add(Convert.ToByte(sWord, 2));
                    }
                }

                //  If there is anything in the buffer, make sure it is retrieved
                if (oStringBuilder.Length > 0)
                {
                    string sWord = oStringBuilder.ToString();

                    //  Convert the word and add it onto the list
                    if (String.IsNullOrEmpty(sWord) == false)
                    {
                        oByteList.Add(Convert.ToByte(sWord, 2));
                    }
                }

                //  Write compressed file
                string sCompressedFileName = Path.Combine(oFileInfo.Directory.FullName, String.Format("{0}.compressed", oFileInfo.Name.Replace(oFileInfo.Extension, "")));
                if (File.Exists(sCompressedFileName) == true)
                {
                    File.Delete(sCompressedFileName);
                }

                using (FileStream oFileStream = File.OpenWrite(sCompressedFileName))
                {
                    oFileStream.Write(oByteList.ToArray(), 0, oByteList.Count);
                }
            }
        }

        public void Decompress(string FileName)
        {
            FileInfo oFileInfo = new FileInfo(FileName);

            if (oFileInfo.Exists == true)
            {
                string sCompressedFileName = String.Format("{0}.compressed", oFileInfo.FullName.Replace(oFileInfo.Extension, ""));

                byte[] oBuffer = null;
                using (FileStream oFileStream = File.OpenRead(sCompressedFileName))
                {
                    oBuffer = new byte[oFileStream.Length];
                    oFileStream.Read(oBuffer, 0, oBuffer.Length);
                }

                //  Find the zero node
                HuffmanNode oZeroHuffmanNode = oRootHuffmanNode;
                while (oZeroHuffmanNode.Left != null)
                {
                    oZeroHuffmanNode = oZeroHuffmanNode.Left;
                }

                //  Unpack the file contents
                HuffmanNode oCurrentHuffmanNode = null;
                StringBuilder oStringBuilder = new StringBuilder();

                for (int i = 0; i < oBuffer.Length; i++)
                {
                    string sBinaryWord = "";
                    byte oByte = oBuffer[i];

                    if (oByte == 0)
                    {
                        sBinaryWord = oZeroHuffmanNode.BinaryWord;
                    }
                    else
                    {
                        sBinaryWord = Convert.ToString(oByte, 2);
                    }

                    if ((sBinaryWord.Length < 8) && (i < (oBuffer.Length - 1)))
                    {
                        //  Pad binary word out to 8 places
                        StringBuilder oBinaryStringBuilder = new StringBuilder(sBinaryWord);
                        while (oBinaryStringBuilder.Length < 8)
                        {
                            oBinaryStringBuilder.Insert(0, "0");
                        }

                        sBinaryWord = oBinaryStringBuilder.ToString();
                    }

                    //  Use the binary word to navigate the tree looking for the value
                    for (int j = 0; j < sBinaryWord.Length; j++)
                    {
                        char cValue = sBinaryWord[j];

                        if (oCurrentHuffmanNode == null)
                        {
                            oCurrentHuffmanNode = oRootHuffmanNode;
                        }

                        if (cValue == '0')
                        {
                            oCurrentHuffmanNode = oCurrentHuffmanNode.Left;
                        }
                        else
                        {
                            oCurrentHuffmanNode = oCurrentHuffmanNode.Right;
                        }

                        if ((oCurrentHuffmanNode.Left == null) && (oCurrentHuffmanNode.Right == null))
                        {
                            //  No more child nodes to choose from, so this must be a value node
                            oStringBuilder.Append(oCurrentHuffmanNode.Value.Value);
                            oCurrentHuffmanNode = null;
                        }
                    }
                }

                //  Write out file
                string sUncompressedFileName = Path.Combine(oFileInfo.Directory.FullName, String.Format("{0}.uncompressed", oFileInfo.Name.Replace(oFileInfo.Extension, "")));

                if (File.Exists(sUncompressedFileName) == true)
                {
                    File.Delete(sUncompressedFileName);
                }

                using (StreamWriter oStreamWriter = new StreamWriter(File.OpenWrite(sUncompressedFileName)))
                {
                    oStreamWriter.Write(oStringBuilder.ToString());
                }
            }
        }

        private static List<HuffmanNode> GetInitialNodeList()
        {
            List<HuffmanNode> oGetInitialNodeList = new List<HuffmanNode>();

            for (int i = Char.MinValue; i < Char.MaxValue; i++)
            {
                oGetInitialNodeList.Add(new HuffmanNode((char)(i)));
            }

            return oGetInitialNodeList;
        }

        private static void SortNodes(HuffmanNode Node, List<HuffmanNode> Nodes)
        {
            if (Nodes.Contains(Node) == false)
            {
                Nodes.Add(Node);
            }

            if (Node.Left != null)
            {
                SortNodes(Node.Left, Nodes);
            }

            if (Node.Right != null)
            {
                SortNodes(Node.Right, Nodes);
            }
        }

        private static List<HuffmanNode> UpdateNodeParents(List<HuffmanNode> Nodes)
        {
            //  Assign parent nodes
            while (Nodes.Count > 1)
            {
                int iOperations = (Nodes.Count / 2);
                for (int iOperation = 0, i = 0, j = 1; iOperation < iOperations; iOperation++, i += 2, j += 2)
                {
                    if (j < Nodes.Count)
                    {
                        HuffmanNode oParentHuffmanNode = new HuffmanNode(Nodes[i], Nodes[j]);
                        Nodes.Add(oParentHuffmanNode);

                        Nodes[i] = null;
                        Nodes[j] = null;
                    }
                }

                //  Remove null nodes
                Nodes = Nodes
                    .Where(m => (m != null))
                    .OrderBy(m => (m.Weight))   //  Choose the lowest weightings first
                    .ToList();
            }

            return Nodes;
        }
    }
}
