using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;
using CounterBlock;


public class FileToMemoryStream
{
	
	public IEnumerator FileLoading(string strFilePath, System.Action<MemoryStream> result = null)
	{
		MemoryStream memStream = null;
		#if SERVER || TOOL 
		{
		//CDefine.CommonLog("1__" + strFilePath); //chamto test
		memStream = new MemoryStream(File.ReadAllBytes(strFilePath));
		}

		#elif UNITY_IPHONE || UNITY_ANDROID || UNITY_EDITOR
		{

			UnityEngine.WWW wwwUrl = new UnityEngine.WWW(strFilePath);

			while (!wwwUrl.isDone)
			{   
				if (wwwUrl.error != null)
				{
					DebugWide.LogRed("error : " + wwwUrl.error.ToString());
					yield break;
				}
				//DebugWide.LogGreen("wwwUrl.progress---" + wwwUrl.progress);
				yield return null;
			}

			if (wwwUrl.isDone)
			{   
				//DebugWide.LogGreen("wwwUrl.isDone---size : "+wwwUrl.size);
				DebugWide.LogGreen("wwwUrl.isDone---bytesLength : "+wwwUrl.bytes.Length);
				memStream = new MemoryStream(wwwUrl.bytes);
			}
		}
		#endif

		if (null != result)
		{   
			result(memStream);
		}
		DebugWide.LogGreen("*** "+strFilePath+" WWW Loading complete");
		yield return memStream;
	}

}




public class HashToStringMap
{
	Dictionary<int, string> _hashStringMap = new Dictionary<int, string> ();

	public void Add(int hash, string str)
	{
		if (false == _hashStringMap.ContainsKey (hash)) 
		{
			_hashStringMap.Add (hash, str);
		} 
	}

	public string GetString(int hash)
	{
		if (_hashStringMap.ContainsKey (hash)) 
		{
			return _hashStringMap [hash];
		}

		throw new KeyNotFoundException ();

		return ""; //존재하지 않는 키값
	}

	public string GetString_ForAssetFile(int hash)
	{
		string tempStr = this.GetString (hash);
		return Regex.Replace (tempStr,"[?<>:*|\"]",""); //애셋파일 이름에 들어가면 안되는 특수문자 제거 ?, <, >, :, *, |, ".
	}
}


namespace XML_Data
{
	//string => hashValue
	//value문자열의 인덱스값으로 key해쉬 값을 찾는다 
	public class ValueToKeyMap
	{

		private Dictionary<int,int> _valueToKeyMap = new Dictionary<int, int> ();


		public void Add(int src_idx, int dst_idx)
		{
			if (_valueToKeyMap.ContainsKey (src_idx)) 
			{
				//키값이 이미 있으면 값을 갱신한다
				_valueToKeyMap [src_idx] = dst_idx;
			} else 
			{
				_valueToKeyMap.Add (src_idx, dst_idx);
			}
		}


		public int GetDstHash(int src_idx)
		{

			if (_valueToKeyMap.ContainsKey (src_idx)) 
			{
				return _valueToKeyMap [src_idx];
			}

			throw new KeyNotFoundException ();

			return -1; //존재하지 않는 키값
		}

	}

	public class DictInfo
	{
		
		public class Meaning : List<int>
		{
			public enum eKind
			{
				None,
				Vocabulary,	//단어 
				Idiom,		//숙어 
				Sentence,	//문장
				Part,		//문장의 부분
			}
			public eKind kind;
		}

		private int _id_number = -1; //고유번호 
		private int _hashTitle = -1;

		private ValueToKeyMap _valueToKeyMap = new ValueToKeyMap();
		private Dictionary<int, DictInfo.Meaning> _data = new Dictionary<int, DictInfo.Meaning> ();
		private List<int> _sequence = new List<int>(); //xml 데이터의 순서를 기록한다. 가사를 순서대로 재생하기 위하여 필요함 


		public void SetID_Number(int number)
		{
			_id_number = number;
		}
		public int GetID_Number()
		{
			return _id_number;
		}

		public Dictionary<int, DictInfo.Meaning> GetData()
		{
			return _data;
		}

		public List<int> GetSequence()
		{
			return _sequence;
		}

		public string GetTitle()
		{
			return Single.hashString.GetString(_hashTitle);
		}
		public void SetTitle(int hashTitle)
		{
			_hashTitle = hashTitle;
		}

		public void Add(int hashKey, DictInfo.Meaning mMore)
		{
			
			if (_data.ContainsKey (hashKey)) 
			{
				//키값이 이미 있으면 값을 갱신한다
				_data [hashKey] = mMore;
			} else 
			{
				_data.Add (hashKey, mMore);
			}

			foreach (int mOne in mMore) 
			{
				//DebugWide.LogBlue (mOne +  " ----  " + hashKey);
				_valueToKeyMap.Add (mOne, hashKey);
			}

			_sequence.Add (hashKey); //순서저장
		}


		public Meaning GetMeaningFromKey(int hashKey)
		{
			if (_data.ContainsKey (hashKey)) 
			{
				return _data [hashKey];
			}

			throw new KeyNotFoundException ();

			return null; //존재하지 않는 키값
		}

		public Meaning GetMeaningFromKey(string textKey)
		{
			return this.GetMeaningFromKey (textKey.GetHashCode ());
		}

		public string GetMeaningFromKey_First(string textKey)
		{
			int hashValue = GetMeaningFromKey (textKey).ElementAt (0); 

			return Single.hashString.GetString (hashValue);
		}


		public string GetTextFromValue(int hashValue)
		{
			int hashKey = _valueToKeyMap.GetDstHash (hashValue);
			return Single.hashString.GetString (hashKey);
		}

		public string GetTextFromValue(string meaningValue)
		{
			return this.GetTextFromValue (meaningValue.GetHashCode());
		}

	}
	
	public class Dict_English : FileToMemoryStream
	{

		private string m_strFileName = "Dict_English.xml";

		private bool _bCompleteLoad = false;
		public bool bCompleteLoad
		{
			get { return _bCompleteLoad; }
		}

		public Dictionary<int, DictInfo> _dictInfoMap = new Dictionary<int, DictInfo>();



		public void Print ()
		{
			DebugWide.LogBlue ("==================== "+ m_strFileName +" ====================");
			foreach(DictInfo info in _dictInfoMap.Values)
			{
				DebugWide.LogBlue ("=== DictInfo - title : " + info.GetTitle ());

				foreach (KeyValuePair<int,DictInfo.Meaning> kv in info.GetData()) 
				{
					DebugWide.LogBlue ("========= DictInfo - <eng> : " + Single.hashString.GetString(kv.Key));

					foreach (int v in kv.Value) 
					{
						DebugWide.LogBlue ("========= DictInfo - <kor> : " + Single.hashString.GetString(v));
					}
				}

			}
		}


		public IEnumerator LoadXML()
		{
			//내부 코루틴 부분
			//------------------------------------------------------------------------
			//DebugWide.LogBlue(GlobalConstants.ASSET_PATH + m_strFileName); //chamto test
			MemoryStream stream = null;
			//IEnumerator irator = this.FileLoading(GlobalConstants.ASSET_PATH + m_strFileName, value => stream = value);
			IEnumerator irator = this.FileLoading(GlobalConstants.ASSET_PATH + m_strFileName,null);
			yield return irator;

			stream = irator.Current as MemoryStream; //이뮬레이터의 양보반환값을 가져온다
			if (null == stream)
			{
				DebugWide.Log("error : failed LoadFromFile : " + GlobalConstants.ASSET_PATH + m_strFileName);
				yield break;
			}
			this.Parse_MemoryStream (stream);


			//Print(); //chamto test

		}

		private void Parse_MemoryStream(MemoryStream stream)
		{
			_bCompleteLoad = false;
			_dictInfoMap.Clear();

			//------------------------------------------------------------------------
			//<root>
			//			<DictInfo title="start_of_game" >
			//				<eng num="0" kind="vocabulary" text="memory">
			//					<kor meaning="추억"  />
			//					<kor meaning="기억"  />
			//				</eng>
			//				<eng num="1" kind="idiom" text="used to ~">
			//					<kor meaning="뭐뭐 하곤 했다" />
			//					<kor meaning="한때는 뭐뭐 이었다" />
			//					<kor meaning="뭐뭐에 익숙하다" />
			//				</eng>
			//			</DictInfo>
			//</root>

			XmlDocument Xmldoc = new XmlDocument();
			Xmldoc.Load(stream);

			XmlElement root_element = Xmldoc.DocumentElement; 	//<root>		
			XmlNodeList secondList = root_element.ChildNodes;	//<DictInfo>
			XmlNodeList thirdList = null;	//<eng>
			XmlNodeList fourthList = null;	//<kor>
			XmlAttributeCollection attrs = null;
			XmlNode xmlNode = null;
			//Debug.Log ("loadXML : " + secondList.Count); //chamto test
			DictInfo item = null;
			for (int i = 0; i < secondList.Count; ++i) 
			{
				//DebugWide.LogBlue(secondList[i] + " ------- i:" + i);
				if (false == (secondList [i] is System.Xml.XmlElement)) //System.Xml.XmlComment 주석일 때는 처리 하지 않는다
					continue;
				
				item = new DictInfo();

				//==================== <DictInfo title> ====================
				xmlNode = secondList[i].Attributes.GetNamedItem("num");
				item.SetID_Number (int.Parse (xmlNode.Value));
				xmlNode = secondList[i].Attributes.GetNamedItem("title");
				Single.hashString.Add (xmlNode.Value.GetHashCode (), xmlNode.Value);
				item.SetTitle(xmlNode.Value.GetHashCode());

				DictInfo.Meaning meaning = null;
				DictInfo.Meaning.eKind eKind = DictInfo.Meaning.eKind.None;

				//==================== <eng> ====================
				thirdList = secondList[i].ChildNodes; //<DictInfo>
				for (int j = 0; j < thirdList.Count; ++j) 
				{
					//DebugWide.LogBlue(thirdList[j] + " ------- j:" + j);
					if (false == (thirdList [j] is System.Xml.XmlElement)) //System.Xml.XmlComment 주석일 때는 처리 하지 않는다
						continue;
					
					//==================== <kor> ====================
					meaning = new DictInfo.Meaning ();
					fourthList = thirdList[j].ChildNodes; //<kor>
					for (int k = 0; k < fourthList.Count; ++k) 
					{
						//DebugWide.LogBlue(fourthList[k] + " ------- k:" + k);
						if (false == (fourthList [k] is System.Xml.XmlElement)) //System.Xml.XmlComment 주석일 때는 처리 하지 않는다
							continue;
						
						attrs = fourthList[k].Attributes;
						foreach(XmlNode n in attrs)
						{
							switch(n.Name)
							{
							case "meaning":
								Single.hashString.Add (n.Value.GetHashCode (), n.Value);
								meaning.Add (n.Value.GetHashCode ());
								//DebugWide.LogBlue ("     <kor> " + n.Name + "  =  " + n.Value + "  " + eKind.ToString());
								break;
							}
						}
					}
					//====================//====================

					attrs = thirdList[j].Attributes; //<eng>
					foreach(XmlNode n in attrs)
					{
						switch(n.Name)
						{
						case "kind":
							if ("vocabulary" == n.Value)
								eKind = DictInfo.Meaning.eKind.Vocabulary;
							if ("idiom" == n.Value)
								eKind = DictInfo.Meaning.eKind.Idiom;
							if ("sentence" == n.Value)
								eKind = DictInfo.Meaning.eKind.Sentence;
							if ("sentence" == n.Value)
								eKind = DictInfo.Meaning.eKind.Part;
							break;
						case "text":
							{
								Single.hashString.Add (n.Value.GetHashCode (), n.Value);
								item.Add (n.Value.GetHashCode(), meaning);
								//DebugWide.LogBlue ("<eng> " + n.Name + "  =  " + n.Value );
							}
							break;
						}
					}

					meaning.kind = eKind;


				}
				//DebugWide.LogBlue (xmlNode.Value + "  ----  " + xmlNode.Value.GetHashCode());
				_dictInfoMap.Add(item.GetID_Number(), item);
			}

			_bCompleteLoad = true;
		}//func end


	}//class end

}//namespace end

