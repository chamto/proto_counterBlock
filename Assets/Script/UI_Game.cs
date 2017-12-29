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
		_root = GameObject.Find ("Panel_root").GetComponent<CanvasRenderer>();
		StartCoroutine (GlobalFunctions.FadeIn (_root , 1.0f));
		FrameControl.SetDeltaTime_30FPS (); //30Fps 기준으로 처리가 늦어진 프레임의 경과시간을 재설정한다.  30Fps 고정프레임으로 사용하겠다는 것이 아님.
		Single.hierarchy.Init ();



//		_crtMgr = new CharacterManager ();
//		_crtMgr.Init (CHARACTER_COUNT);


		//=================================================
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
}
