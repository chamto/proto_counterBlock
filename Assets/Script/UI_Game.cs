using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CounterBlock;

public class UI_Game : MonoBehaviour 
{

	CanvasRenderer _root = null;

	// Use this for initialization
	void Start () 
	{
		//=================================================
		//                    초 기 화 
		//=================================================
		Single.hierarchy.Init ();
		_root = GameObject.Find ("Panel_root").GetComponent<CanvasRenderer>();


		StartCoroutine (GlobalFunctions.FadeIn (_root , 1.0f));
		//=================================================
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
