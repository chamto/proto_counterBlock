using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Controller : MonoBehaviour 
{

	private Animator _ani = null;
	private TriggerProcess _tPcs = null;

	void Start () 
	{
		_ani = this.GetComponent<Animator> ();	
		_tPcs = this.GetComponent<TriggerProcess> ();
	}

	// Update is called once per frame
	void Update () {
		
	}


	public void AttackUp()
	{
		_ani.speed = 0.5f;
		_ani.SetInteger ("state", (int)eAniState.Attack_up);

		_ani.SetInteger ("state", (int)eAniState.Attack_hand);

	}

	public void AttackHand()
	{
		_ani.SetInteger ("state", (int)eAniState.Attack_hand);
	}
}
