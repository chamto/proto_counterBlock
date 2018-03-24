using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using CounterBlock;


public class UI_Title : UI_MonoBase 
{
	
	// Use this for initialization
	void Start () 
	{
		base.Init_UI ();

		StartCoroutine (GlobalFunctions.FadeOut (Single.panelRoot , 2.0f));
		StartCoroutine (GlobalFunctions.LoadScene (GlobalConstants.Scene.MAIN_MENU, 3.5f));
	}
	
	// Update is called once per frame
	void Update () {
		
	}

}



