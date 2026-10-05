using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace graph
{
    class Graph3D
    {
        List<List<int>> adjacency = new List<List<int>>();
        List<List<int>> sets = new List<List<int>>();
        List<int> nodesVisited = new List<int>();
        Random rnd = new Random();
        int height;
        int width;
        int depth;

        public Graph3D(int height, int width, int depth)
        {
            this.height = height;
            this.width = width;
            this.depth = depth;
            for (int i = 0; i < height * width * depth; i++)
            {
                sets.Add(new List<int>());
                sets[i].Add(i);
            }
            for (int i = 0; i < width * height * depth; i++)
            {
                adjacency.Add(new List<int>());
                for (int j = 0; j < width * height * depth; j++)
                {
                    adjacency[i].Add(0);
                }

            }

            
        }


        public int GetColumn(int node)
        {
            return node % width;
        }

        public int GetRow(int node)
        {
            return (node - ((node / (width * height))) * (width * height)) / width;
        }

        public int GetDepth(int node)
        {
            return node / (width * height);
        }

        public List<List<int>> GetAdjacency()
        {
            return adjacency;
        }
        public void DisplaySets()
        {
            for (int i = 0; i < sets.Count; i++)
            {
                for (int j = 0; j < sets[i].Count; j++)
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
                    Console.Write($"{adjacency[i][j]}|");
                }
                Console.WriteLine();
            }
        }

        public void DisplayConnections()
        {
            List<int> connections = new List<int>();
            for (int i = 0; i < adjacency.Count; i++)
            {
                for (int j = 0; j < adjacency[i].Count; j++)
                {
                    if (adjacency[i][j] == 1)
                    {
                        connections.Add(j);
                    }
                }
                Console.Write($"{i}- ");
                for (int j = 0; j < connections.Count; j++)
                {
                    Console.Write($"{connections[j]},");
                }
                connections.Clear();
                Console.WriteLine();
            }
        }

        public void AddEdge(int node1, int node2)
        {
            int temp = 0;
            int temp2 = 0;
            if ((node1 < width * height * depth && node2 < width * height * width) && (GetColumn(node1) == GetColumn(node2) || GetRow(node1) == GetRow(node2)  || GetDepth(node1) == GetDepth(node2)))
            {

                for (int i = 0; i < sets.Count; i++)
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
            int node1 = rnd.Next(0, width * height*depth);
            int node2 = getRandomAdjacentNode(node1);

            return (node1, node2);

        }

        public void Wilsons()
        {
            nodesVisited.Clear();
            int startNode = rnd.Next(0, width * height);
            int endNode = rnd.Next(0, width * height);
            int currentNode;
            int nextNode;
            List<int> unvisitedAjacentNodes;
            List<int> nodesInWalk = new List<int>();
            Stack<int> currentWalk = new Stack<int>();
            while (startNode == endNode)
            {
                endNode = rnd.Next(0, width * height*depth);
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

            while (nodesVisited.Count < (width * height*depth))
            {
                nodesInWalk.Clear();
                currentNode = rnd.Next(0, width * height * depth);
                while (nodesVisited.Contains(currentNode))
                {
                    currentNode = rnd.Next(0, width * height * depth);
                }
                nextNode = currentNode;
                while (!nodesVisited.Contains(nextNode))
                {
                    currentNode = nextNode;
                    nodesVisited.Add(currentNode);
                    nodesInWalk.Add(currentNode);
                    while (nodesInWalk.Contains(nextNode))
                        nextNode = getRandomAdjacentNode(currentNode);

                    AddEdge(nextNode, currentNode);
                }
            }
        }
        public void DisplaySoloution(Stack<int> soloution)
        {
            Console.WriteLine();
            for(int i = 0;i < soloution.Count;i++)
            {
                Console.Write(soloution.Pop());
            }
        }
        public List<int> DepthFirstSearchSolver(int startNode, int endNode)
        {
            Stack<int> stack = new Stack<int>();
            //List<int> nodesVisited = new List<int>();
            nodesVisited.Clear();
            int currentNode;
            stack.Push(startNode);
            nodesVisited.Add(startNode);
            while(stack.Count > 0)
            {
                currentNode = stack.Pop();
                if (currentNode == endNode)
                    return nodesVisited;
                foreach(int node in GetUnvisitedAdjacentNodes(currentNode))
                {
                    stack.Push(node);
                    nodesVisited.Add(node);
                }
            }
            return nodesVisited;
        }
        public void Kruskals()
        {
            //int count = 0;
            while (sets.Count > 1)
            {
                //Console.WriteLine(sets.Count);
                (int num1, int num2) = GetRandomEdge();
                //Console.WriteLine($"num1 - {num1} num2 - {num2}");
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
            int currentNode = rnd.Next(0, width * height);
            stack.Push(currentNode);
            nodesVisited.Add(currentNode);
            while (stack.Count > 0)
            {
                //Console.WriteLine("ooooo");
                currentNode = stack.Pop();
                //Console.WriteLine(GetUnvisitedAdjacentNodes(currentNode).Count);
                if (GetUnvisitedAdjacentNodes(currentNode).Count > 0)
                {
                    //Console.WriteLine("oodhhdo");
                    stack.Push(currentNode);
                    unvisitedAjacentNodes = GetUnvisitedAdjacentNodes(currentNode);
                    nextNode = unvisitedAjacentNodes[rnd.Next(0, unvisitedAjacentNodes.Count)];
                    AddEdge(nextNode, currentNode);
                    nodesVisited.Add(nextNode);
                    stack.Push(nextNode);

                }

            }
        }

        public List<int> GetUnvisitedAdjacentNodes(int node)
        {
            List<int> unvistedAdjacentNodes = new List<int>();
            for (int i = 0; i < 6; i++)
            {
                if (!(GetColumn(node) == 0 && i == 0) && !(GetColumn(node) == (width-1) && i == 1) && !(GetRow(node) == 0 && i == 2) && !(GetRow(node) == (height - 1) && i == 3) && !(GetDepth(node) == 0 && i == 4) && !(GetDepth(node) == (depth - 1) && i == 5)) /// fisnihsh this you bastardd a hjfuhdsvnvnvkiholfjakdjgsldkjhjm it has been fonsihed 
                    switch (i)
                    {
                        case 0:
                            if (!nodesVisited.Contains(node - 1))
                            {
                                unvistedAdjacentNodes.Add(node - 1);
                            }


                            break;
                        case 1:
                            if (!nodesVisited.Contains(node + 1))
                            {
                                unvistedAdjacentNodes.Add(node + 1);
                            }
                            break;
                        case 2:
                            if (!nodesVisited.Contains(node - width))
                            {
                                unvistedAdjacentNodes.Add(node - width);
                            }

                            break;
                        case 3:
                            if (!nodesVisited.Contains(node + width))
                            {
                                unvistedAdjacentNodes.Add(node + width);
                            }

                            break;
                        case 4:
                            if (!nodesVisited.Contains(node - (width*height)))
                            {
                                unvistedAdjacentNodes.Add(node - (width * height));
                            }
                            break;
                        case 5:
                            if(!nodesVisited.Contains(node + (height * width)))
                            {
                                unvistedAdjacentNodes.Add(node + (height * width));
                            }
                            break;
                    }


            }
            return unvistedAdjacentNodes;
        }

        public int getRandomAdjacentNode(int node1)
        {
            int node2;
            int direction = rnd.Next(0, 6);
            while ((GetColumn(node1) == 0 && direction == 0) || (GetColumn(node1) == (width - 1) && direction == 1) || (GetRow(node1) == 0 && direction == 2) || (GetRow(node1) == (height - 1) && direction == 3) || (GetDepth(node1) == 0 && direction == 4) || (GetDepth(node1) == (depth - 1) && direction == 5))
                direction = rnd.Next(0, 6);
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
                case 4:
                    node2 = node1 - (width * height);
                    break;
                case 5:
                    node2 = node1 + (width * height);
                    break;

                default:
                    node2 = node1 - 1;
                    break;
            }
            return node2;
        }



    }
}
