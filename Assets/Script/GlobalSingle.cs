using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Global Single
/// </summary>
public class Single
{
	
//	public static WideUseCoroutine coroutine
//	{
//		get
//		{
//			return CSingleton<WideUseCoroutine>.Instance;
//		}
//	}
//
//	public static IEnumerator startCoroutine
//	{
//		set
//		{
//			coroutine.Start_Async(value);
//
//		}
//	}

	public static HierarchyPreLoader hierarchy
	{
		get
		{
			return CSingleton<HierarchyPreLoader>.Instance;
		}
	}

	public static ParticleController particle
	{
		get
		{
			return CSingletonMono<ParticleController>.Instance;
		}
	}

	public static CounterBlock.ResourceManager resource
	{
		get
		{
			return CSingleton<CounterBlock.ResourceManager>.Instance;
		}
	}


	private static Transform _ui_root = null;
	public static Transform ui_root
	{
		get
		{
			if (null == _ui_root) 
			{
				_ui_root = Single.hierarchy.FindOnlyActive<Transform> ("Canvas");
			}
			return _ui_root;
		}
	}
}
