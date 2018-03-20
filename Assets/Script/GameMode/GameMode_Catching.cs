using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CounterBlock;

public class GameMode_Catching : MonoBehaviour 
{

	Transform _fireOff = null;
	Transform _fireOn = null;

	List<SpriteRenderer> _fireOffList = new List<SpriteRenderer> ();

	// Use this for initialization
	void Start () 
	{
		_fireOff = Single.hierarchy.GetTransform ("1_Game/1_dokkaebi/0_fireOff");
		_fireOn = Single.hierarchy.GetTransform ("1_Game/1_dokkaebi/0_fireOn");
		//_fireOff.gameObject.SetActive (false);

		SpriteRenderer sr = null;
		const int MAX_FIREOFF_COUNT = 21;
		for (int i = 0; i < MAX_FIREOFF_COUNT; i++) 
		{
			sr = Single.hierarchy.GetTypeObject<SpriteRenderer> ("1_Game/1_dokkaebi/0_fireOff/fire_off_" + i);
			_fireOffList.Add (sr);
		}

		StartCoroutine (Update_FireOff());
	}
	
	// Update is called once per frame
	void Update () 
	{
		int rand = Single.rand.Next (0, 21);
		if (0 == rand) 
		{
			//1초 대기 
		}
		if (1 == rand) 
		{
			//3초 대기
		}
	}

	IEnumerator Update_FireOff()
	{
		for (int i = 0; i < 100; i++) 
		{
			int rand = Single.rand.Next (0, 21);

			bool activeSelf = _fireOffList [rand].gameObject.activeSelf;
			_fireOffList [rand].gameObject.SetActive (!activeSelf);

			yield return new WaitForSeconds(Single.rand.Next(1,3));
		}

	}
}
