using System.Collections;
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
		//DebugWide.LogBool (_tPcs.name.Equals("Character"),"sword - " + _tPcs.name + " - trigger Enter : " +  other.tag + "  " + other.name);

		_tPcs.SetMyColliderKind (eColliderKind.Weapon);
		_tPcs.SetOppColliderKind (other);
		//_tPcs.OnEnter (other , this.transform);
	}
	void OnTriggerStay(Collider other)
	{
		//DebugWide.LogBool (_tPcs.name.Equals("Character"),"sword - " + _tPcs.name + " - trigger Stay : " +  other.tag + "  " + other.name);

		//_tPcs.SetMyColliderKind (eColliderKind.Weapon);
		//_tPcs.SetOppColliderKind (_tPcs.oppColliderKind);
		//_tPcs.OnStay (other , this.transform);
	}
	void OnTriggerExit(Collider other)
	{
		//DebugWide.LogBool (_tPcs.name.Equals("Character"), "sword - " + _tPcs.name + " - trigger Exit : " +  other.tag + "  " + other.name);

		_tPcs.SetMyColliderKind (eColliderKind.None);
		_tPcs.SetOppColliderKind (eColliderKind.None);
		//_tPcs.OnExit (other , this.transform);
	}

	void OnCollisionEnter(Collision collision) 
	{
		_tPcs.SetMyColliderKind (eColliderKind.Weapon);
		_tPcs.SetOppColliderKind (collision.collider);
		_tPcs.OnEnter (collision , this.transform);
	}

	void OnCollisionStay(Collision collision) 
	{
		_tPcs.OnStay (collision , this.transform);
	}

	void OnCollisionExit(Collision collision) 
	{
		_tPcs.SetMyColliderKind (eColliderKind.None);
		_tPcs.SetOppColliderKind (eColliderKind.None);
		_tPcs.OnExit (collision , this.transform);
	}
}
