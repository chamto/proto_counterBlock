using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using CounterBlock;

public class UI_Title : MonoBehaviour 
{
	Camera _mainCamera = null;
	Canvas _canvasRoot = null;
	CanvasRenderer _panelRoot = null;

	// Use this for initialization
	void Start () 
	{
		//=================================================
		//                    초 기 화 
		//=================================================
		_mainCamera = GameObject.Find ("Main Camera").GetComponent<Camera>();
		_canvasRoot = GameObject.Find ("Canvas").GetComponent<Canvas>().rootCanvas; 
		_panelRoot = GameObject.Find ("Panel_root").GetComponent<CanvasRenderer>();

		ResolutionController.CalcViewportRect (_canvasRoot, _mainCamera); //화면크기조정

		StartCoroutine (GlobalFunctions.FadeOut (_panelRoot , 2.0f));
		StartCoroutine (GlobalFunctions.LoadScene (GlobalConstants.Scene.MAIN_MENU, 3.5f));
		//=================================================
	}
	
	// Update is called once per frame
	void Update () {
		
	}

}



