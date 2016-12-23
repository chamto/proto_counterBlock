using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_AI : MonoBehaviour 
{

	private Animator _ani = null;
	private TriggerProcess _tPcs = null;
	private CharacterController _charCtrl = null;

	void Start () 
	{
		_ani = this.GetComponent<Animator> ();	
		_tPcs = this.GetComponent<TriggerProcess> ();
		_charCtrl = this.GetComponent<CharacterController> (); 
	}
	
	// Update is called once per frame
	void Update () 
	{
		_charCtrl.AttackUp ();
		//_ani.SetInteger ("state", (int)eAniState.Attack_hand);	
	}
}
