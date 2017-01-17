using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class HierarchyPreLoader
{
	private UInt32 _keySecquence = 0;
	protected Dictionary<string, UInt32> _pathToKey = new Dictionary<string, UInt32>();
	protected Dictionary<UInt32, Transform> _keyToData = new Dictionary<UInt32, Transform>();
	protected Dictionary<Transform, string> _dataToPath = new Dictionary<Transform, string>();
	
	private UInt32 createKey()
	{
		//사용후 반환된 키목록에, 키가 있으면 먼저 반환한다.
		//code..
		
		return _keySecquence++;
	}
	//path -> key -> data
	//data -> path
	public UInt32 PathToKey(string path)
	{
		return _pathToKey [path];
	}
	public UInt32 PathToKey(Transform data, string remainderPath)
	{
		return _pathToKey [this.DataToPath(data) + remainderPath];
	}
	public string DataToPath(Transform data)
	{
		return _dataToPath [data];
	}
	public Transform GetData(UInt32 key)
	{
		return _keyToData [key];
	}
	public Transform GetData(string path)
	{
		return _keyToData [ this.PathToKey(path) ];
	}
	
	private void PreOrderTraversal(string path , Transform data)
	{
		//1. visit
		//DebugWide.LogRed (path +"    "+ data.name); //chamto test
		_pathToKey.Add (path, this.createKey ());
		_keyToData.Add (_pathToKey [path], data);
		_dataToPath.Add (data, path);

		
		//2. traversal
		Transform[] tfoList = data.GetComponentsInChildren<Transform> (true);
		foreach(Transform child in tfoList)
		{
			if(child != data && child.parent == data) 
			{
				this.PreOrderTraversal(path+"/"+child.name, child);
			}


		}
	}
	
	public void Init()
	{
		_pathToKey.Clear ();
		_keyToData.Clear ();
		_dataToPath.Clear ();
		_keySecquence = 0;

		List<GameObject> rootObjects = new List<GameObject>();
		foreach (Transform root in UnityEngine.Object.FindObjectsOfType<Transform>())
		{
			if (root.parent == null)
			{
				//DebugWide.LogRed(root.name);
				this.PreOrderTraversal ("/"+root.name, root);
			}
		}

		//TestPrint(); //chamto test
	}
	
	public void TestPrint()
	{
		Debug.Log ("---------- HierarchyLoader : TestPrint ----------");
		foreach(KeyValuePair<Transform, string> keyValue in _dataToPath)
		{
			Debug.Log(keyValue.Key.name + " : " + keyValue.Value);
		}
	}
}
