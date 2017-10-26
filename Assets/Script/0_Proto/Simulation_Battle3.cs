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


	/// <summary>
	/// frame skip 시 해당프레임의 deltaTime을 최소 프레임시간으로 설정한다.
	/// </summary>
	public class FrameControl
	{
		static private float _deltaTime_mix = 0f;
		static private float _deltaTime_max = 0f;

		static public void SetDeltaTime_30FPS()
		{
			FrameControl.SetDeltaTime_FPS (30f);
		}

		static public void SetDeltaTime_FPS(float fps)
		{
			_deltaTime_mix = 1f / fps;
			_deltaTime_max = _deltaTime_mix * 2f; //최소시간의 2배 한다. 
		}

		static public float DeltaTime_Mix()
		{
			return _deltaTime_mix;
		}

		static public float DeltaTime_Max()
		{
			return _deltaTime_max;
		}

		static public float DeltaTime()
		{
			//전프레임이 허용프레임 시간의 최대치를 넘었다면 최소시간을 반환한다.
			if (Time.deltaTime > _deltaTime_max) 
			{
				//DebugWide.LogBlue ("FrameControl - frameSkip detected !!! - DeltaTime : "+Time.deltaTime);//chamto test
				return _deltaTime_mix;
			}


			return Time.deltaTime;
		}


	}


	public class Simulation_Battle3 : MonoBehaviour 
	{

		private CharacterManager _crtMgr = null;
		private ResourceManager _rscMgr = null;

		//====//====//====//====//====//====//====//====
		private UI_Battle _ui_battle = null;
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
			
			FrameControl.SetDeltaTime_30FPS (); //30Fps 기준으로 설정한다.  30Fps 고정프레임으로 사용하겠다는 것이 아님.

			_crtMgr = new CharacterManager ();
			_crtMgr.Init (CHARACTER_COUNT);

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
				UI_CharacterCard card = _ui_battle.AddCharacter (chter);

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





		public Vector3[] GetPaths(Vector3 start)
		{
			
			Vector3[] list = new Vector3[4];
			list[0] = Single.hierarchy.Find<Transform> ("1_Paths/p (0)").localPosition + start;
			list[1] = Single.hierarchy.Find<Transform> ("1_Paths/p (1)").localPosition + start;
			list[2] = Single.hierarchy.Find<Transform> ("1_Paths/p (2)").localPosition + start;
			list[3] = Single.hierarchy.Find<Transform> ("1_Paths/p (3)").localPosition + start;


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

			float time = 5f;
			bundle._gameObject.SetActive (true);
			iTween.Stop (bundle._gameObject);
			bundle._ui.RevertData_All ();

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

		}

		void FixedUpdate()
		{
			//DebugWide.LogBlue (" UnityDelta : "+Time.deltaTime + "   Udelta : " + Udelta); //chamto test

		}


		// Update is called once per frame
		void Update () 
		{

			//frame test
			//if(5f < Time.time)
			{
				//Time.fixedDeltaTime : 고정프레임 설정
				//System.DateTime.Now.Millisecond;
				//QualitySettings.vSyncCount
				//Thread.Sleep (1000);
				//DebugWide.LogBlue (" UnityDelta : "+Time.deltaTime + "   dateMs: " + System.DateTime.Now.Millisecond); //chamto test
			}

			//1. key input
			//2. UI update
			//3. data update


			//test
			if (Input.GetKeyUp ("z")) 
			{
				//DebugWide.LogBlue (" UnityDelta : "+Time.deltaTime + "   A : " + Time.time); //chamto test
				
				CharDataBundle bundle;
				bundle._gameObject = _ui_1Player._effects [UI_CharacterCard.eEffect.Empty].gameObject;
				bundle._data = _ui_1Player.data;
				bundle._ui = _ui_1Player;
				StartCoroutine ("AniStart_Attack_1_Random", bundle);

				DebugWide.LogBlue ("===========================");
			}
			//DebugWide.LogBlue (_ui_1Player._actions [0].transform.localPosition); //chamto test

			//////////////////////////////////////////////////
			//1p

			//attack
			if (Input.GetKeyUp ("q")) 
			{
				//iTween.PunchPosition(_1pSprite_01.gameObject, iTween.Hash("x",20,"loopType","loop","time",0.5f));
				//iTween.MoveBy(_1pSprite_01.gameObject, iTween.Hash("x", 30, "easeType", "easeInOutExpo", "loopType", "pingPong", "delay", .1));
				//DebugWide.LogBlue ("1p - keyinput");
				_ui_1Player.data.Attack_1 ();

				//Effect.FadeIn (_ui_1Player._effect[UI_CharacterCard.eEffect.Hit].gameObject, 0.7f);
			}
			//block
			if (Input.GetKeyUp ("w")) 
			{
				//DebugWide.LogBlue ("1p - keyinput");
				_ui_1Player.data.Block ();

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
				_ui_2Player.data.Attack_1 ();
			}
			//block
			if (Input.GetKeyUp ("p")) 
			{
				//DebugWide.LogBlue ("2p - keyinput");
				_ui_2Player.data.Block ();
			}


			_ui_battle.Update_UI ();
			_crtMgr.Update (); //갱신순서 중요!!!! , start 상태는 1Frame 뒤 변경되는데, 갱신순서에 따라 ui에서 탐지 못할 수 있다. fixme:콜백함수로 처리해야함  


		}//end Update

	}//end class Simulation 




}//end namespace 


