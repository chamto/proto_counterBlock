using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animator_Char : MonoBehaviour 
{

	public enum eAniState : int
	{
		Idle 			= 0,
		Attack_up		= 1,
		Attack_hand		= 2,
		Attack_middle	= 3,
		Attack_down		= 4,
		Attack_sting	= 7,
		Block_up		= 5,
		Block_middle	= 6,
		Damage			= 8,
	}

	private Animator _ani = null;
	private TriggerProcess _tPcs = null;

	void Start () 
	{
		_ani = this.GetComponent<Animator> ();	
		_tPcs = this.GetComponent<TriggerProcess> ();
	}
	
	void Update()
	{
		this.Update_KeyInput ();


	}


	void Update_KeyInput () 
	{
		//====================================
		if (Input.GetKeyDown ("w")) 
		{
			_ani.SetInteger ("state", (int)eAniState.Attack_up);
		}
		if (Input.GetKeyDown ("e")) 
		{
			_ani.SetInteger ("state", (int)eAniState.Attack_hand);
		}
		if (Input.GetKeyDown ("s")) 
		{
			_ani.SetInteger ("state", (int)eAniState.Attack_middle);
		}
		if (Input.GetKeyDown ("d")) 
		{
			_ani.SetInteger ("state", (int)eAniState.Attack_down);
		}
		if (Input.GetKeyDown ("q")) 
		{
			_ani.SetInteger ("state", (int)eAniState.Attack_sting);
		}


		//====================================
		if (Input.GetKeyDown ("up")) 
		{
			_ani.SetInteger ("state", (int)eAniState.Block_up);
		}

		//====================================
		if (Input.GetKeyDown ("down")) 
		{
			_ani.SetInteger ("state", (int)eAniState.Block_middle);
		}

		//====================================
		if (Input.GetKey ("left") ) 
		{
			transform.Translate (Vector3.back * Time.deltaTime * 3.5f);
		}
		if (Input.GetKey ("right") && _tPcs.DetectedStatus() == eCollisionStatus.None) 
		{
			transform.Translate (Vector3.forward * Time.deltaTime * 5.5f);
		}


		//====================================
		if (Input.GetKeyUp ("up") || Input.GetKeyUp("down") || Input.GetKeyUp("q") || Input.GetKeyUp("w")
			|| Input.GetKeyUp("e") || Input.GetKeyUp("s") || Input.GetKeyUp("d")) 
		{
			_ani.SetInteger ("state", (int)eAniState.Idle);
		}

	}



	void OnTriggerEnter(Collider other)
	{

		switch (_tPcs.DetectedStatus ()) 
		{
		case eCollisionStatus.Damage:
			{
				_ani.SetInteger ("state", (int)eAniState.Damage);
			}
			break;
		case eCollisionStatus.Block_Body:
			{
				//_ani.SetInteger ("state", (int)eAniState.Damage);
			}
			break;
		case eCollisionStatus.Block_Weapon:
			{
				//_ani.SetInteger ("state", (int)eAniState.Damage);
			}
			break;
		}

	}
	void OnTriggerStay(Collider other)
	{
		

	}
	void OnTriggerExit(Collider other)
	{
		_ani.SetInteger ("state", (int)eAniState.Idle);
	}
}
