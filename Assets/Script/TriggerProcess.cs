using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum eCollisionStatus
{
	None = 0,
	Damage,
	Block_Body,
	Block_Weapon,
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
}

public class TriggerProcess : MonoBehaviour 
{
	
	private eColliderKind _myCollider = eColliderKind.None;
	private eColliderKind _oppCollider = eColliderKind.None;

	public eColliderKind myCollider 
	{
		get
		{
			return _myCollider;
		}
	}
	public eColliderKind oppCollider
	{
		get
		{
			return _oppCollider;
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetMyColliderKind(eColliderKind my)
	{
		this._myCollider = my;
	}
	public void SetOppColliderKind(eColliderKind opp)
	{
		this._oppCollider = opp;
	}
	public void SetOppColliderKind(Collider opp)
	{
		this._oppCollider = eColliderKind.None;

		if (opp.tag.Equals ("weapon")) 
		{
			_oppCollider = eColliderKind.Weapon;
		} 
		else if(opp.name.Equals("body")) 
		{
			_oppCollider = eColliderKind.Body;
		}
		else if(opp.name.Equals("head")) 
		{
			_oppCollider = eColliderKind._Head;
		}
		else if(opp.name.Equals("knife")) 
		{
			_oppCollider = eColliderKind._Knife;
		}

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


		switch (this._myCollider) 
		{

		case eColliderKind.Body:
			{
				//1.myBody <- body : my block_body
				if (eColliderKind.Body == this._oppCollider) 
				{
					detect = eCollisionStatus.Block_Body;
				}
				//2.myBody <- weapon : my damage
				if (eColliderKind.Weapon == this._oppCollider) 
				{
					detect = eCollisionStatus.Damage;
				}
			}	
			break;
		//3.myWeapon -> weapon : opponent block_weapon
		case eColliderKind.Weapon:
			{
				if (eColliderKind.Weapon == this._oppCollider) 
				{
					detect = eCollisionStatus.Block_Weapon;
				}
			}	
			break;
		}

		return detect;
	}



	void OnTriggerEnter(Collider other)
	{
		

	}
	void OnTriggerStay(Collider other)
	{
		
	}
	void OnTriggerExit(Collider other)
	{
		
	}

}
