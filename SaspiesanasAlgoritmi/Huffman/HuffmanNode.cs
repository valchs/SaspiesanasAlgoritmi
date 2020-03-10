using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaspiesanasAlgoritmi.Huffman
{
    public class HuffmanNode
    {
        public HuffmanNode()
        {
        }

        public HuffmanNode(char Value)
        {
            cValue = Value;
        }

        public HuffmanNode(HuffmanNode Left, HuffmanNode Right)
        {
            oLeft = Left;
            oLeft.oParent = this;
            oLeft.bIsLeftNode = true;

            oRight = Right;
            oRight.oParent = this;
            oRight.bIsRightNode = true;

            iWeight = (oLeft.Weight + oRight.Weight);
        }

        private string sBinaryWord;
        private bool bIsLeftNode;
        private bool bIsRightNode;
        private HuffmanNode oLeft;
        private HuffmanNode oParent;
        private HuffmanNode oRight;
        private char? cValue;
        private int iWeight;

        public string BinaryWord
        {
            get
            {
                string sReturnValue = "";

                if (String.IsNullOrEmpty(sBinaryWord) == true)
                {
                    StringBuilder oStringBuilder = new StringBuilder();

                    HuffmanNode oHuffmanNode = this;

                    while (oHuffmanNode != null)
                    {
                        if (oHuffmanNode.bIsLeftNode == true)
                        {
                            oStringBuilder.Insert(0, "0");
                        }

                        if (oHuffmanNode.bIsRightNode == true)
                        {
                            oStringBuilder.Insert(0, "1");
                        }

                        oHuffmanNode = oHuffmanNode.oParent;
                    }

                    sReturnValue = oStringBuilder.ToString();
                    sBinaryWord = sReturnValue;
                }
                else
                {
                    sReturnValue = sBinaryWord;
                }

                return sReturnValue;
            }
        }

        public HuffmanNode Left
        {
            get
            {
                return oLeft;
            }
        }

        public HuffmanNode Parent
        {
            get
            {
                return oParent;
            }
        }

        public HuffmanNode Right
        {
            get
            {
                return oRight;
            }
        }

        public char? Value
        {
            get
            {
                return cValue;
            }
        }

        public int Weight
        {
            get
            {
                return iWeight;
            }
            set
            {
                iWeight = value;
            }
        }

        public override string ToString()
        {
            StringBuilder oStringBuilder = new StringBuilder();

            if (cValue.HasValue == true)
            {
                oStringBuilder.AppendFormat("'{0}' ({1}) - {2} ({3})", cValue.Value, iWeight, BinaryWord, BinaryWord.BinaryStringToInt32());
            }
            else
            {
                if ((oLeft != null) && (oRight != null))
                {
                    if ((oLeft.Value.HasValue == true) && (oRight.Value.HasValue == true))
                    {
                        oStringBuilder.AppendFormat("{0} + {1} ({2})", oLeft.Value, oRight.Value, iWeight);
                    }
                    else
                    {
                        oStringBuilder.AppendFormat("{0}, {1} - ({2})", oLeft, oRight, iWeight);
                    }
                }
                else
                {
                    oStringBuilder.Append(iWeight);
                }
            }

            return oStringBuilder.ToString();
        }


    }
}
