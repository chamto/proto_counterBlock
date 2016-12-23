using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour 
{

	public ParticleSystem[] _particList = null; //4size

	private int _nextIndex = 0;

	// Use this for initialization
	void Start () 
	{
		foreach (ParticleSystem ps in _particList) 
		{
			ps.gameObject.SetActive (false);
		}	
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	private ParticleSystem NextParticle()
	{
		ParticleSystem ps = _particList [_nextIndex];

		_nextIndex++;
		_nextIndex %= _particList.Length;

		return ps;
	}

	public void PlayDamage(Vector3 worldPos)
	{
		ParticleSystem ps = this.NextParticle ();

		ps.Stop ();

		ParticleSystem.EmissionModule emission = ps.emission;
		ParticleSystem.MinMaxCurve rateOverTime = emission.rateOverTime.constant;
		rateOverTime.constant = 10f;
		emission.rateOverTime = rateOverTime;



		ps.transform.position = worldPos;
		ps.gameObject.SetActive (true);
		ps.Play ();
	}
}
