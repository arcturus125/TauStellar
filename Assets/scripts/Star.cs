using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star
{

    //###static stuff
    public static int NoOfPlanetsLowerbound = 3;
    public static int NoOfPlanetsUpperbound = 12;
    public static double StarMassLowerbound = 0.45 * 333000;
    public static double StarMassUpperbound = 20UL * 333000;


    //star details on creation
    public double Mass;
    public int NoOfPlanets;
    public double luminosity;


    //star details
    public string StarName;

    //parent children stuff
    public Planet[] ChildPlanets;


    //star gameobject
    public GameObject StarObject;

    //orbit tracks stuff



    public Star(double MassParam, int NoOfPlanetsParam)
    {
        //star constants init
        Mass = MassParam;
        NoOfPlanets = NoOfPlanetsParam;
        if(Mass/333000 <= 2)
        {
            luminosity = System.Math.Pow((Mass / 333000), 4) * 3.828e26;
        }
        else
        {
            luminosity = System.Math.Pow((Mass / 333000), 3.5) * 3.828e26;
        }



        //circleGameObject = new GameObject[NoOfPlanets];

    }

    /// <summary>
    /// Used to link the Planets (class) to the star (class)
    /// </summary>
    /// <param name="PlanetsParam"></param>
    public void linkPlanets(Planet[] PlanetsParam)
    {
        ChildPlanets = PlanetsParam;
    }
}
