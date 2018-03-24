﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CounterBlock;

public class UI_GameController : UI_MonoBase 
{

	private CharacterManager _crtMgr = null;
	private GameMode_Battle _ui_battle = null;
	//private GameMode_Couple _game_couple = null;
	private GameObject _mainProcess = null;

	// Use this for initialization
	void Start () 
	{
		//=================================================
		//                    초 기 화 
		//=================================================
		base.Init_UI ();

		//StartCoroutine (GlobalFunctions.FadeIn (_panelRoot , 1.0f));

		FrameControl.SetDeltaTime_30FPS (); //30Fps 기준으로 처리가 늦어진 프레임의 경과시간을 재설정한다.  30Fps 고정프레임으로 사용하겠다는 것이 아님.


		Single.resource.Init ();

		_mainProcess = Single.hierarchy.GetGameObject ("0_Main/Process");

		_crtMgr = new CharacterManager ();
		//_crtMgr.Init (CHARACTER_COUNT);

		_mainProcess.AddComponent<MonoInputManager> ();

		//_ui_battle.Init ();

		//this.CreatePlayer ();

		//=================================================
		//					게임 모드 추가
		//=================================================
		_ui_battle = _mainProcess.AddComponent<GameMode_Battle> ();
		//_game_couple =_mainProcess.AddComponent<GameMode_Couple>(); //짝맞추기 모드 추가
		_mainProcess.AddComponent<GameMode_Catching>(); //두더지잡기 모드 추가 

		//================================================
	}

	// Update is called once per frame
	void Update () 
	{
		Single.Update ();

		//갱신순서 중요!!!! , start 상태는 1Frame 뒤 변경되는데, 갱신순서에 따라 ui에서 탐지 못할 수 있다. fixme:콜백함수로 처리해야함  
		//===================
		//1. key input
		//2. UI update
		//3. data update

		//===== key input
		this.Update_Input();

		//===== UI update
		_ui_battle.Update_UI ();

		//===== data update
		_crtMgr.Update (); 


	}//end Update

	public void CreatePlayer()
	{

		int count = 0;
		foreach (Character chter in _crtMgr.Values) 
		{
			UI_CharacterCard card = _ui_battle.AddCharacter (chter);

			if ((chter.GetID () % 2) == 1) 
			{ //홀수는 왼쪽 1 3 5 ...
				//DebugWide.LogBlue(-10f * count + " left " + count); //chamto test
				_ui_battle.SetStartPoint (chter.GetID (), -1f * count, GameMode_Battle.START_POINT_LEFT);	
				//card.data.kind = Character.eKind.Biking;
				card.SetKind(Character.eKind.Biking);

			}
			if ((chter.GetID () % 2) == 0) 
			{ //짝수는 오른쪽 2 4 6 ... 
				//DebugWide.LogBlue(10f * count + " right " + count); //chamto test
				_ui_battle.SetStartPoint (chter.GetID (), 1f * (count-1), GameMode_Battle.START_POINT_RIGHT);
				//card.data.kind = Character.eKind.Seonbi;
				card.SetKind(Character.eKind.Seonbi);

			}

			count++;
		}
	}


	public void Update_Input()
	{
		
	}

}

