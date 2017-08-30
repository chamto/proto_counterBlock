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

		public Behavior GetBehavior()
		{
			return _behavior;
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
		public enum eActionKind
		{
			None,
			Idle,
			AttackBefore,
			AttackValid,
			AttackAfter,

			BlockBefore,
			BlockValid,
			BlockAfter,

			CounterBlock,
			Max
		}

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


		private HierarchyPreLoader _ref_herch = null;

		private Dictionary<eSPRITE_NAME, string> _sprNameDict = null;
		private Dictionary<eSPRITE_NAME, Sprite> _loadedDict = new Dictionary<eSPRITE_NAME, Sprite> ();

		public ResourceManager()
		{
			_ref_herch = CSingleton<HierarchyPreLoader>.Instance;
		}


		public void Init()
		{
			_ref_herch.Init ();

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


			this.Load_BattleCard ();
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

		public Sprite GetAction_Seonbi(eActionKind actionKind)
		{
			switch (actionKind) 
			{
			case eActionKind.None:
			case eActionKind.Idle:
				return this.GetSprite (ResourceManager.eSPRITE_NAME.P1_IDLE);
			case eActionKind.AttackBefore:
				return this.GetSprite (ResourceManager.eSPRITE_NAME.P1_ATTACK_BEFORE);
			case eActionKind.AttackValid:
				return this.GetSprite (ResourceManager.eSPRITE_NAME.P1_ATTACK_VALID);
			case eActionKind.AttackAfter:
				return this.GetSprite (ResourceManager.eSPRITE_NAME.P1_ATTACK_AFTER);
			case eActionKind.BlockBefore:
				return this.GetSprite (ResourceManager.eSPRITE_NAME.P1_BLOCK_BEFORE);
			case eActionKind.BlockValid:
				return this.GetSprite (ResourceManager.eSPRITE_NAME.P1_BLOCK_VALID);
			case eActionKind.BlockAfter:
				return this.GetSprite (ResourceManager.eSPRITE_NAME.P1_BLOCK_AFTER);
			case eActionKind.CounterBlock:
				return this.GetSprite (ResourceManager.eSPRITE_NAME.EMPTY_CARD); //chamto temp value

			}

			return null;
		}

		public Sprite GetAction_Biking(eActionKind actionKind)
		{
			switch (actionKind) 
			{
			case eActionKind.None:
			case eActionKind.Idle:
				return this.GetSprite (ResourceManager.eSPRITE_NAME.P2_IDLE);
			case eActionKind.AttackBefore:
				return this.GetSprite (ResourceManager.eSPRITE_NAME.P2_ATTACK_BEFORE);
			case eActionKind.AttackValid:
				return this.GetSprite (ResourceManager.eSPRITE_NAME.P2_ATTACK_VALID);
			case eActionKind.AttackAfter:
				return this.GetSprite (ResourceManager.eSPRITE_NAME.P2_ATTACK_AFTER);
			case eActionKind.BlockBefore:
				return this.GetSprite (ResourceManager.eSPRITE_NAME.P2_BLOCK_BEFORE);
			case eActionKind.BlockValid:
				return this.GetSprite (ResourceManager.eSPRITE_NAME.P2_BLOCK_VALID);
			case eActionKind.BlockAfter:
				return this.GetSprite (ResourceManager.eSPRITE_NAME.P2_BLOCK_AFTER);
			case eActionKind.CounterBlock:
				return this.GetSprite (ResourceManager.eSPRITE_NAME.EMPTY_CARD); //chamto temp value

			}

			return null;
		}

		public ResourceManager.eSPRITE_NAME GetSpriteName_Seonbi(eActionKind actionKind)
		{
			ResourceManager.eSPRITE_NAME sprName = ResourceManager.eSPRITE_NAME.NONE;

			switch (actionKind) 
			{
			case eActionKind.None:
			case eActionKind.Idle:
				sprName = ResourceManager.eSPRITE_NAME.P1_IDLE;
				break;
			case eActionKind.AttackBefore:
				sprName = ResourceManager.eSPRITE_NAME.P1_ATTACK_BEFORE;
				break;
			case eActionKind.AttackValid:
				sprName = ResourceManager.eSPRITE_NAME.P1_ATTACK_VALID;
				break;
			case eActionKind.AttackAfter:
				sprName = ResourceManager.eSPRITE_NAME.P1_ATTACK_AFTER;
				break;
			case eActionKind.BlockBefore:
				sprName = ResourceManager.eSPRITE_NAME.P1_BLOCK_BEFORE;
				break;
			case eActionKind.BlockValid:
				sprName = ResourceManager.eSPRITE_NAME.P1_BLOCK_VALID;
				break;
			case eActionKind.BlockAfter:
				sprName = ResourceManager.eSPRITE_NAME.P1_BLOCK_AFTER;
				break;
			case eActionKind.CounterBlock:
				sprName = ResourceManager.eSPRITE_NAME.EMPTY_CARD; //chamto temp value
				break;

			}

			return sprName;
		}

		public ResourceManager.eSPRITE_NAME GetSpriteName_Biking(eActionKind actionKind)
		{
			ResourceManager.eSPRITE_NAME sprName = ResourceManager.eSPRITE_NAME.NONE;

			switch (actionKind) 
			{
			case eActionKind.None:
			case eActionKind.Idle:
				sprName = ResourceManager.eSPRITE_NAME.P2_IDLE;
				break;								  
			case eActionKind.AttackBefore:			  
				sprName = ResourceManager.eSPRITE_NAME.P2_ATTACK_BEFORE;
				break;								  
			case eActionKind.AttackValid:			   
				sprName = ResourceManager.eSPRITE_NAME.P2_ATTACK_VALID;
				break;								  
			case eActionKind.AttackAfter:			   
				sprName = ResourceManager.eSPRITE_NAME.P2_ATTACK_AFTER;
				break;								  
			case eActionKind.BlockBefore:			   
				sprName = ResourceManager.eSPRITE_NAME.P2_BLOCK_BEFORE;
				break;								  
			case eActionKind.BlockValid:				
				sprName = ResourceManager.eSPRITE_NAME.P2_BLOCK_VALID;
				break;								  
			case eActionKind.BlockAfter:			
				sprName = ResourceManager.eSPRITE_NAME.P2_BLOCK_AFTER;
				break;
			case eActionKind.CounterBlock:
				sprName = ResourceManager.eSPRITE_NAME.EMPTY_CARD; //chamto temp value
				break;

			}

			return sprName;
		}

		public GameObject CreatePrefab(string prefabPath , Transform parent , string name)
		{
			const string root = "Prefab/";
			GameObject obj =  MonoBehaviour.Instantiate(Resources.Load(root + prefabPath)) as GameObject;
			obj.transform.SetParent (parent,false);
			obj.transform.name = name;

			_ref_herch.PreOrderTraversal (obj.transform);

			return obj;
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


	public class UI_CharacterCard : MonoBehaviour
	{

		public enum eKind
		{
			None,
			Seonbi,
			Biking,
			Max
		};

		public uint _id = 0;
		public eKind _kind = eKind.None;

		public Text _text_explanation { get; set; }
		public Text _text_time { get; set; }
		public Slider _hp_bar { get; set; }

		public Transform _sprites { get; set; }
		public List<Image> _action { get; set; }


		void Start()
		{
			
		}

		public void TurnLeft()
		{
			Vector3 scale = _sprites.localScale;
			scale.x = -1f;
			_sprites.localScale = scale;
		}

		public void TurnRight()
		{
			Vector3 scale = _sprites.localScale;
			scale.x = 1f;
			_sprites.localScale = scale;
		}

		public void SetCharacter(eKind kind)
		{
			_kind = kind;
		}

		static public UI_CharacterCard Create(string name)
		{
			GameObject obj = Single.resource.CreatePrefab ("character_seonbi", Single.ui_root, name);

			string parentPath = Single.hierarchy.GetTransformFullPath (obj.transform);

			UI_CharacterCard ui = obj.AddComponent<UI_CharacterCard> ();
			ui._text_explanation = Single.hierarchy.Find<Text> (parentPath + "/Text_explanation");
			ui._text_time = Single.hierarchy.Find<Text> (parentPath + "/Text_time");
			ui._hp_bar = Single.hierarchy.Find<Slider> (parentPath + "/Slider");
			ui._sprites = Single.hierarchy.Find<Transform> (parentPath + "/Images");
			ui._action = new List<Image> ();
			ui._action.Add (Single.hierarchy.Find<Image> (parentPath + "/Images/Action_00"));
			ui._action.Add (Single.hierarchy.Find<Image> (parentPath + "/Images/Action_01"));
			ui._action.Add (Single.hierarchy.Find<Image> (parentPath + "/Images/Action_02"));

			return ui;
		}

	}

	public class UI_Battle : MonoBehaviour
	{

		private Transform _1P_start = null;
		private Transform _2P_start = null;

		private Dictionary<uint, UI_CharacterCard> _characters = new Dictionary<uint, UI_CharacterCard> ();

		
		public void Init()
		{
			this.transform.SetParent (Single.ui_root, false);

			string parentPath = Single.hierarchy.GetTransformFullPath (Single.ui_root);
			_1P_start = Single.hierarchy.Find<Transform> (parentPath + "/startPoint_1");
			_2P_start = Single.hierarchy.Find<Transform> (parentPath + "/startPoint_2");
		
		}

		public UI_CharacterCard AddCharacter(UI_CharacterCard.eKind kind, uint id)
		{
			UI_CharacterCard card = UI_CharacterCard.Create ("player_"+id.ToString("00"));
			card._kind = kind;
			card._id = id;
			_characters.Add (id, card);

			return card;
		}



		public void SetStartPoint(uint id, float delta_x , uint pointNumber)
		{
			const int START_POINT_LEFT = 1;
			const int START_POINT_RIGHT = 2;

			UI_CharacterCard card = _characters [id];
			Vector3 pos = Vector3.zero;

			switch (pointNumber) 
			{
			case START_POINT_LEFT:
				pos = _1P_start.localPosition;
				card.TurnRight ();
				break;
			case START_POINT_RIGHT:
				pos = _2P_start.localPosition;
				card.TurnLeft ();
				break;
			}


			pos.x += delta_x;
			card.transform.localPosition = pos;

		}


		public Sprite GetAction(UI_CharacterCard.eKind charKind ,ResourceManager.eActionKind actionKind)
		{
			switch (charKind) 
			{
			case UI_CharacterCard.eKind.Seonbi:
				return Single.resource.GetAction_Seonbi (actionKind);
			case UI_CharacterCard.eKind.Biking:
				return Single.resource.GetAction_Biking (actionKind);
			}

			return null;
		}



		private void Update_UI_Explan(Character charData, uint id)
		{
			UI_CharacterCard charUI = _characters [id];

			charUI._text_explanation.text = 
				"  "  + Character.StateToString(charData.CurrentState());

			charUI._text_time.text = 
				Skill.KindToString(charData.CurrentSkillKind()) + "   " +
				charData.GetTimeDelta().ToString("0.0");
		

		}

		private void Update_UI_Card(Character charData, uint id)
		{
			UI_CharacterCard charUI = _characters [id];


			charUI._action[1].gameObject.SetActive (false);
			charUI._action[2].gameObject.SetActive (false);


			charUI._action [0].sprite = this.GetAction (charUI._kind, ResourceManager.eActionKind.Idle);


			if (Skill.eKind.Attack_1 == charData.CurrentSkillKind ()) 
			{
				charUI._action[1].gameObject.SetActive (true);

				switch (charData.CurrentState ()) 
				{
				case Character.eState.Running:
					{

						if (false == charData.Valid_ScopeTime ()) 
						{
							

							charUI._action[1].sprite = this.GetAction (charUI._kind, ResourceManager.eActionKind.AttackBefore);
						} else 
						{

							charUI._action[2].gameObject.SetActive (true);
							charUI._action[2].sprite = this.GetAction (charUI._kind, ResourceManager.eActionKind.AttackValid);


						}
					}

					break;
				case Character.eState.Waiting:
					charUI._action[1].sprite = this.GetAction (charUI._kind, ResourceManager.eActionKind.AttackAfter);
					break;

				}


			}
			if (Skill.eKind.Block_1 == charData.CurrentSkillKind ()) 
			{
				charUI._action[1].gameObject.SetActive (true);
				charUI._action[1].sprite = this.GetAction (charUI._kind, ResourceManager.eActionKind.BlockBefore);
			}


		}//end func


		public void Update_UI(Character charData, uint idx)
		{
			this.Update_UI_Explan (charData, idx);
			this.Update_UI_Card (charData, idx);
			
		}

	}


	public class Simulation_Battle2 : MonoBehaviour 
	{

		private CharacterManager _crtMgr = null;
		private Character _1Player = null;
		private Character _2Player = null;
		private ResourceManager _rscMgr = null;

		//====//====//====//====//====//====
		private UI_Battle _ui_battle = null;


		// Use this for initialization
		void Start () 
		{
			const uint ID_PLAYER_1 = 1;
			const uint ID_PLAYER_2 = 2;

			_crtMgr = new CharacterManager ();
			_crtMgr.Init ();

			_1Player = _crtMgr [ID_PLAYER_1];
			_2Player = _crtMgr [ID_PLAYER_2];

			_rscMgr = CSingleton<ResourceManager>.Instance;
			_rscMgr.Init ();

			_ui_battle = this.gameObject.AddComponent<UI_Battle> ();
			_ui_battle.Init ();

			this.CreatePlayer ();

		}

		public void CreatePlayer()
		{
			foreach (Character chter in _crtMgr.Values) 
			{
				UI_CharacterCard card = _ui_battle.AddCharacter (UI_CharacterCard.eKind.Biking, chter.GetID ());

				if((chter.GetID () % 2) == 1) //홀수는 왼쪽 
					_ui_battle.SetStartPoint (chter.GetID (), 0, 1);					
				if((chter.GetID () % 2) == 0) //짝수는 오른쪽 
					_ui_battle.SetStartPoint (chter.GetID (), 0, 2);

			}
		}


		class Animation
		{
			
			public enum eState
			{
				None,
				Start,
				Running,
				End,
				Max
			}
				
			private eState _state = eState.None;
			private float _accumulate = 0f;
			private float _scaleDelta = 0f;
			private Vector3 _originalPos;

			public void Card_Move(Transform dst, float start, float end, float maxSecond)
			{


				switch (_state) 
				{
				case eState.None:
				case eState.Start:
					{
						_state = eState.Running;
						_accumulate = 0f;
						_scaleDelta = 0f;
						_originalPos = dst.localPosition;
					}
					break;

				case eState.Running:
					{
						_accumulate += Time.deltaTime;
						if (maxSecond <= _accumulate) 
						{
							_state = eState.End;
							break;
						}

						_scaleDelta = Utility.Interpolation.easeOutElastic (start, end, _accumulate/maxSecond);

						//-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
						dst.Translate(_scaleDelta,0,0);
						DebugWide.LogBlue (_scaleDelta); //chamto test
						//-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
					}
					break;
				case eState.End:
					{
						dst.localPosition = _originalPos;
					}
					break;
				}

			}


		}//end class

		//							float maxSecond = 0.5f; 
		//							float scaleDelta = Utility.Interpolation.easeOutElastic (0f,10f, accumulate/maxSecond);
		//
		//							Vector3 pos = _1pSprite_03.transform.localPosition;
		//							pos.x += scaleDelta;
		//							//pos.x = 50f;
		//							_1pSprite_03.transform.localPosition = pos;
		//							//_1pSprite_03.transform.Translate(scaleDelta,0,0);
		//							DebugWide.LogBlue (scaleDelta);
		//
		//							accumulate += Time.deltaTime;






		// Update is called once per frame
		void Update () 
		{

			_crtMgr.Update ();

			foreach (Character chter in _crtMgr.Values) 
			{
				_ui_battle.Update_UI (chter, chter.GetID());
			}


			//////////////////////////////////////////////////
			//1p

			//attack
			if (Input.GetKeyUp ("q")) 
			{
				//iTween.PunchPosition(_1pSprite_01.gameObject, iTween.Hash("x",20,"loopType","loop","time",0.5f));
				//iTween.MoveBy(_1pSprite_01.gameObject, iTween.Hash("x", 30, "easeType", "easeInOutExpo", "loopType", "pingPong", "delay", .1));
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


