using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RNG
{
    //random numbers stores in arrays in a database like fashion for instant access and later use

    //star
    public static int systemName;
    public static int starPlanets;
    public static double starmass;

    //planets
    public static double[] planetmass;
    public static double[] planetOrbitRadius;
    public static int[] planetNumberOfMoons;
    public static int[] planetEpoch;
    public static float[] planetAlbeido;
    public static int[] planetTypeChance;
    public static int[] planetChanceOfHydrosphere;

    public static int[] chanceOfOxygen;
    public static float[] pressureOfOxygen;

    public static int[] chanceOfNitrogen;
    public static float[] pressureOfNitrogen;

    public static int[] chanceOfCO2;
    public static float[] pressureOfCO2;

    public static int[] chanceOfMethane;
    public static float[] pressureOfMethane;

    public static int[] chanceOfHelium;
    public static float[] pressureOfHelium;
    public static float[] pressureOfHeliumIfGasGiant;

    public static int[] chanceOfHydrogen;
    public static float[] pressureOfHydrogen;
    public static float[] pressureOfHydrogenIfGasGiant;

    public static int[] chanceOfAmmonia;
    public static float[] pressureOfAmmonia;

    public static int[] chanceOfCO;
    public static float[] pressureOfCO;

    public static int[] chanceOfNobleGas;
    public static float[] pressureOfNobleGas;

    public static float[] pressureOfH2O;


    //moons
    public static List<double>[] moonmass;
    public static List<double>[] moonOrbitRadius;
    public static List<int>[] moonEpoch;
}
