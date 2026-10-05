using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeStuff
{
    class DFSGenerator : MazeGenerator
    {

        public DFSGenerator(int height, int width, int depth) : base(height, width, depth)
        {

        }

        public override List<List<int>> Generate()
        {
           
            Stack<int> stack = new Stack<int>();
            int nextNode;
            List<int> unvisitedAjacentNodes;
            int currentNode = rnd.Next(0, width * height * depth); // sets the starting node to a random node in the graph
            stack.Push(currentNode); //pushes current node to stack
            nodesVisited.Add(currentNode); // marks current node as visited
            while (stack.Count > 0) // loops until stack is empty
            {
               
                currentNode = stack.Pop(); // new current node is the node taken from the top of the stack
                
                if (GetUnvisitedAdjacentNodes(currentNode).Count > 0) // checks if the current node has unvisited adjacent nodes
                {
                    
                    stack.Push(currentNode); // pushes current node to the stack
                    unvisitedAjacentNodes = GetUnvisitedAdjacentNodes(currentNode); // gets all unvisited adjacent nodes of current node
                    nextNode = unvisitedAjacentNodes[rnd.Next(0, unvisitedAjacentNodes.Count)]; // sets the next node to a random 
                    AddEdge(nextNode, currentNode); // connects the current and next nodes
                    nodesVisited.Add(nextNode); // marks the next node as visited
                    stack.Push(nextNode); // pushes the next node to the stack

                }

            }
            return adjacency;
        }
    }
}
