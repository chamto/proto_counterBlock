using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Utility;

public class PullOut : MonoBehaviour 
{
    //public Transform _root_chatterbox = null;
    public Transform _target_1p = null;
    public Transform _target_2p = null;
    public bool _active_ai_1p = false;
    public bool _active_ai_2p = false;

    private ProtoGame.AI _ai_1p = null;
    private ProtoGame.AI _ai_2p = null;

    //private ProtoGame.GStage _gStage = new ProtoGame.GStage();

	// Use this for initialization
	void Start () 
    {
        //this.gameObject.AddComponent<ProtoGame.UI_Control>();

        ProtoGame.Single.voiceManager.Init();


        //========================================================================
        //========================================================================

        //ProtoGame.Single.objectManager._characters.Add(_target_1p);
        //ProtoGame.Single.objectManager._characters.Add(_target_2p);
        //Vector3 chatPos = new Vector3(0, 0.5f, 0);
        //for (int i = 0; i < 10;i++)
        //{
        //    chatPos.x = (i*1.5f) - 7f;
        //    ProtoGame.Single.objectManager.Create_Chatterbox(_root_chatterbox, i, chatPos );
        //}


        //========================================================================
        //========================================================================

        ProtoGame.KeyInput key1p = _target_1p.gameObject.AddComponent<ProtoGame.KeyInput>();
        key1p.SetMainBody(_target_1p);
        key1p.SelectPlayerNum(ProtoGame.KeyInput.ePlayerNum.Player_1);

        _ai_1p = _target_1p.gameObject.AddComponent<ProtoGame.AI>();
        _ai_1p.SetMainBody(_target_1p);
        _ai_1p.enabled = false;


        //========================================================================
        //========================================================================


        ProtoGame.KeyInput key2p = _target_2p.gameObject.AddComponent<ProtoGame.KeyInput>();
        key2p.SetMainBody(_target_2p);
        key2p.SelectPlayerNum(ProtoGame.KeyInput.ePlayerNum.Player_2);

        _ai_2p = _target_2p.gameObject.AddComponent<ProtoGame.AI>();
        _ai_2p.SetMainBody(_target_2p);
        _ai_2p.enabled = false;


        //========================================================================
        //========================================================================

        //_gStage.Init();
        ProtoGame.Single.gstage.Init();
        ProtoGame.Single.gstage.JumpStage(1);
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

        //_gStage.Update();
        ProtoGame.Single.gstage.Update();
		
	}



}

//========================================================
//==================      전역  객체      ==================
//========================================================
namespace ProtoGame
{
    public static class Single
    {

        public static GStage gstage
        {
            get
            {
                return CSingleton<GStage>.Instance;
            }
        }

        public static VoiceClipManager voiceManager
        {
            get
            {
                return CSingleton<VoiceClipManager>.Instance;
            }
        }

        public static ProtoGame.ObjectManager objectManager
        {
            get
            {
                return CSingleton<ProtoGame.ObjectManager>.Instance;
            }
        }

        public static UI_Control uiControl
        {
            get
            {
                return CSingletonMono<UI_Control>.Instance;
            }
        }

        private static Canvas _canvasRoot = null;
        public static Canvas canvasRoot
        {
            get
            {
                if (null == _canvasRoot)
                {
                    GameObject obj = GameObject.Find("Canvas");
                    if(null != obj)
                    {
                        _canvasRoot = obj.GetComponent<Canvas>();
                    }

                }
                return _canvasRoot;
            }
        }

        private static Transform _chatterboxRoot = null;
        public static Transform chatterboxRoot
        {
            get
            {
                if (null == _chatterboxRoot)
                {
                    GameObject obj = GameObject.Find("0_chatterbox");
                    if (null != obj)
                    {
                        _chatterboxRoot = obj.GetComponent<Transform>();
                    }

                }
                return _chatterboxRoot;
            }
        }
    }

}//end namespace


//========================================================
//==================      음성 관리기      ==================
//========================================================
namespace ProtoGame
{
    public struct VoiceInfo
    {

        public enum eKind
        {
            None,
            Eng_NaverMan_1 = 1,
            Eng_NaverWoman_2 = 2,
            Eng_AmazonMan_3 = 3,
            Eng_AmazonMan_4 = 4,
            Eng_AmazonMan_5 = 5,
            Eng_AmazonWoman_6 = 6,
            Eng_AmazonWoman_7 = 7,
            Eng_AmazonWoman_8 = 8,
            Max,
        }

        public int number;
        public int hash;
        public eKind kind;
        public int speed;


        public VoiceInfo(string fileName)
        {
            number = -1;
            hash = -1;
            kind = eKind.None;
            speed = -1;

            Parsing_VoiceClipName(fileName);
        }

        private void Parsing_VoiceClipName(string fileName)
        {
            //파일저장양식 
            //(0)number _ (1)hash value _ (2)목소리종류 _ (3)말하기속도 _(4)말하기텍스트 
            const int MAX_COUNT = 5;

            char[] delimiterChars = { '_' };
            string[] parts = fileName.Split(delimiterChars, System.StringSplitOptions.RemoveEmptyEntries);

            if (MAX_COUNT == parts.Count())
            {
                number = int.Parse(parts[0]);
                hash = int.Parse(parts[1]);
                kind = (eKind)int.Parse(parts[2]);
                speed = int.Parse(parts[3]);
            }

        }

        public string ToString()
        {
            return "[ " + number + " ]" + "[ " + hash + " ]" + "[ " + kind + " ]" + "[ " + speed + " ]";
        }
    }

    public class AudioClips : Dictionary<int, AudioClip> { } //해쉬키 , 오디오클립
    public class VoiceClipMap : Dictionary<VoiceInfo.eKind, AudioClips>
    {
        public AudioClips GetClips(VoiceInfo.eKind kind)
        {
            if (false == this.ContainsKey(kind))
            {
                this.Add(kind, new AudioClips());
            }
            return this[kind];
        }
    }

   

    public class VoiceClipManager
    {
        
        private VoiceClipMap _voiceClipMap = new VoiceClipMap();


        //==================== XML_DATA ====================
        public XML_Data.Dict_English _dictEng = new XML_Data.Dict_English();



        //==================== Get / Set ====================
       

        //==================== <Method> ====================


        public void ClearAll()
        {
           
            _voiceClipMap.Clear();
            _dictEng.ClearAll();
        }

        public void Init()
        {
           
            this.ClearAll();

            //====== LOAD ======
            this.Load_XML();
          
            this.Load_AudioClip();
        }

        public void Load_XML()
        {
            CounterBlock.Single.coroutine.Start_Sync(_dictEng.LoadXML(), null, "DICT_ENGLISH");
        }


        public void Load_AudioClip()
        {
            AudioClip[] clips = Resources.LoadAll<AudioClip>("Sound/Voice");

            VoiceInfo vInfo;
            for (int i = 0; i < clips.Length; i++)
            {
                vInfo = new VoiceInfo(clips[i].name);
                //DebugWide.LogBlue(clips [i].name);
                //DebugWide.LogBlue(vInfo.ToString()); //chamto test

                _voiceClipMap.GetClips(vInfo.kind).Add(vInfo.hash, clips[i]);

            }
        }


        public AudioClips GetVoiceClips(VoiceInfo.eKind eKind)
        {
            return _voiceClipMap.GetClips(eKind);
        }


        public AudioClip GetAudioClip(VoiceInfo.eKind voiceKind, int XML_dictInfoNum, XML_Data.DictInfo.eKind dictKind, int vocaSeqNum)
        {
            AudioClips clips = this.GetVoiceClips(voiceKind);
            XML_Data.VocaInfoList list = this._dictEng.GetVocaInfoList(XML_dictInfoNum, dictKind);

            int hashKey = list.GetVocaHashKey(vocaSeqNum).hashKey;

            return clips[hashKey];
        }

        public AudioClip GetAudioClip_Group(VoiceInfo.eKind voiceKind, int XML_dictInfoNum, int dictGroupNum, int groupSeqNum)
        {
            AudioClips clips = this.GetVoiceClips(voiceKind);
            XML_Data.VocaInfoList list = this._dictEng.GetVocaInfoGroup(XML_dictInfoNum, dictGroupNum);

            int hashKey = list.GetVocaHashKey(groupSeqNum).hashKey;

            return clips[hashKey];
        }

        public AudioSource _audioSource = null;
        public int _voiceSequence = 0;
        public void Test()
        {
            const int XML_VIVA_LA_VIDA = 100;

            //=================================================
           
            AudioClip clip = this.GetAudioClip(VoiceInfo.eKind.Eng_NaverMan_1, XML_VIVA_LA_VIDA, XML_Data.DictInfo.eKind.Sentence, _voiceSequence);
            int voiceCount = this._dictEng.GetVocaInfoList(XML_VIVA_LA_VIDA, XML_Data.DictInfo.eKind.Sentence).Count;

            //_audioSource.Play (); //chamto test
            //clip = this.GetAudioClip_Group(VoiceInfo.eKind.Eng_NaverMan_1, XML_VIVA_LA_VIDA, 9, 0); //100 : 사전넘버 , 9 : 그룹번호(묶음) , 0 : 그룹의 첫번째 데이터 

            _audioSource.Stop();
            _audioSource.PlayOneShot(clip);
            _voiceSequence++;
            _voiceSequence = _voiceSequence % (voiceCount);
            //=================================================
        }


    }//end class


}//end namespace


//========================================================
//==================        U  I        ==================
//========================================================


//========================================================
//==================     게임 스테이지     ==================
//========================================================
namespace ProtoGame
{
    public class GStage
    {
        //1. 장애물이 하늘에서 1개 떨어진다.
        //2. 장애물이 시끄럽게 말을 한다. 말할때 마다 조금씩 들썩인다 
        //3. 시끄러운 장애물을 밀어서 떨어뜨린다
        //4. 1번으로 돌아간. 장애물의 숫자가 증가한다. 여러개 중에 하나만 말을 한다


        //공을 바깥으로 떨어뜨리는 규칙
        //1. 바로 떨어뜨리기
        //1. 일정 횟수 이상으로 공을 굴린후 떨어뜨리기
        //1. 특정 음성이 나올때 떨어뜨리기
        //1. 공이 특정 색으로 변할때 떨어뜨리기a


        //지정된 시간이 지나면 죽게 처리
        //죽음 처리 - 시간경과 , 목표 미달
        //죽음 처리 - 판에서 떨어진 경우 

        //점수 처리 , 게임 다시시작 , 레벨디자인 진행

        private uint _stageNum = 1;
        private float _gameTime_s = 60f;
        private uint _score = 0;

        private ObjectManager OBJMGR = null;
        private UI_Control UICTR = null;

        public void Init()
        {
            OBJMGR = ProtoGame.Single.objectManager;
            UICTR = ProtoGame.Single.uiControl;
        }

        public void Update()
        {
            if      (0 < _gameTime_s)
                _gameTime_s -= Time.deltaTime;
            else if (0 > _gameTime_s)
            {
                
                _gameTime_s = 0f;

                UICTR.Active_Button_Retry(true); //시간 초과시 다시하기 단추 켜기
            }
            else if(0 == _gameTime_s)
            {}


            _score = OBJMGR.Count_DeathChatterBox();

            //ui갱신
            UICTR.SetTextStage(_stageNum);
            UICTR.SetTextInfo(_gameTime_s, _score);
        }


        public void JumpStage(uint stageNum)
        {
            _stageNum = stageNum;
            _gameTime_s = 60f;
            _score = 0;

            UICTR.Active_Button_Retry(false);
            OBJMGR.CreateFor_StageInfo(stageNum);

        }

    }

}//end namespace

//========================================================
//==================      객체 관리기      ==================
//========================================================
namespace ProtoGame
{
    public class ObjectManager
    {
        public List<Transform> _characters = new List<Transform>();
        public List<Chatterbox> _chatterboxes = new List<Chatterbox>();   //말하는 장애물 



        public void ClearAll()
        {
            
            foreach(Chatterbox t in _chatterboxes)
            {
                GameObject.Destroy(t.gameObject);
            }

            _characters.Clear();
            _chatterboxes.Clear();
        }

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


        //다른 수다박스가 재생중인지 알려준다 
        public bool IsPlaying_ChatterBoxes()
        {
            AudioSource audio = null;
            foreach(Chatterbox t in _chatterboxes)
            {
                audio = t.GetComponent<AudioSource>();
                if(null != audio)
                {
                    //하나라도 재생중
                    if (true == audio.isPlaying)
                        return true;
                }
            }

            //하나도 재생중 아님
            return false;
        }

        public uint Count_DeathChatterBox()
        {
            uint count = 0;
            foreach (Chatterbox t in _chatterboxes)
            {
                if(false == t._isLive)
                {
                    count++;
                }
            }
            return count;
        }

        //____________________________________________
        //                  객체 생성 
        //____________________________________________

        public GameObject CreatePrefab(string prefabPath, Transform parent, string name)
        {
            const string root = "Prefab/";
            GameObject obj = MonoBehaviour.Instantiate(Resources.Load(root + prefabPath)) as GameObject;
            obj.transform.SetParent(parent, false);
            obj.transform.name = name;


            return obj;
        }

        public GameObject Create_Chatterbox(Transform parent, int id,  Vector3 pos)
        {
            GameObject obj = CreatePrefab("Proto/PullOut/Cube_00",parent, "Cube_"+id.ToString("00"));
            Chatterbox cbox = obj.AddComponent<Chatterbox>();
            _chatterboxes.Add(cbox);
            cbox.id = id;
            cbox.transform.localPosition = pos;

            return obj;
        }

        public GameObject Create_Character(Transform parent, int id, Vector3 pos)
        {
            GameObject obj = null;
            return obj;
        }

        public void CreateFor_StageInfo(uint stageNum)
        {

            this.ClearAll();

            //ProtoGame.Single.objectManager._characters.Add(_target_1p);
            //ProtoGame.Single.objectManager._characters.Add(_target_2p);
            Vector3 chatPos = new Vector3(0, 0.5f, 0);
            for (int i = 0; i < 10; i++)
            {
                chatPos.x = (i * 1.5f) - 7f;
                Create_Chatterbox(Single.chatterboxRoot, i, chatPos);
            }
        }



    }

    public class Chatterbox : MonoBehaviour
    {

        public int id
        { get; set; }

        private int _hp = 10;
        public bool _isLive = true;

        private AudioSource _audioSource = null;
        private TextMesh _text_hp = null;

		private void Start()
		{
            _audioSource = this.GetComponent<AudioSource>();
            _text_hp = this.GetComponentInChildren<TextMesh>(true);
            this.SetHP(_hp);
		}

        private void Update()
        {
            if (true == _isLive)
            {
                if(true == Judge_OffTheRing())
                {
                    Death();
                }
                if(0 == _hp)
                {
                    Death();    
                }

            }


        }

        void OnCollisionEnter(Collision col)
        {

            //DebugWide.LogBlue("OnCollisionEnter:  " + col.gameObject.name + "   " + col.gameObject.tag);

            //캐릭터에만 영향을 받는다 
            if("character" == col.gameObject.tag)
            {
                //DebugWide.LogBlue("coll time : " + Time.fixedTime); //chamto test
                Judge_FirstAttacked();
            }


            if(true == _isLive)
            {
                //Speaking();
                Speaking_JustOne();    
            }

        }
        void OnCollisionStay(Collision col)
        {
            //DebugWide.LogBlue("OnCollisionStay:  " + col.gameObject.name);
        }
        void OnCollisionExit(Collision col)
        {
            //DebugWide.LogBlue("OnCollisionExit:  " + col.gameObject.name);
        }

       
        //정해진 순서로 말한다. (다른 수다박스와 상관없이 동작) 
        public int _voiceSequence = 0;
        public void Speaking()
        {
            if (null == _audioSource) return;
            if (true == _audioSource.isPlaying) return;

            //DebugWide.LogBlue("Speaking --------- " + _voiceSequence);

            const int XML_VIVA_LA_VIDA = 100;

            //=================================================

            AudioClip clip = Single.voiceManager.GetAudioClip(VoiceInfo.eKind.Eng_NaverMan_1, XML_VIVA_LA_VIDA, XML_Data.DictInfo.eKind.Sentence, _voiceSequence);
            int voiceCount = Single.voiceManager._dictEng.GetVocaInfoList(XML_VIVA_LA_VIDA, XML_Data.DictInfo.eKind.Sentence).Count;

            //_audioSource.Play (); //chamto test
            //clip = this.GetAudioClip_Group(VoiceInfo.eKind.Eng_NaverMan_1, XML_VIVA_LA_VIDA, 9, 0); //100 : 사전넘버 , 9 : 그룹번호(묶음) , 0 : 그룹의 첫번째 데이터 

            _audioSource.Stop();
            _audioSource.PlayOneShot(clip);
            _voiceSequence++;
            _voiceSequence = _voiceSequence % (voiceCount);
            //=================================================
        }

        //다른 수다박스가 말하지 않을때만 말할 수 있다  
        public void Speaking_JustOne()
        {
            if (true == Single.objectManager.IsPlaying_ChatterBoxes()) return;

            this.Speaking();
        }

        public void SetHP(int hp)
        {
            _hp = hp;
            this._text_hp.text = _hp.ToString();
        }

        public void SetHP_Plus(int plus)
        {
            _hp = _hp + plus;
            if (0 > _hp) _hp = 0;
            this.SetHP(_hp);
        }

		public void Death()
        {
            _isLive = false;
            //_hp = 0;
            this.SetHP(0);
        }

        //____________________________________________
        //                객체 상태 판단 
        //____________________________________________

        //첫번째 공격으로 공격당함
        public void Judge_FirstAttacked()
        {
            this.SetHP_Plus(-1); //hp -1 
        }


        //링에서 떨어짐 
        public bool Judge_OffTheRing()
        {
            const float FLOOR_HEIGHT = 0f;
            const float DELAY = 3f;
            if (FLOOR_HEIGHT - DELAY > this.transform.position.y)
                return true;

            return false;
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

        //public ObjectManager _ref_objects = null;

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

		private void FixedUpdate()
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



                        _target = ProtoGame.Single.objectManager.GetNearCharacter(_mainBody, 10f); 

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


        public void PostureRecovery()
        {
            //chamto test
            //DebugWide.LogBlue(_mainBody.transform.localEulerAngles); //chamto test

            Vector3 angles = _mainBody.transform.localEulerAngles;
            float angle_x = -1.5f * Time.deltaTime;
            float angle_z = -1.5f * Time.deltaTime;

            //180도를 넘는 각도에 대해 음수 각도표현으로 변경
            if (angles.x > 180f) angles.x -= 360f;
            if (angles.z > 180f) angles.z -= 360f;


            if (angles.x < 0) angle_x *= -1f;
            if (angles.z < 0) angle_z *= -1f;

            //DebugWide.LogBlue(angle_x + "   " + angle_z); //chamto test

            angles.x = angles.x + angle_x;
            angles.z = angles.z + angle_z;

            //todo
            //1-45  = -44
            //1-1.5 = -0.5
            //1-0.5 = 0.5

          
            float min_angle = 2f; //허용 최소 각도
            if ((min_angle - Mathf.Abs(angles.x)) > 0) angles.x = 0;
            if ((min_angle - Mathf.Abs(angles.z)) > 0) angles.z = 0;


            //DebugWide.LogRed(angles); //chamto test
            //_mainBody.transform.localEulerAngles = angles;

            _mainBody.GetComponent<Rigidbody>().AddForce(Vector3.down * 4f * Time.deltaTime, ForceMode.Force);
              
           
        }

        public void Up(float MAX_SECOND)
        {
            MAX_SECOND = 0.5f; //test value

            //0.2거리 가는데 0.5초 걸림

            //연속호출 상황시 시간증가값 구하기
            float accumulate = _Interval_Up.GetTimeAccumulate(MAX_SECOND);


            //보간, 이동 처리
            float delta = Interpolation.easeInOutBack(0f, 0.2f, accumulate / MAX_SECOND);
            _mainBody.Translate(Vector3.forward * delta);

            //=============s
            //chamto test
            Vector3 dir = this.GetForwardDirect();
            dir.Normalize();
            //_mainBody.GetComponent<Rigidbody>().AddForce(dir * 50f * delta, ForceMode.VelocityChange);


            //chamto test
            //_mainBody.GetComponent<Rigidbody>().AddExplosionForce(300f, _mainBody.transform.position, 20f, 10f);

            //chamto test2
            //_mainBody.GetComponent<Rigidbody>().MovePosition(_mainBody.position + (dir * 3f));
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
            //_mainBody.Rotate(Vector3.up, -1 * delta);


            //=============
            //물리 처리로 변경 
            _mainBody.GetComponent<Rigidbody>().AddRelativeTorque(Vector3.down * 1f * delta , ForceMode.VelocityChange);
        }


        public void Right(float MAX_SECOND)
        {
            MAX_SECOND = 0.2f; //test value

            float angle = 5f;

            //연속호출 상황시 시간증가값 구하기
            float accumulate = _Interval_Up.GetTimeAccumulate(MAX_SECOND);


            //보간, 이동 처리
            float delta = Interpolation.easeOutCirc(0f, angle, accumulate / MAX_SECOND);
            //_mainBody.Rotate(Vector3.up, 1 * delta);


            //=============
            //물리 처리로 변경 
            _mainBody.GetComponent<Rigidbody>().AddRelativeTorque(Vector3.up * 1f * delta , ForceMode.VelocityChange);

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
            _move.PostureRecovery(); //chamto test
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
