using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using CounterBlock;

public class UI_MainMenu : MonoBehaviour 
{
	

	// Use this for initialization
	void Start () 
	{
		//=================================================
		//                    초 기 화 
		//=================================================
		Single.hierarchy.Init ();
		//=================================================

		CanvasRenderer renderer = GameObject.Find ("Panel_root").GetComponent<CanvasRenderer>();

		StartCoroutine (GlobalFunctions.FadeIn (renderer , 1.0f));

	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	public void Switch_GameMap()
	{
		GameObject map = Single.hierarchy.Find<GameObject> ("Canvas/Panel_root/Panel_map");
		map.SetActive (!map.activeSelf);
	}
}
