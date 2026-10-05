using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MazeStuff
{
    class SettingsSaver
    {
        private string filename;

        public SettingsSaver(string filename) // takes in the filename
        { 
            this.filename = filename;
        }

        public void WriteToFile()
        {
            using (StreamWriter sw = new StreamWriter(filename,false))
            {
                sw.WriteLine(MainManager.Instance.maze.GetHeight()); // writes maze height to file
                sw.WriteLine(MainManager.Instance.maze.GetWidth());// writes maze width to file
                sw.WriteLine(MainManager.Instance.maze.GetDepth());// writes maze depth to file
                sw.WriteLine(MainManager.Instance.maze.GetGenerator());// writes maze generator to file
                sw.WriteLine(MainManager.Instance.maze.GetSolver());// writes maze solver to file
                sw.WriteLine(MainManager.Instance.ThreeD); //writes the current 3D setting
            }
        }

        public void ReadFromFile()
        {
            //List<int> values = new List<int>();
            //int count = 0;
            
            using(StreamReader sr = new StreamReader(filename))
            {
                MainManager.Instance.maze.SetHeight(Convert.ToInt32(sr.ReadLine())); // reads maze height from the file
                MainManager.Instance.maze.SetWidth(Convert.ToInt32(sr.ReadLine())); // reads maze width from the file
                MainManager.Instance.maze.SetDepth(Convert.ToInt32(sr.ReadLine())); // reads maze depth from file
                MainManager.Instance.maze.ChooseGenerator(Convert.ToInt32(sr.ReadLine())); // reads maze generator from file
                MainManager.Instance.maze.ChooseSolver(Convert.ToInt32(sr.ReadLine())); // reads maze solver from file
                MainManager.Instance.ThreeD = Convert.ToBoolean(sr.ReadLine()); // reads current 3D setting from file
            }
            
        }


    }
}
