using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyInput : MonoBehaviour 
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
		this.Update_KeyInput ();
	}

	void Update_KeyInput () 
	{
		//====================================
		if (Input.GetKeyDown ("w")) 
		{
			_ani.SetInteger ("state", (int)eAniState.Attack_up);
		}
		if (Input.GetKeyDown ("e")) 
		{
			_ani.SetInteger ("state", (int)eAniState.Attack_hand);
		}
		if (Input.GetKeyDown ("s")) 
		{
			_ani.SetInteger ("state", (int)eAniState.Attack_middle);
		}
		if (Input.GetKeyDown ("d")) 
		{
			_ani.SetInteger ("state", (int)eAniState.Attack_down);
		}
		if (Input.GetKeyDown ("q")) 
		{
			_ani.SetInteger ("state", (int)eAniState.Attack_sting);
		}


		//====================================
		if (Input.GetKeyDown ("up")) 
		{
			_ani.SetInteger ("state", (int)eAniState.Block_up);
		}

		//====================================
		if (Input.GetKeyDown ("down")) 
		{
			_ani.SetInteger ("state", (int)eAniState.Block_middle);
		}

		//====================================
		if (Input.GetKey ("left") ) 
		{
			transform.Translate (Vector3.back * Time.deltaTime * 3.5f);
		}
		if (Input.GetKey ("right")) 
		{
			if (_tPcs.status == eCollisionStatus.None)
				transform.Translate (Vector3.forward * Time.deltaTime * 5.5f);

		}


		//====================================
		if (Input.GetKeyUp ("up") || Input.GetKeyUp("down") || Input.GetKeyUp("q") || Input.GetKeyUp("w")
			|| Input.GetKeyUp("e") || Input.GetKeyUp("s") || Input.GetKeyUp("d")) 
		{
			_ani.SetInteger ("state", (int)eAniState.Idle);
		}

	}
}
