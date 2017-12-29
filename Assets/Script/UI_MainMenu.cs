﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using CounterBlock;

public class UI_MainMenu : MonoBehaviour 
{

	public enum eMenu : int
	{
		Map,
		Market,
		Option,
		Max
	}

	CanvasRenderer _root = null;
	private GameObject[] _menu = new GameObject[(int)eMenu.Max];


	// Use this for initialization
	void Start () 
	{
		//=================================================
		//                    초 기 화 
		//=================================================
		Single.hierarchy.Init ();
		_root = GameObject.Find ("Panel_root").GetComponent<CanvasRenderer>();
		_menu[(int)eMenu.Map] = Single.hierarchy.Find<GameObject> ("Canvas/Panel_root/Panel_map");
		_menu[(int)eMenu.Market] = Single.hierarchy.Find<GameObject> ("Canvas/Panel_root/Panel_market");
		_menu[(int)eMenu.Option] = Single.hierarchy.Find<GameObject> ("Canvas/Panel_root/Panel_option");

		StartCoroutine (GlobalFunctions.FadeIn (_root , 1.0f));
		//=================================================

	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}


	public void Switch_MenuButton(int menu)
	{
		GameObject btn = _menu[(int)menu];

		foreach (GameObject m in _menu) 
		{
			if(btn != m)
				m.SetActive (false);
		}

		btn.SetActive (!btn.activeSelf);
	}

	private bool _process_load_ = false;
	public void LoadScene_Loading()
	{
		if (true == _process_load_)
			return;
		//DebugWide.LogBlue ("Load...");
		_process_load_ = !_process_load_;
		StartCoroutine (GlobalFunctions.FadeOut (_root , 2.0f));
		StartCoroutine (GlobalFunctions.LoadScene (GlobalConstants.Scene.LOADING, 3.0f));
	}
}