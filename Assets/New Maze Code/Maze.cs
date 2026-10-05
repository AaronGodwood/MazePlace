using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace MazeStuff
{
    class Maze
    {
        protected List<List<int>> adjacency = new List<List<int>>();
        //List<List<int>> sets = new List<List<int>>();
        
        protected Random rnd = new Random();
        protected int width;
        protected int height;
        protected int depth;
        private MazeGenerator generator;
        private int generatorType;
        private MazeSolver solver;
        private int solverType;

        public Maze(int height, int width, int depth)
        {
            this.height = height;
            this.width = width;
            this.depth = depth;
            //for (int i = 0; i < width * height * depth; i++)
            //{
            //    adjacency.Add(new List<int>());
            //    for (int j = 0; j < width * height * depth; j++)
            //    {
            //        adjacency[i].Add(0);
            //    }

            //}
            
        }


        public int GetSolver() // returns the current solver type
        {
            if(solverType == 0)
            {
                return 0;
            }
            return 1;
        }

        public int GetGenerator() // returns the current generator type
        {
            if(generatorType == 0)
            {
                return 0;
            }
            else if(generatorType == 1)
            {
                return 1;
            }
            return 2;
        }
        public int GetHeight() // returns height dimension of the maze
        {
            return height;
        }
        public int GetWidth() // returns width dimension of the maze
        {
            return width;
        }
        public int GetDepth() // returns depth dimension of the maze
        {

            return depth;
        }

        public void SetHeight(int value) // sets height dimension of the maze
        {
            height = value;
        }

        public void SetAdjacency(List<List<int>> value)
        {
            adjacency = value;
        }

        public void SetWidth(int value) // sets width dimension of the maze
        {
            width = value;
        }
        public void SetDepth(int value) // set depth dimension of the maze
        {  
            depth = value; 
        }
        public void GenerateNewMaze() // triggers a maze to be generated and aquires the adjacency matrix
        {
            //MainManager.Instance.Log();
            //generator = new KruskalsGenerator(height, width, depth);
            adjacency = generator.Generate();
            //MainManager.Instance.Log($"Maze:{adjacency.Count}");
        }

        public List<int> SolveMaze() // triggers maze to be solved
        {
            ChooseSolver(GetSolver());
             return solver.Solve(0,width*height*depth-1);
        }

        public List<int> TraceSolve() // traces back the direct route
        {
            return solver.SoloutionTracer(solver.Solve(0,width*height*depth-1));
        }

        public void ChooseSolver(int selection) // selects solver type
        {
            switch (selection)
            {
                case 0:
                    solver = new DFSSolver(height, width, depth, adjacency);
                    solverType = 0;
                    break;

                case 1:
                    solver = new GBFSSolver(height, width, depth, adjacency);
                    solverType = 1;
                    break;
            }
        }
        public void ChooseGenerator(int selection) // selects generator type
        {
            switch(selection)
            {
                case 0:
                    generator = new KruskalsGenerator(height,width,depth);
                    generatorType = 0;
                    break;

                case 1:
                    generator = new DFSGenerator(height,width,depth);
                    generatorType = 1;
                    break;

                case 2:
                    generator = new WilsonsGenerator(height,width,depth);
                    generatorType = 2;
                    break;
            }
        }

        public int GetColumn(int node) // returns the column of a node
        {
            return node % width;
        }

        public int GetRow(int node) // returns the row of a node
        {
            return (node - ((node / (width * height))) * (width * height)) / width;
        }

        public int GetDepth(int node) // returns the depth of a node
        {
            return node / (width * height);
        }

        public List<List<int>> GetAdjacency() // returns adjacency matrix
        {
            return adjacency;
        }

        
    }
}
