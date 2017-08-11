/// <summary>
/// 
/// Simulation_Battle
/// 
/// 20170802 - chamto - anrudco@gmail.com
/// 
/// </summary>

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace CounterBlockSting
{
	public struct BehaviorTime
	{
		public float attack;
		public float block;
		public float idle;


		public void Init()
		{
			attack = 0f;
			block = 0f;
			idle = 0f;

		}
	}


	public class CharacterInfo
	{

		public enum eState
		{
			None 		= 0,
			Attack,
			Attack_Init = Attack,
			Attack_Before,
			Attack_After,
			Block,
			Block_Init = Block,
			Block_Before,
			Block_After,
			Idle,
			Max,
		}


		//(* As : A start , Ae : A end)
		//|--------------<----------|--------------->--------|
		//As            Cs        Ae,Bs            Ce        Be


		//Action cooldown 
		private BehaviorTime _time_before; //A.행동이 완료되기 까지 걸리는 시간
		private BehaviorTime _time_after;  //B.행동이 완료된 후 대기상태로 돌아오는 시간
		private BehaviorTime _scope_start; //C.행동 효과 범위
		private BehaviorTime _scope_end;
		private float 		 _timeDelta; 	//시간변화량

		private uint 	_hp;

		private eState 	_state_prev;
		private eState 	_state_current;
		private eState 	_state_next;
		private bool _state_used;

		public CharacterInfo()
		{

			_time_before.Init ();
			_time_after.Init ();
			_scope_start.Init ();
			_scope_end.Init ();
			_timeDelta = 0f;

			//------------------------------
			_time_before.attack = 2f;
			_time_after.attack = 2f;

			_time_before.block = 1f;
			_time_after.block = 2f;

			_time_before.idle = 2f;
			//------------------------------
			_scope_start.attack = 1.5f;
			_scope_end.attack = 0.0f;
				
			_scope_start.block = 0.0f;
			_scope_end.block = 1f;
			//------------------------------

			this._hp = 10;
			_state_prev = eState.None;
			_state_current = eState.None;
			_state_next = eState.None;
			_state_used = false;
		}
			
		public bool Valid_Attack()
		{
			if (eState.Attack_Before == _state_current) 
			{
				if (_scope_start.attack <= _timeDelta)
					return true;
			}

			if (eState.Attack_After == _state_current) 
			{
				if (_timeDelta <= _scope_end.attack)
					return true;
			}

			return false;
		}

		public bool Valid_Block()
		{
			if (eState.Block_Before == _state_current) 
			{
				if (_scope_start.block <= _timeDelta)
					return true;
			}

			if (eState.Block_After == _state_current) 
			{
				if (_timeDelta <= _scope_end.block)
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

		public void Update_State()
		{
			this._timeDelta += Time.deltaTime;

			switch (this._state_current) 
			{
			case eState.Attack_Init:
				{
					_state_used = false; //init
					this.SetState (eState.Attack_Before);	
				}
				break;
			case eState.Attack_Before:
				{
					
					if (_time_before.attack <= this._timeDelta) 
					{
						this.SetState (eState.Attack_After);
					}		
				}
				break;
			case eState.Attack_After:
				{
					if (_time_after.attack <= this._timeDelta) 
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
				}
				break;
			case eState.Block_Before:
				{
					

					if (_time_before.block <= this._timeDelta) 
					{
						this.SetState (eState.Block_After);
					}
				}
				break;
			case eState.Block_After:
				{
					if (_time_after.block <= this._timeDelta) 
					{
						this.SetState (_state_next);
						_state_next = eState.Idle;
					}
				}
				break;
			case eState.Idle:
			case eState.None:
				{
					_state_used = false; //init

					//default loop 
					if (_time_before.idle <= this._timeDelta) 
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

		}
	}



	namespace Battle
	{
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

			public Judgment(ref CharacterInfo ref_1p , ref CharacterInfo ref_2p)
			{
				_ref_1pInfo = ref_1p;
				_ref_2pInfo = ref_2p;
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



			public Result Judge()
			{
				Result jResult =  Valid_Attack ();

				switch (jResult.state_1p) 
				{
				case eState.AttackSucceed:
					if(false == _ref_1pInfo.GetUsedState()) //한동작에서 처음 공격만 적용
					{
						_ref_1pInfo.SetUsedState ();
						_ref_2pInfo.HP_SubTract (1);
					}
						
					break;
				}

				switch (jResult.state_2p) 
				{
				case eState.AttackSucceed:
					if (false == _ref_2pInfo.GetUsedState ()) 
					{
						_ref_2pInfo.SetUsedState ();
						_ref_1pInfo.HP_SubTract (1);
					}
						
					break;
				}

				return jResult;
			}


		}	

		public class Simulation_Battle : MonoBehaviour 
		{

			private CharacterInfo _1pInfo = new CharacterInfo();
			private CharacterInfo _2pInfo = new CharacterInfo();

			private Judgment _judgment = null;


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
			//====//====//====//====//====//====



			// Use this for initialization
			void Start () 
			{
				_judgment = new Judgment(ref _1pInfo ,ref _2pInfo);
				_1pImage.color = Color.white;
				_2pImage.color = Color.white;
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
				Judgment.Result jResult =  _judgment.Judge ();
				_1pHpBar.value = _1pInfo.GetHP();
				_2pHpBar.value = _2pInfo.GetHP();
				_1pExplanation1.text = 
					"  "  + JudgmentToString(jResult.state_1p);
				_2pExplanation1.text = 
					"  "  + JudgmentToString(jResult.state_2p);


				_1pImage.color = Color.black;
				_2pImage.color = Color.black;
				_1pExplanation2.color = Color.black;
				_2pExplanation2.color = Color.black;
				if (true == _1pInfo.Valid_Attack()) {
					_1pImage.color = Color.red;
					_1pExplanation2.color = Color.red;
				}
				if (true == _2pInfo.Valid_Attack()) {
					_2pImage.color = Color.red;
					_2pExplanation2.color = Color.red;
				}

				if (true == _1pInfo.Valid_Block()) {
					_1pImage.color = Color.blue;
					_1pExplanation2.color = Color.blue;
				}
				if (true == _2pInfo.Valid_Block()) {
					_2pImage.color = Color.blue;
					_2pExplanation2.color = Color.blue;
				}

				_1pExplanation2.text = 
					StateToString(_1pInfo.CurrentState()) + "  " +
					_1pInfo.GetTimeDelta().ToString("0.0");
				_2pExplanation2.text = 
					StateToString(_2pInfo.CurrentState()) + "   " +
					_2pInfo.GetTimeDelta().ToString("0.0");
			}



			// Update is called once per frame
			void Update () 
			{
				_1pInfo.Update_State ();
				_2pInfo.Update_State ();

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
					DebugWide.LogBlue ("1p - attack");
					_1pInfo.NextState (CharacterInfo.eState.Attack);


				}

				//block
				if (Input.GetKeyUp ("w")) 
				{
					DebugWide.LogBlue ("1p - block");
					_1pInfo.NextState (CharacterInfo.eState.Block);
				}

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
					DebugWide.LogBlue ("2p - attack");
					_2pInfo.NextState (CharacterInfo.eState.Attack);
				}

				//block
				if (Input.GetKeyUp ("p")) 
				{
					DebugWide.LogBlue ("2p - block");
					_2pInfo.NextState (CharacterInfo.eState.Block);
				}

				//auto mode
				if (Input.GetKeyUp ("l")) 
				{
					DebugWide.LogBlue ("2p - auto");
				}




			}//end Update
		}//end class Simulation 

	}//end namespace


}//end namespace 


