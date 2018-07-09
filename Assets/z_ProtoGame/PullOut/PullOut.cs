using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Utility;

public class PullOut : MonoBehaviour 
{

    public Transform _target_1p = null;
    public Transform _target_2p = null;
    public bool _active_target_1p = false;
    public bool _active_target_2p = false;

	// Use this for initialization
	void Start () 
    {
        ProtoGame.KeyInput key1p = _target_1p.gameObject.AddComponent<ProtoGame.KeyInput>();
        key1p.SetTarget(_target_1p);
        key1p.SelectPlayerNum(ProtoGame.KeyInput.ePlayerNum.Player_1);

        ProtoGame.KeyInput key2p = _target_2p.gameObject.AddComponent<ProtoGame.KeyInput>();
        key2p.SetTarget(_target_2p);
        key2p.SelectPlayerNum(ProtoGame.KeyInput.ePlayerNum.Player_2);

        //ProtoGame.AI ai = _target_2p.gameObject.AddComponent<ProtoGame.AI>();
        //ai.SetTarget(_target_2p);

	}
	
	// Update is called once per frame
	void Update () 
    {
        //if(true == _active_target_1p)
        //{
        //    ProtoGame.KeyInput keyInput = _target_1p.gameObject.GetComponent<ProtoGame.KeyInput>();
        //    keyInput.SetTarget(_target_1p);
        //}else
        //{
        //    ProtoGame.KeyInput keyInput = _target_1p.gameObject.GetComponent<ProtoGame.KeyInput>();
        //    keyInput.SetTarget(null);
        //}

        //if (true == _active_target_2p)
        //{
        //    ProtoGame.AI ai = _target_2p.gameObject.GetComponent<ProtoGame.AI>();
        //    ai.SetTarget(_target_2p);
        //}else
        //{
        //    ProtoGame.AI ai = _target_2p.gameObject.GetComponent<ProtoGame.AI>();
        //    ai.SetTarget(null);
        //}
		
	}



}

namespace ProtoGame
{
    public class AI : MonoBehaviour
    {
        private Transform _target = null;
        private Move _move = new Move();

        public void SetTarget(Transform target)
        {
            _target = target;
            _move._target = target;
        }

		private void Update()
		{
            if (null == _target) return;

            _move.Up(0f); //test
		}

	}

}//end namespace


//========================================================
//================== 이동 처리 , 키입력 처리 ==================
//========================================================
namespace ProtoGame
{
    public class CallInterval
    {
        private float _accumulate = 0f;
        private float _prev_accumulate = 0f;
        private float _CALL_TIME_INTERVAL = 0.2f; //함수 호출 간격이 0.2초 이하면 연속호출 상태라 가정함


        public float GetTimeAccumulate(float MAX_SECOND)
        {
            if ((_accumulate - _prev_accumulate) < _CALL_TIME_INTERVAL)
            {   //**** 연속호출 상태 *****

                _accumulate += Time.deltaTime;

                if (_accumulate > MAX_SECOND)
                    _accumulate = 0;

            }
            else
            {   //**** 불연속호출 상태 ****
                _accumulate = 0;
            }

            _prev_accumulate = _accumulate;

            return _accumulate;
        }
    }

    public class Move
    {
        
        public Transform _target = null;

        private CallInterval _Interval_Up = new CallInterval();
        private CallInterval _Interval_Left = new CallInterval();
        private CallInterval _Interval_Right = new CallInterval();


        public void Up(float MAX_SECOND)
        {
            MAX_SECOND = 0.5f; //test value

            //연속호출 상황시 시간증가값 구하기
            float accumulate = _Interval_Up.GetTimeAccumulate(MAX_SECOND);


            //보간, 이동 처리
            float delta = Interpolation.easeInOutBack(0f, 0.2f, accumulate / MAX_SECOND);
            _target.Translate(Vector3.forward * delta);

        }

        public void Down(float MAX_SECOND)
        {
            MAX_SECOND = 4f; //test value

            _target.Translate(Vector3.back * Time.deltaTime * MAX_SECOND); //one second per 4 move

        }


        public void Left(float MAX_SECOND)
        {
            MAX_SECOND = 0.2f; //test value

            //연속호출 상황시 시간증가값 구하기
            float accumulate = _Interval_Up.GetTimeAccumulate(MAX_SECOND);


            //보간, 이동 처리
            float delta = Interpolation.easeOutCirc(0f, 5f, accumulate / MAX_SECOND);
            _target.Rotate(Vector3.up, -1 * delta);
        }


        public void Right(float MAX_SECOND)
        {
            MAX_SECOND = 0.2f; //test value

            //연속호출 상황시 시간증가값 구하기
            float accumulate = _Interval_Up.GetTimeAccumulate(MAX_SECOND);


            //보간, 이동 처리
            float delta = Interpolation.easeOutCirc(0f, 5f, accumulate / MAX_SECOND);
            _target.Rotate(Vector3.up, 1 * delta);
        }
    }

    public class KeyInput : MonoBehaviour 
    {
        public enum ePlayerNum
        {
            Player_1 = 0,
            Player_2 = 1
        }
        public enum eKeyName
        {
            UP = 0,
            DOWN,
            LEFT,
            RIGHT
        }

        private ePlayerNum _playerNum = ePlayerNum.Player_1;


        private string[,] _keys = { { "w", "s", "a", "d" } , { "up", "down", "left", "right" }};


        private Move _move = new Move();


        public void SelectPlayerNum(ePlayerNum playerNum)
        {
            _playerNum = playerNum;
        }

        public void SetTarget(Transform target)
        {
            _move._target = target;
        }
		

		void FixedUpdate()
        {
            this.Up();
            this.Down();
            this.Left();
            this.Right();
        }


        public void Up()
        {
            if (Input.GetKeyUp(_keys[(int)_playerNum,(int)eKeyName.UP]) )
            {
                //Debug.Log ("key-up: up state");
            }
            else if (Input.GetKey(_keys[(int)_playerNum, (int)eKeyName.UP]))
            {
                //Debug.Log ("key-up: down state");
                _move.Up(0f);
            }
        }

        public void Down()
        {
            if (Input.GetKey(_keys[(int)_playerNum, (int)eKeyName.DOWN]))
            {
                _move.Down(0f);   
            }
        }

        
        public void Left()
        {
            if (Input.GetKeyUp(_keys[(int)_playerNum, (int)eKeyName.LEFT]))
            {
                
            }
            else if (Input.GetKey(_keys[(int)_playerNum, (int)eKeyName.LEFT]))
            {
                _move.Left(0f);
            }
        }

        
        public void Right()
        {
            if (Input.GetKeyUp(_keys[(int)_playerNum, (int)eKeyName.RIGHT]))
            {
                
            }
            else if (Input.GetKey(_keys[(int)_playerNum, (int)eKeyName.RIGHT]))
            {
                _move.Right(0f);
            }
        }
    }//end class

}//end namespace
