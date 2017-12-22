using System;
using System.Net;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//아마존 폴리 TTS link
//https://us-east-2.console.aws.amazon.com/polly/home/SynthesizeSpeech

//naver TTS 
//계정 : https://developers.naver.com/apps/#/myapps/2Lpz1sFLrJhFy1tY9SFR/overview
//문서 : https://developers.naver.com/docs/clova/api/#/CSS/API_Guide.md#RequestParameter
public class NaverTTS
{
	//-5이면 0.5배 빠른 속도이고 5이면 0.5배 느린 속도입니다
	public const int MAX_SPEED = -5;
	public const int MIN_SPEED = 5;
	public const int BASIC_SPEED = 0;

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
		Man,
		Woman,
	}

	private int 	_speed = BASIC_SPEED;
	private string 	_speaker = "jinho";
	private string  _path = PATH_StreamingAssets;
	private string _fileName = "tts.mp3";

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

	public void SetSpeed(int speed)
	{
		if (MAX_SPEED < speed)
			speed = MAX_SPEED;

		if (MIN_SPEED > speed)
			speed = MIN_SPEED;

		_speed = speed;
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
		DebugWide.LogBlue ("status="+ status);
		//Console.WriteLine("status="+ status);

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
}




public class NaverTTSBehaviour : MonoBehaviour 
{
	NaverTTS _tts = new NaverTTS();
	// Use this for initialization
	void Start () 
	{
		string text = "Update is called once per frame 안녕";
		_tts.SetSpeaker (NaverTTS.eLanguage.English, NaverTTS.eSex.Woman);
		_tts.SetSpeed (NaverTTS.MAX_SPEED);
		//_tts.SetPath (NaverTTS.PATH_StreamingAssets);
		_tts.SetPath (NaverTTS.PATH_Voice);
		_tts.SetFileName (text);
		_tts.Request (text);
		//_tts.Request ("안녕 하시렵니까  드루와 드루와 ㄲㄲㄲㄱ 드루와 드루와 ㄲㄲㄲㄱ드루와 드루와 ㄲㄲㄲㄱ드루와 ");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}


//===============================

