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


	private AnchorPoint _leftAnchor = null;

	void Start () 
	{
		_leftAnchor = new AnchorPoint (_Left);
		_leftAnchor.SetAnchorRateZ (-0.5f);
	}
		


	public void AttackRight()
	{
		//ref : http://itween.pixelplacement.com/documentation.php
		//iTween.MoveBy(_Right.gameObject, iTween.Hash("z", 2, "easeType", "easeInOutExpo", "loopType", "pingPong", "delay", .1));
		iTween.PunchPosition(_Right.gameObject, iTween.Hash("z",3,"loopType","loop","time",0.5f));
	}


	float accumulate = 0;
	float maxSecond = 1.5f;
	float maxSize = 5f;
	float directAngle = 5f;
	float angleSum = 0;
	void AttackLeft () 
	{
		//    second  -> size
		//min 0s 	  -> 0
		//    1s 	  -> 1.5
		//max 2s 	  -> 3
		//
		//2s : 3 = deltaTime(accumulate) : deltaSize
		//3 * deltaTime = 2s * deltaSize 
		//3 * deltaTime / 2s = deltaSize
		//!!! _leftAnchor.ScaleZ( maxSize * accumulate / maxSecond);

		accumulate += Time.deltaTime;
		float scaleDelta = 1 + Mathf.Abs (Interpolation.punch (maxSize, accumulate / maxSecond));
		//float scaleDelta = Interpolation.easeOutBounce(1,maxSize,accumulate/maxSecond);
		//Debug.Log (accumulate + "   " + scaleDelta); //chamto test
		_leftAnchor.ScaleZ (scaleDelta);

		if (maxSecond <= accumulate) 
		{
			accumulate = 0; 			//a. repeat
			//accumulate = maxSecond;	//b. one time
		}

		_leftAnchor.RotateX (directAngle);


		if (5f < angleSum || angleSum < -20f) {
			directAngle *= -1;
		}
		angleSum += directAngle;


	}


	void Update ()
	{
		this.AttackLeft ();

		if (true == _Active) 
		{
			//this.AttackLeft ();
			//this.AttackRight ();
		}
		_Active = false;
	}
}


