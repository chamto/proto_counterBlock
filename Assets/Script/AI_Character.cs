using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Character : MonoBehaviour 
{

	private Animator _ani = null;
	private TriggerProcess _tPcs = null;

	void Start () 
	{
		_ani = this.GetComponent<Animator> ();	
		_tPcs = this.GetComponent<TriggerProcess> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		_ani.SetInteger ("state", (int)eAniState.Attack_up);	
	}
}
