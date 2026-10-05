using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeStuff
{
    class WilsonsGenerator : MazeGenerator
    {

        public WilsonsGenerator(int height, int width, int depth) : base(height, width, depth)
        {

        }


        public override List<List<int>> Generate() // performs Wilson's generation algorithm
        {
            nodesVisited.Clear();
            int startNode = rnd.Next(0, width * height); // gets a random start node for first walk
            int endNode = rnd.Next(0, width * height); // gets a random end node for first walk
            int currentNode;
            int nextNode;
            List<int> unvisitedAjacentNodes;
            List<int> nodesInWalk = new List<int>();
            Stack<int> currentWalk = new Stack<int>();
            while (startNode == endNode)  // makes sure the start node and end node are not the same
            {
                endNode = rnd.Next(0, width * height * depth);
            }
            currentWalk.Push(startNode); //adds start node to current walk
            currentNode = startNode;
            nodesVisited.Add(currentNode);
            while (currentNode != endNode) // 'walks' in a random direction until it reaches a dead end or reaches the end node
            {

                if (GetUnvisitedAdjacentNodes(currentNode).Count > 0)
                {

                    currentWalk.Push(currentNode);
                    unvisitedAjacentNodes = GetUnvisitedAdjacentNodes(currentNode);
                    nextNode = unvisitedAjacentNodes[rnd.Next(0, unvisitedAjacentNodes.Count)];
                    AddEdge(nextNode, currentNode);
                    nodesVisited.Add(nextNode);
                    currentWalk.Push(nextNode);

                }
                else
                    break;
                currentNode = currentWalk.Pop();

            }
            //nextNode = -1;
            currentWalk.Clear();
            
            while (nodesVisited.Count < (width * height * depth)) // performs walks that meet the original random walk until all nodes are connected
            {
                currentWalk = new Stack<int>();
                nodesInWalk.Clear();
                currentNode = rnd.Next(0, width * height * depth); // assigns a random start node
                while (nodesVisited.Contains(currentNode)) // makes sure that start node has not already been visited
                {
                    currentNode = rnd.Next(0, width * height * depth);
                }
                currentWalk.Push(currentNode);
                nextNode = currentNode;
                while (!nodesVisited.Contains(nextNode)) // walks until it reaches a previous walk and connects them
                {
                    if (GetUnvisitedAdjacentNodes(nextNode).Count > 0)
                    {

                        currentNode = nextNode;

                        nodesVisited.Add(currentNode);
                        nodesInWalk.Add(currentNode);
                        nextNode = GetRandomAdjacentNode(currentNode);
                        while (nodesInWalk.Contains(nextNode)) // makes sure next node is not already in the current walk
                            nextNode = GetRandomAdjacentNode(currentNode);

                        currentWalk.Push(nextNode);
                        AddEdge(nextNode, currentNode);
                    }

                    nextNode = currentWalk.Pop();

                }

            }
            return adjacency;
        }
    }
}
