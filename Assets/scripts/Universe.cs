using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Universe : MonoBehaviour
{
    
    //code started on 23/11/2018
    /// <summary>
    /// the seed of the universe - having the same seed will cause the exact same world to load twice
    /// </summary>
    public static string seed;
    public int numberOfSystemClusters;
    public int maxNumberOfSystemclusters = 100;//default 100
    public int minNumberOfSystemClusters = 30;// default 30
    int numberOfSystemsInCluster;
    public int minNumberOfSystemsInCluster = 3;
    public int maxNumberOfSystemsInCluster = 15;
    //distance links
    /// <summary>
    /// the minimum space gap there must be between systems
    /// </summary>
    public int minSystemLink = 3;
    /// <summary>
    /// the maximum space gap there must be between linked systems
    /// </summary>
    public int maxSystemLink = 8;
    /// <summary>
    /// the maximum distance a system can spawn from it's cluster's home position
    /// </summary>
    public int maxDistanceFromClusterOrigin = 29;
    /// <summary>
    /// the maximum space gap there must be between linked system clusters
    /// </summary>
    public int minsystemClusterLink = 90;
    /// <summary>
    /// the minimum space gap there must be between system clusters
    /// </summary>
    public int maxSystemClusterLink = 120;

    public Color SystemLink =  new Color(0.25f,0.25f,0.25f);
    public Color SystemClusterLink =  new Color(0.75f, 0.75f, 0.75f);
    int systemNumber= 0;

    
    private AssetBundle myLoadedAssetBundle;
    private string[] scenePaths;
    /// <summary>
    /// a list of classes to link every system to its gamobject. within this holds its coordinates, x and y
    /// </summary>
    public List<SystemObjectLink> systemObjectLinkArray = new List<SystemObjectLink>();
    public List<float> systemClusterCoOrdsX = new List<float>();
    public List<float> systemClusterCoOrdsY = new List<float>();

    public static List<SolarSystem> solarSystems;


    //run once during Startup.cs
    public void Initialise() {
        

        CameraScript.ChangeScene("SolarSystem");
            SolarSystem.PreGenSolarSystemGameObjects();
        CameraScript.ChangeScene("Universe");
        GenerateUniverseSeed();
        GenerateUniverse();
    }

    //generates the universe by pregenerating each system and placing it in a web of solar systems
    private void GenerateUniverse()
    {
        float x = 0;//x position of the cluster
        float y = 0;//y position of the cluster
        float positionX = x;//x position of the system
        float positionY = y;//y position of the system


        float lastx = 0;
        float lasty = 0;
        numberOfSystemClusters = Random.Range(minNumberOfSystemClusters, maxNumberOfSystemclusters);
        
        for (int i = 0; i < numberOfSystemClusters; i++)
        {

            positionX = 0;
            positionY = 0;


            //generating system clusters
            bool CanValidPositionBeFound = GenerateNewSystemClusterPosition(ref x, ref y, i);
            if (CanValidPositionBeFound == true)
            {
                createSystemCluster(x, y, ref positionX, ref positionY);

                /*/drawing lines between each system cluster
                if (i > 0)
                {
                    GameObject LinerendererForCluster = new GameObject("LinerendererForCluster" + i);
                    LineRenderer lr = LinerendererForCluster.AddComponent<LineRenderer>();
                    lr.sortingOrder = -2;
                    lr.material.color = SystemClusterLink;
                    lr.SetPosition(0, new Vector2(x, y));
                    lr.SetPosition(1, new Vector2(lastx, lasty));
                    lr.startWidth = 0.5f;
                    lr.endWidth = 0.5f;

                }*/




                //loop fluff
                lastx = x;
                lasty = y;
            }
            else
            {
                break;
            }
        }
        Debug.Log("Universe Generation Complete!");
    }
    





    //====================================
    //==========system clusters===========
    //====================================

    //from the previous system cluster, it picks a direction and a distance to place the next system cluster
    private bool GenerateNewSystemClusterPosition(ref float x, ref float y, int systemClusterNumber)
    {
        bool CancelOperation = false;
        if (systemClusterNumber != 0)
        {
            int errorEscape = 0;
            for (int validationLoop = 0; validationLoop < 1; validationLoop++)
            {
            //### Randomising position ###
                //Offset System from previous system.
                int bearing = Random.Range(1, 360);
                int distance = Random.Range(minsystemClusterLink, maxSystemClusterLink);
                x = Mathf.Sin((float)bearing) * distance + x;
                y = Mathf.Cos((float)bearing) * distance + y;
            //### Validation ###
                //check if not too close to other clusters
                for (int validationI = 0; validationI < systemClusterCoOrdsX.Count; validationI++)
                {
                    if (((x - systemClusterCoOrdsX[validationI]) < minsystemClusterLink) && ((x - systemClusterCoOrdsX[validationI]) > -minsystemClusterLink))
                    {
                        if (((y - systemClusterCoOrdsY[validationI]) < minsystemClusterLink) && ((y - systemClusterCoOrdsY[validationI]) > -minsystemClusterLink))
                        {
                            if (errorEscape < 40)
                            {
                                errorEscape++;
                                validationLoop = -1;
                            }
                            else
                            {
                                CancelOperation = true;
                            }
                            //if the generated sytem cluster position does not fit the requirements, then it shoudl be generated again.
                        }
                    }
                }
                //check that cluster is not too far away from universe origin
                if(x+y > 5000)
                {
                    if (errorEscape < 40)
                    {
                        errorEscape++;
                        validationLoop = -1;
                    }
                    else
                    {
                        CancelOperation = true;
                        break;
                    }
                }
            }
            if (CancelOperation == true)
            {
                return false;
            }
            return true;
        }
        return true;
    }

    private void createSystemCluster(float clusterX, float clusterY, ref float systemX, ref float systemY)//x and y = location of system cluster, positionX and positionY = position of new system being generated
    {
        numberOfSystemsInCluster = Random.Range(minNumberOfSystemClusters, maxNumberOfSystemsInCluster);    //generate random number for how many systems should be genereated in this cluster
        GameObject LastSystem = null;   //variable to store previous system that has been generated. this is used to draw the lines between each system
        for (int ii = 0; ii < numberOfSystemsInCluster; ii++)   //loop to create systems in the cluster
        {
            //pick the position to make a new system
            bool wasValidPosFound = GenerateNewSystemPosition(ref systemX, ref systemY, ii, clusterX, clusterY);
            // if a valid position could be found:
            if (wasValidPosFound == true)
            {
                GameObject currentSystem = CreateSystem(systemX, systemY, clusterX, clusterY);

                //draw line between current system and last generated system
                if (ii > 0)
                {
                    LineRenderer lr = currentSystem.AddComponent<LineRenderer>();

                    lr.sortingOrder = -1;
                    lr.material.color = SystemLink;
                    lr.SetPosition(0, LastSystem.transform.localPosition);
                    lr.SetPosition(1, currentSystem.transform.localPosition);
                    lr.startWidth = 0.3f;
                    lr.endWidth = 0.3f;

                }
                // check for close  systems
                for (int i = 0; i < systemObjectLinkArray.Count; i++)
                {
                    if ( Mathf.Sqrt(Mathf.Pow((systemX+clusterX-systemObjectLinkArray[i].x),2) + Mathf.Pow(systemY+clusterY-systemObjectLinkArray[i].y, 2) )< maxSystemLink/8 )
                    {
                        if ((systemX + clusterX - systemObjectLinkArray[i].x) + (systemY + clusterY - systemObjectLinkArray[i].y) > -maxSystemLink/8)
                        {
                            GameObject systemLink = new GameObject("system link");
                            LineRenderer lr = systemLink.AddComponent<LineRenderer>();

                            lr.sortingOrder = -1;
                            lr.material.color = SystemLink;
                            lr.SetPosition(0, new Vector2(systemObjectLinkArray[i].x, systemObjectLinkArray[i].y));
                            lr.SetPosition(1, new Vector2(systemX + clusterX, systemY + clusterY));
                            lr.startWidth = 0.3f;
                            lr.endWidth = 0.3f;
                        }
                    }
                }

                //loop fluff
                systemNumber++;
                LastSystem = currentSystem;
            }
            // if a valid position could not be found
            else
            {
                break;
                //if a valid position to create the solar system could not be found then abort this system cluster and move on to the next
            }
        }
        //add the cluster's coordinates to the list and move on to the next
        systemClusterCoOrdsX.Add(clusterX);
        systemClusterCoOrdsY.Add(clusterY);
    }




    //============================
    //========== systems==========
    //============================

    /// <summary>
    /// this function is responsible for randomly(seeded) picking a valid locaton to spawn the next system
    /// </summary>
    /// <param name="systemX">The x coordinate of the last system is passed in. the x coordinate of the new system is returned</param>
    /// <param name="systemY">The y coordinate of the last system is passed in. the y coordinate of the new system is returned</param>
    /// <param name="systemsInClusterIndex"> the number of the system in the cluster./param>
    private bool GenerateNewSystemPosition(ref float systemX, ref float systemY, int systemsInClusterIndex, float clusterX, float clusterY)
    {
        bool CancelOperation = false;
        int errorEscape = 0;
        if (systemsInClusterIndex != 0)
        {
            for (int validationLoop = 0; validationLoop < 1;validationLoop++)
            {
            // ### randomising position ###

                //Offset System from previous system.
                int bearing = Random.Range(1, 360);
                int distance = Random.Range(minSystemLink, maxSystemLink);
                systemX = Mathf.Sin((float)bearing) * distance + systemX;
                systemY = Mathf.Cos((float)bearing) * distance + systemY;

            // ### validate placement
                //make sure that the system isnt too close to any others, if it is, redraw it somewhere else
                for (int validationI = 0; validationI < systemObjectLinkArray.Count; validationI++)
                {
                    if ((((systemX+clusterX) - systemObjectLinkArray[validationI].x) < minSystemLink) && (((systemX + clusterX) - systemObjectLinkArray[validationI].x) > -minSystemLink))
                    {
                        if ((((systemY+clusterY) - systemObjectLinkArray[validationI].y) < minSystemLink) && (((systemY + clusterY) - systemObjectLinkArray[validationI].y) > -minSystemLink))
                        {

                            //if an invalid position is chosec, then a new one will be generated
                            //... if a valid position cannot be found after 40 iterations then the operation is aborted
                            //... causing the universe to stop attempting to generate systems in this cluster and instead move onto the next cluster.

                            if(errorEscape < 40)
                            {
                                errorEscape++;
                                validationLoop = -1;
                            }
                            else
                            {
                                CancelOperation = true;
                            }
                        }
                    }
                }
                //make sure planet isnt too far away from the origin of the cluster it belongs to
                if(Mathf.Sqrt(Mathf.Pow( systemX,2) + Mathf.Pow(systemY, 2)) > 20)
                {


                    //if an invalid position is chosec, then a new one will be generated
                    //... if a valid position cannot be found after 40 iterations then the operation is aborted
                    //... causing the universe to stop attempting to generate systems in this cluster and instead move onto the next cluster.
                    if (errorEscape < 40)
                    {
                        errorEscape++;
                        validationLoop = -1;
                    }
                    else
                    {
                        CancelOperation = true;
                        break;
                    }
                }
                    
            }
            //return false if the operation has been cancelled at any point
            if(CancelOperation== true)
            {
                return false;
            }
            //... otherwise return true
            return true;
        }
        return true;
    }

    /// <summary>
    /// this function is what actually creates the system, places the gameobject on the screen and links it to the system's class
    /// </summary>
    /// <param name="systemX"> the x coordinate offset from clusterX to determine the global position of the system to be created</param>
    /// <param name="systemY"> the y coordinate offset from clusterY to determine the global position of the system to be created</param>
    /// <param name="clusterX"> the X coordinate of the system cluster, combined with systemX makes the global X position of the system</param>
    /// <param name="clusterY"> the Y coordinate of the system cluster, combined with systemY makes the global Y position of the system</param>
    /// <returns></returns>
    private GameObject CreateSystem(float systemX, float systemY, float clusterX, float clusterY)//x and y = location of system cluster, positionX and positionY = position of new system being generated
    {
        //###object stuff
        GameObject newSystem = GameObject.CreatePrimitive(PrimitiveType.Sphere);//generate sphere on screen
        newSystem.name = "solarsystem";
        newSystem.transform.position = new Vector2(systemX+clusterX, systemY+clusterY);//set spheres position to the system position
        newSystem.tag = "SolarSystem";
        //####class stuff
        SolarSystem newSystemClass = new SolarSystem(systemNumber);

        //###finalise class stuff
        SystemObjectLink newSystemObjectLink = newSystem.AddComponent<SystemObjectLink>();
        newSystemObjectLink.x = systemX+clusterX;
        newSystemObjectLink.y = systemY+clusterY;
        newSystemObjectLink.LinkedClass = newSystemClass;

        systemObjectLinkArray.Add(newSystemObjectLink); // add the system i just created to an array so they can be kept track of later
        return newSystem;
    }

    void GenerateUniverseSeed()
    {
        seed = "8202773224653578656576127305560212814355305003377573328711035856783216170278424866205773227824008806207581883867506484254062415074254574511367147801475000085526533533387830177431712473113161136748880357682257544867644844010376523223187312715428064046730507";
        //seed = Seed.generateNew();
        string shortSeed = Seed.shorten(Universe.seed);
        Random.InitState(int.Parse(shortSeed));
    }
    public static string getSeed()
    {
        string shortSeed = Seed.shorten(Universe.seed);
        return shortSeed;
    }
}
