using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CounterBlock;

public class GameMode_Couple : MonoBehaviour
{
	public Transform _loading = null;
	private Vector3 _loading_angle = Vector3.zero;
	private Transform _speaker_shout = null;
	private Button _btnScore = null;

	private List<PineCone_Card> _pineCones = new List<PineCone_Card>();
	private List<int> _randomTable = new List<int> ();

	const int MAX_PAINCONES = 8;
	const int MAX_SELECT_COUNT = 2;


	void Start()
	{

		_loading = GameObject.Find ("loading_serpent").GetComponent<Transform> ();
		_loading_angle = _loading.eulerAngles;
		_speaker_shout = GameObject.Find ("sp_shout").GetComponent<Transform> ();
		_speaker_shout.gameObject.SetActive (false);
		_btnScore = GameObject.Find ("Score").GetComponent<Button>();
		_btnScore.gameObject.SetActive (false);

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
			card._GameMode_Couple = this;
			_pineCones.Add(card);

			bundle._gameObject = _pineCones [i].gameObject;
			StartCoroutine("Rolling",bundle); 
		}

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

	public void DeSelectAll()
	{

		// 
		int selectCount = 0;
		for(int i=0;i<_pineCones.Count;i++)
		{
			if (_pineCones[i]._isSelect)
			{
				selectCount++;
			}
		}


		if (MAX_SELECT_COUNT <= selectCount) 
		{
			for (int i = 0; i < _pineCones.Count; i++) 
			{
				_pineCones [i].DeSelect ();
			}
		}

	}

	public void SoundStopAll()
	{
		for(int i=0;i<_pineCones.Count;i++)
		{
			_pineCones [i]._audioSource.Stop ();
		}	
	}

	public void Discrimination()
	{
		//짝판별
		List<int> selected = new List<int> ();
		for(int i=0;i<_pineCones.Count;i++)
		{
			if (_pineCones[i]._isSelect)
			{
				selected.Add (i);
			}
		}

		if (MAX_SELECT_COUNT == selected.Count) 
		{

			if(_pineCones[selected[0]]._coupleNumber == _pineCones[selected[1]]._coupleNumber)
			{
				CharDataBundle bundle;
				bundle._data = null;
				bundle._ui = null;
				bundle._gameObject = _speaker_shout.gameObject;
				StartCoroutine("Shout",bundle); 

				_pineCones [selected [0]].End ();
				_pineCones [selected [1]].End ();

				StartCoroutine ("EndProcess", 2f);
			}
		}

		int endCount = 0;
		for(int i=0;i<_pineCones.Count;i++)
		{
			if (true == _pineCones[i]._isEnd)
			{
				endCount++;
			}
		}
		if (MAX_PAINCONES == endCount) 
		{
			//게임 완료 
			_btnScore.gameObject.SetActive(true);
		}
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

		//yield return new WaitForSeconds(Single.rand.Next (4, 10));
		//StartCoroutine("Shout",bundle); 
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

	public IEnumerator EndProcess(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);

		//뱀고리에 넣는다 - 임시처리
		for (int i = 0; i < _pineCones.Count; i++) 
		{
			if (true == _pineCones [i]._isEnd) 
			{
				_pineCones [i].transform.position = _loading.position;
				_pineCones [i].transform.localScale = Vector3.one;
			}
		}
	}

}

//솔방울카드
public class PineCone_Card : MonoBehaviour
{
	public int _idx = -1;

	public AudioSource _audioSource { get; set; }
	public int _voiceSequence = 0;

	public int _coupleNumber = 0;

	public GameMode_Couple _GameMode_Couple = null;

	public bool _isSelect = false;
	public bool _isEnd = false;


	void Start()
	{
		_audioSource = gameObject.GetComponent<AudioSource>();
	}

	public void DeSelect()
	{
		if (true == _isSelect) 
		{
			CharDataBundle bundle;
			bundle._data = null;
			bundle._ui = null;
			bundle._gameObject = gameObject;
			StartCoroutine("Scale_Down",bundle); 
		}
		_isSelect = false;

	}

	public void End()
	{
		if (false == _isEnd && true == _isSelect) 
		{
			_isEnd = true;

			CharDataBundle bundle;
			bundle._data = null;
			bundle._ui = null;
			bundle._gameObject = gameObject;
			StartCoroutine("MoveTo",bundle);
		}


	}

	void TouchBegan() 
	{
		if (true == _isEnd)
			return;

		//DebugWide.LogBlue (gameObject); //chamto test
		_GameMode_Couple.SoundStopAll();
		_GameMode_Couple.DeSelectAll ();

		//=================================================
		AudioClips clips = null;
		if (_isSelect) {
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

		if(false == _isSelect)
			StartCoroutine("Scale_Up",bundle); 
		else 
			StartCoroutine("Scale_Down",bundle); 


		_isSelect = !_isSelect;
		//======

		//판단 
		_GameMode_Couple.Discrimination();

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

	public IEnumerator MoveTo(CharDataBundle bundle)
	{

		float time = 2f;
		bundle._gameObject.SetActive (true);
		iTween.Stop (bundle._gameObject);

		//iTween.MoveTo(bundle._gameObject, _GameMode_Couple._loading.position, time);
		iTween.ShakeScale(bundle._gameObject, new Vector3(0.2f,0.2f,1f), time);


		yield return new WaitForSeconds(time);

		iTween.Stop (bundle._gameObject);
	}

}