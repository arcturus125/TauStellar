using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarComponent : MonoBehaviour{
    //code started on 01/11/2018

    public Star LinkedStar;

    
    public Material orbitTrackMaterial;
    public static float trackWidth = 2; // used in Zoom
    public int OrbitTrackSegments = 999;
    public static bool runOnce = false;

    void Start()
    {
        //try move this elsewhere - nor relavant to star
        orbitTrackMaterial = Resources.Load("Materials/orbitTrack.mat", typeof(Material)) as Material;
    }
        
    // Update is called once per frame
    void Update () {
        OrbitTracks();
    }
    void OrbitTracks()
    {
        for (int ii = 0; ii < LinkedStar.NoOfPlanets; ii++)
        {
            SolarSystem.planetOrbitTracks[ii].transform.position = 
                new Vector3(SolarSystem.planetOrbitTracks[ii].transform.position.x, SolarSystem.planetOrbitTracks[ii].transform.position.y, 2);
            LineRenderer line = SolarSystem.planetOrbitTracks[ii].GetComponent<LineRenderer>();

            line.positionCount = OrbitTrackSegments + 1;
            line.useWorldSpace = false;
            line.widthMultiplier = trackWidth * CameraScript.scale/8;
            line.startWidth = trackWidth * SolarSystem.PlanetGraphicsScale;
            line.endWidth = trackWidth * SolarSystem.PlanetGraphicsScale;
            Material mat = new Material(Shader.Find(" Diffuse"));
            mat.color = Color.grey;
            line.material = mat;


            float x;
            float z;

            float angle = 20f;

            for (int i = 0; i < (OrbitTrackSegments + 1); i++)
            {
                x = Mathf.Sin(Mathf.Deg2Rad * angle) * (float)LinkedStar.ChildPlanets[ii].OrbitRadius;
                z = Mathf.Cos(Mathf.Deg2Rad * angle) * (float)LinkedStar.ChildPlanets[ii].OrbitRadius;

                line.SetPosition(i, new Vector3(x, z));

                angle += (360f / OrbitTrackSegments);
            }
        }
    }
}
