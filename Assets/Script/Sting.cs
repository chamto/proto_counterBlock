using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtendPart_Unity;


public class Sting : MonoBehaviour 
{

	public Transform _Left	= null;
	public Transform _Right	= null;
	public bool 	_Active = false;

	//private Vector3	_leftPos;
	//private Vector3	_leftScale;
	private AnchorPoint _leftAnchor = null;

	private Vector3 _leftGlobalPos;

	public void AttackLeft_Rotate()
	{
		
		_Left.RotateAround (_leftGlobalPos, Vector3.left, 45f);
	}



	public void AttackLeft()
	{
		//Vector3 pos = _leftPos;
		//Vector3 scale = _leftScale;
//		scale.z = 3f;
//
//		Vector3 mv = Vector3.zero;
//		mv.z = (1 - (1 * scale.z)) * -0.5f;
//		_Left.localPosition = _leftPos + mv;
//		_Left.localScale = scale;

		Vector3 scale = _Left.localScale;
		scale.z = 3f;
		_leftAnchor.Scale (scale);



	}


	public void AttackRight()
	{
		//ref : http://itween.pixelplacement.com/documentation.php
		iTween.MoveBy(_Right.gameObject, iTween.Hash("z", 2, "easeType", "easeInOutExpo", "loopType", "pingPong", "delay", .1));
	}

	//first stance
	public void ReadyStance()
	{
		//_Left.localPosition = _leftPos;
		//_Left.localScale = _leftScale;
	}

	// Use this for initialization
	void Start () 
	{
		_leftGlobalPos = _Left.position;
		//_leftPos = _Left.localPosition;
		//_leftScale = _Left.localScale;

		_leftAnchor = new AnchorPoint (_Left);
		_leftAnchor.SetAnchorRate (new Vector3 (0,0,-0.5f));
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (true == _Active) 
		{
			this.AttackLeft ();
			//this.AttackLeft_Rotate ();

			//this.AttackRight ();
		}
		_Active = false;
	}
}
