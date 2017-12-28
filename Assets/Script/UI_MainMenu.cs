using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI_MainMenu : MonoBehaviour {

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
