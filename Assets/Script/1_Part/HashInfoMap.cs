using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CounterBlockSting;

//string <=> hash <= index
public class HashInfoMap
{
	
	public struct InfoStruct
	{
		public int 			hashValue;
		public string 		str;
		public Transform 	tr;

		public InfoStruct(int pHashValue, string pStr)
		{
			hashValue = pHashValue;
			str = pStr;
			tr = null;
		}

		public InfoStruct(int pHashValue, string pStr, Transform pTr)
		{
			hashValue = pHashValue;
			str = pStr;
			tr = pTr;
		}

	}

	private Dictionary<int,InfoStruct> _hashMap = new Dictionary<int, InfoStruct> ();


	public void Add(int key, Transform originalTr)
	{
		this.Add (key, originalTr.name, originalTr);
	}

	public void Add(int key, string originalStr, Transform originalTr)
	{
		InfoStruct info;
		if (true == _hashMap.TryGetValue (key, out info)) 
		{
			UnityEngine.Assertions.Assert.IsFalse (true, "이미 있는 키값을 추가 하려함");
		}

		this._hashMap.Add(key,new InfoStruct(originalStr.GetHashCode(),originalStr,originalTr));
	}

	public Transform GetTransform(int key)
	{
		return this._hashMap[key].tr;
	}
	public Transform GetTransform(eHashIdx eKey)
	{
		return this._hashMap[(int)eKey].tr;
	}
	public string GetString(int key)
	{
		return this._hashMap[key].str;
	}
	public int GetHash(int key)
	{
		return this._hashMap[key].hashValue;
	}

	public int GetHash(eHashIdx eKey)
	{
		return this._hashMap[(int)eKey].hashValue;
	}

}
