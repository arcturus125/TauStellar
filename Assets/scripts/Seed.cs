using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class Seed : MonoBehaviour {
    
    public static string generateNew()
    {
        int[] numArray = new int[256];
        char[] charArray = new char[256];
        for(int i = 0; i < 256; i++)
        {
            numArray[i] = Random.Range(0,9);
            string tempstring = numArray[i].ToString();
            charArray[i] = tempstring.ToCharArray()[0];
        }
        string seed = new string(charArray);
        Debug.Log(seed);
        return seed;
    }
    public static int random(string seed, int index, int lowerbound, int upperbound)
    {
        char[] charArray = seed.ToCharArray();//turn the (string) seed into a character array
        int digits = (upperbound - lowerbound).ToString().Length;//find out how many random digits are needed
        char[] chars = new char[digits];
        for (int i = 0; i < digits; i++)//generate seeded numbers  that is the specified digits long
        {
            chars[i] = charArray[i + index];
        }

        string stringNumbers = new string(chars); // turnit into a number
        int seededNumber = int.Parse(stringNumbers);
        seededNumber = seededNumber + lowerbound;//make sure it fits in the bounds
        seededNumber = seededNumber % upperbound;
        
        return seededNumber;
    }
    public static float randomFloat(string seed, int index, float lowerbound, float upperbound)
    {
        char[] charArray = seed.ToCharArray();//turn the (string) seed into a character array
        float digits = upperbound - lowerbound;//find the range of the lowerbound and upperbound
        if (digits > 200)
        {
            digits = 200;
        }
        char[] chars = new char[digits.ToString().Length];
        for (int i = 0; i < digits.ToString().Length; i++)
        {
            if (i > (256 - index))
            {
                break;
            }
            chars[i] = charArray[i + index];
        }

        string stringNumbers = new string(chars);
        float seededNumber = (float)int.Parse(stringNumbers);
        seededNumber = seededNumber + lowerbound;
        seededNumber = seededNumber % upperbound;
        
        return seededNumber;
    }
    public static string shorten(string input)
    {
        input.ToCharArray();
        int shortenedLength = 5;
        char[] charArray = new char[shortenedLength];
        for(int i = 0; i < shortenedLength; i++)
        {
            charArray[i] = input[i];
        }
        string output = new string(charArray);
        return output;
    }
}
