using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtendPart_Unity;
using Utility;

public class Move : MonoBehaviour 
{

	public Transform _knight = null;

	void Start () 
	{
		
	}
	
	float accumulate_up = 0;
	float accumulate_left = 0;
	float accumulate_right = 0;
	void Update () 
	{
		
		if (Input.GetKeyUp ("up")) 
		{
			accumulate_up = 0;
			//Debug.Log ("key-up: up state");
		}
		else if (Input.GetKey ("up")) 
		{
			const float MAX_SECOND = 0.5f;
			if (accumulate_up > MAX_SECOND)
				accumulate_up = 0;
			
			accumulate_up += Time.deltaTime;
			float delta = Interpolation.easeInOutBack (0f,0.2f, accumulate_up/MAX_SECOND);
			_knight.Translate (Vector3.forward * delta);
			//Debug.Log ("key-up: down state");
			 
		}
		if (Input.GetKey ("down")) 
		{
			_knight.Translate (Vector3.back * Time.deltaTime * 4f); //one second per 4 move
			//Debug.Log ("down");
		}


		if (Input.GetKeyUp ("left")) 
		{
			accumulate_left = 0;
		}
		else if (Input.GetKey ("left")) 
		{
			const float MAX_SECOND = 0.5f;
			if (accumulate_left > MAX_SECOND)
				accumulate_left = 0;

			accumulate_left += Time.deltaTime;
			float delta = Interpolation.easeInOutBack (0f,10f, accumulate_left/MAX_SECOND);
			_knight.Rotate (Vector3.up, -1 * delta); 
		}



		if (Input.GetKeyUp ("right")) 
		{
			accumulate_right = 0;
		}
		else if (Input.GetKey ("right")) 
		{
			const float MAX_SECOND = 0.5f;
			if (accumulate_right > MAX_SECOND)
				accumulate_right = 0;

			accumulate_right += Time.deltaTime;
			float delta = Interpolation.easeInOutBack (0f,10f, accumulate_right/MAX_SECOND);
			_knight.Rotate (Vector3.up, 1 * delta);
			//Debug.Log ("r");
		}
	}
}
