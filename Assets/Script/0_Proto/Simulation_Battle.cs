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
		Attack_Before = Attack,
		Attack_After,
		Block,
		Block_Before = Block,
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

	public uint 	_hp;

	public eState 	_state_prev;
	public eState 	_state_current;
	public eState 	_state_next;

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

		this._hp = 10;
		_state_prev = eState.None;
		_state_current = eState.None;
		_state_next = eState.None;
	}

	public float GetTimeDelta()
	{
		return _timeDelta;
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

	public void Update()
	{
		this._timeDelta += Time.deltaTime;

		switch (this._state_current) 
		{
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



public class Simulation_Battle : MonoBehaviour 
{

	private CharacterInfo _1pInfo = new CharacterInfo();
	private CharacterInfo _2pInfo = new CharacterInfo();

	public Text _1pExplanation = null;
	public Text _2pExplanation = null;

	// Use this for initialization
	void Start () 
	{
			
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


	void UIUpdate()
	{
		
		_1pExplanation.text = StateToString(_1pInfo._state_current) + "  " + _1pInfo.GetTimeDelta().ToString("0.0");
		_2pExplanation.text = StateToString(_2pInfo._state_current) + "  " + _2pInfo.GetTimeDelta().ToString("0.0");;
	}


	// Update is called once per frame
	void Update () 
	{
		_1pInfo.Update ();
		_2pInfo.Update ();

		this.UIUpdate ();

		//restart
		if (Input.GetKeyDown ("v")) 
		{	
			DebugWide.LogBlue ("restart");
		}

		//////////////////////////////////////////////////
		//1p

		//attack
		if (Input.GetKeyDown ("q")) 
		{
			DebugWide.LogBlue ("1p - attack");
			_1pInfo.NextState (CharacterInfo.eState.Attack);
		}

		//block
		if (Input.GetKeyDown ("w")) 
		{
			DebugWide.LogBlue ("1p - block");
			_1pInfo.NextState (CharacterInfo.eState.Block);
		}

		//auto mode
		if (Input.GetKeyDown ("a")) 
		{
			DebugWide.LogBlue ("1p - auto");
		}


		//////////////////////////////////////////////////
		//2p

		//attack
		if (Input.GetKeyDown ("o")) 
		{
			DebugWide.LogBlue ("2p - attack");
			_2pInfo.NextState (CharacterInfo.eState.Attack);
		}

		//block
		if (Input.GetKeyDown ("p")) 
		{
			DebugWide.LogBlue ("2p - block");
			_2pInfo.NextState (CharacterInfo.eState.Block);
		}

		//auto mode
		if (Input.GetKeyDown ("l")) 
		{
			DebugWide.LogBlue ("2p - auto");
		}



		
	}
}
