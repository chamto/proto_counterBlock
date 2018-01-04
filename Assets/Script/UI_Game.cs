using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CounterBlock;

public class UI_Game : UI_MonoBase 
{
	
	// Use this for initialization
	void Start () 
	{
		//=================================================
		//                    초 기 화 
		//=================================================
		base.Init_UI ();

		StartCoroutine (GlobalFunctions.FadeIn (_panelRoot , 1.0f));

		FrameControl.SetDeltaTime_30FPS (); //30Fps 기준으로 처리가 늦어진 프레임의 경과시간을 재설정한다.  30Fps 고정프레임으로 사용하겠다는 것이 아님.


//		_crtMgr = new CharacterManager ();
//		_crtMgr.Init (CHARACTER_COUNT);


		//=================================================
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
}
