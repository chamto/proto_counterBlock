using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger_body : MonoBehaviour 
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
		//DebugWide.LogBool (_tPcs.name.Equals("Character"),"body - " + _tPcs.name + " - trigger Enter : " +  other.tag + "  " + other.name);

		_tPcs.SetMyColliderKind (eColliderKind.Body);
		_tPcs.SetOppColliderKind (other);

	}
	void OnTriggerStay(Collider other)
	{
		//DebugWide.LogBool (_tPcs.name.Equals("Character"),"body - " + _tPcs.name + " - trigger Stay : " +  other.tag + "  " + other.name);

		_tPcs.SetMyColliderKind (eColliderKind.Body);
		_tPcs.SetOppColliderKind (other);
		//_tPcs.SetOppColliderKind (_tPcs.oppColliderKind);

	}
	void OnTriggerExit(Collider other)
	{
		//DebugWide.LogBool (_tPcs.name.Equals("Character"), "body - " + _tPcs.name + " - trigger Exit : " +  other.tag + "  " + other.name);
		//DebugWide.LogBlue (_tPcs.DetectedStatus() + "  my:" + _tPcs.myColliderKind + "  opp:" + _tPcs.oppColliderKind);

		_tPcs.SetMyColliderKind (eColliderKind.None);
		_tPcs.SetOppColliderKind (eColliderKind.None);
	}
}
