using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Unity.IO.LowLevel.Unsafe;
using priority_queue;

namespace graph
{
    class Graph
    {
        
        public List<List<int>> adjacency = new List<List<int>>();
        public List<List<int>> sets = new List<List<int>>();
        public List<int> nodesVisited = new List<int>();
        public Random rnd = new Random();
        public int height;
        public int width;

        public Graph(int height, int width)
        {
            this.height = height;
            this.width = width;
            for(int i = 0; i < height*width; i++)
            {
                sets.Add(new List<int>());
                sets[i].Add(i);
            }
            for (int i = 0; i < width*height; i++)
            {
                adjacency.Add(new List<int>());
                for (int j = 0; j < width*height; j++)
                {
                    adjacency[i].Add(0);
                }
                
            }
        }

        public int GetRow(int node)
        {
            return node / width;
        }

        public int GetColumn(int node)
        {
            return node % width;
        }

        public List<List<int>> GetAdjacency()
        {
            return adjacency;
        }

        public void DisplaySets()
        {
            for(int i = 0;i < sets.Count;i++)
            {
                for(int j = 0; j < sets[i].Count;j++)
                {
                    Console.Write(sets[i][j]);
                }
                Console.WriteLine();
            }
        }

        public void DisplayAdjecency()
        {
            for (int i = 0; i < adjacency.Count; i++)
            {
                Console.WriteLine("_---------------");
                for (int j = 0; j < adjacency[i].Count; j++)
                {
                    Console.Write($"{adjacency[i][j]}|" );
                }
                Console.WriteLine();
            }
        }

        public void DisplayConnections()
        {
            List<int> connections = new List<int>();
            for(int i = 0; i< adjacency.Count; i++)
            {
                for(int j = 0; j< adjacency[i].Count; j++)
                {
                    if (adjacency[i][j] == 1)
                    {
                        connections.Add(j);
                    }
                }
                Console.Write($"{i}- ");
                for (int j = 0;j < connections.Count;j++)
                {
                    Console.Write($"{connections[j]},");
                }
                connections.Clear();
                Console.WriteLine() ;
            }
        }

        public void AddEdge(int node1, int node2)
        {
            int temp = 0;
            int temp2 = 0;
            if((node1 < width*height && node2 < width*height) && (GetColumn(node1) == GetColumn(node2) || GetRow(node2) == GetRow(node2) ))
            {
                
                for(int i = 0; i< sets.Count; i++)
                {
                    if (sets[i].Contains(node2))
                    {
                        temp = i;
                    }
                }
                for (int i = 0; i < sets.Count; i++)
                {
                    if (sets[i].Contains(node1))
                    {
                        temp2 = i;
                    }
                }
                if (temp != temp2)
                {
                    adjacency[node1][node2] = 1;
                    adjacency[node2][node1] = 1;
                    for (int i = 0; i < sets[temp].Count; i++)
                    {
                        sets[temp2].Add(sets[temp][i]);
                    }

                    sets.RemoveAt(temp);
                }
                 
                

            }
            
        }

        public (int node1, int node2) GetRandomEdge()
        {
            int node1 = rnd.Next(0, width * height);
            int node2 = getRandomAdjacentNode(node1);

            return (node1, node2);

        }

        public void Wilsons()
        {
            
            nodesVisited.Clear();
            int startNode= rnd.Next(0, width * height);
            int endNode = rnd.Next(0, width * height);
            int currentNode;
            int nextNode;
            List<int> unvisitedAjacentNodes;
            List<int> nodesInWalk = new List<int>();
            Stack<int> currentWalk = new Stack<int>();
            while (startNode == endNode)
            {
                endNode = rnd.Next(0, width * height);
            }
            currentWalk.Push(startNode);
            currentNode = startNode;
            nodesVisited.Add(currentNode);
            while (currentNode != endNode)
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
            
            while (nodesVisited.Count < (width*height))
            {
                nodesInWalk.Clear();
                currentNode = rnd.Next(0, width * height);
                while(nodesVisited.Contains(currentNode))
                {
                    currentNode = rnd.Next(0,width * height);
                }
                nextNode = currentNode;
                while(!nodesVisited.Contains(nextNode))
                {
                    currentNode = nextNode;
                    nodesVisited.Add(currentNode);
                    nodesInWalk.Add(currentNode);
                    while(nodesInWalk.Contains(nextNode))
                        nextNode =getRandomAdjacentNode(currentNode);
                    
                    AddEdge(nextNode, currentNode);
                }
            }
        }

        public void DisplaySoloution(Stack<int> soloution)
        {
            Console.WriteLine("Stack");
            for (int i = 0; i < soloution.Count; i++)
            {
                Console.Write($"{soloution.Pop()},");
            }
            Console.WriteLine();
        }

        public void DisplayList(List<int> list)
        {
            Console.WriteLine("List");
            for (int i = 0; i < list.Count; i++)
            {
                Console.Write($"{list[i]},");
            }
            Console.WriteLine();
        }
        public double DistanceFinder(int currentNode, int endNode)
        {
            return Math.Sqrt(Math.Pow(GetRow(endNode) - GetRow(currentNode), 2) + Math.Pow(GetColumn(endNode) - GetColumn(currentNode), 2));
        }

        public List<int> GreedyBestFirstSolver(int startNode, int endNode)
        {
            PriorityQueue<int> priorityQueue = new PriorityQueue<int>();
            nodesVisited.Clear();
            //int run = 0;
            int currentNode;
            priorityQueue.Enqueue(startNode, DistanceFinder(startNode, endNode));
            nodesVisited.Add(startNode);
            while (priorityQueue.Count > 0)
            {
                currentNode = priorityQueue.Dequeue();
                if (currentNode == endNode)
                {
                    //nodesVisited.Add(currentNode);
                    return nodesVisited;
                }
                foreach (int node in GetUnvisitedConnectedNodes(currentNode))
                {
                    priorityQueue.Enqueue(node, DistanceFinder(node, endNode));
                    nodesVisited.Add(node);
                }
            }
            return nodesVisited;
        }
        public List<int> DepthFirstSearchSolver(int startNode, int endNode)
        {
            Stack<int> stack = new Stack<int>();
            //List<int> nodesVisited = new List<int>();
            int run = 0;
            nodesVisited.Clear();
            int currentNode;
            stack.Push(startNode);
            nodesVisited.Add(startNode);
            Console.WriteLine($"Start Node-{startNode}  End Node-{endNode}");
            while (stack.Count > 0)
            {
                Console.WriteLine(run++);
                currentNode = stack.Pop();
                if (currentNode == endNode)
                {
                    //nodesVisited.Add(currentNode);
                    return nodesVisited;
                }
                foreach (int node in GetUnvisitedConnectedNodes(currentNode))
                {
                    Console.WriteLine($"Node-{node}");
                    
                }
                foreach (int node in GetUnvisitedConnectedNodes(currentNode))
                {
                    Console.WriteLine($"Current Node-{node}");
                    stack.Push(node);
                    //DisplaySoloution(stack);
                    Console.WriteLine(stack.Count);
                    nodesVisited.Add(node);
                    DisplayList(nodesVisited);
                }
            }
            //Console.WriteLine("Ajksx");
            DisplayList(nodesVisited);
            return nodesVisited;
        }

        public List<int >GetUnvisitedConnectedNodes(int node)
        {
            List<int> unvisitedConnections = new List<int>();
            List<int> connections = GetConnectedNodes(node);
            for (int i = 0; i< connections.Count;i++)
            {
                if (!nodesVisited.Contains(connections[i]))
                {
                    unvisitedConnections.Add(connections[i]);
                }
            }
            return unvisitedConnections;
        }

        public List<int> GetConnectedNodes(int node)
        {
            List<int> connections = new List<int>();
            for (int j = 0; j < adjacency[node].Count; j++)
            {
                if (adjacency[node][j] == 1)
                {
                    connections.Add(j);
                }
            }
            return connections;
        }

        public void Kruskals()
        {
            //int count = 0;
            while(sets.Count > 1)
            {
                (int num1, int num2) = GetRandomEdge();
                AddEdge(num1, num2);
                //Console.WriteLine();
                //Console.WriteLine(count++);
                //DisplaySets();
            }
        }

        public void RandomisedDepthFirst()
        {
            Stack<int> stack = new Stack<int>();
            int nextNode;
            List<int> unvisitedAjacentNodes;
            int currentNode = rnd.Next(0,width*height);
            stack.Push(currentNode);
            nodesVisited.Add(currentNode);
            while(stack.Count > 0)
            {
                //Console.WriteLine("ooooo");
                currentNode = stack.Pop();
                //Console.WriteLine(GetUnvisitedAdjacentNodes(currentNode).Count);
                if(GetUnvisitedAdjacentNodes(currentNode).Count > 0)
                {
                    //Console.WriteLine("oodhhdo");
                    stack.Push(currentNode);
                    unvisitedAjacentNodes = GetUnvisitedAdjacentNodes(currentNode);
                    nextNode = unvisitedAjacentNodes[rnd.Next(0,unvisitedAjacentNodes.Count)];
                    AddEdge(nextNode, currentNode);
                    nodesVisited.Add(nextNode);
                    stack.Push(nextNode);

                }

            }
        }

        public List<int> GetUnvisitedAdjacentNodes(int node)
        {
            List<int> unvistedAdjacentNodes = new List<int>();
            for(int i = 0;i<4;i++)
            {
                if(!(GetColumn(node) == 0 && i == 0) && !(GetColumn(node) == (width - 1) && i == 1) && !(GetRow(node) == 0 && i == 2) && !(GetRow(node) == (height - 1) && i == 3))
                    switch (i)
                    {
                        case 0:
                            if(!nodesVisited.Contains(node - 1))
                            {
                                unvistedAdjacentNodes.Add(node - 1);
                            }
                            

                            break;
                        case 1:
                            if(!nodesVisited.Contains(node + 1))
                            {
                                unvistedAdjacentNodes.Add(node + 1);
                            }
                            break;
                        case 2:
                            if(!nodesVisited.Contains(node-width))
                            {
                                unvistedAdjacentNodes.Add(node - width);
                            }
                            
                            break;
                        case 3:
                            if(!nodesVisited.Contains(node+width))
                            {
                                unvistedAdjacentNodes.Add(node + width);
                            }
                            
                            break;
                    }
                

            }
            return unvistedAdjacentNodes;
        }

        public int getRandomAdjacentNode(int node1)
        {
            int node2;
            int direction = rnd.Next(0, 4);
                while ((GetColumn(node1) == 0 && direction == 0) || (GetColumn(node1) == (width-1) && direction == 1) || (GetRow(node1) == 0 && direction == 2) || (GetRow(node1) == (height - 1) && direction == 3))
                direction = rnd.Next(0, 4);
            switch (direction)
            {
                case 0:
                    node2 = node1 - 1;
                    break;
                case 1:
                    node2 = node1 + 1;
                    break;
                case 2:
                    node2 = node1 - width;
                    break;
                case 3:
                    node2 = node1 + width;
                    break;

                default:
                    node2 = node1 - 1;
                    break;
            }
            return node2;
        }



    }
}
