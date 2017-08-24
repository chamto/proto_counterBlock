/// <summary>
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

	public class Behavior
	{		
		//--------<<============>>----------
		//    openTime_0 ~ openTime_1
		// 시간범위 안에 있어야 콤보가 된다
		public const float MAX_OPEN_TIME = 10f;
		public const float MIN_OPEN_TIME = 0f;

		//===================================

		public float allTime;		//동작 전체 시간 
		public float scopeTime_0;	//동작 유효 범위 : 0(시작) , 1(끝)  
		public float scopeTime_1;
		public float openTime_0; 	//다음 동작 연결시간 : 0(시작) , 1(끝)  
		public float openTime_1; 

		public  Behavior()
		{
			
			allTime = 0f;
			scopeTime_0 = 0f;
			scopeTime_1 = 0f;
			openTime_0 = 0f;
			openTime_1 = 0f;

		}
	}

	public class Character
	{
		public enum eState
		{
			None 		= 0,

			Start,
			Running,
			End,
			Waiting,

			Max,
		}

		static public string StateToString (Character.eState state)
		{
			switch (state) 
			{
			case Character.eState.None:
				return "None";
			case Character.eState.Start:
				return "Start";
			case Character.eState.Running:
				return "Running";
			case Character.eState.End:
				return "End";
			case Character.eState.Waiting:
				return "Waiting";
			
			}

			return "None";
		}

		//====================================

		//고유정보
		private uint 	_id;
		private uint 	_hp;

		//동작정보
		private Behavior _behavior = null;
		private Skill 	_skill_current = null;
		private float 	_timeDelta; 	//시간변화량

		//상태정보
		private eState 	_state_current;

		//====================================

		public CharacterManager ref_parent { get; set;} 

		public SkillBook ref_skillBook { get{ return CSingleton<SkillBook>.Instance; }}

		//====================================

		public void SetID(uint id)
		{
			this._id = id;
		}

		public uint GetID()
		{
			return this._id;
		}

		public float GetTimeDelta()
		{
			return _timeDelta;
		}

		public eState CurrentState()
		{
			return _state_current;
		}

		public void SetState(eState setState)
		{
			_state_current = setState;
		}

		public Skill.eKind CurrentSkillKind()
		{
			if(null != _skill_current)
				return _skill_current.kind;

			return Skill.eKind.None;
		}

		//====================================

		public Character()
		{
			_hp = 10;

			_timeDelta = 0f;

			_state_current = eState.None;
		}

		public void Init()
		{
			this.Idle ();
		}

		public void SetSkill(Skill.eKind kind)
		{
			_skill_current = ref_skillBook [kind];
			_behavior = _skill_current.FirstBehavior ();

			SetState (eState.Start);
		}


		public void Attack ()
		{
			//DebugWide.LogBlue (_id + " : Attack"); //chamto test

			SetSkill (Skill.eKind.Attack_1);
		}

		public void Idle()
		{
			SetSkill (Skill.eKind.Idle);
		}

		public void Update()
		{
			
			switch (this._state_current) 
			{
			case eState.Start:
				{
					this._timeDelta = 0f;
					SetState (eState.Running);
				}
				break;
			case eState.Running:
				{
					this._timeDelta += Time.deltaTime;

					if (_behavior.allTime <= this._timeDelta) 
					{
						this.SetState (eState.End);
					}	
				}
				break;
			case eState.End:
				{
					_behavior = _skill_current.NextBehavior ();

					if (null == _behavior)
						Idle (); //스킬 동작을 모두 꺼냈으면 아이들로 돌아간다. 
					else
						SetState (eState.Start);
						
				}
				break;
			case eState.Waiting:
				{
					//콤보 입력 대기시간 동안 기다린다. 
				}
				break;
			
			}

		}//end func

	}

	public class CharacterManager : Dictionary<uint, Character>
	{
		private uint id_Sequence = 0;
		//todo : 제거된 id를 관리하는 리스트 필요 , 추후 메모리풀 구현

		public void Init()
		{
			this.AddCharacter ();
			this.AddCharacter ();

			this.TestPrint ();
		}

		public uint GetIDSequence()
		{
			return ++this.id_Sequence;
		}

		public uint AddCharacter()
		{
			uint ID = this.GetIDSequence ();
			Character crt = new Character ();
			this.Add (ID, crt);

			crt.SetID (ID);
			crt.ref_parent = this;
			crt.Init ();

			return ID;
		}

		public void Update()
		{
			foreach (Character crt in this.Values) 
			{
				crt.Update ();
			}
		}

		public void TestPrint()
		{
			foreach (KeyValuePair<uint,Character> pair in this) 
			{
				DebugWide.LogGreen (pair.Key + "  " + pair.Value);
			}
		}

	}


	public class SkillManager
	{
		
	}


	public class Skill : List<Behavior>
	{

		public enum eKind
		{
			None,
			Idle,
			Hit,

			Attack_1,
			Attack_2Combo,
			Attack_3Combo,

			Block_1,
			Block_2Combo,
			Block_3Combo,

			CounterBlock,
			Max
		}

		static public string KindToString (Skill.eKind kind)
		{
			switch (kind) 
			{
			case Skill.eKind.None:
				return "None";
			case Skill.eKind.Idle:
				return "Idle";
			case Skill.eKind.Hit:
				return "Hit";

			case Skill.eKind.Attack_1:
				return "Attack_1";
			case Skill.eKind.Attack_2Combo:
				return "Attack_2Combo";
			case Skill.eKind.Attack_3Combo:
				return "Attack_3Combo";

			case Skill.eKind.Block_1:
				return "Block_1";
			case Skill.eKind.Block_2Combo:
				return "Block_2Combo";
			case Skill.eKind.Block_3Combo:
				return "Block_3Combo";

			case Skill.eKind.CounterBlock:
				return "CounterBlock";
			
			}

			return "None";
		}
		//========================================

		private int _index_current = 0;


		//========================================

		public eKind kind { get; set; }

		//========================================

		public Behavior FirstBehavior()
		{
			_index_current = 0; //index 초기화

			if (this.Count == 0)
				return null;

			return this [_index_current];
		}

		public Behavior NextBehavior()
		{
			if (this.Count > _index_current) 
			{
				//마지막 인덱스임
				if (this.Count == _index_current + 1)
					return null;

				_index_current++;
				return this [_index_current];
			}

			return null;
		}




		//========================================

		//스킬 명세서
		static public Skill Details_Idle()
		{
			Skill skinfo = new Skill ();

			skinfo.kind = eKind.Idle;

			Behavior bhvo = new Behavior ();
			bhvo.allTime = 1f;

			bhvo.scopeTime_0 = 0f;
			bhvo.scopeTime_1 = 0f;
			bhvo.openTime_0 = Behavior.MIN_OPEN_TIME;
			bhvo.openTime_1 = Behavior.MAX_OPEN_TIME;
			skinfo.Add (bhvo);

			return skinfo;
		}

		static public Skill Details_Hit()
		{
			Skill skinfo = new Skill ();

			skinfo.kind = eKind.Hit;

			Behavior bhvo = new Behavior ();
			bhvo.allTime = 1f;

			bhvo.scopeTime_0 = 0f;
			bhvo.scopeTime_1 = 0f;
			bhvo.openTime_0 = Behavior.MIN_OPEN_TIME;
			bhvo.openTime_1 = Behavior.MAX_OPEN_TIME;
			skinfo.Add (bhvo);

			return skinfo;
		}


		static public Skill Details_Attack_1()
		{
			Skill skinfo = new Skill ();

			skinfo.kind = eKind.Attack_1;

			Behavior bhvo = new Behavior ();
			bhvo.allTime = 1f;

			bhvo.scopeTime_0 = 0f;
			bhvo.scopeTime_1 = 0f;
			bhvo.openTime_0 = Behavior.MIN_OPEN_TIME;
			bhvo.openTime_1 = Behavior.MAX_OPEN_TIME;
			skinfo.Add (bhvo);

			return skinfo;
		}



		static public Skill Details_Block_1()
		{
			Skill skinfo = new Skill ();

			skinfo.kind = eKind.Block_1;

			Behavior bhvo = new Behavior ();
			bhvo.allTime = 1f;

			bhvo.scopeTime_0 = 0f;
			bhvo.scopeTime_1 = 0f;
			bhvo.openTime_0 = Behavior.MIN_OPEN_TIME;
			bhvo.openTime_1 = Behavior.MAX_OPEN_TIME;
			skinfo.Add (bhvo);


			return skinfo;
		}


	}


	public class SkillBook : Dictionary<Skill.eKind, Skill>
	{
		public SkillBook()
		{
			this.Add (Skill.eKind.Idle, Skill.Details_Idle ());
			this.Add (Skill.eKind.Hit, Skill.Details_Hit ());
			this.Add (Skill.eKind.Attack_1, Skill.Details_Attack_1 ());

			this.Add (Skill.eKind.Block_1, Skill.Details_Block_1 ());

		}
	}

	public class Simulation_Battle2 : MonoBehaviour 
	{

		private CharacterManager _crtMgr = null;
		private Character _1Player = null;
		private Character _2Player = null;

		//====//====//====//====//====//====
		//ui connect
		public Text _1pExplanation1 = null;
		public Text _2pExplanation1 = null;
		public Text _1pExplanation2 = null;
		public Text _2pExplanation2 = null;

		public Slider _1pHpBar = null;
		public Slider _2pHpBar = null;

		public Image _1pImage = null;
		public Image _2pImage = null;

		public Image _1pSprite_01 = null;
		public Image _1pSprite_02 = null;
		public Image _1pSprite_03 = null;

		public Image _2pSprite_01 = null;
		public Image _2pSprite_02 = null;
		public Image _2pSprite_03 = null;
		//====//====//====//====//====//====


		// Use this for initialization
		void Start () 
		{
			_crtMgr = new CharacterManager ();
			_crtMgr.Init ();

			_1Player = _crtMgr [1];
			_2Player = _crtMgr [2];

		}

		void Update_UI()
		{
			

			//====//====//====//====//====//====

			_1pExplanation1.text = 
				"  "  + Character.StateToString(_1Player.CurrentState());
			_2pExplanation1.text = 
				"  "  + Character.StateToString(_2Player.CurrentState());


			_1pExplanation2.text = 
				Skill.KindToString(_1Player.CurrentSkillKind()) + "  " +
				_1Player.GetTimeDelta().ToString("0.0");
			_2pExplanation2.text = 
				Skill.KindToString(_2Player.CurrentSkillKind()) + "   " +
				_2Player.GetTimeDelta().ToString("0.0");
			

		}



		// Update is called once per frame
		void Update () 
		{

			_crtMgr.Update ();

			this.Update_UI ();
			//////////////////////////////////////////////////
			//1p

			//attack
			if (Input.GetKeyUp ("q")) 
			{
				DebugWide.LogBlue ("1p - keyinput");
				_1Player.Attack ();
			}

			//////////////////////////////////////////////////
			//2p

			//attack
			if (Input.GetKeyUp ("o")) 
			{
				DebugWide.LogBlue ("2p - keyinput");
				_2Player.Attack ();
			}

		}//end Update

	}//end class Simulation 




}//end namespace 


