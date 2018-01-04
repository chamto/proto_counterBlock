using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CounterBlock;

public class UI_Loading : UI_MonoBase 
{



	// Use this for initialization
	void Start () 
	{
		//=================================================
		//                    초 기 화 
		//=================================================
		base.Init_UI ();

		StartCoroutine (GlobalFunctions.FadeIn (_panelRoot , 1.0f));
		StartCoroutine (LoadResource_AndScene());
		//=================================================


	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	public void LoadScene_Game()
	{

		StartCoroutine (GlobalFunctions.FadeOut (_panelRoot , 2.0f));
		StartCoroutine (GlobalFunctions.LoadScene (GlobalConstants.Scene.GAME, 3.0f));
	}

	IEnumerator LoadResource_AndScene()
	//public void LoadResource_AndScene()
	{

		yield return new WaitForSeconds (1.5f);

		//CSingleton<ResourceManager>.Instance.Init (); //todo : 비동기 지원되게 다시 작성해야함 

		//todo : 내가 만든 코루틴에 WaitForTime 기능 넣기

		LoadScene_Game ();
	}
}
