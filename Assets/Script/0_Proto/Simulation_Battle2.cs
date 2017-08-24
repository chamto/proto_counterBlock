/// <summary>
/// 
/// Simulation_Battle2
/// 
/// 20170825 - chamto - anrudco@gmail.com
/// 
/// </summary>

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace CounterBlock
{

	public struct BehaviorTime
	{
		//--------<<============>>----------
		//    minOpenTime ~ maxOpenTime
		// 시간범위 안에 있어야 콤보가 된다
		public const float MAX_OPEN_TIME = 10f;
		public const float MIN_OPEN_TIME = 0f;

		//===================================

		public float time_before;
		public float time_after;
		public float scope_start;
		public float scope_end;

		public float max_openTime; //최대 연결시간 : 최대 시간 안에 동작 해야 함
		public float min_openTime; //최소 연결시간 : 최소 시간 이후에 동작 해야 함

		public void Init()
		{
			time_before = 0f;
			time_after = 0f;
			scope_start = 0f;
			scope_end = 0f;
		}
	}



	public class Simulation_Battle2 : MonoBehaviour 
	{



		// Use this for initialization
		void Start () 
		{
			

		}



		// Update is called once per frame
		void Update () 
		{

			//////////////////////////////////////////////////
			//1p

			//attack
			if (Input.GetKeyUp ("q")) 
			{
				DebugWide.LogBlue ("1p - keyinput");
			}

			//////////////////////////////////////////////////
			//2p

			//attack
			if (Input.GetKeyUp ("o")) 
			{
				DebugWide.LogBlue ("2p - keyinput");
			}

		}//end Update

	}//end class Simulation 




}//end namespace 


