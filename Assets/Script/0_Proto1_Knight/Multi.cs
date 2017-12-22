using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Multi : MonoBehaviour
{
	
	private Character_HashInfoMap _hashMap = new Character_HashInfoMap();
	public Character_HashInfoMap hashMap
	{
		get { return _hashMap; }
	}

	private Animator _animator = null;
	public Animator animator
	{
		get { return _animator; }
	}

	private CharacterAnimator _charAnimator = null;
	public CharacterAnimator charAnimator
	{
		get { return _charAnimator; }
	}

	private TriggerProcess _trgPrcs = null;
	public TriggerProcess trgPrcs
	{
		get { return _trgPrcs; }
	}



	void Awake()
	{
		//DebugWide.LogBlue ("Multi awake");

		_hashMap.InitHashInfoMap (this.transform);

		_animator = this.GetComponent<Animator> ();
		_charAnimator = this.GetComponent<CharacterAnimator> ();

		_trgPrcs = this.GetComponent<TriggerProcess> ();

	}

}	



public class Character_HashInfoMap : HashInfoMap
{
	
	public int roothash
	{
		get{ return this.GetHash (eHashIdx.Root);}
	}

	public Vector3 root
	{
		get{ return this.GetTransform (eHashIdx.Root).position;}
	}

	public Vector3 forward
	{
		get{ return this.GetTransform (eHashIdx.ForwardZ).position - this.root;}
	}

	public Vector3 back
	{
		get{ return this.root - this.GetTransform (eHashIdx.ForwardZ).position;}
	}


	delegate  Transform DeleMethod(string path);
	public void InitHashInfoMap(Transform transform)
	{
		
		string basePath = "/"+transform.parent.name+"/"+transform.name;
		DeleMethod GetTForm = p => CounterBlock.Single.hierarchy.GetData (basePath + p);

		//DebugWide.LogBlue (basePath);

		this.Add ((int)eHashIdx.Root, GetTForm(""));
		this.Add ((int)eHashIdx.ForwardZ, GetTForm("/forwardZ"));

		this.Add ((int)eHashIdx.Bone_Body, GetTForm("/b_body"));
		this.Add ((int)eHashIdx.Bone_Mesh_Body, GetTForm("/b_body/body"));
		this.Add ((int)eHashIdx.Bone_Neck, GetTForm("/b_body/b_neck"));
		this.Add ((int)eHashIdx.Bone_Mesh_Head, GetTForm("/b_body/b_neck/head"));

		this.Add ((int)eHashIdx.Bone_Arm_Left, GetTForm("/b_body/b_arm_left"));
		this.Add ((int)eHashIdx.Bone_Mesh_Hand_Left, GetTForm("/b_body/b_arm_left/hand_left"));
		this.Add ((int)eHashIdx.Bone_Weapon_Sword_Left, GetTForm("/b_body/b_arm_left/hand_left/weapon_01/sword_ik"));
		this.Add ((int)eHashIdx.Bone_Weapon_Sword_EndPosition_Left, GetTForm("/b_body/b_arm_left/hand_left/weapon_01/sword_ik/endPos"));

		this.Add ((int)eHashIdx.Bone_Arm_Right, GetTForm("/b_body/b_arm_right"));
		this.Add ((int)eHashIdx.Bone_Mesh_Hand_Right, GetTForm("/b_body/b_arm_right/hand_right"));
		this.Add ((int)eHashIdx.Bone_Weapon_Sword_Right, GetTForm("/b_body/b_arm_right/hand_right/weapon_01/sword_ik"));
		this.Add ((int)eHashIdx.Bone_Weapon_Sword_EndPosition_Right, GetTForm("/b_body/b_arm_right/hand_right/weapon_01/sword_ik/endPos"));
	}

	public bool IsMyTransform(Transform compareTr)
	{
		Multi compareMulti = compareTr.GetComponentInParent<Multi> ();
		if (null != compareMulti) 
		{
			//DebugWide.LogBlue (compareMulti.hashMap.roothash +"   "+ this.roothash);
			if (compareMulti.hashMap.roothash == this.roothash)
				return true;
		}
		return false;
	}
}
