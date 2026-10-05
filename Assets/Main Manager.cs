using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MazeStuff;

class MainManager : MonoBehaviour
{
    public static MainManager Instance;
    public Maze maze = new Maze(10,10,1);
    public bool ThreeD;

    private void Awake() //makes sure only one instance of main mamnager exists at one time
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    public void Update()
    {
        
    }

    public void Log(string str)
    {
        Debug.Log(str);
    }


}
