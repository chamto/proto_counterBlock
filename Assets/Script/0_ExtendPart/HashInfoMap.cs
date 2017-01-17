using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum eHashIdx : int
{
	None = 0,
	Bone_Root,
	Bone_Body,
	Bone_Mesh_Body,
	Bone_Neck,
	Bone_Mesh_Head,

	Bone_Arm_Left,
	Bone_Mesh_Hand_Left,
	Bone_Weapon_Sword_Left,
	Bone_Weapon_Sword_EndPosition_Left,

	Bone_Arm_Right,
	Bone_Mesh_Hand_Right,
	Bone_Weapon_Sword_Right,
	Bone_Weapon_Sword_EndPosition_Right,

}

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

	public void Add(int key, string originalStr, Transform originalTr)
	{
		InfoStruct info;
		if (true == _hashMap.TryGetValue (key, out info)) 
		{
			UnityEngine.Assertions.Assert.IsFalse (false, "이미 있는 키값을 추가 하려함");
		}

		this._hashMap.Add(key,new InfoStruct(originalStr.GetHashCode(),originalStr,originalTr));
	}

	public Transform GetTransform(int key)
	{
		return this._hashMap[key].tr;
	}
	public string GetString(int key)
	{
		return this._hashMap[key].str;
	}
	public int GetHash(int key)
	{
		return this._hashMap[key].hashValue;
	}
}
