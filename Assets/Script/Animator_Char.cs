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
		Damage			= 8,
	}

	Animator _ani = null;

	void Start () 
	{
		_ani = this.GetComponent<Animator> ();	
	}
	
	void Update()
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
		if (Input.GetKey ("right") && _isCollision == false) 
		{
			transform.Translate (Vector3.forward * Time.deltaTime * 5.5f);
		}




		//====================================
		if (Input.GetKeyUp ("up") || Input.GetKeyUp("down") || Input.GetKeyUp("q") || Input.GetKeyUp("w")
			|| Input.GetKeyUp("e") || Input.GetKeyUp("s") || Input.GetKeyUp("d")) 
		{
			_ani.SetInteger ("state", (int)eAniState.Idle);
		}

	}


	public bool _isCollision = false;
	void OnTriggerEnter(Collider other)
	{
		

		//transform.Translate (transform.forward * -0.5f);
		if (other.name == "body" || other.name == "sword")
		{
			
			_isCollision = true;
		}

		if (other.name == "sword") 
		{
				
		}

		if (other.name == "knife") 
		{
			_ani.SetInteger ("state", (int)eAniState.Damage);
		}


		//Debug.Log ("body - " + _ani.name + " - trigger Enter : " +  other.tag + "  " + other.name);

	}
	void OnTriggerStay(Collider other)
	{
		//Debug.Log ("body - " + _ani.name + " - trigger Stay : " +  other.tag + "  " + other.name);

		if (other.name == "body" || other.name == "sword")
		{
			_isCollision = true;
		}


	}
	void OnTriggerExit(Collider other)
	{
		//Debug.Log ("body - " + _ani.name + " - trigger Exit : " +  other.tag + "  " + other.name);

		if (other.name == "knife") 
		{
			_ani.SetInteger ("state", (int)eAniState.Idle);
		}

		if (other.name == "body" || other.name == "sword") 
		{
			
			_isCollision = false;
		}

	}
}
