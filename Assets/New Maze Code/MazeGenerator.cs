using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeStuff
{
    abstract class MazeGenerator : Maze
    {
        protected List<int> nodesVisited = new List<int>();
        public MazeGenerator(int height, int width, int depth) : base(height, width, depth)
        {
            for (int i = 0; i < width * height * depth; i++) // populates the adjacency matrix with width*height*depth empty lists then populates those lists with the same amount of 0s
            {
                adjacency.Add(new List<int>());
                for (int j = 0; j < width * height * depth; j++)
                {
                    adjacency[i].Add(0);
                }

            }
        }

        public abstract List<List<int>> Generate(); // abstract generate method
        protected (int node1, int node2) GetRandomEdge()
        {
            int node1 = rnd.Next(0, width * height * depth);
            int node2 = GetRandomAdjacentNode(node1);

            return (node1, node2);

        }

        protected List<int> GetUnvisitedAdjacentNodes(int node) // gets the unvisited adjacent nodes of the passed node
        {
            List<int> unvistedAdjacentNodes = new List<int>();
            for (int i = 0; i < 6; i++) // checks each possible direction an adjacent node could be in
            {
                if (!(GetColumn(node) == 0 && i == 0) && !(GetColumn(node) == (width - 1) && i == 1) && !(GetRow(node) == 0 && i == 2) && !(GetRow(node) == (height - 1) && i == 3) && !(GetDepth(node) == 0 && i == 4) && !(GetDepth(node) == (depth - 1) && i == 5)) // checks the current direction does not go off the edge of the grid
                    switch (i)
                    {
                        case 0:
                            if (!nodesVisited.Contains(node - 1)) // checks node to the left is not already visited
                            {
                                unvistedAdjacentNodes.Add(node - 1); // adds node to the left
                            }


                            break;
                        case 1:
                            if (!nodesVisited.Contains(node + 1)) // checks node to the right is not already visited
                            {
                                unvistedAdjacentNodes.Add(node + 1); // adds node to the right
                            }
                            break;
                        case 2:
                            if (!nodesVisited.Contains(node - width)) // checks behind node is not already visited
                            {
                                unvistedAdjacentNodes.Add(node - width); // adds behind node
                            }

                            break;
                        case 3:
                            if (!nodesVisited.Contains(node + width)) // checks infront node is not already visited
                            {
                                unvistedAdjacentNodes.Add(node + width); // adds infront node
                            }

                            break;
                        case 4:
                            if (!nodesVisited.Contains(node - (width * height))) // checks below node is not already visited
                            {
                                unvistedAdjacentNodes.Add(node - (width * height)); // adds below node
                            }
                            break;
                        case 5:
                            if (!nodesVisited.Contains(node + (height * width))) // checks above node is not already visited
                            {
                                unvistedAdjacentNodes.Add(node + (height * width)); // adds above node
                            }
                            break;
                    }


            }
            return unvistedAdjacentNodes;
        }

        protected int GetRandomAdjacentNode(int node1)
        {
            int node2;
            int direction = rnd.Next(0, 6); // gets a random direction
            while ((GetColumn(node1) == 0 && direction == 0) || (GetColumn(node1) == (width - 1) && direction == 1) || (GetRow(node1) == 0 && direction == 2) || (GetRow(node1) == (height - 1) && direction == 3) || (GetDepth(node1) == 0 && direction == 4) || (GetDepth(node1) == (depth - 1) && direction == 5)) // checks that direction will not go off the edge of the graph and regenerates if so
                direction = rnd.Next(0, 6);
            switch (direction)
            {
                case 0:
                    node2 = node1 - 1; // gets left node
                    break;
                case 1:
                    node2 = node1 + 1; // gets right node
                    break;
                case 2:
                    node2 = node1 - width; //gets behind node
                    break;
                case 3:
                    node2 = node1 + width; // gets infront node
                    break;
                case 4:
                    node2 = node1 - (width * height); // gets below node
                    break;
                case 5:
                    node2 = node1 + (width * height); //gets above node
                    break;

                default:
                    node2 = node1 - 1;
                    break;
            }
            return node2; // returns chosen node
        }
        protected virtual void AddEdge(int node1, int node2)
        {
            
            if ((node1 < width * height * depth && node2 < width * height * depth) && (GetColumn(node1) == GetColumn(node2) || GetRow(node1) == GetRow(node2) || GetDepth(node1) == GetDepth(node2))) // checks nodes could be adjacent
            {

                
                
                    adjacency[node1][node2] = 1; // adds connection in adjacency matrix
                    adjacency[node2][node1] = 1;// adds connection in adjacency matrix

            }

        }
    }
}
