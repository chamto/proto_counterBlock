using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CounterBlock;


public class FunctionSpeed : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		

		float value = 0f;
		for (int i = 2 ; i < 1000; i+=10) 
		//for (int i = 10000 - 10 ; i < 10000; i++)  //100
		//for (int i = 1000 - 10 ; i < 1000; i++) //100
		//for (int i = 100 - 10 ; i < 100; i++) //10
		//for (int i = 10 - 10 ; i < 10; i++) //1
		{
			value = (float)i;
			DebugWide.LogBlue ( i+": fast : "+Util.Sqrt_Quick_2 (value) + "  1?:" + Mathf.Sqrt(value) *  Util.RSqrt_Quick_2 (value));
			DebugWide.LogRed ("        normal : "+Mathf.Sqrt(value));
		}


	}

	public void TestSqrt()
	{

		float value = 0f;
		float sum = 0f;
		for (int i = 1 ; i < 100000; i++) 
		{
			value = (float)i;
			//sum = Mathf.Sqrt (value); //6.94ms - 13.87ms
			//sum = Util.Sqrt_Quick_2 (value); //6.79ms - 14.95ms
			//sum = Util.RSqrt_Quick_2 (value); //6.74ms - 14.51ms
			sum = Util.Sqrt_Quick_7 (value); //5.68ms - 14.71ms

		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		TestSqrt ();
	}
}
