using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Startup : MonoBehaviour
{
    //code started on 25/11/2018
    public GameObject TimeKeeper;
    public static Universe universe;
    public static bool sceneSetupFinished = false;
    public static bool sceneCreationFinished = false;
    void Start ()
    {
        if(sceneSetupFinished == false && sceneCreationFinished == true)
        {
            CameraSetup();
            UniverseSetup();
        }
        if (sceneCreationFinished == false)//makes sure this only runs once
        {
            Object.DontDestroyOnLoad(TimeKeeper);
            SceneSetup();
        }
    }
    void UniverseSetup()
    {
        universe = new Universe();
        universe.Initialise();
        sceneSetupFinished = true;
    }
    void CameraSetup()
    {
        CameraScript.SolarSystemCamera = new Camera();
        Scene universeScene = SceneManager.GetSceneByName("UniverseMap");
        SceneManager.SetActiveScene(universeScene);
        CameraScript.InitCameras();
        CameraScript.currentScene = "Universe";
        CameraScript.ChangeScene("Universe");
    }
    void SceneSetup()
    {
        SceneManager.LoadScene("UniverseMap");
        SceneManager.LoadScene("SystemMap", LoadSceneMode.Additive);

        sceneCreationFinished = true;
    }
}
