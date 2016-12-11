using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtendPart_Unity;
using Utility;


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
		_leftAnchor.ScaleZ(3f);
	}


	public void AttackRight()
	{
		//ref : http://itween.pixelplacement.com/documentation.php
		//iTween.MoveBy(_Right.gameObject, iTween.Hash("z", 2, "easeType", "easeInOutExpo", "loopType", "pingPong", "delay", .1));
		iTween.PunchPosition(_Right.gameObject, iTween.Hash("z",3,"loopType","loop","time",0.5f));
	}

	//first stance
//	public void ReadyStance()
//	{
//	}

	// Use this for initialization
	void Start () 
	{
		_leftGlobalPos = _Left.position;

		_leftAnchor = new AnchorPoint (_Left);
		_leftAnchor.SetAnchorRateZ (-0.5f);
		//_leftAnchor.SetAnchorRate (new Vector3 (0,0,-0.5f));
	}
	
	float accumulate = 0;
	float maxSecond = 1.5f;
	float maxSize = 5f;
	void Update () 
	{
		//min 0s -> 0
		//    1s -> 1.5
		//max 2s -> 3
		//
		//2 : 3 = dt : ?
		//3dt = 2? 
		//3dt/2 = ?

		accumulate += Time.deltaTime;
		float scaleDelta = 1 + Mathf.Abs (Interpolation.punch (maxSize, accumulate / maxSecond));
		//float scaleDelta = easeOutBounce(1,maxSize,accumulate/maxSecond);
		//Debug.Log (accumulate + "   " + scaleDelta); //chamto test
		_leftAnchor.ScaleZ (scaleDelta);
		//_leftAnchor.ScaleZ(maxSize*accumulate/maxSecond);
		//Debug.Log (accumulate + "   " + this.punch(maxSize, accumulate/maxSecond)); //chamto test
		if (maxSecond <= accumulate) {
			accumulate = 0;
			//accumulate = maxSecond;
		}

		if (true == _Active) 
		{
			//this.AttackLeft ();
			//this.AttackLeft_Rotate ();

			//this.AttackRight ();
		}
		_Active = false;
	}

}


