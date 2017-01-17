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

	public IK2Chain _ik_armLeft = null;
	public IK2Chain _ik_armRight = null;

	//private TrailRenderer _trail = null;
	void Start () 
	{
		//_trail = this.GetComponentInChildren<TrailRenderer> ();

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



	void Update () 
	{
		
	}


	public void OnTransitionEnter()
	{
		//_trail.enabled = false;
	}
	public void OnTransitionExit()
	{
		_firstTrigger = true;	

//		_trail.enabled = true;
//		_trail.endWidth = 1;
//		_trail.startWidth = 4;
	}

	private bool _availableAttack = false;
	public void OnAniAttackEnter ()
	{
		_availableAttack = true;

	}
	public void OnAniAttackExit()
	{
		_availableAttack = false;
		_ik_armLeft.ToggleOff ();
		_ik_armRight.ToggleOff ();

	}


//	int test_physice1 = Animator.StringToHash("Base Layer.test_physice1");
//	int test_physice2 = Animator.StringToHash("Base Layer.test_physice2");
	bool _firstTrigger = true;
	public void OnEnter(Collider other , Transform src)
	{
		_status = this.DetectedStatus ();	
		Animator ani = this.GetComponent<Animator> ();
		//AnimatorStateInfo info =  ani.GetCurrentAnimatorStateInfo(0);


		if (other.tag == "dummy" || other.tag == "weapon") 
		{
			
			if (src.name.Equals ("hand_left")) 
			{
				_ik_armLeft.ToggleOn ();
				_ik_armLeft._targetPos.position = _ik_armLeft._targetEndPos.position;
				//Single.particle.PlayDamage (_ik_armLeft._targetEndPos.position);
			}

			if (src.name.Equals ("hand_right") && true == _availableAttack && true == _firstTrigger)
			{
				//DebugWide.LogBlue ("first!!");
				_firstTrigger = false;
				_ik_armRight.ToggleOn ();
				_ik_armRight._targetPos.position = _ik_armRight._targetEndPos.position;

				//관절2에서 검의 방향으로 광선을 쏘아 충돌체가 있는지 검사한다.
				//충돌체가 없을때만 “IK목표점"을 갱신한다.
//				RaycastHit rh;
//				if (false == other.Raycast (new Ray (_ik_armRight._joint_2.position, _ik_armRight.Joint2Dir ()), out rh, 10f)) 
//				{
//					_ik_armRight.ToggleOn ();
//					_ik_armRight._targetPos.position = other.ClosestPointOnBounds (_ik_armRight._targetEndPos.position);
//				}
				Single.particle.PlayDamage (_ik_armRight._targetEndPos.position);
			} 

		}
			
		//DebugWide.LogBlue("normalizedTime :" + info.normalizedTime + "  length  :" + info.length + "  speedMultiplier :" + info.speedMultiplier + "  speed :" + info.speed);

	}

	public void OnStay(Collider other , Transform src)
	{
		_status = this.DetectedStatus ();

	}

	public void OnExit(Collider other , Transform src)
	{
		_status = this.DetectedStatus ();

	}

	//public Transform _fowardZ = null;
	ContactPoint [] cpList = null;
	public void OnEnter(Collision collision , Transform src)
	{
		cpList = collision.contacts;

		//_trail.enabled = false;

		_status = this.DetectedStatus ();	
		Animator ani = this.GetComponent<Animator> ();
		//AnimatorStateInfo info =  ani.GetCurrentAnimatorStateInfo(0);


		if (collision.transform.tag == "dummy" || collision.transform.tag == "weapon" || collision.transform.name == "head") 
		{

			if (src.name.Equals ("hand_left")) 
			{
				_ik_armLeft.ToggleOn ();
				_ik_armLeft._targetPos.position = _ik_armLeft._targetEndPos.position;
				Single.particle.PlayDamage (_ik_armLeft._targetEndPos.position);
			}

//			if (src.name.Equals ("hand_right") && true == _availableAttack && true == _firstTrigger)
//			{
//				//DebugWide.LogBlue ("first!!");
//				_firstTrigger = false;
//				_ik_armRight.ToggleOn ();
//				_ik_armRight._targetPos.position = _ik_armRight._targetEndPos.position;
//
//				CSingletonMono<ParticleController>.Instance.PlayDamage (collision.contacts[0].point);
//			}

			if (src.name.Equals ("hand_right") && true == _availableAttack)
			{
				//관절2에서 검의 방향으로 광선을 쏘아 충돌체가 있는지 검사한다.
				//충돌체가 없을때만 “IK목표점"을 갱신한다.
				RaycastHit rh;
				if (false == collision.collider.Raycast (new Ray (_ik_armRight._joint_2.position, _ik_armRight.Joint2Dir ()), out rh, 10f)) 
				{
					_ik_armRight.ToggleOn ();
					_ik_armRight._targetPos.position = collision.contacts[0].point;
				}
				Single.particle.PlayDamage (collision.contacts[0].point);
			}

		}
	}

	public void OnStay(Collision collision , Transform src)
	{		
		cpList = collision.contacts;
	}

	public void OnExit(Collision collision , Transform src)
	{
		cpList = null;
	}

	void OnDrawGizmos2()
	{
		if (null == cpList)
			return;
		//Gizmos.DrawIcon(Vector3.zero,"iTweenIcon", true);
		Gizmos.color = Color.green;
		Vector3 prev = cpList [0].point;
		foreach (ContactPoint cp in cpList) 
		{
			Gizmos.DrawSphere (cp.point, 0.5f);
			//Gizmos.DrawLine(prev, cp.point);
			prev = cp.point;
		}
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
