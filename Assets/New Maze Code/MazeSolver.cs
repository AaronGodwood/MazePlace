using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace MazeStuff
{
    abstract class MazeSolver
    {
        protected List<List<int>> adjacency;
        protected int width;
        protected int height;
        protected int depth;

        public MazeSolver(int height, int width, int depth, List<List<int>> adjacency)
        {
            this.adjacency = adjacency;
            this.width = width;
            this.height = height;
            this.depth = depth;
        }


        protected int GetColumn(int node) // returns the column of a node
        {
            return node % width;
        }

        protected int GetRow(int node) // returns the row of a node
        {
            return (node - ((node / (width * height))) * (width * height)) / width;
        }

        protected int GetDepth(int node) // returns the depth of a node
        {
            return node / (width * height);
        }
        public abstract List<int> Solve(int startNode, int endNode); // abstract solve method

        public List<int> SoloutionTracer(List<int> nodesVisited) // traces back the direct path to the end
        {
            List<int> backwardsTrace = new List<int>();
            int currentIndex = nodesVisited.Count - 1; //sets current position in nodesVisited list to the last element
            List<int> connections;
            backwardsTrace.Add(nodesVisited[currentIndex]); // adds the last node to the backwards trace
            for (int i = currentIndex - 1; i >= 0; i--) // iterates backwards from the current index
            {
                connections = GetConnectedNodes(nodesVisited[i]); // gets the connections of the current iteration node
                if (connections.Contains(nodesVisited[currentIndex])) // if the current iteration node is connected to the current node
                {
                    backwardsTrace.Add(nodesVisited[i]); // add it to the trace
                    currentIndex = i; // sets the iteration node to the current node

                }
            }
            

            return backwardsTrace;
        }

        protected List<int> GetConnectedNodes(int node) // gets all connected nodes to the passed node
        {
            List<int> connections = new List<int>();
            for (int j = 0; j < adjacency[node].Count; j++) // iterates through each element in the nodes row of the adjacency matrix
            {
                if (adjacency[node][j] == 1) // if there's a one add it to the connections list
                {
                    connections.Add(j);
                }
            }
            return connections; // returns the list of connections
        }

        protected List<int> GetUnvisitedConnectedNodes(int node, List<int> nodesVisited) // gets unvisited connected nodes
        {
            List<int> unvisitedConnections = new List<int>();
            List<int> connections = GetConnectedNodes(node); // gets all connected nodes
            for (int i = 0; i < connections.Count; i++) // iterates through all connected nodes
            {
                if (!nodesVisited.Contains(connections[i])) // if the connected node has not been visited add it to the list of unvisited nods
                {
                    unvisitedConnections.Add(connections[i]);
                }
            }
            return unvisitedConnections; // return the list of unvisited connections
        }

    }
}
