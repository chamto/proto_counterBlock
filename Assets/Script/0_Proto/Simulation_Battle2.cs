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

		public float runningTime;	//동작 전체 시간 
		public float scopeTime_0;	//동작 유효 범위 : 0(시작) , 1(끝)  
		public float scopeTime_1;
		public float rigidTime;		//동작 완료후 경직 시간
		public float openTime_0; 	//다음 동작 연결시간 : 0(시작) , 1(끝)  
		public float openTime_1; 

		public  Behavior()
		{
			
			runningTime = 0f;
			scopeTime_0 = 0f;
			scopeTime_1 = 0f;
			rigidTime = 0f;
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

		public bool Valid_ScopeTime()
		{
			if (eState.Running == _state_current) 
			{
				if (_behavior.scopeTime_0 <= _timeDelta && _timeDelta <= _behavior.scopeTime_1)
					return true;
			}

			return false;
		}

		public bool Valid_OpenTime()
		{
			if (eState.Running == _state_current) 
			{
				if (_behavior.openTime_0 <= _timeDelta && _timeDelta <= _behavior.openTime_1)
					return true;
			}

			return false;
		}

		public void SetSkill(Skill.eKind kind)
		{
			_skill_current = ref_skillBook [kind];
			_behavior = _skill_current.FirstBehavior ();

			SetState (eState.Start);
		}


		public void Attack_1 ()
		{
			if (Skill.eKind.Idle == _skill_current.kind || true == this.Valid_OpenTime ()) 
			{
				//아이들상태거나 연결시간안에 행동이 들어온 경우
				SetSkill (Skill.eKind.Attack_1);

				//DebugWide.LogBlue ("succeced!!! "); //chamto test
			}
		}
			

		public void Block()
		{
			if (Skill.eKind.Idle == _skill_current.kind || true == this.Valid_OpenTime ()) 
			{
				//아이들상태거나 연결시간안에 행동이 들어온 경우
				SetSkill (Skill.eKind.Block_1);

				//DebugWide.LogBlue ("succeced!!! "); //chamto test
			}

		}

		public void Idle()
		{
			SetSkill (Skill.eKind.Idle);
		}

		public void Update()
		{
			this._timeDelta += Time.deltaTime;
			
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
					
					if (_behavior.runningTime <= this._timeDelta) 
					{
						//동작완료
						this.SetState (eState.Waiting);
					}	
				}
				break;
			case eState.Waiting:
				{
					//DebugWide.LogBlue (_behavior.rigidTime + "   " + (this._timeDelta - _behavior.allTime));
					if (_behavior.rigidTime <= (this._timeDelta - _behavior.runningTime)) 
					{
						this.SetState (eState.End);
					}	

				}
				break;
			case eState.End:
				{
					_behavior = _skill_current.NextBehavior ();

					if (null == _behavior) 
					{
						//스킬 동작을 모두 꺼냈으면 아이들상태로 들어간다
						Idle ();
					} else 
					{
						//다음 스킬 동작으로 넘어간다
						SetState (eState.Start);
					}
						
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
			bhvo.runningTime = 1f;

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
			bhvo.runningTime = 1f;
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
			bhvo.runningTime = 1f;
			bhvo.scopeTime_0 = 0.4f;
			bhvo.scopeTime_1 = 0.8f;
			bhvo.rigidTime = 0.3f;
			bhvo.openTime_0 = 0.7f;
			bhvo.openTime_1 = 1f;
			skinfo.Add (bhvo);

			return skinfo;
		}

		static public Skill Details_Attack_3Combo()
		{
			Skill skinfo = new Skill ();

			skinfo.kind = eKind.Attack_3Combo;

			Behavior bhvo = new Behavior ();
			bhvo.runningTime = 1f;
			bhvo.scopeTime_0 = 0f;
			bhvo.scopeTime_1 = 0f;
			bhvo.openTime_0 = Behavior.MIN_OPEN_TIME;
			bhvo.openTime_1 = Behavior.MAX_OPEN_TIME;
			skinfo.Add (bhvo);

			bhvo = new Behavior ();
			bhvo.runningTime = 1f;
			bhvo.scopeTime_0 = 0f;
			bhvo.scopeTime_1 = 0f;
			bhvo.openTime_0 = Behavior.MIN_OPEN_TIME;
			bhvo.openTime_1 = Behavior.MAX_OPEN_TIME;
			skinfo.Add (bhvo);

			bhvo = new Behavior ();
			bhvo.runningTime = 1f;
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
			bhvo.runningTime = 1f;
			bhvo.scopeTime_0 = 0f;
			bhvo.scopeTime_1 = 0f;
			bhvo.openTime_0 = Behavior.MIN_OPEN_TIME;
			bhvo.openTime_1 = Behavior.MAX_OPEN_TIME;
			skinfo.Add (bhvo);


			return skinfo;
		}

		static public Skill Details_CounterBlock()
		{
			Skill skinfo = new Skill ();

			skinfo.kind = eKind.CounterBlock;

			Behavior bhvo = new Behavior ();
			bhvo.runningTime = 1f;
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
			this.Add (Skill.eKind.Attack_3Combo, Skill.Details_Attack_3Combo ());

			this.Add (Skill.eKind.Block_1, Skill.Details_Block_1 ());

			this.Add (Skill.eKind.CounterBlock, Skill.Details_CounterBlock ());

		}
	}


	public class ResourceManager
	{
		public enum eSPRITE_NAME
		{
			NONE,
			EMPTY_CARD,

			P1_IDLE,
			P1_ATTACK_BEFORE,
			P1_ATTACK_VALID,
			P1_ATTACK_AFTER,
			P1_BLOCK_BEFORE,
			P1_BLOCK_VALID,
			P1_BLOCK_AFTER,

			P2_IDLE,
			P2_ATTACK_BEFORE,
			P2_ATTACK_VALID,
			P2_ATTACK_AFTER,
			P2_BLOCK_BEFORE,
			P2_BLOCK_VALID,
			P2_BLOCK_AFTER,
			MAX
		}



		private Dictionary<eSPRITE_NAME, string> _sprNameDict = null;
		private Dictionary<eSPRITE_NAME, Sprite> _loadedDict = new Dictionary<eSPRITE_NAME, Sprite> ();


		public void Init()
		{
			_sprNameDict = new Dictionary<eSPRITE_NAME, string> ()
			{
				{eSPRITE_NAME.EMPTY_CARD, "empty_card"},
				{eSPRITE_NAME.P1_IDLE, "p1_idle"},
				{eSPRITE_NAME.P1_ATTACK_BEFORE, "p1_attack_before"},
				{eSPRITE_NAME.P1_ATTACK_VALID, "p1_attack_valid"},
				{eSPRITE_NAME.P1_ATTACK_AFTER, "p1_attack_after"},
				{eSPRITE_NAME.P1_BLOCK_BEFORE, "p1_block_before"},
				{eSPRITE_NAME.P1_BLOCK_VALID, "p1_block_valid"},
				{eSPRITE_NAME.P1_BLOCK_AFTER, "p1_block_after"},

				{eSPRITE_NAME.P2_IDLE, "p2_idle"},
				{eSPRITE_NAME.P2_ATTACK_BEFORE, "p2_attack_before"},
				{eSPRITE_NAME.P2_ATTACK_VALID, "p2_attack_valid"},
				{eSPRITE_NAME.P2_ATTACK_AFTER, "p2_attack_after"},
				{eSPRITE_NAME.P2_BLOCK_BEFORE, "p2_block_before"},
				{eSPRITE_NAME.P2_BLOCK_VALID, "p2_block_valid"},
				{eSPRITE_NAME.P2_BLOCK_AFTER, "p2_block_after"}
			};

		}

		public void Load_BattleCard()
		{
			Sprite[] sprites = Resources.LoadAll <Sprite>("Texture/battleCard");

			for(int i=0;i<sprites.Length;i++)
			{
				//20170813 chamto fixme - value 값이 없을 때의 예외 처리가 없음 
				//ref : https://stackoverflow.com/questions/2444033/get-dictionary-key-by-value
				eSPRITE_NAME key = _sprNameDict.FirstOrDefault(x => x.Value == sprites [i].name).Key;
				_loadedDict.Add (key, sprites [i]);
			}


		}

		public Sprite GetSprite(eSPRITE_NAME eName)
		{
			//20170813 chamto fixme - enum 값이 없을 때의 예외 처리가 없음 
			return _loadedDict [eName];
		}

		public void TestPrint()
		{
			foreach (Sprite s in _loadedDict.Values) 
			{
				DebugWide.LogBlue (s.name);
			}
			foreach (eSPRITE_NAME s in _loadedDict.Keys) 
			{
				DebugWide.LogBlue (s);
			}
		}
	}//end class




	public class Simulation_Battle2 : MonoBehaviour 
	{

		private CharacterManager _crtMgr = null;
		private Character _1Player = null;
		private Character _2Player = null;
		private ResourceManager _rscMgr = null;

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

			_rscMgr = CSingleton<ResourceManager>.Instance;
			_rscMgr.Init ();
			_rscMgr.Load_BattleCard ();

		}

		void Update_UI_Explan()
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

		void Update_UI_Card()
		{
			
			//====//====//====//====//====//====

			_1pSprite_02.gameObject.SetActive (false);
			_1pSprite_03.gameObject.SetActive (false);
			_2pSprite_02.gameObject.SetActive (false);
			_2pSprite_03.gameObject.SetActive (false);

			_1pSprite_01.sprite = _rscMgr.GetSprite(ResourceManager.eSPRITE_NAME.P1_IDLE);
			_2pSprite_01.sprite = _rscMgr.GetSprite(ResourceManager.eSPRITE_NAME.P2_IDLE);

			//1p
			if (Skill.eKind.Attack_1 == _1Player.CurrentSkillKind ()) 
			{
				_1pSprite_02.gameObject.SetActive (true);

				switch (_1Player.CurrentState ()) 
				{
				case Character.eState.Running:
					{
						
						if (false == _1Player.Valid_ScopeTime ()) 
						{
							_1pSprite_02.sprite = _rscMgr.GetSprite(ResourceManager.eSPRITE_NAME.P1_ATTACK_BEFORE);		
						} else 
						{
							_1pSprite_03.gameObject.SetActive (true);
							_1pSprite_03.sprite = _rscMgr.GetSprite (ResourceManager.eSPRITE_NAME.P1_ATTACK_VALID);
						}
					}

					break;
				case Character.eState.Waiting:
					_1pSprite_02.sprite = _rscMgr.GetSprite(ResourceManager.eSPRITE_NAME.P1_ATTACK_AFTER);
					break;
				
				}
					

			}
			if (Skill.eKind.Block_1 == _1Player.CurrentSkillKind ()) 
			{
				_1pSprite_02.gameObject.SetActive (true);
				_1pSprite_02.sprite = _rscMgr.GetSprite(ResourceManager.eSPRITE_NAME.P1_BLOCK_BEFORE);
			}


			//2p
			if (Skill.eKind.Attack_1 == _2Player.CurrentSkillKind ()) 
			{
				_2pSprite_02.gameObject.SetActive (true);

				switch (_2Player.CurrentState ()) 
				{
				case Character.eState.Running:
					{

						if (false == _2Player.Valid_ScopeTime ()) 
						{
							_2pSprite_02.sprite = _rscMgr.GetSprite(ResourceManager.eSPRITE_NAME.P2_ATTACK_BEFORE);		
						} else 
						{
							_2pSprite_03.gameObject.SetActive (true);
							_2pSprite_03.sprite = _rscMgr.GetSprite (ResourceManager.eSPRITE_NAME.P2_ATTACK_VALID);
						}
					}

					break;
				case Character.eState.Waiting:
					_2pSprite_02.sprite = _rscMgr.GetSprite(ResourceManager.eSPRITE_NAME.P2_ATTACK_AFTER);
					break;

				}
			}
			if (Skill.eKind.Block_1 == _2Player.CurrentSkillKind ()) 
			{
				_2pSprite_02.gameObject.SetActive (true);
				_2pSprite_02.sprite = _rscMgr.GetSprite(ResourceManager.eSPRITE_NAME.P2_BLOCK_BEFORE);
			}

			//====//====//====//====//====//====


		}//end func



		// Update is called once per frame
		void Update () 
		{

			_crtMgr.Update ();

			this.Update_UI_Explan ();
			this.Update_UI_Card ();

			//////////////////////////////////////////////////
			//1p

			//attack
			if (Input.GetKeyUp ("q")) 
			{
				//DebugWide.LogBlue ("1p - keyinput");
				_1Player.Attack_1 ();
			}
			//block
			if (Input.GetKeyUp ("w")) 
			{
				//DebugWide.LogBlue ("1p - keyinput");
				_1Player.Block ();
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

		}//end Update

	}//end class Simulation 




}//end namespace 


