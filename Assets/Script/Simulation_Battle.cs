/// <summary>
/// 
/// Simulation_Battle2
/// 
/// 20170825 - chamto - anrudco@gmail.com
/// 
/// </summary>

using System;
using System.Linq;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



namespace CounterBlock
{
	
	public class Simulation_Battle : UI_MonoBase 
	{

		private CharacterManager _crtMgr = null;
		//private ResourceManager _rscMgr = null;

		//====//====//====//====//====//====//====//====
		private GameMode_Battle _ui_battle = null;
		private UI_CharacterCard _ui_1Player = null;
		private UI_CharacterCard _ui_2Player = null;
		//====//====//====//====//====//====//====//====
		private const uint ID_PLAYER_1 = 1;
		private const uint ID_PLAYER_2 = 2;
		private const int CHARACTER_COUNT = 2;
		//====//====//====//====//====//====//====//====

		// Use this for initialization
		void Start () 
		{
			base.Init_UI ();

			FrameControl.SetDeltaTime_30FPS (); //30Fps 기준으로 설정한다.  30Fps 고정프레임으로 사용하겠다는 것이 아님.

			Single.resource.Init ();

			_crtMgr = new CharacterManager ();
			_crtMgr.Init (CHARACTER_COUNT);

			this.gameObject.AddComponent<MonoInputManager> ();
			_ui_battle = this.gameObject.AddComponent<GameMode_Battle> ();
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





		public Vector3[] GetPaths(Vector3 start)
		{

			string pathName = "p01";
			Vector3[] list = new Vector3[6];
			list[0] = Single.hierarchy.GetTransform ("1_Paths/"+pathName+"/node (0)").localPosition + start;
			list[1] = Single.hierarchy.GetTransform ("1_Paths/"+pathName+"/node (1)").localPosition + start;
			list[2] = Single.hierarchy.GetTransform ("1_Paths/"+pathName+"/node (2)").localPosition + start;
			list[3] = Single.hierarchy.GetTransform ("1_Paths/"+pathName+"/node (3)").localPosition + start;
			list[4] = Single.hierarchy.GetTransform ("1_Paths/"+pathName+"/node (4)").localPosition + start;
			list[5] = Single.hierarchy.GetTransform ("1_Paths/"+pathName+"/node (5)").localPosition + start;


			return list;
		}

		Vector3 _prev_position_ = Vector3.zero;
		Vector3 _prev_direction_ = Vector3.right;
		public void AniUpdate_Attack_1_RandomTT(Transform tr)
		{
			Vector3 current_dir = tr.localPosition - _prev_position_;

			//tr.localRotation.SetFromToRotation (_prev_direction_ , current_dir);
			//tr.localRotation.SetLookRotation (current_dir , Vector3.back);

			Vector3 dir = tr.localPosition - _prev_position_;
			Vector3 euler = tr.localEulerAngles;
			euler.z = Mathf.Atan2(dir.y , dir.x) * Mathf.Rad2Deg;
			tr.localEulerAngles = euler;

			_prev_direction_ = current_dir;
			_prev_position_ = tr.localPosition;


			//DebugWide.LogBlue ("AniUpdate_Attack_1_Random : " + current_dir + "  " + _prev_direction_ + "  " );//chamto test
		}


		public IEnumerator AniStart_Attack_1_Random(CharDataBundle bundle)
		{

			float time = 3f;
			bundle._gameObject.SetActive (true);
			iTween.Stop (bundle._gameObject);
			bundle._ui.RevertData_All ();
			DebugWide.LogBlue ("start----- " + Time.time);


			Vector3 start = bundle._gameObject.transform.localPosition;
			Vector3[] list = GetPaths (start);

			_prev_position_ = list [0];

			iTween.MoveTo(bundle._gameObject, iTween.Hash(
				"time", time
				,"easetype",  "easeOutBack"
				,"path", list
				//,"orienttopath",true
				//,"axis","z"
				,"islocal",true //로컬위치값을 사용하겠다는 옵션. 대상객체의 로컬위치값이 (0,0,0)이 되는 문제 있음. 직접 대상객체 로컬위치값을 더해주어야 한다.
				,"movetopath",false //현재객체에서 첫번째 노드까지 자동으로 경로를 만들겠냐는 옵션. 경로 생성하는데 문제가 있음. 비활성으로 사용해야함
				//"looktarget",new Vector3(5,-5,7)
				,"onupdate","AniUpdate_Attack_1_RandomTT"
				,"onupdatetarget",gameObject
				,"onupdateparams",bundle._gameObject.transform
			));

			yield return new WaitForSeconds(time);

			iTween.Stop (bundle._gameObject);
			bundle._gameObject.SetActive (false);
			DebugWide.LogBlue ("end----- " + Time.time);

		}

		void FixedUpdate()
		{
			//DebugWide.LogBlue (" UnityDelta : "+Time.deltaTime + "   Udelta : " + Udelta); //chamto test

		}


		Coroutine _prev_coroutine_ = null;
		// Update is called once per frame
		void Update () 
		{

			Single.Update ();

			//frame test
			//if(5f < Time.time)
			//{
				//Time.fixedDeltaTime : 고정프레임 설정
				//System.DateTime.Now.Millisecond;
				//QualitySettings.vSyncCount
				//Thread.Sleep (1000);
				//DebugWide.LogBlue (" UnityDelta : "+Time.deltaTime + "   dateMs: " + System.DateTime.Now.Millisecond); //chamto test
			//}

			//===================
			//1. key input
			//2. UI update
			//3. data update

			//===== 입력 갱신 =====
			this.Update_Input();

			//===== 후처리 갱신 =====
			_ui_battle.Update_UI ();
			_crtMgr.Update (); //갱신순서 중요!!!! , start 상태는 1Frame 뒤 변경되는데, 갱신순서에 따라 ui에서 탐지 못할 수 있다. fixme:콜백함수로 처리해야함  


		}//end Update

		public void Update_Input()
		{
			if (Input.GetKeyUp ("z")) 
			{
				//DebugWide.LogBlue (" UnityDelta : "+Time.deltaTime + "   A : " + Time.time); //chamto test

				CharDataBundle bundle;
				bundle._gameObject = _ui_1Player._effects [UI_CharacterCard.eEffect.Empty].gameObject;
				bundle._data = _ui_1Player.GetData();
				bundle._ui = _ui_1Player;
				//StopCoroutine ("AniStart_Attack_1_Random");
				if(null != _prev_coroutine_)
					StopCoroutine(_prev_coroutine_);
				_prev_coroutine_ = StartCoroutine ("AniStart_Attack_1_Random", bundle);


			}
			//DebugWide.LogBlue (_ui_1Player._actions [0].transform.localPosition); //chamto test

			//////////////////////////////////////////////////
			//1p

			//attack
			if (Input.GetKeyUp ("q")) 
			{
				_ui_1Player.GetData().Attack_Weak ();
				//Effect.FadeIn (_ui_1Player._effect[UI_CharacterCard.eEffect.Hit].gameObject, 0.7f);
			}
			if (Input.GetKeyUp ("w")) 
			{
				_ui_1Player.GetData().Attack_Strong ();
			}
			//block
			if (Input.GetKeyUp ("e")) 
			{
				_ui_1Player.GetData().Block ();

				//iTween.ShakeScale(_ui_1Player._effect[UI_CharacterCard.eEffect.Hit].gameObject,new Vector3(0.2f,0.8f,0.2f), 1f); //!!!!
				//iTween.ScaleTo(_ui_1Player._effect[UI_CharacterCard.eEffect.Hit].gameObject, new Vector3(0.2f,0.2f,0.2f), 0.7f);
				//iTween.ScaleFrom(_ui_1Player._effect[UI_CharacterCard.eEffect.Hit].gameObject, Vector3.zero, 0.4f);

				//Effect.FadeOut (_ui_1Player._effect[UI_CharacterCard.eEffect.Hit].gameObject, 1f);
			}


			//////////////////////////////////////////////////
			//2p

			//attack
			if (Input.GetKeyUp ("i")) 
			{
				//DebugWide.LogBlue ("2p - keyinput");
				_ui_2Player.GetData().Attack_Weak ();
			}
			if (Input.GetKeyUp ("o")) 
			{
				//DebugWide.LogBlue ("2p - keyinput");
				_ui_2Player.GetData().Attack_Strong ();
			}
			//block
			if (Input.GetKeyUp ("p")) 
			{
				//DebugWide.LogBlue ("2p - keyinput");
				_ui_2Player.GetData().Block ();
			}
		}

		public void OnAttackWeak(int playerNum)
		{
			if (1 == playerNum) 
			{
				_ui_1Player.GetData().Attack_Weak ();
			}
			if (2 == playerNum) 
			{
				_ui_2Player.GetData().Attack_Weak ();
			}
		}

		public void OnAttackStrong(int playerNum)
		{
			if (1 == playerNum) 
			{
				_ui_1Player.GetData().Attack_Strong ();
			}
			if (2 == playerNum) 
			{
				_ui_2Player.GetData().Attack_Strong ();
			}
		}

		public void OnBlock(int playerNum)
		{
			if (1 == playerNum) 
			{
				_ui_1Player.GetData().Block ();
			}
			if (2 == playerNum) 
			{
				_ui_2Player.GetData().Block ();
			}
		}


	}//end class Simulation 

}//end namespace 


