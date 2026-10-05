using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using MazeStuff;
using UnityEngine.UI;
using TMPro;
using System;
using System.Net.Http.Headers;

public class Menu : MonoBehaviour
{
    //private Maze maze = new Maze(10,10,10);
    //private ObjectManager objectManager;
    public Slider widthSlider;
    public Slider heightSlider;
    public Slider depthSlider;
    public Toggle threeDToggle;
    //private bool threeDToggleValue;
    public TextMeshProUGUI widthText;
    public TMP_Dropdown solverDropDown;
    public TMP_Dropdown generatorDropdown;
    public TextMeshProUGUI heightText;
    public TextMeshProUGUI depthText;
    private SettingsSaver settingsSaver = new SettingsSaver("C:\\Users\\azmcl\\MazePlace\\Assets\\settings.txt");
    

    
    public void GenerateButton()
    {
        MainManager.Instance.maze.GenerateNewMaze(); // generates maze

        SceneManager.LoadScene(1); // loads maze visualising scene
        
    }
    
    public void LoadButton()
    {
        SceneManager.LoadScene(3); // loads Load Manager Scene
    }

    public void OptionsButton()
    {
        
        SceneManager.LoadScene(2); // loads options menu
        

    }

    public void QuitButton()
    {
        Application.Quit(); // leaves application
        UnityEditor.EditorApplication.isPlaying = false;
    }

    public void SaveOptionsButton()
    {
        settingsSaver.WriteToFile(); // writes current settings to file
        SceneManager.LoadScene(0); // loads main menu
    }

    public void GeneratorDropDown(int index)
    {
        switch(index)
        {
            case 0:
                MainManager.Instance.maze.ChooseGenerator(0);  // selects kruskal's algorithm generator
                break;

            case 1:
                MainManager.Instance.maze.ChooseGenerator(1);  // selects depth first search generator
                break;

            case 2:
                MainManager.Instance.maze.ChooseGenerator(0); // selects wilson's algorithm generator
                break;

        }
    }

    public void SolverDropDown(int index)
    {
        switch (index)
        {
            case 0:
                MainManager.Instance.maze.ChooseSolver(0); // selects depth first search solver
                break;

            case 1:
                MainManager.Instance.maze.ChooseSolver(1); // selects greedy best first search solver
                break;

        }
    }

    public void ThreeDToggle()
    {
        if (threeDToggle.isOn == false) // if toggle is currently true switch it to false and set depth to 1 and disable changing of depth
        {
            
            depthSlider.value = 1;
            depthSlider.enabled = false;
            MainManager.Instance.ThreeD = false;
        }
        else // if toggle is currently false switch it to true and re-enable changing of depth
        {
            
            depthSlider.enabled = true;
            MainManager.Instance.ThreeD = true;
        }
            
        


    }
    
    public void UpdateSettings()
    {
        widthText.text = "Maze Width: " + widthSlider.value;
        MainManager.Instance.maze.SetWidth(Convert.ToInt32(widthSlider.value));
        heightText.text = "Maze Height: " + heightSlider.value;
        MainManager.Instance.maze.SetHeight(Convert.ToInt32(heightSlider.value));
        depthText.text = "Maze Depth: " + depthSlider.value;
        MainManager.Instance.maze.SetDepth(Convert.ToInt32(depthSlider.value));
    }

    
    
        
    
    // Start is called before the first frame update
    void Start()
    {
        settingsSaver.ReadFromFile(); // reads setting from file and loads them as current settings
        
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            widthSlider.value = MainManager.Instance.maze.GetWidth();
            heightSlider.value = MainManager.Instance.maze.GetHeight();
            depthSlider.value = MainManager.Instance.maze.GetDepth();
            solverDropDown.value = MainManager.Instance.maze.GetSolver();
            generatorDropdown.value = MainManager.Instance.maze.GetGenerator();
            threeDToggle.isOn = MainManager.Instance.ThreeD;

        }
        
    }
            

    // Update is called once per frame
    void Update()
    {
        if(SceneManager.GetActiveScene().buildIndex == 2 )
            UpdateSettings();
    }
}
