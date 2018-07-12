using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Utility;

public class PullOut : MonoBehaviour 
{

    public Transform _target_1p = null;
    public Transform _target_2p = null;
    public bool _active_ai_1p = false;
    public bool _active_ai_2p = false;

    private ProtoGame.AI _ai_1p = null;
    private ProtoGame.AI _ai_2p = null;

    private ProtoGame.GObjects _objects = new ProtoGame.GObjects();

	// Use this for initialization
	void Start () 
    {

        _objects._characters.Add(_target_1p);
        _objects._characters.Add(_target_2p);

        //========================================================================
        //========================================================================

        ProtoGame.KeyInput key1p = _target_1p.gameObject.AddComponent<ProtoGame.KeyInput>();
        key1p.SetMainBody(_target_1p);
        key1p.SelectPlayerNum(ProtoGame.KeyInput.ePlayerNum.Player_1);

        _ai_1p = _target_1p.gameObject.AddComponent<ProtoGame.AI>();
        _ai_1p.SetMainBody(_target_1p);
        _ai_1p.enabled = false;

        _ai_1p._ref_objects = _objects;
        //========================================================================
        //========================================================================


        ProtoGame.KeyInput key2p = _target_2p.gameObject.AddComponent<ProtoGame.KeyInput>();
        key2p.SetMainBody(_target_2p);
        key2p.SelectPlayerNum(ProtoGame.KeyInput.ePlayerNum.Player_2);

        _ai_2p = _target_2p.gameObject.AddComponent<ProtoGame.AI>();
        _ai_2p.SetMainBody(_target_2p);
        _ai_2p.enabled = false;

        _ai_2p._ref_objects = _objects;

	}
	
	// Update is called once per frame
	void Update () 
    {
        if(true == _active_ai_1p)
        {
            _ai_1p.enabled = true;
        }else
        {
            _ai_1p.enabled = false;
        }

        if (true == _active_ai_2p)
        {
            _ai_2p.enabled = true;
        }else
        {
            _ai_2p.enabled = false;
        }
		
	}



}


//========================================================
//==================  ==================
//========================================================
namespace ProtoGame
{
    public class GObjects
    {
        public List<Transform> _characters = new List<Transform>();


        //최대 반경이내에서 가장 가까운 객체를 반환한다
        public Transform GetNearCharacter(Transform exceptChar , float maxRadius)
        {

            //todo : 추후구현하기

            foreach(Transform t in _characters)
            {
                if (t == exceptChar) continue;

                return t;
            }

            return null;
        }
    }

}//end namespace 


//========================================================
//==================       인공 지능      ==================
//========================================================
namespace ProtoGame
{
    public class AI : MonoBehaviour
    {
        private Transform _mainBody = null;
        private Transform _target = null;
        private Move _move = new Move();

        public GObjects _ref_objects = null;

        public enum eState
        {
            Detect, //탐지
            Chase,  //추격
            Attack,  //공격
            Escape, //도망
            Roaming, //배회하기
        }
        private eState _state = eState.Roaming;

        public void SetMainBody(Transform mainBody)
        {
            _mainBody = mainBody;
            _move._mainBody = mainBody;
        }

		private void Update()
		{
            if (null == _mainBody) return;

            this.StateUpdate();
		}


        public bool Situation_Is_AttackTarget()
        {
            return false;
        }

        public bool Situation_Is_AttackRange()
        {
            return false;
        }

        public void StateUpdate()
        {
            switch(_state)
            {
                case eState.Detect:
                {
                        //공격대상이 맞으면 추격한다.
                        if(true == Situation_Is_AttackTarget())
                        {
                            _state = eState.Chase;
                        }
                        //공격대상이 아니면 다시 배회한다.
                        else
                        {
                            _state = eState.Roaming;
                        }

                }break;
                
                case eState.Chase:
                {
                        //공격사정거리까지 이동했으면 공격한다. 
                        if(true == Situation_Is_AttackRange())
                        {
                            _state = eState.Attack;
                        }
                        //거리가 멀리 떨어져 있으면 다시 배회한다.
                        {
                            _state = eState.Roaming;
                        }

                }
                break;
                case eState.Attack:
                {
                        //못이길것 같으면 도망간다.
                        {
                            _state = eState.Escape;
                        }

                        //적을 잡았으면 다시 배회한다.
                        {
                            _state = eState.Roaming;
                        }

                }
                break;
                case eState.Escape:
                {
                        //일정 거리 안에 적이 있으면 탐지한다.
                        {
                            _state = eState.Detect;
                        }

                        //다시 배회한다.
                        {
                            _state = eState.Roaming;
                        }
                }
                break;
                case eState.Roaming:
                {
                        //일정 거리 안에 적이 있으면 탐지한다.
                        if(false)
                        {
                            _state = eState.Detect;
                        }



                        _target = _ref_objects.GetNearCharacter(_mainBody, 10f); 

                        switch(_move.DirectionalInspection(_target.localPosition))
                        {
                            case Move.eDirection.LEFT:
                                {
                                    _move.Left(0f);
                                    //DebugWide.LogBlue("LEFT");
                                }
                                break;
                            case Move.eDirection.RIGHT:
                                {
                                    _move.Right(0f);
                                    //DebugWide.LogBlue("RIGHT");
                                }
                                break;
                        }

                        _move.Up(0f);

                }
                break;
            }
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
        public enum eDirection
        {
            CENTER,
            LEFT,
            RIGHT,
        }

        public Transform _mainBody = null;

        private CallInterval _Interval_Up = new CallInterval();
        private CallInterval _Interval_Left = new CallInterval();
        private CallInterval _Interval_Right = new CallInterval();


        public void Up(float MAX_SECOND)
        {
            MAX_SECOND = 0.5f; //test value

            //0.2거리 가는데 0.5초 걸림

            //연속호출 상황시 시간증가값 구하기
            float accumulate = _Interval_Up.GetTimeAccumulate(MAX_SECOND);


            //보간, 이동 처리
            float delta = Interpolation.easeInOutBack(0f, 0.2f, accumulate / MAX_SECOND);
            _mainBody.Translate(Vector3.forward * delta);

        }

        public void Down(float MAX_SECOND)
        {
            MAX_SECOND = 4f; //test value

            _mainBody.Translate(Vector3.back * Time.deltaTime * MAX_SECOND); //one second per 4 move

        }


        public void Left(float MAX_SECOND)
        {
            MAX_SECOND = 0.2f; //test value

            float angle = 5f; //5도 회전하는데 0.2초 걸림 

            //연속호출 상황시 시간증가값 구하기
            float accumulate = _Interval_Up.GetTimeAccumulate(MAX_SECOND);


            //보간, 이동 처리
            float delta = Interpolation.easeOutCirc(0f, angle, accumulate / MAX_SECOND);
            _mainBody.Rotate(Vector3.up, -1 * delta);
        }


        public void Right(float MAX_SECOND)
        {
            MAX_SECOND = 0.2f; //test value

            float angle = 5f;

            //연속호출 상황시 시간증가값 구하기
            float accumulate = _Interval_Up.GetTimeAccumulate(MAX_SECOND);


            //보간, 이동 처리
            float delta = Interpolation.easeOutCirc(0f, angle, accumulate / MAX_SECOND);
            _mainBody.Rotate(Vector3.up, 1 * delta);
        }


        //객체의 전진 방향을 반환한다.
        public Vector3 GetForwardDirect()
        {
            return Quaternion.Euler(_mainBody.localEulerAngles) * Vector3.forward;
        }


        //내방향을 기준으로 목표위치가 어디쪽에 있는지 반환한다.  
        public eDirection DirectionalInspection(Vector3 targetPos)
        {

            Vector3 mainDir = GetForwardDirect();

            Vector3 targetTo = targetPos - _mainBody.localPosition;

            //mainDir.Normalize();
            //targetTo.Normalize();

            Vector3 dir = Vector3.Cross(mainDir, targetTo);
            //dir.Normalize();
            //DebugWide.LogBlue("mainDir:" + mainDir + "  targetTo:" + targetTo + "   cross:" + dir.y);

            float angle = Vector3.Angle(mainDir, targetTo);
            angle = Mathf.Abs(angle);

            if (angle < 3f) return eDirection.CENTER; //사이각이 3도 보다 작다면 중앙으로 여긴다 

            //외적의 y축값이 음수는 왼쪽방향 , 양수는 오른쪽방향 
            if (dir.y < 0)
                return eDirection.LEFT;
            else if (dir.y > 0)
                return eDirection.RIGHT;

            return eDirection.CENTER; 
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

        public void SetMainBody(Transform mainBody)
        {
            _move._mainBody = mainBody;
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
