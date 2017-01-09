using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum eCollisionStatus
{
	None = 0,
	Hit,
	Damage,
	Block_Body,
	Block_Weapon,
	Block_Objects,
}

public enum eColliderKind
{
	None = 0,
	Body,
	_Head,
	_HandRight,
	_HandLeft,
	Weapon,
	_Knife,	//the blade of a weapon
	Objects,
}

public class TriggerProcess : MonoBehaviour 
{
	private eCollisionStatus _status = eCollisionStatus.None;
	private eColliderKind _myColliderKind = eColliderKind.None;
	private eColliderKind _oppColliderKind = eColliderKind.None;

	public eColliderKind myColliderKind 
	{
		get
		{
			return _myColliderKind;
		}
	}
	public eColliderKind oppColliderKind
	{
		get
		{
			return _oppColliderKind;
		}
	}
	public eCollisionStatus status
	{
		get
		{
			return _status;
		}
	}

	public IK2Chain _ik2Chain = null;

	// Use this for initialization
	void Start () 
	{
		_ik2Chain = this.GetComponent<IK2Chain> ();
		
	}
	
	// Update is called once per frame
	void Update () 
	{
	}

	public void SetMyColliderKind(eColliderKind my)
	{
		this._myColliderKind = my;
	}
	public void SetOppColliderKind(eColliderKind opp)
	{
		this._oppColliderKind = opp;
	}
	public void SetOppColliderKind(Collider opp)
	{
		this._oppColliderKind = eColliderKind.None;

		if (opp.tag.Equals ("weapon")) 
		{
			_oppColliderKind = eColliderKind.Weapon;
		} 
		else if(opp.name.Equals("body")) 
		{
			_oppColliderKind = eColliderKind.Body;
		}
		else if(opp.name.Equals("hand_left")) 
		{
			_oppColliderKind = eColliderKind._HandLeft;
		}
		else if(opp.name.Equals("hand_right")) 
		{
			_oppColliderKind = eColliderKind._HandRight;
		}
		else if(opp.name.Equals("head")) 
		{
			_oppColliderKind = eColliderKind._Head;
		}
		else if(opp.name.Equals("knife")) 
		{
			_oppColliderKind = eColliderKind._Knife;
		}
		else
			_oppColliderKind = eColliderKind.Objects;


	}

	public eCollisionStatus DetectedStatus()
	{

		//* body head sword knife
		//myBody <- body	:	my block_body
		//myBody <- weapon	: 	my damage
		//myBody <- knife	:	my damage

		//myHead <- weapon 	:	my damage 

		//myWeapon -> body	:	opponent damage
		//myWeapon -> weapon:	opponent block_weapon

		//==========================================

		eCollisionStatus detect = eCollisionStatus.None;


		switch (this._myColliderKind) 
		{

		case eColliderKind.Body:
			{
				//1.myBody <- body : my block_body
				if (eColliderKind.Body == this._oppColliderKind) 
				{
					detect = eCollisionStatus.Block_Body;
				}
				if (eColliderKind._HandLeft == this._oppColliderKind) 
				{
					detect = eCollisionStatus.Block_Body;
				}
				if (eColliderKind._HandRight == this._oppColliderKind) 
				{
					detect = eCollisionStatus.Block_Body;
				}
				if (eColliderKind._Head == this._oppColliderKind) 
				{
					detect = eCollisionStatus.Block_Body;
				}
				//2.myBody <- weapon : my damage
				if (eColliderKind.Weapon == this._oppColliderKind) 
				{
					detect = eCollisionStatus.Damage;
				}
			}	
			break;
		//3.myWeapon -> weapon : opponent block_weapon
		case eColliderKind.Weapon:
			{
				if (eColliderKind.Weapon == this._oppColliderKind) 
				{
					detect = eCollisionStatus.Block_Weapon;
				}
				if (eColliderKind.Body == this._oppColliderKind) 
				{
					detect = eCollisionStatus.Hit;
				}
				if (eColliderKind._HandLeft == this._oppColliderKind) 
				{
					detect = eCollisionStatus.Hit;
				}
				if (eColliderKind._HandRight == this._oppColliderKind) 
				{
					detect = eCollisionStatus.Hit;
				}
				if (eColliderKind._Head == this._oppColliderKind) 
				{
					detect = eCollisionStatus.Hit;
				}
				if (eColliderKind.Objects == this._oppColliderKind) 
				{
					detect = eCollisionStatus.Block_Objects;
				}
			}	
			break;
		}

		return detect;
	}


	public void EnterAniState()
	{
		
		//DebugWide.LogBlue ("1111 ");
	}
	public void ExirAniState()
	{
		//DebugWide.LogBlue ("2222 ");
		_ik2Chain.ToggleOff ();

	}

	//IK targetPos 설정 문제 : 이 문제의 해결을 위해서는 가정이 필요하다.
	//- 가정 : 한 애니메이션이 진행되는 동안 1번만 충돌한다. 
//	int test_physice1 = Animator.StringToHash("Base Layer.test_physice1");
//	int test_physice2 = Animator.StringToHash("Base Layer.test_physice2");
//	Vector3 targetPos = Vector3.zero;
//	float aniTime = 0f;
	public void OnEnter(Collider other)
	{
		_status = this.DetectedStatus ();	

		if (other.tag == "dummy") 
		{
			_ik2Chain.ToggleOn ();
			_ik2Chain._targetPos.position = _ik2Chain._targetEndPos.position;
		}

		//DebugWide.LogBlue ("OnEnter " + other.name);
			
		//Animator ani = this.GetComponent<Animator> ();
		//AnimatorStateInfo info =  ani.GetCurrentAnimatorStateInfo(0);
		//DebugWide.LogBlue("normalizedTime :" + info.normalizedTime + "  length  :" + info.length + "  speedMultiplier :" + info.speedMultiplier + "  speed :" + info.speed);


	}

	public void OnStay(Collider other)
	{
		_status = this.DetectedStatus ();
		//_ik2Chain._targetPos.position = other.contacts[0].point;
	}

	public void OnExit(Collider other)
	{
		_status = this.DetectedStatus ();

		//DebugWide.LogBlue ("OnExit " + other.name);

	}

	public void OnEnter(Collision collision) 
	{
		
	}

	public void OnStay(Collision collision) 
	{
		
	}

	public void OnExit(Collision collision) 
	{
		
	}

//	void OnTriggerEnter(Collider other)
//	{
//		//DebugWide.LogBool (_tPcs.name.Equals("Character"), "animator - " + _tPcs.name + " - trigger Enter : " +  other.tag + "  " + other.name);
//		_status = this.DetectedStatus ();
//	}
//	void OnTriggerStay(Collider other)
//	{
//		//DebugWide.LogBool (_tPcs.name.Equals("Character"),"---animator status Stay--------"+ _tPcs.name + "  "+_tPcs.DetectedStatus()); //chamto test
//		_status = this.DetectedStatus ();
//
//	}
//	void OnTriggerExit(Collider other)
//	{
//		//DebugWide.LogBool (_tPcs.name.Equals("Character"), "animator - " + _tPcs.name + " - trigger Exit : " +  other.tag + "  " + other.name);
//		_status = this.DetectedStatus ();
//	}

}
