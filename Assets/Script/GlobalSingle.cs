using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Global Single
/// </summary>
/// 

namespace CounterBlock
{
	public static class Single
	{

		//갱신처리
		public static void Update()
		{
			Single.coroutine.Update ();
		}

		public static WideCoroutine coroutine
		{
			get
			{
				return CSingleton<WideCoroutine>.Instance;
			}
		}
	
		public static HashToStringMap hashString
		{
			get
			{
				return CSingleton<HashToStringMap>.Instance;
			}
		}

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

		private static Transform _game_root = null;
		public static Transform game_root
		{
			get
			{
				if (null == _game_root) 
				{
					_game_root = Single.hierarchy.FindOnlyActive<Transform> ("0_Game");
				}
				return _game_root;
			}
		}

		private static System.Random _rand = new System.Random();
		static public System.Random rand
		{
			get{ return _rand; }
		}
	}
}

