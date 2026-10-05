using System;
using MazeStuff;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEditor.MemoryProfiler;
using UnityEngine;


class ObjectManager : MonoBehaviour
{
    
   
    

    public GameObject PWall;
    public GameObject LWall;
    public GameObject UWall;
    public GameObject TWall;
    public GameObject SWall;
    public GameObject CWall;
    public GameObject FWall;
    public GameObject HWall;
    public Material endMaterial;
    //public Material translucentMaterial;
    public GameObject WallToSpawn;
    public GameObject StringToSpawn;
    public GameObject StringToSpawn2;
    public GameObject FloorToSpawn;
    private Maze maze;
    private System.Random rnd = new System.Random();

    private Quaternion Quaternion = Quaternion.identity;
    

    



    public int GetColumn(int node) // returns the column of the node
    {
        return node % maze.GetWidth();
    }

    public int GetRow(int node) // returns the row of the node
    {
        return (node - ((node / (maze.GetWidth() * maze.GetHeight()))) * (maze.GetWidth() * maze.GetHeight())) / maze.GetWidth();
    }

    public int GetDepth(int node) // returns the maze.GetDepth() of the node
    {
        return node / (maze.GetWidth() * maze.GetHeight());
    }
    IEnumerator SolveMaze(List<int> path) // adds the solver markers into the scene
    {


        //Wait for 4 seconds


        GameObject newString;
        
        for (int i = 0; i < path.Count; i++) // gets each node in the path of markers
        {
            newString = Instantiate(StringToSpawn, new Vector3(20 * GetColumn(path[i]), 4 + 20 * GetDepth(path[i]), 20 * GetRow(path[i])), Quaternion); // spawn a red square at that nodes in game position
            newString.name = $"String:{path[i]}"; // name the object
            //GameObject.Find(Convert.ToString(path[i])).GetComponent<Renderer>().material = redMaterial;
            yield return new WaitForSeconds((float)0.01); // wait  0.01 seconds
        }

        path = maze.TraceSolve();
        for (int i = 0; i < path.Count; i++) // gets each node in the path of markers
        {
            newString = Instantiate(StringToSpawn2, new Vector3(20 * GetColumn(path[i]), 4 + 20 * GetDepth(path[i]), 20 * GetRow(path[i])), Quaternion); // spawn a red square at that nodes in game position
            newString.name = $"String:{path[i]}"; // name the object
            //GameObject.Find(Convert.ToString(path[i])).GetComponent<Renderer>().material = greenMaterial;
            yield return new WaitForSeconds((float)0.01); // wait  0.01 seconds
        }




    }

    public void AddWalls3D()
    {
        GameObject newWall;
        for (int i = 0; i < maze.GetWidth(); i++)
        {
            for (int j = 0; j < maze.GetHeight(); j++)
            {
                for (int k = 0; k < maze.GetDepth(); k++)
                {
                    newWall = Instantiate(WallToSpawn, new Vector3(0 + i * 24, 12 + k * 24, 0 + j * 24), Quaternion);
                    newWall.name = ($"HWall{i},{j},{k}");
                    newWall = Instantiate(WallToSpawn, new Vector3(-12 + i * 24, 12 + k * 24, 12 + j * 24), Quaternion.Euler(0, 90, 0));
                    newWall.name = ($"VWall{i},{j},{k}");
                    newWall = Instantiate(FloorToSpawn, new Vector3(0 + i * 24, 2 + k * 24, 12 + j * 24), Quaternion.Euler(90, 0, 0));
                    newWall.name = ($"FWall{i},{j},{k}");
                }


            }

        }
        for (int i = 0; i < maze.GetHeight(); i++)
        {
            for (int k = 0; k < maze.GetDepth(); k++)
            {
                newWall = Instantiate(WallToSpawn, new Vector3(-12 + maze.GetWidth() * 24, 12 + k * 24, 12 + i * 24), Quaternion.Euler(0, 90, 0));
                newWall.name = ($"VWall{maze.GetWidth()},{i},{k}");

            }
            if (maze.GetDepth() != 1)
            {
                for (int k = 0; k < maze.GetWidth(); k++)
                {

                    newWall = Instantiate(FloorToSpawn, new Vector3(0 + k * 24, 2 + maze.GetDepth() * 24, 12 + i * 24), Quaternion.Euler(90, 0, 0));
                    newWall.name = ($"FWall{i},{k},{maze.GetDepth()}");

                }
            }



        }
        for (int i = 0; i < maze.GetWidth(); i++)
        {
            for (int k = 0; k < maze.GetDepth(); k++)
            {
                newWall = Instantiate(WallToSpawn, new Vector3(0 + i * 24, 12 + k * 24, 0 + maze.GetHeight() * 24), Quaternion);
                newWall.name = ($"HWall{i},{maze.GetHeight()},{k}");
            }
        }
    }

    public void StartSolve() // starts the SolveMaze coroutine
    {
        
        StartCoroutine(SolveMaze(maze.SolveMaze()));
        
    }

    public void NewWalls3D()
    {
        int objectType;
        
        List<int> connections = new List<int>();

        for (int i = 0; i < maze.GetAdjacency().Count; i++)
        {
            objectType = 1;
            //name = Convert.ToString(i) +" ";
            for (int j = 0; j < maze.GetAdjacency()[i].Count; j++) // gets the connections for eaach node
            {
                //Debug.Log(i + j);
                if (maze.GetAdjacency()[i][j] == 1)
                {
                    connections.Add(j);
                    
                }

            }

            for (int j = 0; j < connections.Count; j++) // hashes each node to get what shape its connections correspond to 
            {
                if (GetColumn(i) == GetColumn(connections[j]) && GetDepth(i) == GetDepth(connections[j])) // if they are in the same column
                {
                    if (i < connections[j])
                        objectType *= 5; // multiplies the object type by 5 if the connection is ahead
                    else
                        objectType *= 2; // multiplies the object type by 2 if the connection is behind

                }
                else if (GetRow(i) == GetRow(connections[j]) && GetDepth(i) == GetDepth(connections[j])) // if they are in the same row
                {
                    if (i < connections[j])
                        objectType *= 7; // multiplies by 7 if the connection is to the right
                    else
                        objectType *= 3; // multiplies by 3 if the connection is to the left
                }
                else if (GetRow(i) == GetRow(connections[j]) && GetColumn(i) == GetColumn(connections[j])) // if they are in the same maze.GetDepth()
                {
                    if (i < connections[j])
                        objectType *= 13; // multiplies by 13 if the connection is above
                    else
                        objectType *= 11; // multiples by 11 if the connection is below
                }
            }

            GameObject nodeToSpawn = null;

            switch (objectType) // instantiates the correct shape at the correct orientation depending on the hash of the objectType
            {
                case 2:
                    nodeToSpawn = Instantiate(FWall,new Vector3(GetColumn(i)*20,2+GetDepth(i)*20,GetRow(i)*20),UnityEngine.Quaternion.Euler(-90,180,0));
                    break;

                case 3:
                    nodeToSpawn = Instantiate(FWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(-90, 270, 0));
                    break;

                case 5:
                    nodeToSpawn = Instantiate(FWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(-90, 0, 0));
                    break;

                case 7:
                    nodeToSpawn = Instantiate(FWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(-90, 90, 0));
                    break;

                case 13:
                    nodeToSpawn = Instantiate(FWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(180, 0, 0));
                    break;

                case 11:
                    nodeToSpawn = Instantiate(FWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(180, 0, 180));
                    break;




                case 6:
                    nodeToSpawn = Instantiate(CWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(0, 180, 0));
                    break;

                case 14:
                    nodeToSpawn = Instantiate(CWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(0, 90, 0));
                    break;

                case 15:
                    nodeToSpawn = Instantiate(CWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(0, 270, 0));
                    break;

                case 35:
                    nodeToSpawn = Instantiate(CWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(0, 0, 0));
                    break;

                case 22:
                    nodeToSpawn = Instantiate(CWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(90, 90, 0));
                    break;

                case 33:
                    nodeToSpawn = Instantiate(CWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(90, 180, 0));
                    break;

                case 55:
                    nodeToSpawn = Instantiate(CWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(90, 270, 0));
                    break;

                case 77:
                    nodeToSpawn = Instantiate(CWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(90, 0, 0));
                    break;

                case 26:
                    nodeToSpawn = Instantiate(CWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(-90, 90, 0));
                    break;

                case 39:
                    nodeToSpawn = Instantiate(CWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(-90, 180, 0));
                    break;

                case 65:
                    nodeToSpawn = Instantiate(CWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(-90, 270, 0));
                    break;

                case 91:
                    nodeToSpawn = Instantiate(CWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(-90, 0, 0));
                    break;



                case 30:
                    nodeToSpawn = Instantiate(UWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(0, 270, 0));
                    break;

                case 42:
                    nodeToSpawn = Instantiate(UWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(0, 180, 0));
                    break;

                case 70:
                    nodeToSpawn = Instantiate(UWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(0, 90, 0));
                    break;

                case 105:
                    nodeToSpawn = Instantiate(UWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(0, 0, 0));
                    break;

                case 286:
                    nodeToSpawn = Instantiate(UWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(0, 180, 90));
                    break;

                case 429:
                    nodeToSpawn = Instantiate(UWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(0, 270, 90));
                    break;

                case 715:
                    nodeToSpawn = Instantiate(UWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(0, 0, 90));
                    break;

                case 1001:
                    nodeToSpawn = Instantiate(UWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(0, 90, 90));
                    break;

                case 110:
                    nodeToSpawn = Instantiate(UWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(90, 90, 0));
                    break;

                case 231:
                    nodeToSpawn = Instantiate(UWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(90, 0, 0));
                    break;

                case 130:
                    nodeToSpawn = Instantiate(UWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(-90, 90, 0));
                    break;

                case 273:
                    nodeToSpawn = Instantiate(UWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(-90, 0, 0));
                    break;



                case 330:
                    nodeToSpawn = Instantiate(LWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(90, 0, 0));
                    break;

                case 462:
                    nodeToSpawn = Instantiate(LWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(90, 270, 0));
                    break;

                case 770:
                    nodeToSpawn = Instantiate(LWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(90, 180, 0));
                    break;

                case 1155:
                    nodeToSpawn = Instantiate(LWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(90, 90, 0));
                    break;

                case 390:
                    nodeToSpawn = Instantiate(LWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(-90, 0, 0));
                    break;

                case 546:
                    nodeToSpawn = Instantiate(LWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(-90, 270, 0));
                    break;

                case 910:
                    nodeToSpawn = Instantiate(LWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(-90, 180, 0));
                    break;

                case 1365:
                    nodeToSpawn = Instantiate(LWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(-90, 90, 0));
                    break;

                case 858:
                    nodeToSpawn = Instantiate(LWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(0, 270, 0));
                    break;

                case 2002:
                    nodeToSpawn = Instantiate(LWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(0, 180, 0));
                    break;

                case 2145:
                    nodeToSpawn = Instantiate(LWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(0, 0, 0));
                    break;

                case 5005:
                    nodeToSpawn = Instantiate(LWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(0, 90, 0));
                    break;



                case 154:
                    nodeToSpawn = Instantiate(HWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(0, 180, 0));
                    break;

                case 66:
                    nodeToSpawn = Instantiate(HWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(0, 270, 0));
                    break;

                case 165:
                    nodeToSpawn = Instantiate(HWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(0, 0, 0));
                    break;

                case 385:
                    nodeToSpawn = Instantiate(HWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(0, 90, 0));
                    break;

                case 78:
                    nodeToSpawn = Instantiate(HWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(-90, 270, 0));
                    break;

                case 182:
                    nodeToSpawn = Instantiate(HWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(-90, 180, 0));
                    break;

                case 195:
                    nodeToSpawn = Instantiate(HWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(-90, 0, 0));
                    break;

                case 455:
                    nodeToSpawn = Instantiate(HWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(-90, 90, 0));
                    break;



                case 210:
                    nodeToSpawn = Instantiate(PWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(0, 0, 0));
                    break;

                case 1430:
                    nodeToSpawn = Instantiate(PWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(-90, 90, 0));
                    break;

                case 3003:
                    nodeToSpawn = Instantiate(PWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(-90, 0, 0));
                    break;



                


                case 10:
                    nodeToSpawn = Instantiate(TWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(-90, 0, 0));
                    break;

                case 21:
                    nodeToSpawn = Instantiate(TWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(-90, 90, 0));
                    break;

                case 143:
                    nodeToSpawn = Instantiate(TWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(0, 0, 0));
                    break;



                case 2310:
                    nodeToSpawn = Instantiate(SWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(180, 0, 0));
                    break;

                case 2730:
                    nodeToSpawn = Instantiate(SWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(0, 0, 0));
                    break;

                case 4290:
                    nodeToSpawn = Instantiate(SWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(-90, 90, 0));
                    break;

                case 6006:
                    nodeToSpawn = Instantiate(SWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(-90, 0, 0));
                    break;

                case 10010:
                    nodeToSpawn = Instantiate(SWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(-90, 270, 0));
                    break;

                case 15015:
                    nodeToSpawn = Instantiate(SWall, new Vector3(GetColumn(i) * 20, 2 + GetDepth(i) * 20, GetRow(i) * 20), UnityEngine.Quaternion.Euler(-90, 180, 0));
                    break;

                case 30030:
                    nodeToSpawn = new GameObject();
                    break;

            }
            if(nodeToSpawn == null)
                Debug.Log($"Error:{objectType}");
            connections.Clear();
            nodeToSpawn.name = Convert.ToString(i);
            if (i == (maze.GetWidth() * maze.GetHeight() * maze.GetDepth())-1)
            {
                nodeToSpawn.GetComponent<Renderer>().material = endMaterial;
            }

        }
    }

    public void NewWalls2D()
    {
        int objectType;
        List<int> connections = new List<int>();
        
        for (int i = 0; i < maze.GetAdjacency().Count; i++)
        {
            objectType = 1;
            for (int j = 0; j < maze.GetAdjacency()[i].Count; j++)
            {
                //Debug.Log(i + j);
                if (maze.GetAdjacency()[i][j] == 1)
                {
                    connections.Add(j);
                }

            }

            for (int j = 0; j < connections.Count; j++)
            {
                if (GetColumn(i) == GetColumn(connections[j]))
                {
                    if(i < connections[j])
                        objectType *= 5;
                    else
                        objectType *= 2;
                    
                }
                else if (GetRow(i) == GetRow(connections[j]))
                {
                    if (i < connections[j])
                        objectType *= 7;
                    else
                        objectType *= 3;
                }
            }

            GameObject nodeToSpawn = null;
            
            switch (objectType)
            {
                case 2:
                    nodeToSpawn = Instantiate(CWall, new Vector3(GetColumn(i)*20, 2, GetRow(i) * 20), Quaternion.Euler(-90,90,0));
                    break;

                case 3:
                    nodeToSpawn = Instantiate(CWall, new Vector3(GetColumn(i) * 20, 2, GetRow(i) * 20), Quaternion.Euler(-90, 180, 0));
                    break;

                case 5:
                    nodeToSpawn = Instantiate(CWall, new Vector3(GetColumn(i) * 20, 2, GetRow(i) * 20), Quaternion.Euler(-90, 270, 0));
                    break;

                case 7:
                    nodeToSpawn = Instantiate(CWall, new Vector3(GetColumn(i) * 20, 2, GetRow(i)*20), Quaternion.Euler(-90, 0, 0));
                    break;

                case 6:
                    nodeToSpawn = Instantiate(HWall, new Vector3(GetColumn(i) * 20, 2, GetRow(i) * 20), Quaternion.Euler(-90, 270, 0));
                    break;

                case 14:
                    nodeToSpawn = Instantiate(HWall, new Vector3(GetColumn(i) * 20, 2, GetRow(i) * 20), Quaternion.Euler(-90, 180, 0));
                    break;

                case 15:
                    nodeToSpawn = Instantiate(HWall, new Vector3(GetColumn(i) * 20, 2, GetRow(i) * 20), Quaternion.Euler(-90,0, 0));
                    break;

                case 35:
                    nodeToSpawn = Instantiate(HWall, new Vector3(GetColumn(i) * 20, 2, GetRow(i) * 20), Quaternion.Euler(-90, 90, 0));
                    break;

                case 30:
                    nodeToSpawn = Instantiate(LWall, new Vector3(GetColumn(i) * 20, 2, GetRow(i) * 20), Quaternion.Euler(-90, 0, 0));
                    break;

                case 42:
                    nodeToSpawn = Instantiate(LWall, new Vector3(GetColumn(i) * 20, 2, GetRow(i)*20), Quaternion.Euler(-90, 270, 0));
                    break;

                case 70:
                    nodeToSpawn = Instantiate(LWall, new Vector3(GetColumn(i) * 20, 2, GetRow(i) * 20), Quaternion.Euler(-90, 180, 0));
                    break;

                case 105:
                    nodeToSpawn = Instantiate(LWall, new Vector3(GetColumn(i) * 20, 2, GetRow(i) * 20), Quaternion.Euler(-90, 90, 0));
                    break;

                case 10:
                    nodeToSpawn = Instantiate(UWall, new Vector3(GetColumn(i) * 20, 2, GetRow(i)*20), Quaternion.Euler(-90, 90, 0));
                    break;

                case 21:
                    nodeToSpawn = Instantiate(UWall, new Vector3(GetColumn(i) * 20, 2, GetRow(i)*20), Quaternion.Euler(-90, 0, 0));
                    break;

                case 210:
                    nodeToSpawn = Instantiate(SWall, new Vector3(GetColumn(i) * 20, 2, GetRow(i) * 20), Quaternion.Euler(0, 0, 0));
                    break;


            }
            connections.Clear();
            nodeToSpawn.name = Convert.ToString(i);
            if (i == (maze.GetWidth() * maze.GetHeight() * maze.GetDepth()) - 1)
            {
                nodeToSpawn.GetComponent<Renderer>().material = endMaterial;
            }
        }
    }
    public void RemoveWalls3D()
    {
        Debug.Log($"Object3{maze.GetAdjacency().Count}");
        List<int> connections = new List<int>();
        for (int i = 0; i < maze.GetAdjacency().Count; i++)
        {
            
            for (int j = 0; j < maze.GetAdjacency()[i].Count; j++)
            {
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
        switch (rnd.Next(0, 6))
        {
            case 0:
                Destroy(GameObject.Find($"HWall{rnd.Next(0, maze.GetWidth())},{0},{rnd.Next(0, maze.GetDepth())}"));
                Debug.Log($"Destroyed Hwall bottommm");
                break;

            case 1:
                Destroy(GameObject.Find($"HWall{rnd.Next(0, maze.GetWidth())},{maze.GetHeight()},{rnd.Next(0, maze.GetDepth())}"));
                Debug.Log($"Destroyed Hwall toppp");
                break;

            case 2:
                Destroy(GameObject.Find($"VWall{0},{rnd.Next(0, maze.GetHeight())},{rnd.Next(0, maze.GetDepth())}"));
                Debug.Log($"Destroyed Vwall botommmm");
                break;

            case 3:
                Destroy(GameObject.Find($"VWall{maze.GetWidth()},{rnd.Next(0, maze.GetHeight())},{rnd.Next(0, maze.GetDepth())}"));
                Debug.Log($"Destroyed Vwall topppp");
                break;

            case 4:
                Destroy(GameObject.Find($"FWall{rnd.Next(0, maze.GetWidth())},{rnd.Next(0, maze.GetHeight())},{0}"));
                Debug.Log($"Destroyed Fwall bottommmmmmmmm");
                break;

            case 5:
                Destroy(GameObject.Find($"FWall{rnd.Next(0, maze.GetWidth())},{rnd.Next(0, maze.GetHeight())},{maze.GetDepth()}"));
                Debug.Log($"Destroyed Fwall toppppppp");
                break;
        }




    }
    // Start is called before the first frame update
    void Start()
    {
        maze = MainManager.Instance.maze;
        
        //maze.GetHeight() = maze.Getmaze.GetHeight()();
        
        
        //AddWalls3D();
        if(MainManager.Instance.ThreeD == true)
            NewWalls3D();
        else
            NewWalls2D();
        
        //RemoveWalls3D();
    }

    // Update is called once per frame
    void Update()
    {
        
        maze = MainManager.Instance.maze;
        
        
        
    }
}
