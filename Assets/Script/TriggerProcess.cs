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

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
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
			}	
			break;
		}

		return detect;
	}




	void OnTriggerEnter(Collider other)
	{
		//DebugWide.LogBool (_tPcs.name.Equals("Character"), "animator - " + _tPcs.name + " - trigger Enter : " +  other.tag + "  " + other.name);
		_status = this.DetectedStatus ();
	}
	void OnTriggerStay(Collider other)
	{
		//DebugWide.LogBool (_tPcs.name.Equals("Character"),"---animator status Stay--------"+ _tPcs.name + "  "+_tPcs.DetectedStatus()); //chamto test
		_status = this.DetectedStatus ();

	}
	void OnTriggerExit(Collider other)
	{
		//DebugWide.LogBool (_tPcs.name.Equals("Character"), "animator - " + _tPcs.name + " - trigger Exit : " +  other.tag + "  " + other.name);
		_status = this.DetectedStatus ();
	}

}
