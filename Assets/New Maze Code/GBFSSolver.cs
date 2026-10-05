using priority_queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeStuff
{
    class GBFSSolver : MazeSolver
    {
        public GBFSSolver(int height, int width, int depth, List<List<int>> adjacency) : base(height, width, depth, adjacency)
        {
            this.adjacency = adjacency;
        }

        public double DistanceFinder(int currentNode, int endNode) // works out the distance from the current node to the end node using pythagoras's theorem
        {
            return Math.Sqrt(Math.Pow(GetRow(endNode) - GetRow(currentNode), 2) + Math.Pow(GetColumn(endNode) - GetColumn(currentNode), 2) + Math.Pow(GetDepth(endNode) - GetDepth(currentNode), 2));
        }

        public override List<int> Solve(int startNode, int endNode)
        {
            PriorityQueue<int> priorityQueue = new PriorityQueue<int>(); // creates a priority queue with minimum priority first
            List<int> unvisitedConnectedNodes;
            List<int> nodesVisited = new List<int>();
            nodesVisited.Clear();
            //int run = 0;
            int currentNode;
            priorityQueue.Enqueue(startNode, DistanceFinder(startNode, endNode)); // enqueues the start node with distance to end node as the priority
            nodesVisited.Add(startNode);
            while (priorityQueue.Count > 0) // continues until solution found or all nodes exhausted
            {
                currentNode = priorityQueue.Dequeue(); // moves to node with minimum distance to end node
                if (currentNode == endNode) // breaks if solution is found
                {
                    
                    return nodesVisited;
                }

                unvisitedConnectedNodes = GetUnvisitedConnectedNodes(currentNode,nodesVisited);
                foreach (int node in unvisitedConnectedNodes) //adds each connected node to the current node to the priority queue
                {
                    priorityQueue.Enqueue(node, DistanceFinder(node, endNode));
                    nodesVisited.Add(node);
                }
            }
            return nodesVisited;
        }

        
    }
}
