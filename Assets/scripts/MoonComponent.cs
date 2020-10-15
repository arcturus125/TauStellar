using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonComponent : MonoBehaviour{

    public Moon LinkedMoon;
    public static bool RenderMoons = true;
    public bool isActive;
    public Planet parentPlanet;


    static readonly double gravitationalConstant = 6.673e-11;
    static readonly double Pi = 3.1412;



    //#####   Maths code (do not change!)
    public void findOrbitalPeriodOfMoon(double MassOfParent)//using the mass of the parent Planet, find the orbital period of the Moon
    {
        
        //LinkedMoon.OrbitalPeriod = (2 * Pi) * Mathf.Sqrt((float)((LinkedMoon.OrbitRadius * LinkedMoon.OrbitRadius * LinkedMoon.OrbitRadius) /gravitationalConstant*(MassOfParent * 5.97e12)));

        LinkedMoon.OrbitalPeriod = ((2 * Pi * Mathf.Sqrt(Mathf.Pow(10, 27) * Mathf.Pow((float)LinkedMoon.OrbitRadius, 3)) * Mathf.Sqrt((float)gravitationalConstant * ((float)MassOfParent * (float)5.97e12)) / (gravitationalConstant * (MassOfParent * 5.97e12)))) / 1000000;
    }
public void findBearingFromParentToChild(double time, double orbitalPeriod)//use the orbital period of the Moon and the time to figure out an angle bearing from the parent Planet to the Moon
    {
        LinkedMoon.BearingFromParentToChild = ((((10 * time) % orbitalPeriod) / orbitalPeriod) * 360) + LinkedMoon.Epoch;
    }
    public void findx()//uses trig to find the x co-ordinate
    {
        LinkedMoon.X = Mathf.Sin((float)LinkedMoon.BearingFromParentToChild) * LinkedMoon.OrbitRadius + LinkedMoon.ParentPlanet.PlanetObject.transform.position.x;
    }
    public void findy()//uses trig to find the y co-ordinate
    {
        LinkedMoon.Y = Mathf.Cos((float)LinkedMoon.BearingFromParentToChild) * LinkedMoon.OrbitRadius + LinkedMoon.ParentPlanet.PlanetObject.transform.position.y;
    }
    public void positionMoon()
    {
        double test = LinkedMoon.Mass;
        findOrbitalPeriodOfMoon(LinkedMoon.ParentPlanet.Mass);//finds the orbital period of the Moon
        findBearingFromParentToChild(GameTime.time, LinkedMoon.OrbitalPeriod);// draw a line from the parent Planet to the Moon
                                                                          //from that line, use trigenometry to find the x and y co-ordinate of the Moon (for this instance of time)
        findx();
        findy();
        //move the Moon to said co-ordinates
        this.transform.parent.transform.position = new Vector3((float)LinkedMoon.X, (float)LinkedMoon.Y, 1);
    }
    void Update()
    {
        if (isActive == true)
        {
            positionMoon();
        }
    }
}
