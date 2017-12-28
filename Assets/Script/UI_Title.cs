using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI_Title : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
	{
		CanvasRenderer renderer = GameObject.Find ("Panel_root").GetComponent<CanvasRenderer>();

		StartCoroutine (GlobalFunctions.FadeOut (renderer , 2.0f));
		StartCoroutine (GlobalFunctions.LoadScene (GlobalConstants.Scene.MAIN_MENU, 3.5f));

	}
	
	// Update is called once per frame
	void Update () {
		
	}


}

