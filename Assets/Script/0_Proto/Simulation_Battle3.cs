﻿/// <summary>
/// 
/// Simulation_Battle2
/// 
/// 20170825 - chamto - anrudco@gmail.com
/// 
/// </summary>

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace CounterBlock
{



	public class Simulation_Battle3 : MonoBehaviour 
	{

		private CharacterManager _crtMgr = null;
		private Character _1Player = null;
		private Character _2Player = null;
		private ResourceManager _rscMgr = null;

		//====//====//====//====//====//====
		private UI_Battle _ui_battle = null;
		private UI_CharacterCard _ui_1Player = null;
		private UI_CharacterCard _ui_2Player = null;


		// Use this for initialization
		void Start () 
		{
			const uint ID_PLAYER_1 = 1;
			const uint ID_PLAYER_2 = 2;
			const int CHARACTER_COUNT = 2;

			_crtMgr = new CharacterManager ();
			_crtMgr.Init (CHARACTER_COUNT);

			_1Player = _crtMgr [ID_PLAYER_1];
			_2Player = _crtMgr [ID_PLAYER_2];

			_rscMgr = CSingleton<ResourceManager>.Instance;
			_rscMgr.Init ();

			_ui_battle = this.gameObject.AddComponent<UI_Battle> ();
			_ui_battle.Init ();

			this.CreatePlayer ();

			_ui_1Player = _ui_battle.GetCharacter (ID_PLAYER_1);
			_ui_2Player = _ui_battle.GetCharacter (ID_PLAYER_2);

		}

		public void CreatePlayer()
		{

			int count = 0;
			foreach (Character chter in _crtMgr.Values) 
			{
				UI_CharacterCard card = _ui_battle.AddCharacter (UI_CharacterCard.eKind.Biking, chter.GetID ());

				if ((chter.GetID () % 2) == 1) 
				{ //홀수는 왼쪽 1 3 5 ...
					//DebugWide.LogBlue(-10f * count + " left " + count); //chamto test
					_ui_battle.SetStartPoint (chter.GetID (), -1f * count, UI_Battle.START_POINT_LEFT);	

				}
				if ((chter.GetID () % 2) == 0) 
				{ //짝수는 오른쪽 2 4 6 ... 
					//DebugWide.LogBlue(10f * count + " right " + count); //chamto test
					_ui_battle.SetStartPoint (chter.GetID (), 1f * (count-1), UI_Battle.START_POINT_RIGHT);

				}

				count++;
			}
		}







		// Update is called once per frame
		void Update () 
		{
			//1. key input
			//2. UI update
			//3. data update



			//////////////////////////////////////////////////
			//1p

			//attack
			if (Input.GetKeyUp ("q")) 
			{
				//iTween.PunchPosition(_1pSprite_01.gameObject, iTween.Hash("x",20,"loopType","loop","time",0.5f));
				//iTween.MoveBy(_1pSprite_01.gameObject, iTween.Hash("x", 30, "easeType", "easeInOutExpo", "loopType", "pingPong", "delay", .1));
				//DebugWide.LogBlue ("1p - keyinput");
				_1Player.Attack_1 ();


				//Effect.FadeIn (_ui_1Player._effect[UI_CharacterCard.eEffect.Hit].gameObject, 0.7f);
			}
			//block
			if (Input.GetKeyUp ("w")) 
			{
				//DebugWide.LogBlue ("1p - keyinput");
				_1Player.Block ();

				//iTween.ShakeScale(_ui_1Player._effect[UI_CharacterCard.eEffect.Hit].gameObject,new Vector3(0.2f,0.8f,0.2f), 1f); //!!!!
				//iTween.ScaleTo(_ui_1Player._effect[UI_CharacterCard.eEffect.Hit].gameObject, new Vector3(0.2f,0.2f,0.2f), 0.7f);
				//iTween.ScaleFrom(_ui_1Player._effect[UI_CharacterCard.eEffect.Hit].gameObject, Vector3.zero, 0.4f);

				//Effect.FadeOut (_ui_1Player._effect[UI_CharacterCard.eEffect.Hit].gameObject, 1f);
			}


			//////////////////////////////////////////////////
			//2p

			//attack
			if (Input.GetKeyUp ("o")) 
			{
				//DebugWide.LogBlue ("2p - keyinput");
				_2Player.Attack_1 ();
			}
			//block
			if (Input.GetKeyUp ("p")) 
			{
				//DebugWide.LogBlue ("2p - keyinput");
				_2Player.Block ();
			}


			foreach (Character chter in _crtMgr.Values) 
			{
				_ui_battle.Update_UI (chter, chter.GetID());
			}
			_crtMgr.Update (); //갱신순서 중요!!!! , start 상태는 1Frame 뒤 변경되는데, 갱신순서에 따라 ui에서 탐지 못할 수 있다. fixme:콜백함수로 처리해야함  


		}//end Update

	}//end class Simulation 




}//end namespace 

