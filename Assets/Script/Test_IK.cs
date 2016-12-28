using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_IK : MonoBehaviour 
{
	public Animator _animator = null;
	public Transform	_joint = null;
	public Transform	_target = null;
	public Transform	_character = null;
	public Transform	_forward = null;

	public bool 		_active = false;
	public float 		_speed	=	1f;

	int test_physice1 = Animator.StringToHash("Base Layer.test_physice1");
	int test_physice2 = Animator.StringToHash("Base Layer.test_physice2");

	// Use this for initialization
	void Start () {
		
	}
	
	float aniTime = 0;
	void FixedUpdate () 
	{
		//target x-axic rotate

//		aniTime += Time.deltaTime;
//		aniTime %= 2;
		//_animator.Play("test_physice1",0,aniTime);

		if (Input.GetKey ("up")) 
			_character.Translate (Vector3.forward * Time.deltaTime *5);
		if (Input.GetKey ("down")) 
			_character.Translate (Vector3.back * Time.deltaTime *5);
		if (Input.GetKey ("left")) 
			_character.Rotate (Vector3.up, -1f);
		if (Input.GetKey ("right")) 
			_character.Rotate (Vector3.up, 1f);

		_forward.position = _character.position + _character.forward * 5f;


//		AnimatorStateInfo info =  _animator.GetCurrentAnimatorStateInfo(0);
//		if (0.1f >= info.normalizedTime) 
//		{
//			_animator.SetFloat ("direction", 1);
//		}

	}

	void OnTriggerEnter(Collider other)
	{
		if (_active == false)
			return;
		if (!other.tag.Equals ("dummy"))
			return;
		float direction = Vector3.Dot (_character.forward, other.transform.position - _character.position);
		if (direction < 0)
			return;
		
//			_joint.LookAt(_target);

		AnimatorStateInfo info =  _animator.GetCurrentAnimatorStateInfo(0);
		//if (test_physice1 == info.nameHash) 
		{
			aniTime = info.normalizedTime;
			//_animator.Stop ();
		}
	}
	void OnTriggerStay(Collider other)
	{
		if (_active == false)
			return;
		if (!other.tag.Equals ("dummy"))
			return;
		float direction = Vector3.Dot (_character.forward, other.transform.position - _character.position);
		if (direction < 0)
			return;



		aniTime -= Time.deltaTime;
		aniTime = aniTime < 0 ? 0 : aniTime; 
		AnimatorStateInfo info =  _animator.GetCurrentAnimatorStateInfo(0);
		//if (test_physice1 == info.nameHash) 
		{
			//_animator.SetFloat ("direction", -1);
			_animator.Play (info.fullPathHash, 0, aniTime);
			//DebugWide.LogBlue ("triggerExit - namehash : length - " + info.length + " : normalTime - " + info.normalizedTime );
		}

	}
	void OnTriggerExit(Collider other)
	{
		if (_active == false)
			return;
		if (!other.tag.Equals ("dummy"))
			return;
		float direction = Vector3.Dot (_character.forward, other.transform.position - _character.position);
		if (direction < 0)
			return;

		AnimatorStateInfo info =  _animator.GetCurrentAnimatorStateInfo(0);
		_animator.Play (info.fullPathHash,0,0);
		//_animator.Play (info.fullPathHash,0,aniTime);
		//_animator.speed = 0.1f;
		//_animator.SetFloat ("direction", 1);

//		if(true != waiting)
//			StartCoroutine (WaitForAnimation (0.3f));
	}

	bool waiting = false;
	IEnumerator WaitForAnimation(float ratio)
	{
		float accu = 0;
		while (float.Epsilon + accu < ratio) 
		{
			waiting = true;
			accu += Time.deltaTime;
			yield return new WaitForEndOfFrame ();
		}

		//_animator.speed = 1;
		AnimatorStateInfo info =  _animator.GetCurrentAnimatorStateInfo(0);
		_animator.Play (info.fullPathHash,0,0);

		DebugWide.LogBlue ("coru");

		waiting = false;
	}
}
