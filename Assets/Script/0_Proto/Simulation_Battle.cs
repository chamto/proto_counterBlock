﻿/// <summary>
/// 
/// Simulation_Battle
/// 
/// 20170802 - chamto - anrudco@gmail.com
/// 
/// </summary>

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace CounterBlockSting
{
	

	public struct BehaviorTime
	{
		//--------<<============>>----------
		//    minOpenTime ~ maxOpenTime
		// 시간범위 안에 있어야 콤보가 된다
		public const float MAX_OPEN_TIME = 10f;
		public const float MIN_OPEN_TIME = 0f;

		//===================================

		public float time_before;
		public float time_after;
		public float scope_start;
		public float scope_end;

		public float max_openTime; //최대 연결시간 : 최대 시간 안에 동작 해야 함
		public float min_openTime; //최소 연결시간 : 최소 시간 이후에 동작 해야 함

		public void Init()
		{
			time_before = 0f;
			time_after = 0f;
			scope_start = 0f;
			scope_end = 0f;
		}
	}


	public class CharacterInfo
	{

		public enum eState
		{
			None 		= 0,

			Init,
			Before,
			After,

			Attack,
			Attack_Init = Attack,
			Attack_Before,
			Attack_After,
			Block,
			Block_Init = Block,
			Block_Before,
			Block_After,
			Idle,
			Hit,
			Max,
		}


		//Action cooldown 
		//(* As : A start , Ae : A end)
		//|--------------<----------|--------------->--------|
		//As            Cs        Ae,Bs            Ce        Be
		//A.행동이 완료되기 까지 걸리는 시간
		//B.행동이 완료된 후 대기상태로 돌아오는 시간
		//C.행동 효과 범위


		private BehaviorTime _behavior;
		private eSkillKind _skill_current;

		private float 		 _timeDelta; 	//시간변화량

		private uint 	_hp;

		private eState 	_state_prev;
		private eState 	_state_current;
		private eState 	_state_next;
		private bool _state_used;

		private SkillBook _skillBook = null;

		public CharacterInfo()
		{
			_skillBook = CSingleton<SkillBook>.Instance;
			_behavior.Init();
			_skill_current = eSkillKind.None;

			_timeDelta = 0f;

			this._hp = 30;
			_state_prev = eState.None;
			_state_current = eState.None;
			_state_next = eState.None;
			_state_used = false;
		}
			
		public bool IsAttacking()
		{
			if (eState.Attack_Init == _state_current ||
				eState.Attack_Before == _state_current ||
				eState.Attack_After == _state_current )
				return true;
			return false;
		}

		public bool IsBlocking()
		{
			if (eState.Block_Init == _state_current ||
				eState.Block_Before == _state_current ||
				eState.Block_After == _state_current )
				return true;
			return false;
		}

		public bool Valid_Attack()
		{
			if (eState.Attack_Before == _state_current) 
			{
				//if (_scope_start.attack <= _timeDelta)
				if (_behavior.scope_start <= _timeDelta)
					return true;
			}

			if (eState.Attack_After == _state_current) 
			{
				//if (_timeDelta <= _scope_end.attack)
				if (_timeDelta <= _behavior.scope_end)
					return true;
			}

			return false;
		}

		public bool Valid_Block()
		{
			if (eState.Block_Before == _state_current) 
			{
				//if (_scope_start.block <= _timeDelta)
				if (_behavior.scope_start <= _timeDelta)
					return true;
			}

			if (eState.Block_After == _state_current) 
			{
				//if (_timeDelta <= _scope_end.block)
				if (_timeDelta <= _behavior.scope_end)
					return true;
			}

			return false;
		}


		public uint GetHP()
		{
			return _hp;
		}
		public void SetHP(uint hp)
		{
			_hp = hp;
		}

		public void HP_SubTract(uint amount)
		{
			if (0 == _hp)
				return;
			
			_hp -= amount;
		}

		public void HP_Addition(uint amount)
		{
			const uint TEMP_MAXHP = 100; //20170807 chamto fixme - temp value
			if (TEMP_MAXHP <= _hp)
				return;

			_hp += amount;

		}

		//동작을 사용 했다고 표시 , 한동작을 한번만 공격충돌 시키기 위해 필요 
		public void SetUsedState()
		{
			_state_used = true;
		}

		public bool GetUsedState()
		{
			return _state_used;
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
			this._timeDelta = 0f;
			_state_prev = _state_current;
			_state_current = setState;

		}

		public void NextState(eState nextState)
		{
			_state_next = nextState;
		}

		public void SetBehavior(CounterBlockSting.eSkillKind kind, int sequence)
		{
			_behavior = _skillBook [kind].ElementAt(sequence);
		}

		public void SetBegavior_CounterBlock()
		{
			this.SetBehavior (CounterBlockSting.eSkillKind.CounterBlock, 0);
		}



		public void Update_State()
		{
			this._timeDelta += Time.deltaTime;

			switch (this._state_current) 
			{
			case eState.Init:
				{
					_state_used = false; //init
					this.SetState (eState.Before);	

					 //_behavior = _skillBook [CounterBlockSting.Battle.eSkillKind.Attack_1].ElementAt(0);
				}
				break;
			case eState.Before:
				{
					if (_behavior.time_before <= this._timeDelta) 
					{
						this.SetState (eState.After);
					}		
				}
				break;
			case eState.After:
				{
					if (_behavior.time_after <= this._timeDelta) 
					{
						this.SetState (eState.Init);
					}
				}
				break;
			case eState.Attack_Init:
				{
					_state_used = false; //init
					this.SetState (eState.Attack_Before);	

					_behavior = _skillBook [CounterBlockSting.eSkillKind.Attack_1].ElementAt(0);
				}
				break;
			case eState.Attack_Before:
				{
					
					//if (_time_before.attack <= this._timeDelta) 
					if (_behavior.time_before <= this._timeDelta) 
					{
						this.SetState (eState.Attack_After);
					}		
				}
				break;
			case eState.Attack_After:
				{
					//if (_time_after.attack <= this._timeDelta) 
					if (_behavior.time_after <= this._timeDelta) 
					{
						this.SetState (_state_next);
						_state_next = eState.Idle;
					}
				}
				break;
			case eState.Block_Init:
				{
					_state_used = false; //init
					this.SetState (eState.Block_Before);

					_behavior = _skillBook [CounterBlockSting.eSkillKind.Block_1].ElementAt(0);
				}
				break;
			case eState.Block_Before:
				{
					//if (_time_before.block <= this._timeDelta) 
					if (_behavior.time_before <= this._timeDelta) 
					{
						this.SetState (eState.Block_After);
					}
				}
				break;
			case eState.Block_After:
				{
					//if (_time_after.block <= this._timeDelta) 
					if (_behavior.time_after <= this._timeDelta) 
					{
						this.SetState (_state_next);
						_state_next = eState.Idle;
					}
				}
				break;
			case eState.Idle:
			case eState.Hit:
			case eState.None:
				{
					_state_used = false; //init

					//default loop 
					//if (_time_before.idle <= this._timeDelta) 
					if (_behavior.time_before <= this._timeDelta) 
					{
						this.SetState (eState.Idle);
					}

					//다음 상태가 있을 경우
					if (eState.Idle != _state_next) 
					{
						this.SetState (_state_next);
						_state_next = eState.Idle;
					}
				}
				break;
			}

		}//end func




	}//end class

	public class ResourceMgr 
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



		/// <summary>
		/// 판정
		/// Judgment.
		/// </summary>
		public class Judgment
		{

			public struct Result
			{
				public eState state_1p;
				public eState state_2p;

				public Result(eState s1p, eState s2p)
				{
					state_1p = s1p;
					state_2p = s2p;
				}

				public void Init()
				{
					state_1p = eState.None;
					state_2p = eState.None;
				}
			}

			public enum eState
			{
				None = 0,

				AttackSucceed,
				AttackFail,
				AttackIdle, //공격 헛 동작 : 멀리서 공격한 경우

				BlockSucceed,
				BlockFail,
				BlockIdle, //방어 헛 동작

				Max
			}


			//Simulation <-> Judgment
			private CharacterInfo _ref_1pInfo = null;
			private CharacterInfo _ref_2pInfo = null;

			private  Result _result; 


			//==========================================

			//public Result result { get { return _result; }} 
			public Result GetResult()
			{
				return _result;
			}

			public Judgment(ref CharacterInfo ref_1p , ref CharacterInfo ref_2p)
			{
				_ref_1pInfo = ref_1p;
				_ref_2pInfo = ref_2p;

				_result.Init ();
			}

			/// <summary>
			/// Succeeds the attack.
			/// </summary>
			/// <returns>
			/// 	none = 0 , 1p => 1 , 2p => 2 
			/// </returns>
			public Result Valid_Attack()
			{
				//todo : 거리에 대한 처리 추가 해야 함

				CharacterInfo.eState state1p =  _ref_1pInfo.CurrentState();
				CharacterInfo.eState state2p =  _ref_2pInfo.CurrentState();

				Result result = new Result (eState.None , eState.None);

				//attack
				if (true == _ref_1pInfo.Valid_Attack () && false == _ref_2pInfo.Valid_Block ()) 
				{
					//!!!!!!//!!!!!!//!!!!!!//!!!!!!//!!!!!!//!!!!!!
					result.state_1p = eState.AttackSucceed; 
					//!!!!!!//!!!!!!//!!!!!!//!!!!!!//!!!!!!//!!!!!!

					if(CharacterInfo.eState.Block_Before == state2p || CharacterInfo.eState.Block_After == state2p)
						result.state_2p = eState.BlockFail;
					else
						result.state_2p = eState.None;
				}

				if (true == _ref_2pInfo.Valid_Attack () && false == _ref_1pInfo.Valid_Block ()) 
				{
					if(CharacterInfo.eState.Block_Before == state1p || CharacterInfo.eState.Block_After == state1p)
						result.state_1p = eState.BlockFail;
					else
						result.state_1p = eState.None;

					//!!!!!!//!!!!!!//!!!!!!//!!!!!!//!!!!!!//!!!!!!
					result.state_2p = eState.AttackSucceed;
					//!!!!!!//!!!!!!//!!!!!!//!!!!!!//!!!!!!//!!!!!!
				}

				//block
				if (CharacterInfo.eState.Block_Before == state1p || CharacterInfo.eState.Block_After == state1p) 
				{
					if (true == _ref_1pInfo.Valid_Block () )
					{

						if (true == _ref_2pInfo.Valid_Attack ()) {
							result.state_1p = eState.BlockSucceed;	
							result.state_2p = eState.AttackFail;
						} else 
						{
							result.state_1p = eState.BlockIdle;	
						}

					}
				}

				if (CharacterInfo.eState.Block_Before == state2p || CharacterInfo.eState.Block_After == state2p) 
				{
					if (true == _ref_2pInfo.Valid_Block () )
					{

						if (true == _ref_1pInfo.Valid_Attack ()) {
							result.state_2p = eState.BlockSucceed;	
							result.state_1p = eState.AttackFail;
						} else 
						{
							result.state_2p = eState.BlockIdle;	
						}

					}
				}





				return result;
			}


			public void Update()
			{
				_result =  Valid_Attack ();


				//apply jugement : HP
				//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
				switch (_result.state_1p) 
				{
				case eState.AttackSucceed:
					if(false == _ref_1pInfo.GetUsedState()) //한동작에서 처음 공격만 적용
					{
						_ref_1pInfo.SetUsedState ();
						_ref_2pInfo.HP_SubTract (1);
						_ref_2pInfo.SetState (CharacterInfo.eState.Hit);
					}

					break;
				}

				switch (_result.state_2p) 
				{
				case eState.AttackSucceed:
					if (false == _ref_2pInfo.GetUsedState ()) 
					{
						_ref_2pInfo.SetUsedState ();
						_ref_1pInfo.HP_SubTract (1);
						_ref_1pInfo.SetState (CharacterInfo.eState.Hit);
					}

					break;
				}
				//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

				//apply jugement : behaviour state
				//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
				//방어 성공시 반격방어스킬을 적용 한다
				switch (_result.state_1p) 
				{
				case eState.BlockSucceed:
					{
						_ref_1pInfo.SetBegavior_CounterBlock ();
					}

					break;
				}

				switch (_result.state_2p) 
				{
				case eState.BlockSucceed:
					{
						_ref_2pInfo.SetBegavior_CounterBlock ();
					}

					break;
				}

				//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<


			}//end Update

		}	



		public enum eSkillKind
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

		public class SkillInfo : List<BehaviorTime>
		{
			
			public eSkillKind skillKind { get; set; }

			//스킬 명세서
			static public SkillInfo Details_Idle()
			{
				SkillInfo skinfo = new SkillInfo ();

				skinfo.skillKind = eSkillKind.Idle;

				BehaviorTime bTime = new BehaviorTime ();
				bTime.time_before = 1f;
				bTime.time_after = 0;
				bTime.scope_start = 0f;
				bTime.scope_end = 0f;
				bTime.max_openTime = BehaviorTime.MAX_OPEN_TIME;
				bTime.min_openTime = BehaviorTime.MIN_OPEN_TIME;
				skinfo.Add (bTime);

				return skinfo;
			}

			static public SkillInfo Details_Hit()
			{
				SkillInfo skinfo = new SkillInfo ();

				skinfo.skillKind = eSkillKind.Hit;

				BehaviorTime bTime = new BehaviorTime ();
				bTime.time_before = 1f;
				bTime.time_after = 0.5f;
				bTime.scope_start = 0f;
				bTime.scope_end = 0f;
				bTime.max_openTime = BehaviorTime.MAX_OPEN_TIME;
				bTime.min_openTime = BehaviorTime.MIN_OPEN_TIME;
				skinfo.Add (bTime);

				return skinfo;
			}


			static public SkillInfo Details_Attack_1()
			{
				SkillInfo skinfo = new SkillInfo ();

				skinfo.skillKind = eSkillKind.Attack_1;

				BehaviorTime bTime = new BehaviorTime ();
				bTime.time_before = 0.5f;
				bTime.time_after = 0.8f;
				bTime.scope_start = 0.5f;
				bTime.scope_end = 0f;
				bTime.max_openTime = BehaviorTime.MAX_OPEN_TIME;
				bTime.min_openTime = BehaviorTime.MIN_OPEN_TIME;
				skinfo.Add (bTime);

				return skinfo;
			}

			static public SkillInfo Details_Attack_3Combo()
			{
				SkillInfo skinfo = new SkillInfo ();

				skinfo.skillKind = eSkillKind.Attack_3Combo;

				//1combo
				BehaviorTime bTime = new BehaviorTime ();
				bTime.time_before = 0.5f;
				bTime.time_after = 0.1f;
				bTime.scope_start = 0.3f;
				bTime.scope_end = 0f;
				bTime.max_openTime = BehaviorTime.MAX_OPEN_TIME;
				bTime.min_openTime = BehaviorTime.MIN_OPEN_TIME;
				skinfo.Add (bTime);

				//2combo
				bTime = new BehaviorTime ();
				bTime.time_before = 0.2f;
				bTime.time_after = 0.2f;
				bTime.scope_start = 0.2f;
				bTime.scope_end = 0f;
				bTime.max_openTime = 0.5f;
				bTime.min_openTime = 0.2f;
				skinfo.Add (bTime);

				//3combo
				bTime = new BehaviorTime ();
				bTime.time_before = 0.2f;
				bTime.time_after = 1f;
				bTime.scope_start = 0.2f;
				bTime.scope_end = 0f;
				bTime.max_openTime = 0.5f;
				bTime.min_openTime = 0.2f;
				skinfo.Add (bTime);

				return skinfo;
			}

			static public SkillInfo Details_Block_1()
			{
				SkillInfo skinfo = new SkillInfo ();

				skinfo.skillKind = eSkillKind.Block_1;

				BehaviorTime bTime = new BehaviorTime ();
				bTime.time_before = 0.5f;
				bTime.time_after = 0.5f;
				bTime.scope_start = 0.1f;
				bTime.scope_end = 0.5f;
				bTime.max_openTime = BehaviorTime.MAX_OPEN_TIME;
				bTime.min_openTime = BehaviorTime.MIN_OPEN_TIME;
				skinfo.Add (bTime);

				return skinfo;
			}

			static public SkillInfo Details_CounterBlock()
			{
				SkillInfo skinfo = new SkillInfo ();

				skinfo.skillKind = eSkillKind.CounterBlock;

				BehaviorTime bTime = new BehaviorTime ();
				bTime.time_before = 0.5f;
				bTime.time_after = 0.1f; //후동작이 짧아진다
				bTime.scope_start = 0f;
				bTime.scope_end = 0f;
				bTime.max_openTime = 0.5f;
				bTime.min_openTime = BehaviorTime.MIN_OPEN_TIME;
				skinfo.Add (bTime);

				return skinfo;
			}
		}


		public class SkillBook : Dictionary<eSkillKind, SkillInfo>
		{
			public SkillBook()
			{
				this.Add (eSkillKind.Idle, SkillInfo.Details_Idle ());
				this.Add (eSkillKind.Hit, SkillInfo.Details_Hit ());
				this.Add (eSkillKind.Attack_1, SkillInfo.Details_Attack_1 ());
				this.Add (eSkillKind.Attack_3Combo, SkillInfo.Details_Attack_3Combo ());
				this.Add (eSkillKind.Block_1, SkillInfo.Details_Block_1 ());
				this.Add (eSkillKind.CounterBlock, SkillInfo.Details_CounterBlock ());
			}
		}

		public class SkillMgr
		{
			
			public enum eGestureKind
			{
				None,
				Attack_Sword,
				Block_Sword,
				CounterBlock,
				Max
			}

			//키 , 간격 
			public struct GestureInterval
			{
				public eGestureKind kind;
				public float interval;

				public GestureInterval(eGestureKind k, float inter)
				{
					kind = k;
					interval = inter;
				}
			}

			private class ListInterval : List<GestureInterval>
			{
				private float _lastInputTime = 0;
				public float lastInputTime { get; set; }
			}
			private class DictInterval : Dictionary<uint, ListInterval>  //캐릭터번호 , 키입력간격
			{}

			private DictInterval _dictInterval = new DictInterval();

			public void InsertInterval(uint characterNum, eGestureKind eKind)
			{
				ListInterval list = null;
				float lastInputTime = 0;
				if (true == _dictInterval.TryGetValue (characterNum, out list)) {
					lastInputTime = list.lastInputTime;

				}
					
				GestureInterval gi = new GestureInterval ();
				gi.kind = eKind;
				gi.interval = Time.time - lastInputTime;

				if (null != list) {
					list.lastInputTime = Time.time;
				}
				DebugWide.LogBlue (gi.interval + "  : interval"); //chamto test

				this.InsertInterval (characterNum, gi);
			}

			public void InsertInterval(uint characterNum, GestureInterval gstInterval)
			{
				ListInterval list = null;
				if (false == _dictInterval.TryGetValue (characterNum, out list)) 
				{
					list = new ListInterval ();
					_dictInterval.Add (characterNum, list);
				}

				list.Add (gstInterval);

			}

			public void Update()
			{
				
			}
			//어떤 입력이 스킬이냐

			//3콤보
			//2콤보
			//1타

			

		}

		public class Simulation_Battle : MonoBehaviour 
		{

			private CharacterInfo _1pInfo = new CharacterInfo();
			private CharacterInfo _2pInfo = new CharacterInfo();

			private Judgment _judgment = null;
			private SkillMgr _skillMgr = null;

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

			private ResourceMgr _refResourceMgr = null;
			//====//====//====//====//====//====

			// Use this for initialization
			void Start () 
			{
				_refResourceMgr = CSingleton<ResourceMgr>.Instance;
				_refResourceMgr.Init ();
				_refResourceMgr.Load_BattleCard ();


				_judgment = new Judgment(ref _1pInfo ,ref _2pInfo);
				_skillMgr = new SkillMgr ();

				_1pImage.color = Color.white;
				_2pImage.color = Color.white;


				_1pHpBar.maxValue = _1pInfo.GetHP ();
				_1pHpBar.value = _1pInfo.GetHP ();
				_2pHpBar.maxValue = _2pInfo.GetHP ();
				_2pHpBar.value = _2pInfo.GetHP ();

			}

			public string StateToString (CharacterInfo.eState state)
			{
				switch (state) 
				{
				case CharacterInfo.eState.Attack_Before:
					return "Attack_Before";
				case CharacterInfo.eState.Attack_After:
					return "Attack_After";
				case CharacterInfo.eState.Block_Before:
					return "Block_Before";
				case CharacterInfo.eState.Block_After:
					return "Block_After";
				case CharacterInfo.eState.Idle:
					return "Idle";
				}

				return "None";
			}

			public string JudgmentToString (Judgment.eState state)
			{
				switch (state) 
				{
				case Judgment.eState.AttackSucceed:
					return "AttackSucceed";
				case Judgment.eState.AttackFail:
					return "AttackFail";
				case Judgment.eState.AttackIdle:
					return "AttackIdle";
				case Judgment.eState.BlockSucceed:
					return "BlockSucceed";
				case Judgment.eState.BlockFail:
					return "BlockFail";
				case Judgment.eState.BlockIdle:
					return "BlockIdle";
				
				}

				return "None";
			}


			void Update_UI()
			{
				Judgment.Result jResult =  _judgment.GetResult();
				_1pHpBar.value = _1pInfo.GetHP();
				_2pHpBar.value = _2pInfo.GetHP();

				//====//====//====//====//====//====

				_1pExplanation1.text = 
					"  "  + JudgmentToString(jResult.state_1p);
				_2pExplanation1.text = 
					"  "  + JudgmentToString(jResult.state_2p);


				_1pExplanation2.text = 
					StateToString(_1pInfo.CurrentState()) + "  " +
					_1pInfo.GetTimeDelta().ToString("0.0");
				_2pExplanation2.text = 
					StateToString(_2pInfo.CurrentState()) + "   " +
					_2pInfo.GetTimeDelta().ToString("0.0");


				//====//====//====//====//====//====

				_1pSprite_02.gameObject.SetActive (false);
				_1pSprite_03.gameObject.SetActive (false);
				_2pSprite_02.gameObject.SetActive (false);
				_2pSprite_03.gameObject.SetActive (false);

				_1pSprite_01.sprite = _refResourceMgr.GetSprite(ResourceMgr.eSPRITE_NAME.P1_IDLE);
				_2pSprite_01.sprite = _refResourceMgr.GetSprite(ResourceMgr.eSPRITE_NAME.P2_IDLE);

				//1p
				if (CharacterInfo.eState.Attack_Before == _1pInfo.CurrentState ()) 
				{
					_1pSprite_02.gameObject.SetActive (true);
					_1pSprite_02.sprite = _refResourceMgr.GetSprite(ResourceMgr.eSPRITE_NAME.P1_ATTACK_BEFORE);
				}
				if (CharacterInfo.eState.Attack_After == _1pInfo.CurrentState ()) 
				{
					//_1pSprite_02.gameObject.SetActive (true);
					_1pSprite_01.sprite = _refResourceMgr.GetSprite(ResourceMgr.eSPRITE_NAME.P1_ATTACK_AFTER);
				}
				if (CharacterInfo.eState.Block_Before == _1pInfo.CurrentState ()) 
				{
					_1pSprite_02.gameObject.SetActive (true);
					_1pSprite_02.sprite = _refResourceMgr.GetSprite(ResourceMgr.eSPRITE_NAME.P1_BLOCK_BEFORE);
				}
				if (CharacterInfo.eState.Block_After == _1pInfo.CurrentState ()) 
				{
					//_1pSprite_02.gameObject.SetActive (true);
					_1pSprite_01.sprite = _refResourceMgr.GetSprite(ResourceMgr.eSPRITE_NAME.P1_BLOCK_AFTER);
				}
				if (CharacterInfo.eState.Hit == _1pInfo.CurrentState ()) 
				{
					//_1pSprite_02.gameObject.SetActive (true);
					_1pSprite_01.sprite = _refResourceMgr.GetSprite(ResourceMgr.eSPRITE_NAME.EMPTY_CARD);
				}

				//2p
				if (CharacterInfo.eState.Attack_Before == _2pInfo.CurrentState ()) 
				{
					_2pSprite_02.gameObject.SetActive (true);
					_2pSprite_02.sprite = _refResourceMgr.GetSprite(ResourceMgr.eSPRITE_NAME.P2_ATTACK_BEFORE);
				}
				if (CharacterInfo.eState.Attack_After == _2pInfo.CurrentState ()) 
				{
					//_2pSprite_02.gameObject.SetActive (true);
					_2pSprite_01.sprite = _refResourceMgr.GetSprite(ResourceMgr.eSPRITE_NAME.P2_ATTACK_AFTER);
				}
				if (CharacterInfo.eState.Block_Before == _2pInfo.CurrentState ()) 
				{
					_2pSprite_02.gameObject.SetActive (true);
					_2pSprite_02.sprite = _refResourceMgr.GetSprite(ResourceMgr.eSPRITE_NAME.P2_BLOCK_BEFORE);
				}
				if (CharacterInfo.eState.Block_After == _2pInfo.CurrentState ()) 
				{
					//_2pSprite_02.gameObject.SetActive (true);
					_2pSprite_01.sprite = _refResourceMgr.GetSprite(ResourceMgr.eSPRITE_NAME.P2_BLOCK_AFTER);
				}
				if (CharacterInfo.eState.Hit == _2pInfo.CurrentState ()) 
				{
					//_2pSprite_02.gameObject.SetActive (true);
					_2pSprite_01.sprite = _refResourceMgr.GetSprite(ResourceMgr.eSPRITE_NAME.EMPTY_CARD);
				}


				//====//====//====//====//====//====

				_1pImage.color = Color.black;
				_2pImage.color = Color.black;
				_1pExplanation2.color = Color.black;
				_2pExplanation2.color = Color.black;
				_1pSprite_01.color = Color.white;
				_1pSprite_02.color = Color.white;
				_1pSprite_03.color = Color.white;
				_2pSprite_01.color = Color.white;
				_2pSprite_02.color = Color.white;
				_2pSprite_03.color = Color.white;
				if (true == _1pInfo.Valid_Attack()) {
					_1pImage.color = Color.red;
					_1pExplanation2.color = Color.red;

					_1pSprite_03.gameObject.SetActive (true);
					_1pSprite_03.sprite = _refResourceMgr.GetSprite(ResourceMgr.eSPRITE_NAME.P1_ATTACK_VALID);


					if (Judgment.eState.AttackSucceed == jResult.state_1p) 
					{
						_2pSprite_01.color = Color.red;
						_2pSprite_02.color = Color.red;
						_2pSprite_03.color = Color.red;
					}
				}
				if (true == _2pInfo.Valid_Attack()) 
				{
					_2pImage.color = Color.red;
					_2pExplanation2.color = Color.red;

					_2pSprite_03.gameObject.SetActive (true);
					_2pSprite_03.sprite = _refResourceMgr.GetSprite(ResourceMgr.eSPRITE_NAME.P2_ATTACK_VALID);

					if (Judgment.eState.AttackSucceed == jResult.state_2p) 
					{
						_1pSprite_01.color = Color.red;
						_1pSprite_02.color = Color.red;
						_1pSprite_03.color = Color.red;
					}
				}

				if (true == _1pInfo.Valid_Block()) 
				{
					_1pImage.color = Color.blue;
					_1pExplanation2.color = Color.blue;

					_1pSprite_03.gameObject.SetActive (true);
					_1pSprite_03.sprite = _refResourceMgr.GetSprite(ResourceMgr.eSPRITE_NAME.P1_BLOCK_VALID);

					if (Judgment.eState.BlockSucceed == jResult.state_1p) 
					{
						_1pSprite_01.color = Color.blue;
						_1pSprite_02.color = Color.blue;
						_1pSprite_03.color = Color.blue;
					}
				}
				if (true == _2pInfo.Valid_Block()) 
				{
					_2pImage.color = Color.blue;
					_2pExplanation2.color = Color.blue;

					_2pSprite_03.gameObject.SetActive (true);
					_2pSprite_03.sprite = _refResourceMgr.GetSprite(ResourceMgr.eSPRITE_NAME.P2_BLOCK_VALID);

					if (Judgment.eState.BlockSucceed == jResult.state_2p) 
					{
						_2pSprite_01.color = Color.blue;
						_2pSprite_02.color = Color.blue;
						_2pSprite_03.color = Color.blue;
					}
				}


				if (0 == _1pInfo.GetHP ())
					_1pSprite_01.gameObject.SetActive (false);
				if (0 == _2pInfo.GetHP ())
					_2pSprite_01.gameObject.SetActive (false);
				//====//====//====//====//====//====

			}

			//무기 휘두르기 : 키입력을 먼저한 쪽이 공격, 늦게한 쪽이 방어가 된다. 
			public void Wield_Weapon(uint inputPNum)
			{
				const uint PLAYERNUM_1P = 1;
				const uint PLAYERNUM_2P = 2;

				Judgment.Result jResult =  _judgment.GetResult();
				DebugWide.LogGreen (jResult.state_1p + "   " + jResult.state_2p); //chamto test

				if (inputPNum == PLAYERNUM_1P) 
				{

					if (true == _2pInfo.IsAttacking ()) 
					{
						//방어 성공시 공격행동으로 전환 시킨다
						if (Judgment.eState.BlockSucceed == jResult.state_1p) {
							_1pInfo.NextState (CharacterInfo.eState.Attack);
							_skillMgr.InsertInterval (inputPNum, SkillMgr.eGestureKind.Attack_Sword);
						} else {
							_1pInfo.NextState (CharacterInfo.eState.Block);
							_skillMgr.InsertInterval (inputPNum, SkillMgr.eGestureKind.Block_Sword);
						}

					} else {
						_1pInfo.NextState (CharacterInfo.eState.Attack);
						_skillMgr.InsertInterval (inputPNum, SkillMgr.eGestureKind.Attack_Sword);
					}
					
				}
				if (inputPNum == PLAYERNUM_2P) 
				{
					if (true == _1pInfo.IsAttacking ()) 
					{
						if (Judgment.eState.BlockSucceed == jResult.state_2p) {
							_2pInfo.NextState (CharacterInfo.eState.Attack);
							_skillMgr.InsertInterval (inputPNum, SkillMgr.eGestureKind.Attack_Sword);
						} else {
							_2pInfo.NextState (CharacterInfo.eState.Block);
							_skillMgr.InsertInterval (inputPNum, SkillMgr.eGestureKind.Block_Sword);
						}

					} else {
						_2pInfo.NextState (CharacterInfo.eState.Attack);
						_skillMgr.InsertInterval (inputPNum, SkillMgr.eGestureKind.Attack_Sword);
					}
				}



			}

			// Update is called once per frame
			void Update () 
			{
				_1pInfo.Update_State ();
				_2pInfo.Update_State ();

				_judgment.Update ();
				this.Update_UI ();

				//restart
				if (Input.GetKeyUp ("v")) 
				{	
					DebugWide.LogBlue ("restart");
				}

				//////////////////////////////////////////////////
				//1p

				//attack
				if (Input.GetKeyUp ("q")) 
				{
					DebugWide.LogBlue ("1p - wield");
					//_1pInfo.NextState (CharacterInfo.eState.Attack);
					this.Wield_Weapon(1);

				}

				//block
//				if (Input.GetKeyUp ("w")) 
//				{
//					DebugWide.LogBlue ("1p - block");
//					_1pInfo.NextState (CharacterInfo.eState.Block);
//				}

				//auto mode
				if (Input.GetKeyUp ("a")) 
				{
					DebugWide.LogBlue ("1p - auto");
				}


				//////////////////////////////////////////////////
				//2p

				//attack
				if (Input.GetKeyUp ("o")) 
				{
					DebugWide.LogBlue ("2p - wield1");
					//_2pInfo.NextState (CharacterInfo.eState.Attack);
					this.Wield_Weapon(2);
				}

				//block
//				if (Input.GetKeyUp ("p")) 
//				{
//					DebugWide.LogBlue ("2p - block");
//					_2pInfo.NextState (CharacterInfo.eState.Block);
//				}

				//auto mode
				if (Input.GetKeyUp ("l")) 
				{
					DebugWide.LogBlue ("2p - auto");
				}




			}//end Update
		}//end class Simulation 




}//end namespace 


