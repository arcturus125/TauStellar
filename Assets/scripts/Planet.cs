using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet {



    // settings
    int howOftenTempCalculate = 10;


    // planet only generated once

    public int ID;
    public string PlanetName { get; set; }
    public int moonIDsStartAt;
    public double Mass { get; set; }
    public double OrbitRadius { get; set; }
    public int NumberOfMoons { get; set; }
    public double Epoch { get; set; }
    public float albeidoBase;
    public double planetArea;
    public string planetType;
    public string hydrosphereStatus = "Not Determined";
    public static double SBconstant = 5.670373e-8;
    public bool HasHydrosphere = false;


    //planet details consistently calcualted

    //temperature
    public double surfaceTemp;
    public double effectiveTemp;
    public double greenhousePressure;

    //Atmosphere
    public bool AtmosphereGenerated = false;
    public List<Atmosphere> PlanetAtmosphere = new List<Atmosphere>();
    public Atmosphere Oxygen;
    public Atmosphere Nitrogen;
    public Atmosphere CO2;
    public Atmosphere Methane;
    public Atmosphere Helium;
    public Atmosphere Hydrogen;
    public Atmosphere Ammonia;
    public Atmosphere CO;
    public Atmosphere NobleGas;
    public Atmosphere H2O;
    public double TotalAtmospericPressure = 0;
    public bool hasCO2 = false;
    public bool hasMethane = false;
    public bool hasH20 = false;


    //planet movement stuff (do not mess with)
    public double OrbitalPeriod { get; set; }
    public double BearingFromParentToChild { get; set; }
    public double X { get; set; }
    public double Y { get; set; }


    //planet gameobject details
    public GameObject PlanetObject;
    // parents and children
    public Star ParentStar;
    public Moon[] childMoons = new Moon[0];




    /// <summary>
    /// This function should only be called when a planet is being generated within the Solarsystem.cs GeneratePlanets Function.
    /// 
    /// 
    /// 
    /// while seemingly escessive, this long list of parameters is neccessarry for the game to work properly
    /// i know it is an eyesore, but you must put up with it because  debugging is much easier with the code layed out like this
    /// </summary>
    public Planet(
        Star ParentStarToOrbit,
        double MassParam,
        double OrbitRadiusParam,
        int NoOfMoonsParam,
        int EpochParam,
        float randomAlbeidoBase,
        int randomPlanetTypeChance,
        int chanceOfHydrospere,

        int chanceOfOxygen,
        float pressureOfOxygen,

        int chanceOfNitrogen,
        float pressureOfNitrogen,

        int chanceOfCO2,
        float pressureOfCO2,

        int chanceOfMethane,
        float pressureOfMethane,

        int chanceOfHelium,
        float pressureOfHelium,
        float pressureOfHeliumIfGasGiant,

        int chanceOfHydrogen,
        float pressureOfHydrogen,
        float pressureOfHydrogenIfGasGiant,

        int chanceOfAmmonia,
        float pressureOfAmmonia,

        int chanceOfCO,
        float pressureOfCO,

        int chanceOfNobleGas,
        float pressureOfNobleGas,

        float pressureOfH2O
        )
    {
        //parameter-to-variable assignment
        ParentStar = ParentStarToOrbit;
        Mass = MassParam;
        OrbitRadius = OrbitRadiusParam;
        NumberOfMoons = NoOfMoonsParam;
        Epoch = EpochParam;
        albeidoBase = 0; // only a rocky planet can have a measureable albeido

        //calculate planet area as the planet is generated
        double topFraction = (3 * MassParam);
        double BottomFracton = (4 / 3.142);
        double CuberootFraction = System.Math.Pow((topFraction / BottomFracton), (1f / 3f));
        planetArea = System.Math.Pow(CuberootFraction, 2);

        //Determining planet types 
        // DOCS: Taustellar > Mechanics > Universe > Planet > Planet Types
        //if mass is greater than 15 then planet must be a gas giant
        if (Mass > 15)
        {
            planetType = "Gas Giant";
            hydrosphereStatus = "Gas";
        }
        //calculateTemp(ParentStarToOrbit);
        //if mass is less than 15, it has an 80% chance to be molten and 20% chance to be rocky
        if (Mass <= 15)
        {
            // 20% chance
            if (randomPlanetTypeChance <= 20)
            {
                planetType = "Rocky";
            }
            else // 80% chance 
            {
                planetType = "Molten";
                hydrosphereStatus = "Gas";
            }
        }

        //if the planet is a gas giant
        if (planetType == "Gas Giant")
        {

        }

        //if the planet is molten
        if (planetType == "Molten")
        {

        }

        //if the planet is rocky
        if (planetType == "Rocky")
        {
            //random planet albeido between 0.3 and 0.5
            //only rocky planets have an albeido, by default it is 0
            albeidoBase = randomAlbeidoBase;

            if (chanceOfHydrospere <= 30)//30% chance for planet to have a hydrosphere
            {
                HasHydrosphere = true;
            }
        }

        if (HasHydrosphere == false)
        {
            hydrosphereStatus = "No Hydrosphere";
        }



        GenerateAtmosphere(chanceOfOxygen, pressureOfOxygen, chanceOfNitrogen, pressureOfNitrogen, chanceOfCO2, pressureOfCO2, chanceOfMethane, pressureOfMethane, chanceOfHelium, pressureOfHelium, pressureOfHeliumIfGasGiant, chanceOfHydrogen, pressureOfHydrogen, pressureOfHydrogenIfGasGiant, chanceOfAmmonia, pressureOfAmmonia, chanceOfCO, pressureOfCO, chanceOfNobleGas, pressureOfNobleGas, pressureOfH2O);

    }

    private void GenerateAtmosphere(int chanceOfOxygen, float pressureOfOxygen, int chanceOfNitrogen, float pressureOfNitrogen, int chanceOfCO2, float pressureOfCO2, int chanceOfMethane, float pressureOfMethane, int chanceOfHelium, float pressureOfHelium, float pressureOfHeliumIfGasGiant, int chanceOfHydrogen, float pressureOfHydrogen, float pressureOfHydrogenIfGasGiant, int chanceOfAmmonia, float pressureOfAmmonia, int chanceOfCO, float pressureOfCO, int chanceOfNobleGas, float pressureOfNobleGas, float pressureOfH2O)
    {
            if (chanceOfOxygen <= 30)
            {
                Oxygen = new Atmosphere("Oxygen", 0, pressureOfOxygen, Mass);
                PlanetAtmosphere.Add(Oxygen);
            }
            if (chanceOfNitrogen <= 50)
            {
                Nitrogen = new Atmosphere("Nitrogen", 0, pressureOfNitrogen, Mass);
                PlanetAtmosphere.Add(Nitrogen);
            }
            if (chanceOfCO2 <= 40)
            {
                CO2 = new Atmosphere("CO2", 0.15f, pressureOfCO2, Mass);
                PlanetAtmosphere.Add(CO2);
                hasCO2 = true;
            }
            if (chanceOfMethane <= 10)
            {
                Methane = new Atmosphere("Methane", 1f, pressureOfMethane, Mass);
                PlanetAtmosphere.Add(Methane);
                hasMethane = true;
            }
            if (planetType == "Gas Giant")
            {
                Helium = new Atmosphere("Helium", 0, pressureOfHeliumIfGasGiant, Mass);
                PlanetAtmosphere.Add(Helium);

                Hydrogen = new Atmosphere("Hydrogen", 0, pressureOfHydrogenIfGasGiant, Mass);
                PlanetAtmosphere.Add(Hydrogen);
            }
            else
            {
                if (chanceOfHelium <= 20)
                {
                    Helium = new Atmosphere("Helium", 0, pressureOfHelium, Mass);
                    PlanetAtmosphere.Add(Helium);
                }
                if (chanceOfHydrogen <= 50)
                {
                    Hydrogen = new Atmosphere("Hydrogen", 0, pressureOfHydrogen, Mass);
                    PlanetAtmosphere.Add(Hydrogen);
                }
            }
            if (chanceOfAmmonia <= 10)
            {
                Ammonia = new Atmosphere("Ammonia", 0, pressureOfAmmonia, Mass);
                PlanetAtmosphere.Add(Ammonia);
            }
            if (chanceOfCO <= 10)
            {
                CO = new Atmosphere("CO", 0, pressureOfCO, Mass);
                PlanetAtmosphere.Add(CO);
            }
            if (chanceOfNobleGas <= 10)
            {
                NobleGas = new Atmosphere("Noble Gas", 0, pressureOfNobleGas, Mass);
                PlanetAtmosphere.Add(NobleGas);
            }
            if (HasHydrosphere == true)
            {
                H2O = new Atmosphere("H2O", 1.2f, pressureOfH2O, Mass);
                PlanetAtmosphere.Add(H2O);
                hasH20 = true;
            }

        AtmosphereGenerated = true;
    }
    public void UpdateAtmosphere()
    {
        if (AtmosphereGenerated == true)
        {
            
            TotalAtmospericPressure = 0;
            greenhousePressure = 0;

            for (int i = 0; i < PlanetAtmosphere.Count; i++)
            {
                TotalAtmospericPressure = +PlanetAtmosphere[i].GasPressure;
            }

            if(hasCO2 == true)
            {
                greenhousePressure += (CO2.GasPressure * 0.15);
            }
            if(hasMethane == true)
            {
                greenhousePressure += +Methane.GasPressure;
            }
            if (hasH20)
            {
                greenhousePressure += (H2O.GasPressure * 1.2);
            }
        }
    }
    void calculateTemp(Star star)
    {
        //temperature

        for (int i = 0; i < PlanetAtmosphere.Count; i++)
        {
            TotalAtmospericPressure += PlanetAtmosphere[i].GasPressure;
        }
        float CO2pressure = 0;
        try
        {
            CO2pressure = CO2.GasPressure;
        }
        catch { }

        float MethanePressure = 0;
        try
        {
            MethanePressure = Methane.GasPressure;
        }
        catch { }

        float H2OPresure = 0;
        try
        {
            H2OPresure = H2O.GasPressure;
        }
        catch { }

        greenhousePressure = (CO2pressure * 0.15) + MethanePressure + (H2OPresure * 1.2);
        effectiveTemp = System.Math.Pow((star.luminosity * (1 - albeidoBase)) / (16 * 3.142 * (System.Math.Pow(OrbitRadius * 100000000, 2)) * SBconstant), 0.25);
        surfaceTemp = effectiveTemp + (1 + TotalAtmospericPressure / 15) + greenhousePressure;
    }
    public Moon[] getChildMoons()
    {
        return childMoons;
        
    }





    // a function to repeat code every x amount of seconds
    public void UpdatePlanet(double time)
    {
        /* Template:
         * 
         * if(Math.floor(time % 10 )== 0)
         * {
         *     //code goes here
         * }
         * 
         * this is run every ten seconds
         */



        //calcualte temperature, and atmosphere every 10 seconds
        if (Math.Floor(time % howOftenTempCalculate) == 0)
        {
            UpdateAtmosphere();
            calculateTemp(ParentStar);
            if (HasHydrosphere == true)
            {
                if ((surfaceTemp-273.15) < 0)
                {
                    if (hydrosphereStatus != "Frozen")
                    {
                        hydrosphereStatus = "Frozen";
                    }
                }
                else if ((surfaceTemp - 273.15) > 99)
                {
                    if (hydrosphereStatus != "Gas")
                    {
                        hydrosphereStatus = "Gas";
                        H2O.GasPressure++;
                    }
                }
                else
                {
                    if (hydrosphereStatus == "Gas")// planet cooling from gas
                    {
                        hydrosphereStatus = "Liquid";
                        H2O.GasPressure--;
                    }
                    else if (hydrosphereStatus == "Frozen")
                    {
                        hydrosphereStatus = "Liquid";
                    }
                }
            }
        }



    }
    public void UpdatePlanetComponentFigures() {


        PlanetObject.GetComponent<PlanetComponent>().PlanetUnsortedName = PlanetName;
        PlanetObject.GetComponent<PlanetComponent>().albeido = albeidoBase;
        PlanetObject.GetComponent<PlanetComponent>().planetarea = planetArea;
        PlanetObject.GetComponent<PlanetComponent>().mass = Mass;
        PlanetObject.GetComponent<PlanetComponent>().HydrosphereStatus = hydrosphereStatus;
        PlanetObject.GetComponent<PlanetComponent>().HasHydrosphere = HasHydrosphere;
        PlanetObject.GetComponent<PlanetComponent>().surfaceTemp = surfaceTemp;
        PlanetObject.GetComponent<PlanetComponent>().effectiveTemp = effectiveTemp;
        PlanetObject.GetComponent<PlanetComponent>().GreenhousePressure = greenhousePressure;
        PlanetObject.GetComponent<PlanetComponent>().totalAtmosphericPressure = TotalAtmospericPressure;
        PlanetObject.GetComponent<PlanetComponent>().PlanetType = planetType;
    }

}
public class Atmosphere
{
    public string Name;
    public float GreenhouseEffect;
    public float GasPressure = 0;
    public double AmountOfGas;


    public Atmosphere(string gasName, float gasGreenhouseEffect,float pressureOfGas,double PlanetMass)
    {
        Name = gasName;
        GreenhouseEffect = gasGreenhouseEffect;
        GasPressure = pressureOfGas;
        AmountOfGas = PlanetMass * pressureOfGas;
    }
}
