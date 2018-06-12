using System;
using System.Net;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CounterBlock;

//아마존 폴리 TTS link
//https://us-east-2.console.aws.amazon.com/polly/home/SynthesizeSpeech

//naver TTS 
//계정 : https://developers.naver.com/apps/#/myapps/2Lpz1sFLrJhFy1tY9SFR/overview
//문서 : https://developers.naver.com/docs/clova/api/#/CSS/API_Guide.md#RequestParameter
public class NaverTTS
{
	
	public const int MAX_ASC_SPEED = 10;
	public const int BASIC_ASC_SPEED = 5;
	public const int MIN_ASC_SPEED = 0;

	public const string PATH_Voice = "Assets/Resources/Sound/Voice/";
	public const string PATH_StreamingAssets = "Assets/StreamingAssets/";

	public enum eLanguage
	{
		Korean,
		English,
		Chinese,
		Spanish,
		Japanese,
	}

	public enum eSex
	{
		None = 0,
		Man = 1,
		Woman = 2,
	}

	private int 	_speed = 0; //-5이면 0.5배 빠른 속도이고 5이면 0.5배 느린 속도입니다
	private string 	_speaker = "jinho";
	private string  _path = PATH_StreamingAssets;
	private string _fileName = "tts.mp3";

	//543210-1-2-3-4-5
	private int ToNaverSpeed(int ascendingSpeed)
	{
		int temp = ascendingSpeed;
		temp = -temp;
		return temp + 5;
	}

	//012345678910
	private int toAscendingSpeed(int naverSpeed)
	{
		int temp = naverSpeed;
		temp = temp - 5;
		return Mathf.Abs (temp);

	}

	public int GetSpeed()
	{
		return toAscendingSpeed (_speed);
	}

	public void SetSpeed(int ascendingSpeed)
	{
		int naverSpeed = ToNaverSpeed (ascendingSpeed);
		
		if (-5 > naverSpeed)
			naverSpeed = -5;

		if (5 < naverSpeed)
			naverSpeed = 5;

		_speed = naverSpeed;
	}
		
	public void SetPath(string path)
	{
		_path = path;
	}

	public void SetFileName(string fileName)
	{
		_fileName = fileName + ".mp3";
	}

	public void SetSpeaker(eLanguage lang , eSex sex)
	{
		//	mijin : 한국어, 여성 음색
		//	jinho : 한국어, 남성 음색
		//	clara : 영어, 여성 음색
		//	matt : 영어, 남성 음색
		//	yuri : 일본어, 여성 음색
		//	shinji : 일본어, 남성 음색
		//	meimei : 중국어, 여성 음색
		//	liangliang : 중국어, 남성 음색
		//	jose : 스페인어, 남성 음색
		//	carmen : 스페인어, 여성 음색

		string temp = "";
		switch (lang) 
		{
		case eLanguage.Korean:
			if (sex == eSex.Man)
				temp = "jinho";
			if (sex == eSex.Woman)
				temp = "mijin";
			break;
		case eLanguage.English:
			if (sex == eSex.Man)
				temp = "matt";
			if (sex == eSex.Woman)
				temp = "clara";
			break;
		case eLanguage.Chinese:
			if (sex == eSex.Man)
				temp = "liangliang";
			if (sex == eSex.Woman)
				temp = "meimei";
			break;
		case eLanguage.Spanish:
			if (sex == eSex.Man)
				temp = "jose";
			if (sex == eSex.Woman)
				temp = "carmen";
			break;
		case eLanguage.Japanese:
			if (sex == eSex.Man)
				temp = "shinji";
			if (sex == eSex.Woman)
				temp = "yuri";
			break;
		}

		_speaker = temp;
	}

	public void Request(string text)
	{
		//ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
		//System.Diagnostics.Process.Start("mozroots","--import --quiet");

		//string text = "좋은 하루 되세요."; // 음성합성할 문자값
		string url = "https://openapi.naver.com/v1/voice/tts.bin";
		HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
		request.Headers.Add("X-Naver-Client-Id", "2Lpz1sFLrJhFy1tY9SFR");
		request.Headers.Add("X-Naver-Client-Secret", "epWAWPAWNX");
		request.Method = "POST";

		byte[] byteDataParams = Encoding.UTF8.GetBytes("speaker="+_speaker+"&speed="+_speed+"&text=" + text);
		request.ContentType = "application/x-www-form-urlencoded";
		request.ContentLength = byteDataParams.Length;
		Stream st = request.GetRequestStream();
		st.Write(byteDataParams, 0, byteDataParams.Length);
		st.Close();
		HttpWebResponse response = (HttpWebResponse)request.GetResponse();

		string status = response.StatusCode.ToString();
		//DebugWide.LogBlue ("status="+ status);

		if(true == File.Exists(_path + _fileName))
			File.Delete (_path + _fileName);

		if (false == Directory.Exists (_path))
			Directory.CreateDirectory (_path);
		
		using (Stream output = File.OpenWrite (_path + _fileName))
		{
			using (Stream input = response.GetResponseStream ()) 
			{
			
				//input.CopyTo(output); //.Net4.0 이후
				this.CopyTo(input, output); //.Net3.5 용

			}
		}

		DebugWide.LogBlue(_path + _fileName +" was created");

	}

	//ref : https://stackoverflow.com/questions/5730863/how-to-use-stream-copyto-on-net-framework-3-5
	public void CopyTo(Stream input, Stream output)
	{
		byte[] buffer = new byte[16 * 1024]; // Fairly arbitrary size
		int bytesRead;

		while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
		{
			output.Write(buffer, 0, bytesRead);
		}
	}

	public bool FileExists()
	{
		return File.Exists (_path + _fileName);
	}
}




public class NaverTTSBehaviour : MonoBehaviour 
{
	NaverTTS _tts = new NaverTTS();
	XML_Data.Dict_English _dictEng = new XML_Data.Dict_English();

	// Use this for initialization
	void Start () 
	{

		CounterBlock.Single.coroutine.Start_Sync (_dictEng.LoadXML (),null,"DICT_ENGLISH");

        CounterBlock.Single.coroutine.Start_Async (Request_NaverTTS());

        //Test_NaverTTS (); //chamto test

        //Test_Pitch_NaverTTS(); //chamto test
	}

	// Update is called once per frame
	void Update () 
	{
		CounterBlock.Single.Update ();		
	}


	public NaverTTS.eSex ToNaverTTS(NaverTTS.eLanguage lang ,VoiceInfo.eKind kind)
	{
		if (NaverTTS.eLanguage.English == lang) 
		{
			switch (kind) 
			{
			case VoiceInfo.eKind.Eng_NaverMan_1:
				return NaverTTS.eSex.Man;
			case VoiceInfo.eKind.Eng_NaverWoman_2:
				return NaverTTS.eSex.Woman;
			}
		}

		return NaverTTS.eSex.None;
	}
	
	IEnumerator Request_NaverTTS()
	{
		string tempStr = "";
		int first = 0;
		int second = 0;
		VoiceInfo.eKind vKind;
		NaverTTS.eLanguage nLang = NaverTTS.eLanguage.English;
		foreach (XML_Data.DictInfo info in _dictEng._dictInfoMap.Values) 
		{
			first++;
			second = 0;
			foreach (int hash in info.GetData().Keys) 
			{
				second++;
				tempStr = CounterBlock.Single.hashString.GetString_ForAssetFile (hash);
				//DebugWide.LogBlue (tempStr);

				//파일저장양식 
				//(0)number _ (1)hash value _ (2)목소리종류 _ (3)말하기속도 _(4)말하기텍스트 

				vKind = VoiceInfo.eKind.Eng_NaverMan_1;
				_tts.SetSpeaker (nLang, ToNaverTTS(nLang, vKind));
				_tts.SetSpeed (NaverTTS.MIN_ASC_SPEED);
				_tts.SetPath (NaverTTS.PATH_Voice);
				_tts.SetFileName (second.ToString("0000")+"_"+hash+"_"+ (int)vKind +"_"+_tts.GetSpeed()+"_"+tempStr);
				if (false == _tts.FileExists()) 
				{
					_tts.Request (tempStr);
					yield return null;
				}

				vKind = VoiceInfo.eKind.Eng_NaverWoman_2;
				_tts.SetSpeaker (nLang, ToNaverTTS(nLang, vKind));
				_tts.SetSpeed (NaverTTS.MIN_ASC_SPEED);
				_tts.SetPath (NaverTTS.PATH_Voice);
				_tts.SetFileName (second.ToString("0000")+"_"+hash+"_"+ (int)vKind +"_"+_tts.GetSpeed()+"_"+tempStr);
				if (false == _tts.FileExists()) 
				{
					_tts.Request (tempStr);
					yield return null;
				}
			}

		}

		DebugWide.LogBlue ("");
		DebugWide.LogBlue ("================= Complete Request_NaverTTS ==================");
	}

	void Test_NaverTTS()
	{
		string text = "Update is called once per frame 안녕";
		_tts.SetSpeaker (NaverTTS.eLanguage.English, NaverTTS.eSex.Woman);
		_tts.SetSpeed (NaverTTS.MAX_ASC_SPEED);
		_tts.SetPath (NaverTTS.PATH_Voice);
		_tts.SetFileName ("2_"+_tts.GetSpeed()+"_"+text);
		_tts.Request (text);

		text = "Update is called once per frame 안녕";
		_tts.SetSpeaker (NaverTTS.eLanguage.English, NaverTTS.eSex.Man);
		_tts.SetSpeed (NaverTTS.BASIC_ASC_SPEED);
		_tts.SetPath (NaverTTS.PATH_Voice);
		_tts.SetFileName ("1_"+_tts.GetSpeed()+"_"+text);
		_tts.Request (text);

		text = "Update is called once per frame 안녕";
		_tts.SetSpeaker (NaverTTS.eLanguage.English, NaverTTS.eSex.Woman);
		_tts.SetSpeed (NaverTTS.MIN_ASC_SPEED);
		_tts.SetPath (NaverTTS.PATH_Voice);
		_tts.SetFileName ("2_"+_tts.GetSpeed()+"_"+text);
		_tts.Request (text);
	}

    void Test_Pitch_NaverTTS()
    {
        string text = "아";
        _tts.SetSpeaker(NaverTTS.eLanguage.Korean, NaverTTS.eSex.Woman);
        _tts.SetSpeed(NaverTTS.BASIC_ASC_SPEED);
        _tts.SetPath(NaverTTS.PATH_Voice);
        _tts.SetFileName("testPitch_2_" + _tts.GetSpeed() + "_" + text);
        _tts.Request(text);

    }

}


//"google tts"
//https://translate.google.com.vn/translate_tts?ie=UTF-8&q=%EC%95%84%EC%95%84%EC%95%84+&tl=ko&client=tw-ob

//===============================

