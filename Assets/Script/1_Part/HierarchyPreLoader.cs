using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class HierarchyPreLoader
{
	private UInt32 _keySecquence = 0;
	protected Dictionary<string, UInt32> 	_pathToKey = new Dictionary<string, UInt32>();
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
	
	public void PreOrderTraversal(string path , Transform data)
	{
		//1. visit
		//DebugWide.LogRed (path +"    "+ data.name); //chamto test
		uint value;
		if (true == _pathToKey.TryGetValue (path, out value)) 
		{
			//Debug.Assert (false);
			//UnityEngine.Assertions.Assert.IsFalse (true);

			//이미 있는 경로일 경우 자식쪽은 탐색을 중지 한다
			DebugWide.LogRed (path + "  이미 탐색한 경로입니다");
			return;
		}

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



	public void PreOrderTraversal( Transform data )
	{
		PreOrderTraversal ("/"+this.GetTransformFullPath (data), data);
	}



	public TaaT Find<TaaT>(string fullPath) where TaaT : class
	{
		//ref : http://answers.unity3d.com/questions/8500/how-can-i-get-the-full-path-to-a-gameobject.html
		Transform f = Resources.FindObjectsOfTypeAll<Transform>().Where(tr => this.GetTransformFullPath (tr) == fullPath).First();

		//return f.GetComponentInChildren (typeof(TaaT), true) as TaaT;
		return f.GetComponentInChildren <TaaT>(true);
	}


	public TaaT FindOnlyActive<TaaT>(string fullPath) where TaaT : class
	{
		//ref : http://answers.unity3d.com/questions/8500/how-can-i-get-the-full-path-to-a-gameobject.html
		Transform f =  Resources.FindObjectsOfTypeAll<Transform> ().Where (
			tr => this.GetTransformFullPath (tr) == fullPath && tr.hideFlags != HideFlags.HideInHierarchy).First ();

		return f.GetComponentInChildren <TaaT>(true);
	}

	//ref : http://answers.unity3d.com/questions/8500/how-can-i-get-the-full-path-to-a-gameobject.html
	public string GetTransformFullPath(Transform transform)
	{
		string path = transform.name;
		while (transform.parent != null)
		{
			transform = transform.parent;
			path = transform.name + "/" + path;
		}
		return path;
	}

	public void Init()
	{
		_pathToKey.Clear ();
		_keyToData.Clear ();
		_dataToPath.Clear ();
		_keySecquence = 0;

		//여러 최상위 루트마다 순회
		foreach (Transform oneOfManyRoots in UnityEngine.Object.FindObjectsOfType<Transform>())
		{
			if (oneOfManyRoots.parent == null)
			{
				
				//DebugWide.LogRed(oneOfManyRoots.name);
				this.PreOrderTraversal ("/"+oneOfManyRoots.name, oneOfManyRoots);
			}
		}

		//TestPrint(); //chamto test
	}
	
	public void TestPrint()
	{
		

		Debug.Log ("---------- HierarchyLoader : TestPrint ----------");
		foreach(KeyValuePair<Transform, string> keyValue in _dataToPath)
		{
			Debug.Log("<color=blue>" + keyValue.Key.name + " : </color> <color=green>" + keyValue.Value + "</color> \n");
		}
	}
}
