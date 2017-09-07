using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CounterBlockSting;

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
	private Multi _multi = null;

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

	private IK2Chain _ik_armLeft = null;
	private IK2Chain _ik_armRight = null;


	void Start () 
	{
		_multi = this.GetComponentInParent<Multi> ();

		_ik_armLeft = _multi.hashMap.GetTransform (eHashIdx.Bone_Arm_Left).GetComponent<IK2Chain> ();
		_ik_armRight = _multi.hashMap.GetTransform (eHashIdx.Bone_Arm_Right).GetComponent<IK2Chain> ();
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

		if (true == _multi.hashMap.IsMyTransform (opp.transform))
			return;
		
		HashInfoMap map = _multi.hashMap;
		int oppHashCode = opp.name.GetHashCode ();


		if(oppHashCode == map.GetHash((int)eHashIdx.Bone_Weapon_Sword_Right))
		{
			_oppColliderKind = eColliderKind.Weapon;
		} 
		else if(oppHashCode == map.GetHash((int)eHashIdx.Bone_Mesh_Body))
		{
			_oppColliderKind = eColliderKind.Body;
		}
		if(oppHashCode == map.GetHash((int)eHashIdx.Bone_Mesh_Hand_Left))
		{
			_oppColliderKind = eColliderKind._HandLeft;
		}
		if(oppHashCode == map.GetHash((int)eHashIdx.Bone_Mesh_Hand_Right))
		{
			_oppColliderKind = eColliderKind._HandRight;
		}
		if(oppHashCode == map.GetHash((int)eHashIdx.Bone_Mesh_Head))
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


	float aniTime = 0;
	void Update () 
	{
//		aniTime += Time.deltaTime;
//		if (1f <= aniTime)
//			aniTime = 0;
//		AnimatorStateInfo info =  _multi.animator.GetCurrentAnimatorStateInfo(0);
//
//		_multi.animator.Play (info.fullPathHash, 0, aniTime);
	}


	public void OnTransitionEnter()
	{
		//DebugWide.LogBlue ("tr_enter");
		//_trail.enabled = false;
	}
	public void OnTransitionExit()
	{
		//DebugWide.LogBlue ("tr_exit");

		_firstTrigger = true;	


		_ik_armRight.ToggleOff (true);
	}

	private bool _availableAttack = false;
	public void OnAniAttackEnter ()
	{
		//DebugWide.LogBlue ("OnAniAttackEnter");

		_availableAttack = true;

	}
	public void OnAniAttackExit()
	{
		//DebugWide.LogBlue ("OnAniAttackExit");
		
		_availableAttack = false;

		_ik_armRight.ToggleOff (true);

	}
		
	private bool _firstTrigger = true;
	public void ComputeIKTarget(Collision oppClsPart, Collider oppCldPart, Transform myPart)
	{
		//Animator ani = this.GetComponent<Animator> ();
		//AnimatorStateInfo info =  ani.GetCurrentAniatorStateInfo(0);

		Vector3 back = _multi.hashMap.back;

//		if(myPart.name.GetHashCode() == _multi.hashMap.GetHash(eHashIdx.Bone_Weapon_Sword_Left))
//		{
//			_ik_armLeft.ToggleOn ();
//			_ik_armLeft._targetPos.position = _ik_armLeft._targetEndPos.position;
//		}

		if(myPart.name.GetHashCode() == _multi.hashMap.GetHash(eHashIdx.Bone_Weapon_Sword_Right))
		{
			_firstTrigger = true;

			if (true == _availableAttack && true == _firstTrigger) 
			{
				
				_firstTrigger = false;
				_ik_armRight.ToggleOn ();
				_ik_armRight._targetPos.position = _ik_armRight._joint2EndPos.position;
				//_ik_armRight._targetPos.position = oppCldPart.ClosestPointOnBounds (oppClsPart.contacts[0].point + back*3);
				//_ik_armRight._targetPos.position = oppClsPart.contacts[0].normal + oppClsPart.contacts[0].point;


			}

		}


	}


	public void OnEnter(Collider other , Transform src)
	{
		_status = this.DetectedStatus ();	

		//DebugWide.LogBlue ("sdfsdfsf");
		this.ComputeIKTarget (null,other, src);

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
		_status = this.DetectedStatus ();	


		if (_status == eCollisionStatus.Hit || _status == eCollisionStatus.Block_Objects || 
			_status == eCollisionStatus.Block_Weapon) 
		{
			if (0 != cpList.Length) 
			{
				this.ComputeIKTarget (collision,collision.collider, src);
				CounterBlock.Single.particle.PlayDamage (cpList[0].point);
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
