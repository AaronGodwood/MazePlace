using System;
using System.Collections;
using System.Collections.Generic;
using MazeStuff;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class HUD : MonoBehaviour
{
    public GameObject pausePanel;
    public GameObject savePanel;
    public Camera mainCamera;
    public Camera birdsEyeCamera;
    private AdjacencySaver adjacencySaver = new AdjacencySaver();
    public Material translucentMaterial;
    public Material defaultMaterial;
    public void SolveButton()
    {
        FindObjectOfType<ObjectManager>().StartSolve(); // calls StartSolve() method from object manager script
    }

    public void SaveButton()
    {
        savePanel.active = true; // activates save panel
        pausePanel.active = false; // deactivates pause panel
    }

    public void PauseButton()
    {
        pausePanel.active = true; // activates pause panel
    }

    public void ChangeViewButton()
    {
        if (mainCamera.enabled == true)
        {
            if (MainManager.Instance.ThreeD == false) // if the maze is 3D
                birdsEyeCamera.transform.position =
                    new Vector3(((float)MainManager.Instance.maze.GetWidth()) * 9, MainManager.Instance.maze.GetHeight() * 12 + MainManager.Instance.maze.GetWidth()*12, ((float)MainManager.Instance.maze.GetHeight()) * 9); // move the maera
            else
            {
                birdsEyeCamera.transform.position =
                    new Vector3((((float)MainManager.Instance.maze.GetWidth()) * 10), MainManager.Instance.maze.GetHeight() * 12 + MainManager.Instance.maze.GetWidth()*12 + 12 * MainManager.Instance.maze.GetDepth() + 50, (((float)MainManager.Instance.maze.GetHeight()) * 10)); // moves the camera
                for (int i = 0; i < MainManager.Instance.maze.GetHeight() * MainManager.Instance.maze.GetWidth() * MainManager.Instance.maze.GetDepth(); i++)
                {
                    try
                    {
                        GameObject.Find(Convert.ToString(i)).GetComponent<Renderer>().material = translucentMaterial;
                    }
                    catch { }


                }
            }
                
            mainCamera.enabled = false; // disabled main camera
            birdsEyeCamera.enabled = true; // enables birds eye camera
        }
        else
        {
            if (MainManager.Instance.ThreeD == true)
            {
                for (int i = 0; i < MainManager.Instance.maze.GetHeight() * MainManager.Instance.maze.GetWidth() * MainManager.Instance.maze.GetDepth(); i++)
                {
                    try
                    {
                        GameObject.Find(Convert.ToString(i)).GetComponent<Renderer>().material = defaultMaterial;
                    }
                    catch { }
                }
            }


            birdsEyeCamera.enabled = false; // disables birds eye camera
            mainCamera.enabled = true; // enables main camera 
        }

    }

    public void MainMenuButton()
    {
        SceneManager.LoadScene(0); // returns to main memory
    }

    public void BackButton()
    {
        pausePanel.active = false; // disables pause panel
    }

    public void SaveBackButton()
    {
        pausePanel.active = true;// enables pause panel
        savePanel.active = false; // disables save panel
    }

    public void Slot1Button()
    {
        adjacencySaver.SetFileName(0);
        adjacencySaver.Write(MainManager.Instance.maze);
    }

    public void Slot2Button()
    {
        adjacencySaver.SetFileName(1);
        adjacencySaver.Write(MainManager.Instance.maze);
    }

    public void Slot3Button()
    {
        adjacencySaver.SetFileName(2);
        adjacencySaver.Write(MainManager.Instance.maze);
    }

    public void Slot4Button()
    {
        adjacencySaver.SetFileName(3);
        adjacencySaver.Write(MainManager.Instance.maze);
    }
    // Start is called before the first frame update
    void Start()
    {
        mainCamera.enabled=true;
        birdsEyeCamera.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
