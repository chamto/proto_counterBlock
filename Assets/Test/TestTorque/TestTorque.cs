using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTorque : MonoBehaviour 
{

    private Rigidbody _rigid = null;

    //ref : https://docs.unity3d.com/kr/530/ScriptReference/ForceMode.html
    public ForceMode _fMode = ForceMode.Acceleration;

	// Use this for initialization
	void Start () 
    {
        _rigid = this.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public float amount = 50f;


    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal") * amount * Time.deltaTime;
        float v = Input.GetAxis("Vertical") * amount * Time.deltaTime;

        _rigid.AddTorque(transform.up * h, _fMode);
        _rigid.AddTorque(transform.right * v, _fMode);

        //_rigid.AddRelativeTorque(transform.up * h, _fMode);
        //_rigid.AddRelativeTorque(transform.right * v, _fMode);

        if (Input.GetKeyUp("a"))
        {
            _rigid.AddExplosionForce(1000f, _rigid.transform.position, 20f, 20f);
        }
    }
}
