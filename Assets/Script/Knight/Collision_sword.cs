using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision_sword : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter (Collision col)
	{
		DebugWide.LogBlue ("OnCollisionEnter:  "+col.gameObject.name);
	}
	void OnCollisionStay (Collision col)
	{
		DebugWide.LogBlue ("OnCollisionStay:  "+col.gameObject.name);
	}
	void OnCollisionExit (Collision col)
	{
		DebugWide.LogBlue ("OnCollisionExit:  "+col.gameObject.name);
	}
}
