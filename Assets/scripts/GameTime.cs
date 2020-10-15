using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTime : MonoBehaviour {
    //code started on 01/11/2018
    /// <summary>
    /// assuming that advance time multiplier is left at 1, this will increment once per second
    /// </summary>
    public static float time = 0;
    public int AdvanceTimeMultiplier = 1;
    
	
	// Update is called once per frame
	void FixedUpdate () {
        time = time + Time.deltaTime * AdvanceTimeMultiplier;
	}
    
}
