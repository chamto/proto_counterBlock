using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBitOperation : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
	{
		//비트연산 연구필요
		DebugWide.LogBlue ("****** Find_2_Multiple ******");
		Find_2_Multiple ();
		DebugWide.LogBlue ("");

		DebugWide.LogBlue ("****** Calc_4_Multiple ******");
		Calc_4_Multiple ();

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Find_2_Multiple()
	{
		int su = 0;
		for(int num=0;num<20;num++)
		{
			//** 2의 배수 찾을때 편한 방법 
			su = (num>>1)<<1;

			DebugWide.LogBlue (num + "  "+ su );	
		}
	}

	public void Calc_4_Multiple()
	{
		int su = 0;
		for(int num=0;num<20;num++)
		{
			//** 숫자를 4의 배수로 맞추기 : %연산보다 빠르다고 함
			su = (num + 3) & (~3); 


			DebugWide.LogBlue (num + "  "+ su );	
		}
	}

}
