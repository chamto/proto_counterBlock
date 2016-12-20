using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animator_Char : MonoBehaviour 
{

	Animator _ani = null;

	void Start () 
	{
		_ani = this.GetComponent<Animator> ();	
	}
	
	// Update is called once per frame
	void Update () 
	{
		//====================================
		if (Input.GetKeyUp ("up")) 
		{
			_ani.SetBool ("attack_up", false);
		}
		else if (Input.GetKeyDown ("up")) 
		{
			_ani.SetBool ("attack_up", true);
		} 
		else if (Input.GetKey ("up")) 
		{
			_ani.SetBool ("attack_up", false);
		}
			


	}

	public void TestAni()
	{
		Debug.Log("sdfsdf");
	}
}
