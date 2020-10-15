using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraScript : MonoBehaviour
{
    //code started on 01/11/2018
    
    //movement controls
    //zoom controls
    /// <summary>
    /// reference to the Camera in the Universe scene
    /// </summary>
    public static Camera UniverseCamera =  new Camera();
    /// <summary>
    /// reference to the Camera in the SolarSystem scene
    /// </summary>
    public static Camera SolarSystemCamera =  new Camera();
    /// <summary>
    /// reference to the camera in the currently selected scene
    /// </summary>
    public static Camera SelectedCamera = new Camera();
    /// <summary>
    /// the target(GameObject) that the camera will lock on to if value is not null
    /// </summary>
    public GameObject Target;
    //public int ZoomSensitivity = 10; // currently doesnt do anything
    /// <summary>
    /// either "Universe" or "SolarSystem" depending on current scene
    /// </summary>
    public static string currentScene;
    /// <summary>
    /// the scale of the camera from its origional starting size. when you zoom out twice as much, this value will be twice as high.
    /// </summary>
    public static float scale = 1;
    public static bool FirstTimeSwitchingToSystemScene = true;
    public float cameraMoveSensitivity = 5;
    int defaultSystemCameraZoom = 200;
    int defaultUniverseCameraZoom = 5;

    // unity generated
    void Start()
    {
    }
    void Update()
    {
        Zoom();
    }
    void LateUpdate()
    {
        lockOnTarget();
        moveCamera();
    }
    void FixedUpdate()
    {
        DetectClick();
        KeyboardInputs();
    }

    void lockOnTarget()
    {
        if (Target)
        {
            transform.position = new Vector3(Target.transform.position.x, Target.transform.position.y, -10f);
        }
    }
    void Zoom()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f || Input.GetKeyDown(KeyCode.PageUp)) // forward
        {
            if (SelectedCamera.orthographicSize > 0.2f)
            {
                SelectedCamera.orthographicSize = SelectedCamera.orthographicSize - (SelectedCamera.orthographicSize * 0.2f);
                //SolarSystem.PlanetGraphicsScale = SelectedCamera.orthographicSize / 50;
                StarComponent.trackWidth -= StarComponent.trackWidth * 0.2f;
                PlanetComponent.trackWidth -= PlanetComponent.trackWidth * 0.2f;
                scale -= scale * 0.2f;
                SolarSystem.ScaleBodies();
            }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f || Input.GetKeyDown(KeyCode.PageDown)) // backwards
        {
            SelectedCamera.orthographicSize += SelectedCamera.orthographicSize * 0.2f; //error here
            //SelectedCamera.orthographicSize = SelectedCamera.orthographicSize + (SelectedCamera.orthographicSize * 0.2f);
            // SolarSystem.PlanetGraphicsScale = SelectedCamera.orthographicSize / 50;
            scale += scale * 0.2f;
            StarComponent.trackWidth += StarComponent.trackWidth * 0.2f;
            PlanetComponent.trackWidth += PlanetComponent.trackWidth * 0.2f;
            SolarSystem.ScaleBodies();
        }
        
        if(currentScene == "SolarSystem")
        {
            if (SelectedCamera.orthographicSize > 100)
            {
                for (int i = 0; i < SolarSystem.ALLmoonObjects.Length; i++)
                {
                    if( SolarSystem.ALLmoonObjects[i].GetComponent<MoonComponent>().isActive == true)
                    {
                        MoonComponent.RenderMoons = false;
                        SolarSystem.ALLmoonObjects[i].SetActive(false);
                    }
                }
            }
            if (SelectedCamera.orthographicSize <= 100)
            {
                for (int i = 0; i < SolarSystem.ALLmoonObjects.Length; i++)
                {
                    if (SolarSystem.ALLmoonObjects[i].GetComponent<MoonComponent>().isActive == true)
                    {
                        SolarSystem.ALLmoonObjects[i].SetActive(true);

                        MoonComponent.RenderMoons = true;
                    }
                }
            }
        }
    }
    void moveCamera()
    {
        if (Input.GetMouseButton(0))
        {
            if((Input.GetAxis("Mouse X") > 0)|| (Input.GetAxis("Mouse X") < 0))
            {
                Target = null;
            }
            if (currentScene == "SolarSystem")
            {
                transform.position = new Vector3(transform.position.x - (Input.GetAxis("Mouse X") * cameraMoveSensitivity * SolarSystem.PlanetGraphicsScale) * scale, transform.position.y - (Input.GetAxis("Mouse Y") * cameraMoveSensitivity * SolarSystem.PlanetGraphicsScale) * scale, -10f);
            }
            else
            {
                //TODO: copy if statement above but replace SolarSystem.PlanetGraphicsScale with the scale of the universe. you will need to create this in the Zoom() function

                transform.position = new Vector3(transform.position.x - (Input.GetAxis("Mouse X") * cameraMoveSensitivity * SolarSystem.PlanetGraphicsScale) * (scale/40), transform.position.y - (Input.GetAxis("Mouse Y") * cameraMoveSensitivity * SolarSystem.PlanetGraphicsScale) * (scale/40), -10f);

            }
        }
    }
    //comment and tidy the below functions
    void KeyboardInputs()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log(currentScene);
            if(currentScene == "SolarSystem")
            {
                SolarSystem.activeSolarSystem.Degenerate();
                SolarSystem.activeSolarSystem.isSystemActive = false;
                SolarSystem.SystemGenerationComplete = false;
                StarComponent.runOnce = false;
                ChangeScene("Universe");
                resetSystemCamera();
            }
        }
    }
    void DetectClick()
    {
        if (Input.GetMouseButtonDown(0) == true)
        {
            //##Turn the cursor position into world position
            Vector3 cursorPosition = Vector3.zero;
            if(currentScene == "Universe")
            {
                Vector3 mousePos = new Vector3(Input.mousePosition.x,Input.mousePosition.y,-10f);
                cursorPosition = UniverseCamera.ScreenToWorldPoint(mousePos);
            }
            else if(currentScene == "SolarSystem")
            {
                Vector3 mousePos;
                mousePos.x = Input.mousePosition.x;
                mousePos.y = Input.mousePosition.y;
                mousePos.z = -1000f;
                cursorPosition = SolarSystemCamera.ScreenToWorldPoint(mousePos);

            }
            cursorPosition.z = -10000;
            RaycastHit[] hit = Physics.RaycastAll(cursorPosition, Vector3.forward,150000f);
            Debug.DrawRay(cursorPosition, Vector3.forward, Color.red, 1000f);
            ClickedOnObject(ref hit);
            
        }

    }
    void ClickedOnObject(ref RaycastHit[] hit)
    {
        for (int i = 0; i < hit.Length; i++)
        {
            if(currentScene == "Universe")
            {
                if (hit[i].collider.gameObject.tag == "SolarSystem")
                {
                    ChangeScene("SolarSystem");
                    resetSystemCamera();
                    SolarSystem clickedSolarSystem = hit[i].collider.gameObject.GetComponent<SystemObjectLink>().LinkedClass;
                    SolarSystem SolarSystemScript = SolarSystemCamera.GetComponent<SolarSystem>() as SolarSystem;
                    SolarSystemScript = clickedSolarSystem;
                    SolarSystemScript.Generate();
                    SolarSystem.ScaleBodies();
                    SolarSystemScript.isSystemActive = true;
                    break;
                }
            }
            if (currentScene == "SolarSystem")
            {

                Debug.Log("     " + hit[i].collider.gameObject.name);
                if (hit[i].collider != null)
                {
                    if (hit[i].collider.gameObject.tag == "Planet")//planets prioritised
                    {
                        Target = hit[i].collider.gameObject;
                        break;
                    }
                    else if (hit[i].collider.gameObject.tag == "Moon")//moons prioritised
                    {
                        Target = hit[i].collider.gameObject;
                        break;
                    }
                    Target = hit[i].collider.gameObject;
                }
            }
        }
    }

    void resetSystemCamera()
    {
        SolarSystemCamera.orthographicSize = defaultSystemCameraZoom;
        scale = 1;
        StarComponent.trackWidth = 2;
        PlanetComponent.trackWidth = 1;
        SolarSystem.ScaleBodies();
    }
    void resetUniverseCamera()
    {
        UniverseCamera.orthographicSize = defaultUniverseCameraZoom;
        scale = 1;
        StarComponent.trackWidth = 2;
        PlanetComponent.trackWidth = 1;
    }
    /// <summary>
    /// depending on input "Universe" or "SolarSystem" the function will disable one camera, enable the other, and update the selectedCamera.
    /// recommended that you use ChangeScene instead.
    /// </summary>
    /// <param name="cameraName"></param>
    public static void ChangeCamera(string cameraName)
    {
        if (cameraName == "Universe")
        {
            SelectedCamera = UniverseCamera;
            UniverseCamera.enabled = true;
            if (SolarSystemCamera)
            {
                SolarSystemCamera.enabled = false;
            }
        }

        else if (cameraName == "SolarSystem")
        {
            SelectedCamera = SolarSystemCamera;
            UniverseCamera.enabled = false;
            if (SolarSystemCamera)
            {
                SolarSystemCamera.enabled = true;
            }
        }
    }
    /// <summary>
    /// depending on "Universe" or "SolarSystem" input, the function will hide one scene and show another.
    /// it will also switch cameras.
    /// </summary>
    /// <param name="SceneName"></param>
    public static void ChangeScene(string SceneName)
    {
        if (SceneName == "Universe")
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("UniverseMap"));
            GameObject[] ScanSystemSceneGameObjects = SceneManager.GetSceneByName("SystemMap").GetRootGameObjects();
            for (int i = 0; i < ScanSystemSceneGameObjects.Length; i++)
            {

                if (ScanSystemSceneGameObjects[i].GetComponent<Camera>() == null) { }
                else { ScanSystemSceneGameObjects[i].SetActive(false); }

                if (ScanSystemSceneGameObjects[i].GetComponent<Light>() == null) { }
                else { ScanSystemSceneGameObjects[i].SetActive(false); }
                //##ARCHIVED CODE
                //ScanSystemSceneGameObjects[i].SetActive(false);

            }
            GameObject[] ScanUniverseSceneGameObjects = SceneManager.GetSceneByName("UniverseMap").GetRootGameObjects();
            for (int i = 0; i < ScanUniverseSceneGameObjects.Length; i++)
            {
                ScanUniverseSceneGameObjects[i].SetActive(true);

            }
            currentScene = "Universe";
            //Debug.Log("scene changed to universe");
            ChangeCamera(SceneName);
        }
        else if (SceneName == "SolarSystem")
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("SystemMap"));
            GameObject[] ScanUniverseSceneGameObjects = SceneManager.GetSceneByName("UniverseMap").GetRootGameObjects();
            for (int i = 0; i < ScanUniverseSceneGameObjects.Length; i++)
            {
                ScanUniverseSceneGameObjects[i].SetActive(false);

            }
            GameObject[] ScanSystemSceneGameObjects = SceneManager.GetSceneByName("SystemMap").GetRootGameObjects();
            for (int i = 0; i < ScanSystemSceneGameObjects.Length; i++)
            {
                //set camera and light to active
                
                if (ScanSystemSceneGameObjects[i].GetComponent<Camera>() == null) { }
                else { ScanSystemSceneGameObjects[i].SetActive(true); }

                if(ScanSystemSceneGameObjects[i].GetComponent<Light>() == null) { }
                else { ScanSystemSceneGameObjects[i].SetActive(true); }
                //#ARCHIVED_CODE
                //ScanUniverseSceneGameObjects[i].SetActive(true);

                currentScene = "SolarSystem";
                ChangeCamera(SceneName);
            }
            //Debug.Log("scene changed to solarsystem");
        }
    }
    /// <summary>
    /// used at the very start of the program to grab the cameras from each scene and store them in appropriate variables.
    /// without this the game cannot find one camera while in another scene.
    /// </summary>
    public static void InitCameras()
    {
        GameObject[] ScanUniverseSceneGameObjects = SceneManager.GetSceneByName("UniverseMap").GetRootGameObjects();
        for (int i = 0; i < ScanUniverseSceneGameObjects.Length; i++)
        {
            if(ScanUniverseSceneGameObjects[i].GetComponent<Camera>() == null)
            {

            }
            else
            {
                UniverseCamera = ScanUniverseSceneGameObjects[i].GetComponent<Camera>();
                SelectedCamera = ScanUniverseSceneGameObjects[i].GetComponent<Camera>();

            }

        }
        GameObject[] ScanSystemSceneGameObjects = SceneManager.GetSceneByName("SystemMap").GetRootGameObjects();
        for (int i = 0; i < ScanSystemSceneGameObjects.Length; i++)
        {
            if (ScanSystemSceneGameObjects[i].GetComponent<Camera>() == null)
            {

            }
            else
            {
                SolarSystemCamera = ScanSystemSceneGameObjects[i].GetComponent<Camera>();
            }

        }
        
    }
}