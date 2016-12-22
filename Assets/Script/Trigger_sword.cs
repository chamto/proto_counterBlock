﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger_sword : MonoBehaviour 
{

	private TriggerProcess _tPcs = null;

	// Use this for initialization
	void Start () 
	{
		_tPcs = this.GetComponentInParent<TriggerProcess> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	void OnTriggerEnter(Collider other)
	{
		//Debug.Log ("sword - " + _tPcs.name + " - trigger Enter : " +  other.tag + "  " + other.name);

		_tPcs.SetMyColliderKind (eColliderKind.Weapon);
		_tPcs.SetOppColliderKind (other);

	}
	void OnTriggerStay(Collider other)
	{
		//Debug.Log ("sword - " + _tPcs.name + " - trigger Stay : " +  other.tag + "  " + other.name);
	}
	void OnTriggerExit(Collider other)
	{
		//Debug.Log ("sword - " + _tPcs.name + " - trigger Exit : " +  other.tag + "  " + other.name);

		_tPcs.SetMyColliderKind (eColliderKind.None);
		_tPcs.SetOppColliderKind (eColliderKind.None);
	}
}
