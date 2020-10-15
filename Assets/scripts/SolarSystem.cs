using System.Collections.Generic;
using UnityEngine;

public class SolarSystem : MonoBehaviour
{
    //code started on 01/11/2018
    //last updated on 21/12/2018

    //###Static variables - the stuff that is the same within every SolarSystem
    /// <summary>
    /// a reference to the solarsystem class that is currently active
    /// </summary>
    public static SolarSystem activeSolarSystem;

    //CAT =  catagory = the parent game object
    public static GameObject starobject;
    public static GameObject CATstarobject;
    public static GameObject[] CATplanetObjects = new GameObject[12];
    public static GameObject[] CATALLmoonObjects = new GameObject[480];
    public static GameObject[] planetObjects = new GameObject[12];
    public static GameObject[] ALLmoonObjects = new GameObject[480];
    public static GameObject[] planetOrbitTracks = new GameObject[12];
    public static GameObject[] moonOrbitTracks = new GameObject[480];

    /// <summary>
    /// the scale of the planets and all orbital bodies in the system
    /// </summary>
    public static float PlanetGraphicsScale = 1;
    /// <summary>
    /// a bool value that is only ever true when the solarsystem on the screen is fully generated
    /// </summary>
    public static bool SystemGenerationComplete = false;

    public static int starFontSize = 50;
    public static int planetFontSize = 40;
    public static int moonFontSize = 30;

    private double temp;
    private Moon tempMoon;
    //BubblePlanetSort Temp variable

    //### Non-static Variables - Stuff that is specific to this instance of SolarSystem
    int SolarSystemNumber;
    public Star star;
    public Planet[] planets;
    int numberOfPlanets;
    public Moon[] ALLmoons;
    /// <summary>
    /// has the system already been generated?
    /// </summary>
    public bool isExpectingEvent = false;
    public bool isSystemActive = false;


    //stored random numbers
   

    static int runAfterSystemCreated = -1;
    int moonID = 0;

    
    public static void PreGenSolarSystemGameObjects()
    {
        // CAT prefix denotes a gameobject acting as a parent catagory to other gameobjects, only the x and y co ordinates of these should ever be changed.

        CATstarobject = new GameObject();
        starobject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        starobject.transform.parent = CATstarobject.transform;
        starobject.name = "starObject";
        starobject.AddComponent<StarComponent>();
        GameObject starobjectText = new GameObject
        {
            name = "Text"
            
        };
        starobjectText.transform.localScale = new Vector3(0.1f, 0.1f);
        starobjectText.transform.parent = starobject.transform;
        starobjectText.AddComponent<TextMesh>();
        starobjectText.GetComponent<TextMesh>().fontSize = starFontSize;
        starobject.SetActive(false);
        starobject.transform.localScale = new Vector3(0.3f, 0.3f); 
        for (int i = 0; i < planetObjects.Length; i++)
        {
            planetObjects[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            CATplanetObjects[i] = new GameObject();
            planetObjects[i].transform.parent = CATplanetObjects[i].transform;
            planetObjects[i].name = "planetObject";
            CATplanetObjects[i].name = "Planet " + i + " slot";
            planetObjects[i].AddComponent<PlanetComponent>();
            GameObject planetObjectText = new GameObject
            {
                name = "Text"
            };
            planetObjectText.transform.localScale = new Vector3(0.2f, 0.2f);
            planetObjectText.transform.parent = planetObjects[i].transform;
            planetObjectText.AddComponent<TextMesh>();
            planetObjectText.GetComponent<TextMesh>().fontSize = planetFontSize;
            planetObjects[i].transform.localScale = new Vector3(0.3f, 0.3f); 
            planetObjects[i].SetActive(false);
        }
        for(int i = 0; i < ALLmoonObjects.Length; i++)
        {
            ALLmoonObjects[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            CATALLmoonObjects[i] = new GameObject();
            ALLmoonObjects[i].transform.parent = CATALLmoonObjects[i].transform;
            ALLmoonObjects[i].name = "moonObject";
            CATALLmoonObjects[i].name = "Moon" + i;
            ALLmoonObjects[i].AddComponent<MoonComponent>().isActive = false;
            GameObject moonObjectText = new GameObject
            {
                name = "Text"
            };
            moonObjectText.transform.localScale = new Vector3(0.3f, 0.3f);
            moonObjectText.transform.parent = ALLmoonObjects[i].transform;
            moonObjectText.AddComponent<TextMesh>();
            moonObjectText.GetComponent<TextMesh>().fontSize = moonFontSize;
            ALLmoonObjects[i].transform.localScale = new Vector3(0.3f, 0.3f);
            ALLmoonObjects[i].SetActive(false);
        }
    }
    void Start()
    {
        //initialise planet orbit tracks
        for (int i = 0; i < planetObjects.Length; i++)
        {
            planetOrbitTracks[i] = new GameObject();
            planetOrbitTracks[i].name = "orbit track for planet " + i;
            planetOrbitTracks[i].AddComponent<LineRenderer>();
            planetOrbitTracks[i].GetComponent<Renderer>().sortingOrder = -2;
            planetOrbitTracks[i].SetActive(false);
        }
        //initialise moon orbit tracks
        for (int i = 0; i < ALLmoonObjects.Length; i++)
        {
            moonOrbitTracks[i] = new GameObject();
            moonOrbitTracks[i].name = "orbit track for moon " + i;
            moonOrbitTracks[i].AddComponent<LineRenderer>();
            moonOrbitTracks[i].GetComponent<Renderer>().sortingOrder = -1;
            moonOrbitTracks[i].GetComponent<LineRenderer>().useWorldSpace = false;
            Material OrbitTrackMaterial = new Material(Shader.Find(" Diffuse"));
            OrbitTrackMaterial.color = new Color(130, 130, 130);
            moonOrbitTracks[i].GetComponent<LineRenderer>().material = OrbitTrackMaterial;
            //moonOrbitTracks[i].SetActive(false);
            ////DELETED: despite not ever being deactivated, i am happy with the way the game looks with it being active all the time
        }
    }
    void Update()
    {
        if (runAfterSystemCreated > 0)
        {
            runAfterSystemCreated--;
        }
        else if(runAfterSystemCreated == 0)
        {
            PostSystemCreation();
            runAfterSystemCreated--;
        }
    }

    /// <summary>
    /// instantiates a solarsystem and pre-generates all the random numbers so that all systems are the same
    /// </summary>
    /// <param name="SystemNumber"></param>
    public SolarSystem(int SystemNumber)
    {
        int NoOfTotalMoons = 0;
        SolarSystemNumber = SystemNumber;
        //pre-generate random numbers for the system
        RNG.systemName = Random.Range(0, FileManagement.getFileLength("Assets/Resources/starNames.txt"));
        RNG.starmass = Random.Range((int)Star.StarMassLowerbound, (int)Star.StarMassUpperbound);
        RNG.starPlanets = Random.Range(Star.NoOfPlanetsLowerbound, Star.NoOfPlanetsUpperbound);

        //set array length for planet arrays
        RNG.planetmass = new double[RNG.starPlanets];
        RNG.planetOrbitRadius = new double[RNG.starPlanets];
        RNG.planetNumberOfMoons = new int[RNG.starPlanets];
        RNG.planetEpoch = new int[RNG.starPlanets];
        RNG.planetAlbeido = new float[RNG.starPlanets];
        RNG.planetTypeChance = new int[RNG.starPlanets];
        RNG.planetChanceOfHydrosphere = new int[RNG.starPlanets];

        RNG.chanceOfOxygen = new int[RNG.starPlanets];
        RNG.pressureOfOxygen = new float[RNG.starPlanets];

        RNG.chanceOfNitrogen = new int[RNG.starPlanets];
        RNG.pressureOfNitrogen = new float[RNG.starPlanets];

        RNG.chanceOfCO2 = new int[RNG.starPlanets];
        RNG.pressureOfCO2 = new float[RNG.starPlanets];

        RNG.chanceOfMethane = new int[RNG.starPlanets];
        RNG.pressureOfMethane = new float[RNG.starPlanets];

        RNG.chanceOfHelium = new int[RNG.starPlanets];
        RNG.pressureOfHelium = new float[RNG.starPlanets];
        RNG.pressureOfHeliumIfGasGiant = new float[RNG.starPlanets];

        RNG.chanceOfHydrogen = new int[RNG.starPlanets];
        RNG.pressureOfHydrogen = new float[RNG.starPlanets];
        RNG.pressureOfHydrogenIfGasGiant = new float[RNG.starPlanets];

        RNG.chanceOfAmmonia = new int[RNG.starPlanets];
        RNG.pressureOfAmmonia = new float[RNG.starPlanets];

        RNG.chanceOfCO = new int[RNG.starPlanets];
        RNG.pressureOfCO = new float[RNG.starPlanets];

        RNG.chanceOfNobleGas = new int[RNG.starPlanets];
        RNG.pressureOfNobleGas = new float[RNG.starPlanets];
        
        RNG.pressureOfH2O = new float[RNG.starPlanets];

         
        //create lists for moon
        RNG.moonmass = new List<double>[RNG.starPlanets];
        RNG.moonOrbitRadius = new List<double>[RNG.starPlanets];
        RNG.moonEpoch = new List<int>[RNG.starPlanets];


        for (int i = 0; i < RNG.starPlanets; i++)
        {
            //pre-generate random numbers for each planet
            RNG.planetmass[i] = (double)Random.Range(0.05f, 20);
            float percentageOfMass = (float)(RNG.planetmass[i] * 150) * 0.4f;
            RNG.planetOrbitRadius[i] = Random.Range(-percentageOfMass, percentageOfMass) + RNG.planetmass[i] * 150;
            RNG.planetNumberOfMoons[i] = (int)Random.Range(0, (float)RNG.planetmass[i] * 2);
            RNG.planetEpoch[i] = Random.Range(0, 360);
            RNG.planetAlbeido[i] = Random.Range(0.3f, 0.5f);
            RNG.planetTypeChance[i] = Random.Range(0, 100);
            RNG.planetChanceOfHydrosphere[i] = Random.Range(0, 100);

            RNG.chanceOfOxygen[i] = Random.Range(0, 100);
            RNG.pressureOfOxygen[i] = Random.Range(0.01f,0.7f);

            RNG.chanceOfNitrogen[i] = Random.Range(0, 100);
            RNG.pressureOfNitrogen[i] = Random.Range(0.01f, 0.7f);

            RNG.chanceOfCO2[i] = Random.Range(0, 100);
            RNG.pressureOfCO2[i] = Random.Range(0.01f, 0.5f);

            RNG.chanceOfMethane[i] = Random.Range(0, 100);
            RNG.pressureOfMethane[i] = Random.Range(0.01f, 0.5f);

            RNG.chanceOfHelium[i] = Random.Range(0, 100);
            RNG.pressureOfHelium[i] = Random.Range(0.01f, 1f);
            RNG.pressureOfHeliumIfGasGiant[i] = Random.Range(100, 1000);

            RNG.chanceOfHydrogen[i] = Random.Range(0, 100);
            RNG.pressureOfHydrogen[i] = Random.Range(0.01f, 1f);
            RNG.pressureOfHydrogenIfGasGiant[i] = Random.Range(100, 1000);

            RNG.chanceOfAmmonia[i] = Random.Range(0, 100);
            RNG.pressureOfAmmonia[i] = Random.Range(0.01f, 2f);

            RNG.chanceOfCO[i] = Random.Range(0, 100);
            RNG.pressureOfCO[i] = Random.Range(0.01f, 2f);
            
            RNG.chanceOfNobleGas[i] = Random.Range(0, 100);
            RNG.pressureOfNobleGas[i] = Random.Range(0.01f, 2f);
            
            RNG.pressureOfOxygen[i] = Random.Range(0.01f, 0.05f);



            //moon random numbers
            RNG.moonmass[i] = new List<double>();
            RNG.moonOrbitRadius[i] = new List<double>();
            RNG.moonEpoch[i] = new List<int>();
            for(int ii = 0; ii < RNG.planetNumberOfMoons[i]; ii++)
            {
                RNG.moonmass[i].Add(Random.Range(0.05f, 20) * RNG.planetmass[i]);
                RNG.moonOrbitRadius[i].Add(Random.Range((float)RNG.planetmass[i] * 0.25f, Mathf.Sqrt((float)(RNG.planetmass[i] * (Mathf.Pow((float)RNG.planetOrbitRadius[i], 2)) / (float)RNG.starmass))));
                RNG.moonEpoch[i].Add(Random.Range(0, 360));
                NoOfTotalMoons++;
            }
        }
        ALLmoons = new Moon[480];
        //ALLmoons = new Moon[NoOfTotalMoons];
    }
    /// <summary>
    /// hides all stars, plantets moons and their orbit tracks
    /// </summary>
    public void Degenerate()
    {
        //star stuff
        starobject.SetActive(false);
        //planet stuff
        for (int i = 0; i < planetObjects.Length; i++)
        {
            //hide all plent objects and planet orbit tracks
            planetObjects[i].SetActive(false);
            planetOrbitTracks[i].SetActive(false);
        }
        //moon stuff
        for (int i = 0; i < ALLmoonObjects.Length; i++)
        {
            // hide all moon objects and moon orbit tracks
            ALLmoonObjects[i].SetActive(false);
            moonOrbitTracks[i].SetActive(false);
            // change the bool isActive in each moon class to false
            ALLmoonObjects[i].GetComponent<MoonComponent>().isActive = false;
        }
    }
    /// <summary>
    /// use to create a system that hasnt already been created. this is used for the first generation only for an intance, after this use Regenerate()
    /// </summary>
    public void Generate()
    {
        string Starname = GenerateStar();
        BubbleSortPlanets();
        BubbleSortMoons();
        //activeSolarSystem = ScriptToLoad;
        activeSolarSystem = this;

        SystemGenerationComplete = true;
        StarComponent.runOnce = true;
        runAfterSystemCreated = 1;
        Debug.Log("System generation complete!");
    }
    string GenerateStar()
    {
        //###pick a name for the star
            string Starname = FileManagement.readLine("Assets/Resources/starNames.txt", RNG.systemName);
        //###ready the gameobject with objectpooling
            starobject.SetActive(true);
            starobject.name = Starname;
            starobject.transform.parent.name = Starname;
            if (starobject.GetComponent<LineRenderer>() == null)
            {
                starobject.AddComponent<LineRenderer>();
            }
            starobject.GetComponent<Renderer>().sortingOrder = 3;
        //###create the star
            star = new Star(RNG.starmass, RNG.starPlanets)
            {
                StarName = Starname,
                //link class to gameobject
                StarObject = starobject
            };
            //link gameobject to class through the component
            starobject.GetComponent<StarComponent>().LinkedStar = star;
        //set text for star object
        starobject.GetComponentInChildren<TextMesh>().text = "  " + star.StarName;
        //link the planets to the star as you create them
        star.ChildPlanets = GeneratePlanets(Starname);
        return Starname;
    }
    Planet[] GeneratePlanets(string Starname)
    {
        //###create the planets array
            //get the number of planets from the parent star
            numberOfPlanets = star.NoOfPlanets;
            //create the array of planets
            planets = new Planet[numberOfPlanets];
            moonID = 0;

        for (int planetIndex = 0; planetIndex < numberOfPlanets; planetIndex++)
        {

            //###ready the object for the planet from the pool of objects
                planetObjects[planetIndex].SetActive(true);
                planetObjects[planetIndex].name = Starname + " - " + (planetIndex + 1);
                planetObjects[planetIndex].transform.parent.name = Starname + " - " + (planetIndex + 1);
                planetObjects[planetIndex].tag = "Planet";

                //increses size of collider so the planet can be clicked on more easily. Default =  0.5f
                planetObjects[planetIndex].GetComponent<SphereCollider>().radius = 0.7f;
                //sets the sorting order so that Planets are always displayed overtop moons
                planetObjects[planetIndex].GetComponent<Renderer>().sortingOrder = 2;

            //###create the planet class
            planets[planetIndex] = new Planet(
                star,
                RNG.planetmass[planetIndex],
                RNG.planetOrbitRadius[planetIndex],
                RNG.planetNumberOfMoons[planetIndex],
                RNG.planetEpoch[planetIndex],
                RNG.planetAlbeido[planetIndex],
                RNG.planetTypeChance[planetIndex],
                RNG.planetChanceOfHydrosphere[planetIndex],
                RNG.chanceOfOxygen[planetIndex],
                RNG.pressureOfOxygen[planetIndex],

                RNG.chanceOfNitrogen[planetIndex],
                RNG.pressureOfNitrogen[planetIndex],

                RNG.chanceOfCO2[planetIndex],
                RNG.pressureOfCO2[planetIndex],

                RNG.chanceOfMethane[planetIndex],
                RNG.pressureOfMethane[planetIndex],

                RNG.chanceOfHelium[planetIndex],
                RNG.pressureOfHelium[planetIndex],
                RNG.pressureOfHeliumIfGasGiant[planetIndex],

                RNG.chanceOfHydrogen[planetIndex],
                RNG.pressureOfHydrogen[planetIndex],
                RNG.pressureOfHydrogenIfGasGiant[planetIndex],

                RNG.chanceOfAmmonia[planetIndex],
                RNG.pressureOfAmmonia[planetIndex],

                RNG.chanceOfCO[planetIndex],
                RNG.pressureOfCO[planetIndex],

                RNG.chanceOfNobleGas[planetIndex],
                RNG.pressureOfNobleGas[planetIndex],

                RNG.pressureOfH2O[planetIndex]
                
                )
            {
                    moonIDsStartAt = moonID,
                    ID = planetIndex,
                    //set name of planet
                    PlanetName = Starname + " - " + (planetIndex + 1),
                    //link class to gameobject
                    PlanetObject = planetObjects[planetIndex]

                };
                //link gameobject to class through the component
                planetObjects[planetIndex].GetComponent<PlanetComponent>().LinkedPlanet = planets[planetIndex];

                
            

            //set display text of planet to PlanetName, made earlier.
            planetObjects[planetIndex].GetComponentInChildren<TextMesh>().text = "  " + planets[planetIndex].PlanetName;
                //link planet to child moons as they are created
                planets[planetIndex].childMoons = GenerateMoons(Starname, planetIndex, planets[planetIndex]);
            
        }

        //rename empte CAT slots to avoid confusion
        for(int resetPlanetCATnames = numberOfPlanets; resetPlanetCATnames < CATplanetObjects.Length; resetPlanetCATnames++)
        {
            planetObjects[resetPlanetCATnames].transform.parent.name = "Planet " + resetPlanetCATnames + " slot";
        }
        for(int resetMoonCATnames = moonID; resetMoonCATnames < CATALLmoonObjects.Length; resetMoonCATnames++)
        {
            ALLmoonObjects[resetMoonCATnames].transform.parent.name = "Moon" + resetMoonCATnames;
        }
        return planets;
    }
    Moon[] GenerateMoons(string Starname, int planetIndex, Planet tempParentPlanet)
    {
        //#####   create the moons
        int numberOfMoons = planets[planetIndex].NumberOfMoons;
        Moon[] moons = new Moon[numberOfMoons];
        for (int moonIndex = 0; moonIndex < numberOfMoons; moonIndex++)
        {
            ///
            /// moonIndex = index of local moon within the planet
            ///     use this when accessing the moon CLASS
            /// moonID = number of the moon relative to all moons
            ///     use this when accessing the moon OBJECT
            ///


            //### ready object for the moon from array of pooled objects
                ALLmoonObjects[moonID].SetActive(true);
                ALLmoonObjects[moonID].name = Starname + " - " + (planetIndex + 1) + " - " + (moonIndex + 1);
                ALLmoonObjects[moonID].transform.parent.name = Starname + " - " + (planetIndex + 1) + " - " + (moonIndex + 1);
                //increses size of collider so the moon can be clicked on more easily. Default =  0.5f
                ALLmoonObjects[moonID].GetComponent<SphereCollider>().radius = 0.5f;
                //changes sorting order of all moon gameobjects so they are rendered behind the planets
                ALLmoonObjects[moonID].GetComponent<Renderer>().sortingOrder = 1;
                //sets the moons to be active (if inactive the upfate function will not run)
                ALLmoonObjects[moonID].GetComponent<MoonComponent>().isActive = true;

            //###create moon class
            moons[moonIndex] = new Moon(star, RNG.moonmass[planetIndex][moonIndex], RNG.moonOrbitRadius[planetIndex][moonIndex], RNG.moonEpoch[planetIndex][moonIndex], moonID)
            {
                //set name of planet
                MoonName = Starname + " - " + (planetIndex + 1) + " - " + (moonIndex + 1),
                //link class to gameobject
                MoonObject = ALLmoonObjects[moonID],
                //link moon to parent planet
                ParentPlanet = tempParentPlanet
            };
            
            //link gameobject to class through component
            ALLmoonObjects[moonID].GetComponent<MoonComponent>().LinkedMoon = moons[moonIndex];
            //add moon to the array of all moons as it is created
            ALLmoons[moonID] = moons[moonIndex];
            //set displaytext of moon to the name we made earlier
            ALLmoonObjects[moonID].GetComponentInChildren<TextMesh>().text = "  " + moons[moonIndex].MoonName;
            //iterate moonID so that meach moon created must have a different ID.
            moonID++;
        }
        //return the array of moons just created to be stored in the parent class
        return moons;
        

    }
    /// <summary>
    /// runs the frame after the system is created
    /// used to edit variables that may not have been created yet
    /// </summary>
    void PostSystemCreation()
    {
        for (int i = 0; i < activeSolarSystem.numberOfPlanets; i++)
        {
            //make the orbit tracks visible
            planetOrbitTracks[i].SetActive(true);
        }

        //TODO
        /* OLD
        for (int i = 0; i < ALLmoonObjects.Length; i++)
        {
            //set the orbit traks to visible
            moonOrbitTracks[moonID].SetActive(true);
        }*/


        //NEW
        //moon stuff
        for (int i = 0; i < ALLmoonObjects.Length; i++)
        {
            // hide all moon objects and moon orbit tracks
            ALLmoonObjects[i].SetActive(true);
            moonOrbitTracks[i].SetActive(true);
            // change the bool isActive in each moon class to false
            ALLmoonObjects[i].GetComponent<MoonComponent>().isActive = true;
        }

        //TODO end
    }



    /// <summary>
    /// used to scale  any orbital bodies on the screen so they remain a constant width, called in Update()
    /// </summary>
    /// <param name="totalMoons"></param>
    public static void ScaleBodies()
    {
        if (SystemGenerationComplete == true)
        {
            starobject.transform.localScale = new Vector3(CameraScript.scale * 15, CameraScript.scale *15, CameraScript.scale * 15);
            for (int i = 0; i < activeSolarSystem.numberOfPlanets; i++)
            {
                planetObjects[i].transform.localScale = new Vector3(CameraScript.scale * 10, CameraScript.scale * 10, CameraScript.scale * 10);
            }
            for (int i = 0; i < activeSolarSystem.ALLmoons.Length; i++)
            {
                try
                {
                    activeSolarSystem.ALLmoons[i].MoonObject.transform.localScale = new Vector3(CameraScript.scale*10, CameraScript.scale*10, CameraScript.scale*10);
                }catch(System.Exception exc)
                {

                }
            }
        }
    }

    void BubbleSortPlanets() //Sorts the planets based on Orbit Radius from their star
    {
        for (int bubblePlanet1 = 0; bubblePlanet1 <= planets.Length-2; bubblePlanet1++)
        {
            for(int bubblePlanet2 = 0; bubblePlanet2 <= planets.Length-2; bubblePlanet2++) //bubble sort
            {
                if(planets[bubblePlanet2].OrbitRadius > planets[bubblePlanet2 + 1].OrbitRadius)
                {
                    temp = planets[bubblePlanet2 + 1].OrbitRadius;
                    planets[bubblePlanet2 + 1].OrbitRadius = planets[bubblePlanet2].OrbitRadius;
                    planets[bubblePlanet2].OrbitRadius = temp; //swaps the order of the 2 planets
                }
            }
        }
        for(int planetIndex = 0; planetIndex <= planets.Length-1; planetIndex++)
        {
            planets[planetIndex].PlanetName = star.StarName + " - " + (planetIndex + 1);
            planetObjects[planetIndex].GetComponentInChildren<TextMesh>().text = star.StarName + " - " + (planetIndex + 1); 
            //changes text to match order in orbit

        }
    }
    void BubbleSortMoons() //sorts the moons based on Orbit Radius from their planet
    {
        for (int planetIndexNo = 0; planetIndexNo < planets.Length; planetIndexNo++)
        {
            for (int bubbleMoon1 = 0; bubbleMoon1 < planets[planetIndexNo].childMoons.Length - 2; bubbleMoon1++)
            {
                for (int bubbleMoon2 = 0; bubbleMoon2 < planets[planetIndexNo].childMoons.Length - 2; bubbleMoon2++) //bubble sort
                {
                    if (planets[planetIndexNo].childMoons[bubbleMoon2].OrbitRadius > planets[planetIndexNo].childMoons[bubbleMoon2 + 1].OrbitRadius)
                    {
                        tempMoon = planets[planetIndexNo].childMoons[bubbleMoon2 + 1];
                        planets[planetIndexNo].childMoons[bubbleMoon2 + 1] = planets[planetIndexNo].childMoons[bubbleMoon2];
                        planets[planetIndexNo].childMoons[bubbleMoon2] = tempMoon;
                        //swaps order of the 2 moons
                    }
                }
            }
        }

        for (int planetNo = 0; planetNo < planets.Length; planetNo++) //per planet
        {
            for (int moonNo = 0; moonNo < planets[planetNo].childMoons.Length; moonNo++) //go through moons
            {
                planets[planetNo].childMoons[moonNo].MoonName = planets[planetNo].PlanetName + " - " + (moonNo + 1);
                planets[planetNo].childMoons[moonNo].MoonObject.GetComponentInChildren<TextMesh>().text = planets[planetNo].PlanetName + " - " + (moonNo + 1);
                //rename moons
            }
        }
    }
    
    
}