using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeStuff
{
    class AdjacencySaver
    {
        private string fileName = "C:\\Users\\azmcl\\OneDrive - Hills Road Sixth Form College\\Computer Science\\Code\\NEA code\\AdjacencySaver\\AdjacencySaver\\file.bin";

        public void SetFileName(int selection) // selects the right filename depending on the selected save slot
        {
            switch (selection)
            {
                case 0:
                    fileName = "C:\\Users\\azmcl\\MazePlace\\Assets\\Save Locations\\slot1.bin";
                    break;

                case 1:
                    fileName = "C:\\Users\\azmcl\\MazePlace\\Assets\\Save Locations\\slot2.bin";
                    break;

                case 2:
                    fileName = "C:\\Users\\azmcl\\MazePlace\\Assets\\Save Locations\\slot3.bin";
                    break;

                case 3:
                    fileName = "C:\\Users\\azmcl\\MazePlace\\Assets\\Save Locations\\slot4.bin";
                    break;
            }
        }
        public void Write(Maze maze) // replace int num with Maze maze
        {

            
            string binaryByte = DecimalToBinary(maze.GetWidth(),8);
            //Console.WriteLine(DecimalToBinary(size, 24));

            using (BinaryWriter bw = new BinaryWriter(File.Open(fileName,FileMode.Create)))
            {
                bw.Write(Convert.ToByte(binaryByte,2)); // writes width as byte
                binaryByte = DecimalToBinary(maze.GetHeight(), 8);
                bw.Write(Convert.ToByte(binaryByte,2)); //writes height
                binaryByte = DecimalToBinary(maze.GetDepth(), 8);
                bw.Write(Convert.ToByte(binaryByte, 2)); // writes depth
                
                binaryByte = string.Empty;
                for (int i = 0; i < maze.GetAdjacency().Count; i++) // writes each line of adjacency matrix one by one
                {
                    for (int j = 0; j < maze.GetAdjacency()[i].Count / 8; j++) // writes one byte of data at a time
                    {
                        
                        for (int k = 0; k < 8; k++) // gets 8 characters of the binary data at a time
                        {
                            binaryByte += Convert.ToString(maze.GetAdjacency()[i][(8 * j) + k]); //adds each character to a single sting


                        }
                        bw.Write(Convert.ToByte(binaryByte,2)); // writes that string
                        binaryByte = string.Empty; //resets the string
                    }
                    for(int j = 0; j < maze.GetAdjacency()[i].Count % 8; j++) // processes the last incomplete byte by iterating to how many element are left in the line
                    {
                        binaryByte += Convert.ToString(maze.GetAdjacency()[i][(maze.GetAdjacency()[i].Count / 8)*8 + j]); // adds each character to a single string
                    }
                    binaryByte = binaryByte.PadRight(8, '0'); //fills the string up to 8 bits
                    bw.Write(Convert.ToByte(binaryByte, 2)); // writes the byte to file
                    binaryByte = string.Empty; //resets string

                }
            }
            
        }
        public List<List<int>> Read() 
        {
            int size;
            string binaryTemp;
            List<List<int>> adjacency = new List<List<int>>();
            List<int> adjacencyLine = new List<int>();
            int width,height,depth;
            using (BinaryReader br = new BinaryReader(File.OpenRead(fileName)))
            {
                width = BinaryToDecimal(DecimalToBinary(Convert.ToInt32(br.ReadByte()),8)); // reads first byte as the width value
                height = BinaryToDecimal(DecimalToBinary(Convert.ToInt32(br.ReadByte()), 8)); // reads second byte as the height value
                depth = BinaryToDecimal(DecimalToBinary(Convert.ToInt32(br.ReadByte()), 8)); //reads third byte as the depth value
                //size = BinaryToDecimal(binaryTemp); // converts size to a decimal value
                size = width * height * depth;
                
                for(int k = 0; k < size; k++) // reads one line of adjacency matrix at a time
                {
                    
                    for (int i = 0; i < size / 8; i++) // reads full bytes
                    {
                        binaryTemp = DecimalToBinary(Convert.ToInt32(br.ReadByte()), 8);
                        for (int j = 0; j < binaryTemp.Length; j++)
                        {
                            adjacencyLine.Add(Convert.ToInt32(Convert.ToString(binaryTemp[j])));
                        }
                    }
                    binaryTemp = DecimalToBinary(Convert.ToInt32(br.ReadByte()), 8);
                    for (int j = 0; j < size % 8; j++) // reads first part of incomplete bytes
                    {
                        adjacencyLine.Add(Convert.ToInt32(Convert.ToString(binaryTemp[j])));
                    }
                    adjacency.Add(adjacencyLine); // adds line of matrix to the matrix
                    adjacencyLine = new List<int>(); // makes sure next line starts empty
                }
                
            }
            adjacency.Add(new List<int>());
            adjacency[^1].Add(width);
            adjacency[^1].Add(height);
            adjacency[^1].Add(depth);
            return adjacency;
        }

        public int BinaryToDecimal(string binary, int position = 0)
        {
            if (position == binary.Length) // if the end of the string is reached 
            {
                return 0;
            }

            return Convert.ToInt32(Convert.ToString(binary[position])) *
                   Convert.ToInt32(Math.Pow(2, binary.Length - position - 1)) +
                   BinaryToDecimal(binary, position + 1); // return the current character * 2^position and add it to the total by calling the method passing the next positon
        }

        public string DecimalToBinary(int decimalNum, int length)
        {
            string binaryString = Convert.ToString(decimalNum,2); // converts the decimal number to a binary string
            return binaryString.PadLeft(length,'0'); // pads it to the left with 0s until it is the passed length
        }

        
    }
}
