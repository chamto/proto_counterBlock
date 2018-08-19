using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using Utility;

public class PullOut : MonoBehaviour
{
    //public Transform _root_chatterbox = null;
    //public Transform _target_1p = null;
    //public Transform _target_2p = null;
    public bool _active_ai_1p = false;
    public bool _active_ai_2p = false;

    //private ProtoGame.AI _ai_1p = null;
    //private ProtoGame.AI _ai_2p = null;

    //private ProtoGame.GStage _gStage = new ProtoGame.GStage();

    // Use this for initialization
    void Start()
    {

        ProtoGame.Single.voiceManager.Init();
        this.gameObject.AddComponent<ProtoGame.TouchProcess>();

        //========================================================================
        //========================================================================


        //ProtoGame.KeyInput key1p = _target_1p.gameObject.AddComponent<ProtoGame.KeyInput>();
        //key1p.SetMainBody(_target_1p);
        //key1p.SelectPlayerNum(ProtoGame.KeyInput.ePlayerNum.Player_1);

        //_ai_1p = _target_1p.gameObject.AddComponent<ProtoGame.AI>();
        //_ai_1p.SetMainBody(_target_1p);
        //_ai_1p.enabled = false;


        ////========================================================================
        ////========================================================================


        //ProtoGame.KeyInput key2p = _target_2p.gameObject.AddComponent<ProtoGame.KeyInput>();
        //key2p.SetMainBody(_target_2p);
        //key2p.SelectPlayerNum(ProtoGame.KeyInput.ePlayerNum.Player_2);

        //_ai_2p = _target_2p.gameObject.AddComponent<ProtoGame.AI>();
        //_ai_2p.SetMainBody(_target_2p);
        //_ai_2p.enabled = false;


        //========================================================================
        //========================================================================

        //_gStage.Init();
        ProtoGame.Single.gstage.Init();
        ProtoGame.Single.gstage.JumpStage(1);
    }

    // Update is called once per frame
    void Update()
    {
        //if(true == _active_ai_1p)
        //{
        //    _ai_1p.enabled = true;
        //}else
        //{
        //    _ai_1p.enabled = false;
        //}

        //if (true == _active_ai_2p)
        //{
        //    _ai_2p.enabled = true;
        //}else
        //{
        //    _ai_2p.enabled = false;
        //}

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

        public static TouchProcess touchProcess
        {
            get
            {
                return CSingletonMono<TouchProcess>.Instance;
            }
        }

        private static Camera _mainCamera = null;
        public static Camera mainCamera
        {
            get
            {
                if (null == _mainCamera)
                {

                    GameObject obj = GameObject.Find("Main Camera");
                    if (null != obj)
                    {
                        _mainCamera = obj.GetComponent<Camera>();
                    }

                }
                return _mainCamera;
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
                    if (null != obj)
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

        private static Transform _characterRoot = null;
        public static Transform characterRoot
        {
            get
            {
                if (null == _characterRoot)
                {
                    GameObject obj = GameObject.Find("0_character");
                    if (null != obj)
                    {
                        _characterRoot = obj.GetComponent<Transform>();
                    }

                }
                return _characterRoot;
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
        private const int _PLAYER_1_ID = 0;

        public void Init()
        {
        }

        public void Update()
        {
            

            if (0 < _gameTime_s)
            {
                _gameTime_s -= Time.deltaTime;

                if (false == Single.objectManager.GetCharacter(_PLAYER_1_ID)._isLive)
                {
                    Retry();
                }

            }

            if (0 > _gameTime_s)
            {
                _gameTime_s = 0f;

                Retry();

            }
            //else if(0 == _gameTime_s)
            //{

            //}

            _score = Single.objectManager.Count_DeathChatterBox();

            //ui갱신
            Single.uiControl.SetTextStage(_stageNum);
            Single.uiControl.SetTextInfo(_gameTime_s, _score);
        }

        public void Retry()
        {
            Single.objectManager.GetCharacterMove(_PLAYER_1_ID).canNot_Move = true;
            Single.uiControl.Active_Button_Retry(true); //시간 초과시 다시하기 단추 켜기
        }


        public void JumpStage(uint stageNum)
        {
            _stageNum = stageNum;
            _gameTime_s = 60f;
            _score = 0;

            Single.touchProcess.DetachAll(); //등록된 모든 터치이벤트 대상 제거
            Single.uiControl.Active_Button_Retry(false);
            Single.objectManager.CreateFor_StageInfo(stageNum);

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
        public List<Character> _characters = new List<Character>();
        public List<Chatterbox> _chatterboxes = new List<Chatterbox>();   //말하는 장애물 



        public void ClearAll()
        {

            foreach (Character t in _characters)
            {
                GameObject.Destroy(t.gameObject);
            }

            foreach (Chatterbox t in _chatterboxes)
            {
                GameObject.Destroy(t.gameObject);
            }

            _characters.Clear();
            _chatterboxes.Clear();
        }

        public Character GetCharacter(int id)
        {
            foreach (Character c in _characters)
            {
                if (c.id == id)
                {
                    return c;
                }
            }

            return null;
        }

        public Movable GetCharacterMove(int id)
        {
            foreach (Character c in _characters)
            {
                if (c.id == id)
                {
                    return c.GetComponent<Movable>();
                }
            }

            return null;
        }

        //최대 반경이내에서 가장 가까운 객체를 반환한다
        public Transform GetNearCharacter(Transform exceptChar, float maxRadius)
        {

            //todo : 추후구현하기

            foreach (Character t in _characters)
            {
                if (t.transform == exceptChar) continue;

                return t.transform;
            }

            return null;
        }


        //다른 수다박스가 재생중인지 알려준다 
        public bool IsPlaying_ChatterBoxes()
        {
            AudioSource audio = null;
            foreach (Chatterbox t in _chatterboxes)
            {
                audio = t.GetComponent<AudioSource>();
                if (null != audio)
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
                if (false == t._isLive)
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

        public Chatterbox Create_Chatterbox(Transform parent, int id, Vector3 pos)
        {
            GameObject obj = CreatePrefab("Proto/PullOut/Cube_00", parent, "Cube_" + id.ToString("00"));
            Chatterbox cbox = obj.AddComponent<Chatterbox>();
            _chatterboxes.Add(cbox);
            cbox.id = id;
            cbox.transform.localPosition = pos;

            return cbox;
        }

        public Character Create_Character(Transform parent, int id, Vector3 pos)
        {
            GameObject obj = CreatePrefab("Proto/PullOut/jongdali", parent, "jongdali_" + id.ToString("00"));
            Character cha = obj.AddComponent<Character>();
            _characters.Add(cha);
            cha.id = id;
            cha.transform.localPosition = pos;

            return cha;
        }

        public void CreateFor_StageInfo(uint stageNum)
        {

            this.ClearAll(); //스테이지 객체 생성전 기존 객체 모두 제거

            Vector3 chatPos = new Vector3(0, 0.5f, -6f);

            Character cha_1p = Create_Character(Single.characterRoot, 0, chatPos);
            Single.touchProcess.Attach_SendObject(cha_1p.gameObject); //전역 터치 이벤트 대상에 등록 

            cha_1p.gameObject.AddComponent<ProtoGame.Movable>();
            ProtoGame.KeyInput key1p = cha_1p.gameObject.AddComponent<ProtoGame.KeyInput>();
            key1p.SelectPlayerNum(ProtoGame.KeyInput.ePlayerNum.Player_2);

            //========================================================================
            //========================================================================


            Character cha_2p = Create_Character(Single.characterRoot, 1, chatPos);
            cha_2p.gameObject.SetActive(false);

            cha_2p.gameObject.AddComponent<ProtoGame.Movable>();
            ProtoGame.KeyInput key2p = cha_2p.gameObject.AddComponent<ProtoGame.KeyInput>();
            key2p.SelectPlayerNum(ProtoGame.KeyInput.ePlayerNum.Player_1);

            AI ai_2p = cha_2p.gameObject.AddComponent<ProtoGame.AI>();
            ai_2p.enabled = false;

            //========================================================================
            //========================================================================




            //========================================================================
            //========================================================================

            chatPos.z = 0;
            for (int i = 0; i < 10; i++)
            {
                chatPos.x = (i * 1.5f) - 7f;
                Create_Chatterbox(Single.chatterboxRoot, i, chatPos);
            }
        }



    }

    //캐릭터 정보 객체 만들기 - 캐릭터 움직임 정보를 얻기 위해서 

    public class Character : MonoBehaviour
    {
        public int id
        { get; set; }

        private int _hp = 10;
        public bool _isLive = true;

        private AudioSource _audioSource = null;
        private TextMesh _text_hp = null;


        public Camera GetCamera()
        {
            return this.GetComponentInChildren<Camera>(true);
        }

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
                if (true == Judge_OffTheRing())
                {
                    Death();
                }
                if (0 == _hp)
                {
                    Death();
                }

            }


        }

        void OnCollisionEnter(Collision col)
        {

            //DebugWide.LogBlue("OnCollisionEnter:  " + col.gameObject.name + "   " + col.gameObject.tag);

            //캐릭터에만 영향을 받는다 
            if ("character" == col.gameObject.tag)
            {

            }


            if (true == _isLive)
            {
                //Speaking();

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


        //링에서 떨어짐 
        public bool Judge_OffTheRing()
        {
            const float FLOOR_HEIGHT = 0f;
            const float DELAY = 3f;
            if (FLOOR_HEIGHT - DELAY > this.transform.position.y)
                return true;

            return false;
        }

        //____________________________________________
        //                터치 이벤트 받기 
        //____________________________________________

        Vector2 __touchStart = Vector3.zero;
        private void TouchBegan() 
        {
            //DebugWide.LogGreen("TouchBegan " + this.name); 

            __touchStart = Single.touchProcess.GetTouchPos();

            //DebugWide.LogGreen(__touchStart);
        }

        Vector2 __touchDir = Vector3.zero;
        private void TouchMoved() 
        {
            //DebugWide.LogBlue("TouchMoved " + this.name); 
            Movable move = this.GetComponent<Movable>();
            __touchDir = __touchStart - Single.touchProcess.GetTouchPos();
            //__touchDir.Normalize();

            //회전방식 구상 
            //1안. 터치한 위치로 회전
            //2안. 터치드래그 위치로 회전

            //move.Rotation(__touchDir);
            //move.Up(0f);
            Ray ray = Camera.main.ScreenPointToRay(Single.touchProcess.GetTouchPos());
            RaycastHit hit3D;
            if (true == Physics.Raycast(ray, out hit3D, Mathf.Infinity))
            {

                transform.position = hit3D.point; //클릭위치를 정확히 구해야 한다 
                //switch (move.DirectionalInspection(hit3D.point))
                //{
                //    case Movable.eDirection.LEFT:
                //        {
                //            move.Left(0f);
                //            DebugWide.LogBlue("LEFT " + hit3D.point);
                //        }
                //        break;
                //    case Movable.eDirection.RIGHT:
                //        {
                //            move.Right(0f);
                //            DebugWide.LogBlue("RIGHT " + hit3D.point);
                //        }
                //        break;
                //    default:
                //        {
                //            move.Up(0f);
                //            DebugWide.LogRed("UP");
                //        }
                //        break;
                //}



            }


            //DebugWide.LogBlue(__touchDir); 
        }
        private void TouchEnded() 
        { 
            //DebugWide.LogRed("TouchEnded " + this.name); 
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
                if (true == Judge_OffTheRing())
                {
                    Death();
                }
                if (0 == _hp)
                {
                    Death();
                }

            }


        }

        void OnCollisionEnter(Collision col)
        {

            //DebugWide.LogBlue("OnCollisionEnter:  " + col.gameObject.name + "   " + col.gameObject.tag);

            //캐릭터에만 영향을 받는다 
            if ("character" == col.gameObject.tag)
            {
                //DebugWide.LogBlue("coll time : " + Time.fixedTime); //chamto test
                Judge_FirstAttacked();

                if (true == _isLive)
                {
                    //Speaking();
                    Speaking_JustOne();
                }
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

            if (null != _text_hp)
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
        //private Transform _mainBody = null;
        private Transform _target = null;
        private Movable _move = null;

        public enum eState
        {
            Detect, //탐지
            Chase,  //추격
            Attack,  //공격
            Escape, //도망
            Roaming, //배회하기
        }
        private eState _state = eState.Roaming;

        private void Start()
        {
            _move = this.GetComponent<Movable>();
        }

        //public void SetMainBody(Transform mainBody)
        //{
        //    _mainBody = mainBody;
        //    _move._mainBody = mainBody;
        //}

        private void FixedUpdate()
        {
            //if (null == _mainBody) return;

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
            switch (_state)
            {
                case eState.Detect:
                    {
                        //공격대상이 맞으면 추격한다.
                        if (true == Situation_Is_AttackTarget())
                        {
                            _state = eState.Chase;
                        }
                        //공격대상이 아니면 다시 배회한다.
                        else
                        {
                            _state = eState.Roaming;
                        }

                    }
                    break;

                case eState.Chase:
                    {
                        //공격사정거리까지 이동했으면 공격한다. 
                        if (true == Situation_Is_AttackRange())
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
                        if (false)
                        {
                            _state = eState.Detect;
                        }



                        _target = ProtoGame.Single.objectManager.GetNearCharacter(this.transform, 10f);
                        if(null != _target)
                        {
                            switch (_move.DirectionalInspection(_target.localPosition))
                            {
                                case Movable.eDirection.LEFT:
                                    {
                                        _move.Left(0f);
                                        //DebugWide.LogBlue("LEFT");
                                    }
                                    break;
                                case Movable.eDirection.RIGHT:
                                    {
                                        _move.Right(0f);
                                        //DebugWide.LogBlue("RIGHT");
                                    }
                                    break;
                            }

                            _move.Up(0f);   
                        }


                    }
                    break;
            }
        }

    }



}//end namespace


//========================================================
//================== 이동 처리 , 입력 처리 ==================
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

    public class Movable : MonoBehaviour
    {
        public enum eDirection
        {
            CENTER,
            LEFT,
            RIGHT,
        }

        //public Transform _mainBody = null;

        private CallInterval _Interval_Up = new CallInterval();
        private CallInterval _Interval_Left = new CallInterval();
        private CallInterval _Interval_Right = new CallInterval();

        private bool _canNot_Move = false;
        public bool canNot_Move
        {
            get
            {
                return _canNot_Move;
            }
            set
            {
                _canNot_Move = value;
            }
        }


        //private void Start()
        //{

        //}

        public void PostureRecovery()
        {
            //chamto test
            //DebugWide.LogBlue(_mainBody.transform.localEulerAngles); //chamto test

            Vector3 angles = this.transform.localEulerAngles;
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

            this.GetComponent<Rigidbody>().AddForce(Vector3.down * 4f * Time.deltaTime, ForceMode.Force);


        }

        public void Up(float MAX_SECOND)
        {

            if (true == _canNot_Move) return;

            MAX_SECOND = 0.5f; //test value

            //0.2거리 가는데 0.5초 걸림

            //연속호출 상황시 시간증가값 구하기
            float accumulate = _Interval_Up.GetTimeAccumulate(MAX_SECOND);


            //보간, 이동 처리
            float delta = Interpolation.easeInOutBack(0f, 0.2f, accumulate / MAX_SECOND);
            this.transform.Translate(Vector3.forward * delta);

            //=============s
            //chamto test
            Vector3 dir = this.GetForwardDirect();
            dir.Normalize();
            //this.GetComponent<Rigidbody>().AddForce(dir * 50f * delta, ForceMode.VelocityChange);


            //chamto test
            //_mainBody.GetComponent<Rigidbody>().AddExplosionForce(300f, _mainBody.transform.position, 20f, 10f);

            //chamto test2
            //_mainBody.GetComponent<Rigidbody>().MovePosition(_mainBody.position + (dir * 3f));
        }

        public void Down(float MAX_SECOND)
        {
            if (true == _canNot_Move) return;

            MAX_SECOND = 4f; //test value


            this.transform.Translate(Vector3.back * Time.deltaTime * MAX_SECOND); //one second per 4 move
        }


        public void Left(float MAX_SECOND)
        {
            if (true == _canNot_Move) return;

            MAX_SECOND = 0.2f; //test value

            float angle = 5f; //5도 회전하는데 0.2초 걸림 

            //연속호출 상황시 시간증가값 구하기
            float accumulate = _Interval_Up.GetTimeAccumulate(MAX_SECOND);


            //보간, 이동 처리
            float delta = Interpolation.easeOutCirc(0f, angle, accumulate / MAX_SECOND);
            //_mainBody.Rotate(Vector3.up, -1 * delta);


            //=============
            //물리 처리로 변경 
            this.GetComponent<Rigidbody>().AddRelativeTorque(Vector3.down * 1f * delta, ForceMode.VelocityChange);
        }


        public void Right(float MAX_SECOND)
        {
            if (true == _canNot_Move) return;

            MAX_SECOND = 0.2f; //test value

            float angle = 5f;

            //연속호출 상황시 시간증가값 구하기
            float accumulate = _Interval_Up.GetTimeAccumulate(MAX_SECOND);


            //보간, 이동 처리
            float delta = Interpolation.easeOutCirc(0f, angle, accumulate / MAX_SECOND);
            //_mainBody.Rotate(Vector3.up, 1 * delta);


            //=============
            //물리 처리로 변경 
            this.GetComponent<Rigidbody>().AddRelativeTorque(Vector3.up * 1f * delta, ForceMode.VelocityChange);

        }

        public void Rotation(Vector3 targetDir)
        {
            if (true == _canNot_Move) return;

            // 목표 방향에 도달했는지 검사 
            float dot = Vector3.Dot(targetDir, this.GetForwardDirect());
            dot = Mathf.Abs(dot);
            if (dot < 0.1f) return; 


            float crossDir = Vector3.SignedAngle(GetForwardDirect(), targetDir, Vector3.up);
            if (crossDir > 0)
                crossDir = 1f;
            else
                crossDir = -1f;

            //5도 회전하는데 0.2초 걸림 
            float ANGLE = 5f * crossDir;
            float MAX_SECOND = 0.2f; 

            //연속호출 상황시 시간증가값 구하기
            float accumulate = _Interval_Up.GetTimeAccumulate(MAX_SECOND);


            //보간, 이동 처리
            float delta = Interpolation.easeOutCirc(0f, ANGLE, accumulate / MAX_SECOND);
            //_mainBody.Rotate(Vector3.up, -1 * delta);


            //=============
            //물리 처리로 변경 
            this.GetComponent<Rigidbody>().AddRelativeTorque(Vector3.down * 1f * delta, ForceMode.VelocityChange);
        
        }


        //객체의 전진 방향을 반환한다.
        public Vector3 GetForwardDirect()
        {
            return Quaternion.Euler(this.transform.localEulerAngles) * Vector3.forward;
        }


        //내방향을 기준으로 목표위치가 어디쪽에 있는지 반환한다.  
        public eDirection DirectionalInspection(Vector3 targetPos)
        {

            Vector3 mainDir = GetForwardDirect();

            Vector3 targetTo = targetPos - this.transform.localPosition;

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


        //회전할 각도 구하기
        public float CalcRotationAngle(Vector3 targetDir)
        {
            //atan2로 각도 구하는 것과 같음. -180 ~ 180 사이의 값을 반환
            return Vector3.SignedAngle(GetForwardDirect(), targetDir, Vector3.up);
            
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


        private string[,] _keys = { { "w", "s", "a", "d" }, { "up", "down", "left", "right" } };


        private Movable _move = null;

        private void Start()
        {
            _move = GetComponent<Movable>();
        }


        public void SelectPlayerNum(ePlayerNum playerNum)
        {
            _playerNum = playerNum;
        }

        //public void SetMainBody(Transform mainBody)
        //{
        //    _move._mainBody = mainBody;
        //}

        //이동 가능 또는 불가능 설정한다 
        //public void SetMovable(bool move)
        //{
        //    _move.canNot_Move = move;
        //}


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
            if (Input.GetKeyUp(_keys[(int)_playerNum, (int)eKeyName.UP]))
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

//가속도계 참고할것  :  https://docs.unity3d.com/kr/530/Manual/MobileInput.html
//마우스 시뮬레이션  :  https://docs.unity3d.com/kr/530/ScriptReference/Input.html   마우스 => 터치로 변환
//========================================================
//==================      터치  처리      ==================
//========================================================
namespace ProtoGame
{

    public class TouchProcess : MonoBehaviour
    {
        
        private GameObject  _TouchedObject = null;
        private List<GameObject> _sendList = new List<GameObject>();

        private Vector2 _prevTouchMovedPos = Vector3.zero;
        public Vector2 prevTouchMovedPos
        {
            get
            {
                return _prevTouchMovedPos;
            }
        }



        void Awake()
        {

            Input.simulateMouseWithTouches = false;
            Input.multiTouchEnabled = false;

        }

        // Use this for initialization
        void Start()
        {

           
        }


        //void Update()
        void FixedUpdate()
        {
            //화면상 ui를 터치했을 경우 터치이벤트를 보내지 않는다 
            if (null != EventSystem.current && null != EventSystem.current.currentSelectedGameObject)
            {
                return;
            }

            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                SendTouchEvent_Device_Target();
                SendTouchEvent_Device_NonTarget();
            }
            else if (Application.platform == RuntimePlatform.OSXEditor)
            {
                SendMouseEvent_Editor_Target();
                SendMouseEvent_Editor_NonTarget();
            }
        }

        //==========================================
        //                 보조  함수
        //==========================================

        public void Attach_SendObject(GameObject obj)
        {
            _sendList.Add(obj);
        }

        public void Detach_SendObject(GameObject obj)
        {
            _sendList.Remove(obj);
        }

        public void DetachAll()
        {
            _sendList.Clear();
        }


        public  Vector2 GetTouchPos()
        {
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                return Input.GetTouch(0).position;
            }
            else if (Application.platform == RuntimePlatform.OSXEditor)
            {
                return Input.mousePosition;
            }

            return Vector2.zero;
        }

        public  bool GetMouseButtonMove(int button)
        {
            if (Input.GetMouseButton(button) && Input.GetMouseButtonDown(button) == false)
            {
                return true;
            }

            return false;
        }

        //==========================================
        //                 이벤트  함수
        //==========================================

        private void SendTouchEvent_Device_NonTarget()
        {
            foreach(GameObject o in _sendList)
            {
                if (Input.touchCount > 0)
                {
                    if (Input.GetTouch(0).phase == TouchPhase.Began)
                    {

                        o.SendMessage("TouchBegan", 0, SendMessageOptions.DontRequireReceiver);

                    }
                    else if (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(0).phase == TouchPhase.Stationary)
                    {

                        o.SendMessage("TouchMoved", 0, SendMessageOptions.DontRequireReceiver);

                    }
                    else if (Input.GetTouch(0).phase == TouchPhase.Ended)
                    {

                        o.SendMessage("TouchEnded", 0, SendMessageOptions.DontRequireReceiver);

                    }
                    else
                    {
                        DebugWide.LogError("Update : Exception Input Event : " + Input.GetTouch(0).phase);
                    }
                }
            }

        }

        private bool __touchBegan = false;
        private void SendMouseEvent_Editor_NonTarget()
        {
            foreach (GameObject o in _sendList)
            {
                //=================================
                //    mouse Down
                //=================================
                if (Input.GetMouseButtonDown(0))
                {
                    //DebugWide.LogBlue("SendMouseEvent_Editor_NonTarget : TouchPhase.Began"); //chamto test
                    if (false == __touchBegan)
                    {
                        o.SendMessage("TouchBegan", 0, SendMessageOptions.DontRequireReceiver);

                        __touchBegan = true;

                    }
                }

                //=================================
                //    mouse Up
                //=================================
                if (Input.GetMouseButtonUp(0))
                {

                    if (true == __touchBegan)
                    {
                        __touchBegan = false;

                        o.SendMessage("TouchEnded", 0, SendMessageOptions.DontRequireReceiver);
                    }

                }


                //=================================
                //    mouse Move
                //=================================
                if (Input_Unity.GetMouseButtonMove(0))
                {

                    //=================================
                    //     mouse Drag 
                    //=================================
                    if (true == __touchBegan)
                    {

                        o.SendMessage("TouchMoved", 0, SendMessageOptions.DontRequireReceiver);

                    }//if
                }//if
            }

        }

        private void SendTouchEvent_Device_Target()
        {
            if (Input.touchCount > 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    //DebugWide.LogError("Update : TouchPhase.Began"); //chamto test
                    _prevTouchMovedPos = this.GetTouchPos();
                    _TouchedObject = SendMessage_TouchObject("TouchBegan_Target", Input.GetTouch(0).position);
                }
                else if (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(0).phase == TouchPhase.Stationary)
                {
                    //DebugWide.LogError("Update : TouchPhase.Moved"); //chamto test

                    if (null != _TouchedObject)
                        _TouchedObject.SendMessage("TouchMoved_Target", 0, SendMessageOptions.DontRequireReceiver);

                    _prevTouchMovedPos = this.GetTouchPos();

                }
                else if (Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    //DebugWide.LogError("Update : TouchPhase.Ended"); //chamto test
                    if (null != _TouchedObject)
                        _TouchedObject.SendMessage("TouchEnded_Target", 0, SendMessageOptions.DontRequireReceiver);
                    _TouchedObject = null;
                }
                else
                {
                    DebugWide.LogError("Update : Exception Input Event : " + Input.GetTouch(0).phase);
                }
            }
        }


        private bool f_isEditorDraging = false;
        private void SendMouseEvent_Editor_Target()
        {

            //=================================
            //    mouse Down
            //=================================
            //Debug.Log("mousedown:" +Input.GetMouseButtonDown(0)+ "  mouseup:" + Input.GetMouseButtonUp(0) + " state:" +Input.GetMouseButton(0)); //chamto test
            if (Input.GetMouseButtonDown(0))
            {
                //Debug.Log ("______________ MouseBottonDown ______________" + m_TouchedObject); //chamto test
                if (false == f_isEditorDraging)
                {

                    _TouchedObject = SendMessage_TouchObject("TouchBegan_Target", Input.mousePosition);
                    if (null != _TouchedObject)
                        f_isEditorDraging = true;
                }

            }

            //=================================
            //    mouse Up
            //=================================
            if (Input.GetMouseButtonUp(0))
            {

                //Debug.Log ("______________ MouseButtonUp ______________" + m_TouchedObject); //chamto test
                f_isEditorDraging = false;

                if (null != _TouchedObject)
                {
                    _TouchedObject.SendMessage("TouchEnded_Target", 0, SendMessageOptions.DontRequireReceiver);
                }

                _TouchedObject = null;

            }


            //=================================
            //    mouse Move
            //=================================
            if (Input_Unity.GetMouseButtonMove(0))
            {

                //=================================
                //     mouse Drag 
                //=================================
                if (f_isEditorDraging)
                {
                    //Debug.Log ("______________ MouseMoved ______________" + m_TouchedObject); //chamto test

                    if (null != _TouchedObject)
                        _TouchedObject.SendMessage("TouchMoved_Target", 0, SendMessageOptions.DontRequireReceiver);


                }//if
            }//if
        }



        private GameObject SendMessage_TouchObject(string callbackMethod, Vector3 touchPos)
        {
           
            Ray ray = Camera.main.ScreenPointToRay(touchPos);

            //Debug.Log ("  -- currentSelectedGameObject : " + EventSystem.current.currentSelectedGameObject); //chamto test


            //2. game input event test
            RaycastHit2D hit2D = Physics2D.Raycast(ray.origin, ray.direction);
            if (hit2D)
            {
                //DebugWide.Log(hit.transform.gameObject.name); //chamto test
                hit2D.transform.gameObject.SendMessage(callbackMethod, 0, SendMessageOptions.DontRequireReceiver);

                return hit2D.transform.gameObject;
            }

            RaycastHit hit3D;
            if (true == Physics.Raycast(ray, out hit3D, Mathf.Infinity))
            {
                hit3D.transform.gameObject.SendMessage(callbackMethod, 0, SendMessageOptions.DontRequireReceiver);

                return hit3D.transform.gameObject;
            }


            return null;
        }

        //콜백함수 원형 : 지정 객체 아래로 터치이벤트를 보낸다 
        private void TouchBegan() { }
        private void TouchMoved() { }
        private void TouchEnded() { }

        //콜백함수 원형 : 지정 객체에 터치이벤트를 보낸다 
        private void TouchBegan_Target() { }
        private void TouchMoved_Target() { }
        private void TouchEnded_Target() { }


    }
    
}//end namespace