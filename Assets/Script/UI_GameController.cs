using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CounterBlock;

public class UI_GameController : UI_MonoBase 
{
	
	private GameObject _mainProcess = null;

	// Use this for initialization
	void Start () 
	{
		//=================================================
		//                    초 기 화 
		//=================================================
		base.Init_UI();

		FrameControl.SetDeltaTime_30FPS (); //30Fps 기준으로 처리가 늦어진 프레임의 경과시간을 재설정한다.  30Fps 고정프레임으로 사용하겠다는 것이 아님.

		Single.resource.Init (); //게임리소스 로딩

		this.gameObject.AddComponent<MonoInputManager> (); //화면터치관리기 추가 


		//=================================================
		//					게임 모드 추가
		//=================================================
		_mainProcess = Single.hierarchy.GetGameObject ("0_Main/Process");
		//_mainProcess.AddComponent<GameMode_Battle> ();
		//_mainProcess.AddComponent<GameMode_Couple>(); //짝맞추기 모드 추가
		//_mainProcess.AddComponent<GameMode_Catching>(); //두더지잡기 모드 추가 
		_mainProcess.AddComponent<GameMode_Talk>(); 
		//================================================
	}

	// Update is called once per frame
	void Update () 
	{
		Single.Update (); //와이드 코루틴 갱신코드

	}//end Update

}

