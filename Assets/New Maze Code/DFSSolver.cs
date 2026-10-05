using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeStuff
{
    class DFSSolver : MazeSolver
    {
        public DFSSolver(int height,int width,int depth, List<List<int>> adjacency): base(height,width,depth, adjacency)
        {
            this.adjacency = adjacency;
        }


        public override List<int> Solve(int startNode, int endNode)
        {
            Stack<int> stack = new Stack<int>();
            List<int> unvisitedConnectedNodes;
            List<int> nodesVisited = new List<int>();
            int currentNode;
            stack.Push(startNode); // pushes start node to stack
            nodesVisited.Add(startNode); // marks start node as visited
            
            while (stack.Count > 0) // repeats until all nodes have been visited
            {
                
                currentNode = stack.Pop(); // sets the current node to the last one pushed to the stack
                if (currentNode == endNode) // if the current node is the end node
                {
                    //nodesVisited.Add(currentNode);
                    return nodesVisited; // the solution has been found and return the path it took
                }

                unvisitedConnectedNodes = GetUnvisitedConnectedNodes(currentNode,nodesVisited); // gets all unvisited connected nodes of the current node
                foreach (int node in unvisitedConnectedNodes) // iterates through each unvisited connected node
                {
                    
                    stack.Push(node); // pushes the current unvisited connected node to the stack
                    
                    nodesVisited.Add(node); // marks the current unvisited connected node as visited
                    
                }
            }
            
            return nodesVisited; // returns the path taken is no solution is found
        }
        
    }
}
