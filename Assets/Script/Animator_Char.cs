using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animator_Char : MonoBehaviour 
{

	public enum eAniState : int
	{
		Idle 			= 0,
		Attack_up		= 1,
		Attack_hand		= 2,
		Attack_middle	= 3,
		Attack_down		= 4,
		Attack_sting	= 7,
		Block_up		= 5,
		Block_middle	= 6,
	}

	Animator _ani = null;

	void Start () 
	{
		_ani = this.GetComponent<Animator> ();	
	}
	
	// Update is called once per frame
	void Update () 
	{

		//_ani["Attack_up"]
		//====================================
		if (Input.GetKeyDown ("up")) 
		{
			_ani.SetInteger ("state", (int)eAniState.Attack_up);
		} 
		else if (Input.GetKey ("up")) 
		{
		}


		//====================================
		if (Input.GetKeyDown ("right")) 
		{
			_ani.SetInteger ("state", (int)eAniState.Attack_hand);
		}

		//====================================
		if (Input.GetKeyDown ("down")) 
		{
			_ani.SetInteger ("state", (int)eAniState.Attack_middle);
		}

		//====================================
		if (Input.GetKey ("left") && Input.GetKey("up")) 
		{
			_ani.SetInteger ("state", (int)eAniState.Block_up);
		}

		//====================================
		if (Input.GetKey ("left") && Input.GetKey("down")) 
		{
			_ani.SetInteger ("state", (int)eAniState.Block_middle);
		}



		if (Input.GetKeyUp ("up") || Input.GetKeyUp("down") || Input.GetKeyUp("left") || Input.GetKeyUp("right")) 
		{
			_ani.SetInteger ("state", (int)eAniState.Idle);
		}

	}

	public void TestAni()
	{
		Debug.Log("sdfsdf");
	}
}
