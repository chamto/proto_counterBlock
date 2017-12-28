using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Loading : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		CanvasRenderer renderer = GameObject.Find ("Panel_root").GetComponent<CanvasRenderer>();

		StartCoroutine (GlobalFunctions.FadeIn (renderer , 1.0f));	
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
