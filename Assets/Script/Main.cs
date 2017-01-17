using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour 
{

	void Awake()
	{
		Single.hierarchy.Init ();
		//Single.hierarchy.TestPrint ();
		//DebugWide.LogBlue ("awake");
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
