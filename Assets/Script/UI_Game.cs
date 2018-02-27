using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CounterBlock;

public class UI_Game : UI_MonoBase 
{

	private CharacterManager _crtMgr = null;
	private UI_Battle _ui_battle = null;
	private GameMode_Couple _game_couple = null;

	// Use this for initialization
	void Start () 
	{
		//=================================================
		//                    초 기 화 
		//=================================================
		base.Init_UI ();

		StartCoroutine (GlobalFunctions.FadeIn (_panelRoot , 1.0f));

		FrameControl.SetDeltaTime_30FPS (); //30Fps 기준으로 처리가 늦어진 프레임의 경과시간을 재설정한다.  30Fps 고정프레임으로 사용하겠다는 것이 아님.


		Single.resource.Init ();

		_crtMgr = new CharacterManager ();
		//_crtMgr.Init (CHARACTER_COUNT);

		this.gameObject.AddComponent<MonoInputManager> ();
		_ui_battle = this.gameObject.AddComponent<UI_Battle> ();
		//_ui_battle.Init ();

		//this.CreatePlayer ();

		//=================================================
		_game_couple = this.gameObject.AddComponent<GameMode_Couple>();

		//=================================================
	}

	// Update is called once per frame
	void Update () 
	{
		Single.Update ();

		//갱신순서 중요!!!! , start 상태는 1Frame 뒤 변경되는데, 갱신순서에 따라 ui에서 탐지 못할 수 있다. fixme:콜백함수로 처리해야함  
		//===================
		//1. key input
		//2. UI update
		//3. data update

		//===== key input
		this.Update_Input();

		//===== UI update
		_ui_battle.Update_UI ();

		//===== data update
		_crtMgr.Update (); 


	}//end Update

	public void CreatePlayer()
	{

		int count = 0;
		foreach (Character chter in _crtMgr.Values) 
		{
			UI_CharacterCard card = _ui_battle.AddCharacter (chter);

			if ((chter.GetID () % 2) == 1) 
			{ //홀수는 왼쪽 1 3 5 ...
				//DebugWide.LogBlue(-10f * count + " left " + count); //chamto test
				_ui_battle.SetStartPoint (chter.GetID (), -1f * count, UI_Battle.START_POINT_LEFT);	
				//card.data.kind = Character.eKind.Biking;
				card.SetKind(Character.eKind.Biking);

			}
			if ((chter.GetID () % 2) == 0) 
			{ //짝수는 오른쪽 2 4 6 ... 
				//DebugWide.LogBlue(10f * count + " right " + count); //chamto test
				_ui_battle.SetStartPoint (chter.GetID (), 1f * (count-1), UI_Battle.START_POINT_RIGHT);
				//card.data.kind = Character.eKind.Seonbi;
				card.SetKind(Character.eKind.Seonbi);

			}

			count++;
		}
	}


	public void Update_Input()
	{
		
	}

}

public class GameMode_Couple : MonoBehaviour
{
	private Transform _loading = null;
	private Vector3 _loading_angle = Vector3.zero;
	private Transform _speaker_shout = null;
	private List<Transform> _pineCones = new List<Transform>();

	private List<int> _randomTable = new List<int> ();

	const int MAX_PAINCONES = 8;


	void Start()
	{

		_loading = GameObject.Find ("loading_serpent").GetComponent<Transform> ();
		_loading_angle = _loading.eulerAngles;
		_speaker_shout = GameObject.Find ("sp_shout").GetComponent<Transform> ();
		_speaker_shout.gameObject.SetActive (false);

		this.RandomTableSetting ();

		CharDataBundle bundle;
		bundle._data = null;
		bundle._ui = null;


		for (int i = 0; i < MAX_PAINCONES ; i++) 
		{
			Transform tr = GameObject.Find ("pinecone_" + i).GetComponent<Transform> ();
			PineCone_Card card = tr.gameObject.AddComponent<PineCone_Card> ();
			card._idx = i;
			card._coupleNumber = _randomTable [i];
			_pineCones.Add(tr);

			bundle._gameObject = _pineCones [i].gameObject;
			StartCoroutine("Rolling",bundle); 
		}

		bundle._gameObject = _speaker_shout.gameObject;
		StartCoroutine("Shout",bundle); 

	}

	public void RandomTableSetting()
	{
		List<XML_Data.DictInfo.VocaInfo> seq = Single.resource.GetDictEng()._dictInfoMap[100].GetSequence(XML_Data.DictInfo.eKind.Sentence); //100 임시 처리

		int rnd_num = -1;
		rnd_num = Single.rand.Next (0, seq.Count - 1 - 3);
		_randomTable.Add (rnd_num);
		rnd_num++;
		_randomTable.Add (rnd_num);
		rnd_num++;
		_randomTable.Add (rnd_num);
		rnd_num++;
		_randomTable.Add (rnd_num);

		_randomTable.Add (_randomTable[0]);
		_randomTable.Add (_randomTable[1]);
		_randomTable.Add (_randomTable[2]);
		_randomTable.Add (_randomTable[3]);

		//섞기
		for (int i = 0; i < 20; i++) 
		{
			int src = Single.rand.Next (0, MAX_PAINCONES-1);
			int dst = Single.rand.Next (0, MAX_PAINCONES-1);
			Swap_RandomTable (src, dst);
		}

		//test print
		string temp = "";
		foreach (int num in _randomTable) 
		{
			temp += num + "  ";
		}
		DebugWide.LogBlue (temp);	

	}

	public void Swap_RandomTable(int src, int dst)
	{
		int temp = 0;
		temp = _randomTable [src];
		_randomTable [src] = _randomTable [dst];
		_randomTable [dst] = temp;
	}

	void Update()
	{
		_loading_angle.z += 1f;
		_loading.localEulerAngles = _loading_angle;
		//_pineCones [0].eulerAngles = new Vector3 (0,0,45f);
	}

	public IEnumerator Shout(CharDataBundle bundle)
	{

		float time = 1.5f;
		bundle._gameObject.SetActive (true);
		iTween.Stop (bundle._gameObject);

		iTween.PunchScale(bundle._gameObject,iTween.Hash("amount",new Vector3(1.5f,1.5f,1f),"loopType","loop","time",time));
		//iTween.ShakeScale(bundle._gameObject,new Vector3(0.5f,0.5f,0.5f), time);

		yield return new WaitForSeconds(time);

		iTween.Stop (bundle._gameObject);
		bundle._gameObject.SetActive (false);

		yield return new WaitForSeconds(Single.rand.Next (4, 10));
		StartCoroutine("Shout",bundle); 
	}

	public IEnumerator Rolling(CharDataBundle bundle)
	{

		float time = 10f;
		bundle._gameObject.SetActive (true);
		iTween.Stop (bundle._gameObject);

		int rand = Single.rand.Next (100, 500);

		//iTween.PunchPosition(charUI._action[2].gameObject, iTween.Hash("x",100,"y",100,"time",0.5f));
		//iTween.PunchPosition(charUI._action[2].gameObject, iTween.Hash("x",50,"loopType","loop","time",0.5f));
		//iTween.PunchRotation(bundle._gameObject,new Vector3(0,0,300f),time);
		iTween.PunchRotation(bundle._gameObject,iTween.Hash("z",rand,"loopType","loop","time",time));
		//iTween.PunchPosition(bundle._gameObject, iTween.Hash("z",-200f,"time",time));	
		//			iTween.MoveBy(bundle._gameObject, iTween.Hash(
		//				"amount", new Vector3(0,0,30f),
		//				"time", time, "easetype",  "easeInOutBounce"//"linear"
		//			));
		//			iTween.RotateBy(bundle._gameObject, iTween.Hash(
		//				"amount", new Vector3(0,30f,0),
		//				"time", time, "easetype",  "easeInOutBounce"//"linear"
		//			));
		//iTween.ShakeScale(bundle._gameObject,new Vector3(0.5f,0.5f,0.5f), time);

		yield return new WaitForSeconds(time);

		iTween.Stop (bundle._gameObject);
		StartCoroutine("Rolling",bundle); 
	}

}

public class PineCone_Card : MonoBehaviour
{
	public int _idx = -1;

	public AudioSource _audioSource { get; set; }
	public int _voiceSequence = 0;

	public int _coupleNumber = 0;

	void Start()
	{
		_audioSource = gameObject.GetComponent<AudioSource>();
	}

	void TouchBegan() 
	{
		DebugWide.LogBlue (gameObject); //chamto test

		//=================================================
		AudioClips clips = null;
		if (true) {
			clips = Single.resource.GetVoiceClipMap ().GetClips (VoiceInfo.eKind.Eng_NaverMan_1);
		} else 
		{
			clips = Single.resource.GetVoiceClipMap ().GetClips (VoiceInfo.eKind.Eng_NaverWoman_2);
		}
		//_audioSource.Play (); //chamto test
		List<XML_Data.DictInfo.VocaInfo> seq = Single.resource.GetDictEng()._dictInfoMap[100].GetSequence(XML_Data.DictInfo.eKind.Sentence); //100 임시 처리
		//List<XML_Data.DictInfo.VocaInfo> seq = Single.resource.GetDictEng()._dictInfoMap[100].GetSequence(6); //100 , 9 임시 처리
		_audioSource.Stop ();
		_audioSource.PlayOneShot(clips[seq[_coupleNumber].hashKey]);

		//=================================================

		CharDataBundle bundle;
		bundle._data = null;
		bundle._ui = null;
		bundle._gameObject = gameObject;
		StartCoroutine("Scale_Up",bundle); 
	}
	void TouchMoved() {}
	void TouchEnded() {}


	public IEnumerator Scale_Up(CharDataBundle bundle)
	{

		float time = 2f;
		bundle._gameObject.SetActive (true);
		iTween.Stop (bundle._gameObject);

		iTween.ScaleTo (bundle._gameObject, new Vector3 (2f,2f,1f), time);


		yield return new WaitForSeconds(time);

		iTween.Stop (bundle._gameObject);
	}

	public IEnumerator Scale_Down(CharDataBundle bundle)
	{

		float time = 2f;
		bundle._gameObject.SetActive (true);
		iTween.Stop (bundle._gameObject);

		iTween.ScaleTo (bundle._gameObject, new Vector3 (1f,1f,1f), time);


		yield return new WaitForSeconds(time);

		iTween.Stop (bundle._gameObject);
	}
}