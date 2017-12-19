using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 네이버 음성합성 Open API 예제
using System;
using System.Net;
using System.Text;
using System.IO;

//naver TTS 계정 
//https://developers.naver.com/apps/#/myapps/2Lpz1sFLrJhFy1tY9SFR/overview

//아마존 폴리 TTS link
//https://us-east-2.console.aws.amazon.com/polly/home/SynthesizeSpeech
public static class NaverTTS
{
	public static void Response(string text)
	{
		//ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
		//System.Diagnostics.Process.Start("mozroots","--import --quiet");

		//string text = "좋은 하루 되세요."; // 음성합성할 문자값
		string url = "https://openapi.naver.com/v1/voice/tts.bin";
		HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
		request.Headers.Add("X-Naver-Client-Id", "2Lpz1sFLrJhFy1tY9SFR");
		request.Headers.Add("X-Naver-Client-Secret", "epWAWPAWNX");
		request.Method = "POST";

		byte[] byteDataParams = Encoding.UTF8.GetBytes("speaker=mijin&speed=0&text=" + text);
		request.ContentType = "application/x-www-form-urlencoded";
		request.ContentLength = byteDataParams.Length;
		Stream st = request.GetRequestStream();
		st.Write(byteDataParams, 0, byteDataParams.Length);
		st.Close();
		HttpWebResponse response = (HttpWebResponse)request.GetResponse();

		string status = response.StatusCode.ToString();
		DebugWide.LogBlue ("status="+ status);
		//Console.WriteLine("status="+ status);

		using (Stream output = File.OpenWrite ("Assets/StreamingAssets/" + "tts.mp3")) 
		{
			using (Stream input = response.GetResponseStream ()) 
			{
			
				//input.CopyTo(output);
				NaverTTS.CopyTo(input, output);

			}
		}

		DebugWide.LogBlue("Assets/StreamingAssets/" +"tts.mp3 was created");
		//Console.WriteLine("c:/tts.mp3 was created");
	}

	//ref : https://stackoverflow.com/questions/5730863/how-to-use-stream-copyto-on-net-framework-3-5
	public static void CopyTo(this Stream input, Stream output)
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

	// Use this for initialization
	void Start () 
	{
		NaverTTS.Response ("안녕 하시렵니까  드루와 드루와 ㄲㄲㄲㄱ 드루와 드루와 ㄲㄲㄲㄱ드루와 드루와 ㄲㄲㄲㄱ드루와 ");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
