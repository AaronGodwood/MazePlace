using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MazeStuff;

public class LoadManger : MonoBehaviour
{

    private AdjacencySaver adjacencySaver = new AdjacencySaver();
    public void BackButton() // returns to main meny
    {
        SceneManager.LoadScene(0);
    }

    public void Slot1Button() // loads maze saved in slot 1 and loads maze visualiser scene
    {
        adjacencySaver.SetFileName(0);
        List<List<int>> adjacency = adjacencySaver.Read();
        MainManager.Instance.maze.SetWidth(adjacency[^1][0]);
        MainManager.Instance.maze.SetHeight(adjacency[^1][1]);
        MainManager.Instance.maze.SetDepth(adjacency[^1][2]);
        adjacency.Remove(adjacency[^1]);
        MainManager.Instance.maze.SetAdjacency(adjacency);
        SceneManager.LoadScene(1);
    }

    public void Slot2Button()// loads maze saved in slot 2 and loads maze visualiser scene
    {
        adjacencySaver.SetFileName(1);
        List<List<int>> adjacency = adjacencySaver.Read();
        MainManager.Instance.maze.SetWidth(adjacency[^1][0]);
        MainManager.Instance.maze.SetHeight(adjacency[^1][1]);
        MainManager.Instance.maze.SetDepth(adjacency[^1][2]);
        adjacency.Remove(adjacency[^1]);
        MainManager.Instance.maze.SetAdjacency(adjacency);
        SceneManager.LoadScene(1);
    }

    public void Slot3Button() // loads maze saved in slot 3 and loads maze visualiser scene
    {
        adjacencySaver.SetFileName(2);
        List<List<int>> adjacency = adjacencySaver.Read();
        MainManager.Instance.maze.SetWidth(adjacency[^1][0]);
        MainManager.Instance.maze.SetHeight(adjacency[^1][1]);
        MainManager.Instance.maze.SetDepth(adjacency[^1][2]);
        adjacency.Remove(adjacency[^1]);
        MainManager.Instance.maze.SetAdjacency(adjacency);
        SceneManager.LoadScene(1);
    }

    public void Slot4Button() // loads maze saved in slot 4 and loads maze visualiser scene
    {
        adjacencySaver.SetFileName(3);
        List<List<int>> adjacency = adjacencySaver.Read();
        MainManager.Instance.maze.SetWidth(adjacency[^1][0]);
        MainManager.Instance.maze.SetHeight(adjacency[^1][1]);
        MainManager.Instance.maze.SetDepth(adjacency[^1][2]);
        adjacency.Remove(adjacency[^1] );
        MainManager.Instance.maze.SetAdjacency(adjacency);
        SceneManager.LoadScene(1);
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
