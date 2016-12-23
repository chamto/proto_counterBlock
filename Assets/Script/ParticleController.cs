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


		//var em = ps.emission;
		ParticleSystem.EmissionModule em = ps.emission;
		//em.rateOverTime = new ParticleSystem.MinMaxCurve (4f);
		em.rateOverTime = 3f;


		//ps.emission.rateOverTime = new ParticleSystem.MinMaxCurve(5f);
		DebugWide.LogBlue (ps.emission.rateOverTime.constant);



		ps.transform.position = worldPos;
		ps.gameObject.SetActive (true);
		ps.Play ();
	}
}
