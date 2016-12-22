using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

public class Animator_Char : MonoBehaviour 
{
	
	private Animator _ani = null;
	private TriggerProcess _tPcs = null;

	void Start () 
	{
		_ani = this.GetComponent<Animator> ();	
		_tPcs = this.GetComponent<TriggerProcess> ();
	}
	
	void Update()
	{
		
	}



	void OnTriggerEnter(Collider other)
	{
		//DebugWide.LogBool (_tPcs.name.Equals("Character"), "animator - " + _tPcs.name + " - trigger Enter : " +  other.tag + "  " + other.name);

		switch (_tPcs.status) 
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

		//Debug.Log ("---animator status Enter--------"+ _tPcs.name + "  "+_tPcs.DetectedStatus()); //chamto test
	}
	void OnTriggerStay(Collider other)
	{
		//DebugWide.LogBool (_tPcs.name.Equals("Character"),"---animator status Stay--------"+ _tPcs.name + "  "+_tPcs.DetectedStatus()); //chamto test


		switch (_tPcs.status) 
		{
		case eCollisionStatus.Damage:
			{
				
			}
			break;
		case eCollisionStatus.Block_Body:
			{
				transform.Translate (Vector3.back * Time.deltaTime * 0.7f);
			}
			break;
		case eCollisionStatus.Block_Weapon:
			{
				
			}
			break;
		}

	}
	void OnTriggerExit(Collider other)
	{
		//DebugWide.LogBool (_tPcs.name.Equals("Character"), "animator - " + _tPcs.name + " - trigger Exit : " +  other.tag + "  " + other.name);


		_ani.SetInteger ("state", (int)eAniState.Idle);
	}
}
