using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger_sword : MonoBehaviour 
{

	private Animator _ani = null;

	// Use this for initialization
	void Start () 
	{
		_ani = this.GetComponentInParent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	void OnTriggerEnter(Collider other)
	{
		Debug.Log ("sword - " + _ani.name + " - trigger Enter : " +  other.tag + "  " + other.name);

	}
	void OnTriggerStay(Collider other)
	{
		//Debug.Log ("sword - " + _ani.name + " - trigger Stay : " +  other.tag + "  " + other.name);
	}
	void OnTriggerExit(Collider other)
	{
		//Debug.Log ("sword - " + _ani.name + " - trigger Exit : " +  other.tag + "  " + other.name);
	}
}
