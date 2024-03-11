using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MazeStuff;
using graph;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class WallGeneration : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject WallToSpawn;
    public GameObject StringToSpawn;
    public GameObject FloorToSpawn;
    //Vector3 Vec = new Vector3(0, 0, 0);
    Quaternion Quaternion = Quaternion.identity;
    private int size = 3;
    private int width = 15;
    private int height = 20;
    private int depth = 1;
    private Graph maze;
    public System.Random rnd = new System.Random();
    //Quaternion Quaternion2 = Quaternion()
    void Start()
    {

        GenerateMaze();
        StartCoroutine(SolveMaze());


    }

    IEnumerator SolveMaze()
    {


        //Wait for 4 seconds


        GameObject newString;
        List<int> path = maze.GreedyBestFirstSolver(0, (width * height) - 1);
        for (int i = 0; i < path.Count; i++)
        {
            newString = Instantiate(StringToSpawn, new Vector3(24 * GetColumn(path[i]), 10, 10 + 24 * GetRow(path[i])), Quaternion);
            newString.name = $"String:{path[i]}";
            yield return new WaitForSeconds((float)0.2);
        }




    }


    public void GenerateMaze()
    {
        maze = new Graph(height, width);
        maze.RandomisedDepthFirst();
        AddWalls3D();
        RemoveWalls3D();
    }

    public void AddWalls()
    {
        GameObject newWall;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                newWall = Instantiate(WallToSpawn, new Vector3(0 + i * 24, 12, 0 + j * 24), Quaternion);
                newWall.name = ($"HWall{i},{j}");
                newWall = Instantiate(WallToSpawn, new Vector3(-12 + i * 24, 12, 10 + j * 24), Quaternion.Euler(0, 90, 0));
                newWall.name = ($"VWall{i},{j}");
                newWall = Instantiate(FloorToSpawn, new Vector3(0 + i * 24, 2, 12 + j * 24), Quaternion.Euler(90, 0, 0));
                newWall.name = ($"FWall{i},{j}");
            }

        }
        for (int i = 0; i < width; i++)
        {
            newWall = Instantiate(WallToSpawn, new Vector3(0 + i * 24, 12, 0 + height * 24), Quaternion);
            newWall.name = ($"HWall{i},{size}");
            
        }
        for(int i = 0; i < height;i++)
        {
            newWall = Instantiate(WallToSpawn, new Vector3(-12 + width * 24, 12, 10 + i * 24), Quaternion.Euler(0, 90, 0));
            newWall.name = ($"VWall{size},{i}");
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

    public void AddWalls3D()
    {
        GameObject newWall;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                for (int  k = 0; k < depth; k++)
                {
                    newWall = Instantiate(WallToSpawn, new Vector3(0 + i * 24, 12 + k * 24, 0 + j * 24), Quaternion);
                    newWall.name = ($"HWall{i},{j},{k}");
                    newWall = Instantiate(WallToSpawn, new Vector3(-12 + i * 24, 12 + k*24, 12 + j * 24), Quaternion.Euler(0, 90, 0));
                    newWall.name = ($"VWall{i},{j},{k}");
                    newWall = Instantiate(FloorToSpawn, new Vector3(0 + i * 24, 2 + k*24, 12 + j * 24), Quaternion.Euler(90, 0, 0));
                    newWall.name = ($"FWall{i},{j},{k}");
                }
                

            }

        }
        for (int i = 0; i < height; i++)
        {
            for(int k = 0; k < depth; k++)
            {
                newWall = Instantiate(WallToSpawn, new Vector3(-12 + width * 24, 12 + k * 24, 12 + i * 24), Quaternion.Euler(0, 90, 0));
                newWall.name = ($"VWall{size},{i},{k}");

            }
            if (depth != 1)
            {
                for (int k = 0; k < width; k++)
                {

                    newWall = Instantiate(FloorToSpawn, new Vector3(0 + k * 24, 2 + depth * 24, 12 + i * 24), Quaternion.Euler(90, 0, 0));
                    newWall.name = ($"FWall{i},{k},{size}");

                }
            }
            


        }
        for(int i = 0; i < width; i++)
        {
            for (int k = 0; k < depth; k++)
            {
                newWall = Instantiate(WallToSpawn, new Vector3(0 + i * 24, 12 + k * 24, 0 + height * 24), Quaternion);
                newWall.name = ($"HWall{i},{size},{k}");
            }
        }
    }


    public void RemoveWalls3D()
    {
        List<int> connections = new List<int>();
        for (int i = 0; i < maze.GetAdjacency().Count; i++)
        {
            for (int j = 0; j < maze.GetAdjacency()[i].Count; j++)
            {
                //Debug.Log(adjacency[i][j]);
                //Debug.Log(i + j);
                if (maze.GetAdjacency()[i][j] == 1)
                {
                    connections.Add(j);
                }
                
            }
            Debug.Log(connections.Count);
            for (int j = 0; j < connections.Count; j++)
            {
                //Debug.Log(j);
                if ((GetColumn(i) == GetColumn(connections[j])) && (GetDepth(i) == GetDepth(connections[j])) && i < connections[j])
                {
                    Destroy(GameObject.Find($"HWall{GetColumn(i)},{GetRow(connections[j])},{GetDepth(i)}"));
                    Debug.Log($"HWall{GetColumn(i)},{GetRow(connections[j])},{GetDepth(i)}");
                    Debug.Log("AAAAAA");
                }
                else if ((GetColumn(i) == GetColumn(connections[j])) && (GetDepth(i) == GetDepth(connections[j])) && i > connections[j]) //gre\t code!
                {
                    Destroy(GameObject.Find($"HWall{GetColumn(i)},{GetRow(i)},{GetDepth(i)}"));
                    Debug.Log($"HWall{GetColumn(i)},{GetRow(i)},{GetDepth(i)}");
                    Debug.Log("BBBBBB");
                }
                else if ((GetRow(i) == GetRow(connections[j])) && (GetDepth(i) == GetDepth(connections[j])) && i < connections[j])
                {
                    Destroy(GameObject.Find($"VWall{GetColumn(connections[j])},{GetRow(i)},{GetDepth(i)}"));
                    Debug.Log($"VWall{GetColumn(connections[j])},{GetRow(i)},{GetDepth(i)}");
                    Debug.Log("CCCCCCC");
                }
                else if (((GetRow(i)) == GetRow(connections[j])) && (GetDepth(i) == GetDepth(connections[j])) && i > connections[j])
                {
                    Destroy(GameObject.Find($"VWall{GetColumn(i)},{GetRow(i)},{GetDepth(i)}"));
                    Debug.Log($"VWall{GetColumn(i)},{GetRow(i)},{GetDepth(i)}");
                    Debug.Log("DDDDDD");
                }
                else if ((GetRow(i) == GetRow(connections[j])) && (GetColumn(i) == GetColumn(connections[j])) && i < connections[j])
                {
                    Destroy(GameObject.Find($"FWall{GetColumn(i)},{GetRow(i)},{GetDepth(connections[j])}"));
                    Debug.Log($"FWall{GetColumn(i)},{GetRow(i)},{GetDepth(connections[j])}");
                    Debug.Log("EEEEEEE");
                }
                else if ((GetRow(i) == GetRow(connections[j])) && (GetColumn(i) == GetColumn(connections[j])) && i > connections[j])
                {
                    Destroy(GameObject.Find($"FWall{GetColumn(i)},{GetRow(i)},{GetDepth(i)}"));
                    Debug.Log($"FWall{GetColumn(i)},{GetRow(i)},{GetDepth(i)}");
                    Debug.Log("FFFFFFF");
                }

            }
            connections.Clear();

        }
        switch(rnd.Next(0,6))
        {
            case 0:
                Destroy(GameObject.Find($"HWall{rnd.Next(0,width)},{0},{rnd.Next(0,depth)}"));
                Debug.Log($"Destroyed Hwall bottommm");
                break;

            case 1:
                Destroy(GameObject.Find($"HWall{rnd.Next(0,width)},{height},{rnd.Next(0, depth)}"));
                Debug.Log($"Destroyed Hwall toppp");
                break;

            case 2:
                Destroy(GameObject.Find($"VWall{0},{rnd.Next(0, height)},{rnd.Next(0, depth)}"));
                Debug.Log($"Destroyed Vwall botommmm");
                break;

            case 3:
                Destroy(GameObject.Find($"VWall{width},{rnd.Next(0, height)},{rnd.Next(0, depth)}"));
                Debug.Log($"Destroyed Vwall topppp");
                break;

            case 4:
                Destroy(GameObject.Find($"FWall{rnd.Next(0, width)},{rnd.Next(0, height)},{0}"));
                Debug.Log($"Destroyed Fwall bottommmmmmmmm");
                break;

            case 5:
                Destroy(GameObject.Find($"FWall{rnd.Next(0, width)},{rnd.Next(0, height)},{depth}"));
                Debug.Log($"Destroyed Fwall toppppppp");
                break;
        }
         

            

    }
    public void RemoveWalls()
    {
        
        //Destroy(GameObject.Find($"HWall{0},{1}"));
        //List<List<int>> adjacency = maze.GetAdjacency();
        List<int> connections = new List<int>();
        for (int i = 0; i < maze.GetAdjacency().Count; i++)
        {
            for (int j = 0; j < maze.GetAdjacency()[i].Count; j++)
            {
                //Debug.Log(adjacency[i][j]);
                //Debug.Log(i + j);
                if (maze.GetAdjacency()[i][j] == 1)
                {
                    connections.Add(j);
                }
            }
            //Debug.Log(connections.Count);
            for (int j = 0; j < connections.Count; j++)
            {
                //Debug.Log(j);
                if(GetColumn(i) == GetColumn(connections[j]) && i < connections[j])
                {
                    Destroy(GameObject.Find($"HWall{GetColumn(i)},{GetRow(connections[j])}"));
                    //Debug.Log($"HWall{i % size},{connections[j] / size}");
                }
                else if(GetColumn(i) == GetColumn(connections[j]) && i > connections[j])
                {
                    Destroy(GameObject.Find($"HWall{GetColumn(i)},{GetRow(i)}"));
                }
                else if ( GetRow(i) == GetRow(connections[j]) &&  i < connections[j])
                {
                    Destroy(GameObject.Find($"VWall{GetColumn(connections[j])},{GetRow(i)}"));
                }
                else if (GetRow(i) == GetRow(connections[j]) && i > connections[j])
                {
                    Destroy(GameObject.Find($"VWall{GetColumn(i)},{GetRow(i)}"));
                }
            }
            connections.Clear();
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
