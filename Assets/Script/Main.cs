using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour 
{

	void Awake()
	{
		CounterBlock.Single.hierarchy.Init ();
		//Single.hierarchy.TestPrint ();
		//DebugWide.LogBlue ("Main awake");
	}

	void OnEnable()
	{
		//DebugWide.LogBlue ("onEnable");
	}
		
	void Start () 
	{
		//DebugWide.LogBlue ("start");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
