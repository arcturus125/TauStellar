using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moon {

    //moon details on creation
    public double Mass = 1;
    public double OrbitRadius;
    public int moonID;

    //moon details
    public string MoonName;
    public int Epoch;

    //moon movement details
    public double BearingFromParentToChild;
    public double OrbitalPeriod;
    public double X;
    public double Y;

    //moon object
    public GameObject MoonObject;

    //parent child stuff
    public Planet ParentPlanet { get; set; }
    public Star ParentStar;
    public Moon[] moonFamily;



    public Moon(Star ParentStarParam, double MassParam, double OrbitRadiusParam, int EpochParam, int moonIndex)
    {
        moonID = moonIndex;
        ParentStar = ParentStarParam;
        Mass = MassParam;
        OrbitRadius = OrbitRadiusParam;
        Epoch = EpochParam;
    }
    public GameObject getObject()
    {
        return SolarSystem.ALLmoonObjects[moonID];
    }
    public MoonComponent getComponent()
    {
        return SolarSystem.ALLmoonObjects[moonID].GetComponent<MoonComponent>();
    }
}
