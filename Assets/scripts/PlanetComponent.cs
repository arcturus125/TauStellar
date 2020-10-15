using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetComponent : MonoBehaviour{
    //code started on 01/11/2018

    public Planet LinkedPlanet;
    //constants
    public static float trackWidth = 1;
    static readonly double Pi = 3.1412;
    static readonly double gravitationalConstant = 6.673e-11;
    public static int OrbitTrackSegments = 50;
    // Planet details


    [Header("Dev Details")]
    public string PlanetUnsortedName;
    public string sortedname =  "planets not sorted";
    public int planetIndex;
    public Material OrbitTrackMaterial;
    [Header("Planet details")]
    public double mass;
    public double planetarea;
    public double albeido;
    public string PlanetType;
    public bool HasHydrosphere;
    public string HydrosphereStatus;
    public double surfaceTemp;
    public double effectiveTemp;
    public double GreenhousePressure;
    public double totalAtmosphericPressure;

    void Start()
    {
        OrbitTrackMaterial = new Material(Shader.Find(" Diffuse"));
        OrbitTrackMaterial.color = new Color(130, 130, 130);
    }
    void Update()
    {
        
        if (SolarSystem.SystemGenerationComplete == true)
        {

            PositionPlanet(); //move the planet to the correct x y co-ordinates based on time
            DrawOrbitalTracks(); //draw new orvital tracks with the freshly reset LineRenderers

        }
        planetUpdateFunction();
        LinkedPlanet.UpdatePlanetComponentFigures();
    }


    public void findOrbitalPeriodOfPlanet(double MassOfParent)//using the mass of the parent star, find the orbital period of the planet
    {
        LinkedPlanet.OrbitalPeriod = ((2 * Pi * Mathf.Sqrt(Mathf.Pow(10, 27) * Mathf.Pow((float)LinkedPlanet.OrbitRadius, 3)) * Mathf.Sqrt((float)gravitationalConstant * ((float)MassOfParent * (float)5.97e12)) / (gravitationalConstant * (MassOfParent * 5.97e12))))/1000000;
    }
    public void findBearingFromParentToChild(double time, double OrbitalPeriod)//use the orbital period of the planet and the time to figure out an angle bearing from the parent star to the planet
    {
        LinkedPlanet.BearingFromParentToChild = ((((10 * time) % OrbitalPeriod)/OrbitalPeriod) * 360) + LinkedPlanet.Epoch;
    }
    public void findx()//uses trig to find the x co-ordinate
    {
        LinkedPlanet.X = Mathf.Sin((float)LinkedPlanet.BearingFromParentToChild) * LinkedPlanet.OrbitRadius;
    }
    public void findy()//uses trig to find the y co-ordinate
    {
        LinkedPlanet.Y = Mathf.Cos((float)LinkedPlanet.BearingFromParentToChild) * LinkedPlanet.OrbitRadius;
    }
    void PositionPlanet()
    {
        // use "findBearingFromParentToChild" draw a line from the parent star to the planet
        //from that line, use trigenometry to find the x and y co-ordinate of the planet (for this instance of time)
        //move the planet to said co-ordinates
        findOrbitalPeriodOfPlanet(LinkedPlanet.ParentStar.Mass);
        findBearingFromParentToChild(GameTime.time, LinkedPlanet.OrbitalPeriod);

        findx();
        findy();

        this.transform.parent.transform.position = new Vector3((float)LinkedPlanet.X, (float)LinkedPlanet.Y,-3);
    }
    void DrawOrbitalTracks()
    {
        if (LinkedPlanet.getChildMoons() != null)//chech if planet has moons
        {
            for (int ii = 0; ii < LinkedPlanet.getChildMoons().Length; ii++)//loop through all the moons
            {
                //move orbit tracks
                SolarSystem.moonOrbitTracks[LinkedPlanet.moonIDsStartAt + ii].transform.position =
                    new Vector3(SolarSystem.moonOrbitTracks[LinkedPlanet.moonIDsStartAt + ii].transform.position.x, SolarSystem.moonOrbitTracks[LinkedPlanet.moonIDsStartAt + ii].transform.position.y);
                //get the lonerenderer that shows circle on the screen
                LineRenderer line = SolarSystem.moonOrbitTracks[LinkedPlanet.moonIDsStartAt + ii].GetComponent<LineRenderer>();

                //change linerenderer properties
                line.positionCount = OrbitTrackSegments + 1;
                line.useWorldSpace = false;
                line.startWidth = 0.01f;
                line.endWidth = 0.01f;
                line.widthMultiplier = trackWidth /2;
                line.material = OrbitTrackMaterial;

                float x;
                float z;

                float angle = 20f;

                for (int i = 0; i < (OrbitTrackSegments + 1); i++)
                {
                    x = Mathf.Sin(Mathf.Deg2Rad * angle) * (float)LinkedPlanet.childMoons[ii].OrbitRadius;
                    z = Mathf.Cos(Mathf.Deg2Rad * angle) * (float)LinkedPlanet.childMoons[ii].OrbitRadius;

                     //line.SetPosition(i, new Vector3(x, z, 0));
                    line.SetPosition(i, new Vector3(x + LinkedPlanet.PlanetObject.transform.position.x, z + LinkedPlanet.PlanetObject.transform.position.y, 0));
                    //// REMOVED: inheritence used to keep tracks in place


                    angle += (360f / OrbitTrackSegments);
                }
            }
        }
    }



    private void planetUpdateFunction()
    {
        LinkedPlanet.UpdatePlanet(GameTime.time);
    }
}
