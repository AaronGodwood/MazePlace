using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeStuff
{
    class KruskalsGenerator : MazeGenerator
    {
        private List<List<int>> sets = new List<List<int>>();
        public KruskalsGenerator(int height, int width, int depth) : base(height, width, depth)
        {
            for (int i = 0; i < height * width * depth; i++) // creates a list sets one for each node that contains only that node
            {
                sets.Add(new List<int>());
                sets[i].Add(i);
            }
        }

        public override List<List<int>> Generate()
        {
            //int count = 0;
            while (sets.Count > 1) // ends when one set as all nodes are connected
            {
                
                (int num1, int num2) = GetRandomEdge(); // gets a random edge between two nodes
                
                AddEdge(num1, num2); // connects the two nodes by that edge if they're not already connected
                
            }
            return adjacency;
        }

        protected override void AddEdge(int node1, int node2)
        {
            int temp = 0;
            int temp2 = 0;
            if ((node1 < width * height * depth && node2 < width * height * depth) && (GetColumn(node1) == GetColumn(node2) || GetRow(node1) == GetRow(node2) || GetDepth(node1) == GetDepth(node2))) // checks nodes are next to each other
            {

                for (int i = 0; i < sets.Count; i++) // finds which set contains first node
                {
                    if (sets[i].Contains(node2))
                    {
                        temp = i;
                    }
                }
                for (int i = 0; i < sets.Count; i++) // finds which set contains second node
                {
                    if (sets[i].Contains(node1))
                    {
                        temp2 = i;
                    }
                }
                if (temp != temp2) // checks they're not already in the same set
                {
                    adjacency[node1][node2] = 1; // connects nodes in the adjacency matrix
                    adjacency[node2][node1] = 1;
                    for (int i = 0; i < sets[temp].Count; i++) // combines sets containing both nodes
                    {
                        sets[temp2].Add(sets[temp][i]);
                    }

                    sets.RemoveAt(temp); // removes set that is no longer needed as has been combined
                }



            }

        }
    }
}
