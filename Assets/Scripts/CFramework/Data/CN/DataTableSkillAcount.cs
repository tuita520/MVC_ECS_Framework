/*=====================================================
* - Editor by tool
* - Don't editor by handself
=====================================================*/
using System;
using UnityEngine;
using System.Collections.Generic;
using Zero.ZeroEngine.Data;


[System.Serializable]
public class SkillAcountExcel {
	public int id;
	public int elementType;
	public int[] affectID;
	public int[] affectRate;
	public int hurtAdd;
	public int hatred;
	public int[] hitEffect;
	public int waitTime;
	public int txtWaiT;
	public int interruptType;
	public int[] myBuffIDs;
	public int[] targetBuffIDs;
}
public class DataTableSkillAcount: DataBase {
	[SerializeField]
	public List<SkillAcountExcel> DataList = new List<SkillAcountExcel>();
	private Dictionary<int, SkillAcountExcel> _dataList = new Dictionary<int, SkillAcountExcel>();
	public void AddInfo(SkillAcountExcel _info)
	{
		DataList.Add(_info);
	}
	public override void Clear()
	{
		_dataList.Clear();
	}
	public override void Init()
	{
		if(_dataList.Count>0) return;
		foreach (var info in DataList)
		{
			_dataList.Add(info.id, info);
		}
	}
	public Dictionary<int, SkillAcountExcel> GetInfo()
	{
		return _dataList;
	}
	public SkillAcountExcel GetInfoById(int _id)
	{
		SkillAcountExcel info;
		if (_dataList.TryGetValue(_id, out info))
		{
			return info;
		}
		return null;
	}
	public List<SkillAcountExcel> GetInfoByNameAndValue(string name,int value)
	{
		List<SkillAcountExcel> tempList = new List<SkillAcountExcel>();
		if (name.Equals("id"))
		{
			foreach (var item in DataList)
			{
				if (item.id== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("elementType"))
		{
			foreach (var item in DataList)
			{
				if (item.elementType== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("hurtAdd"))
		{
			foreach (var item in DataList)
			{
				if (item.hurtAdd== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("hatred"))
		{
			foreach (var item in DataList)
			{
				if (item.hatred== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("waitTime"))
		{
			foreach (var item in DataList)
			{
				if (item.waitTime== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("txtWaiT"))
		{
			foreach (var item in DataList)
			{
				if (item.txtWaiT== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("interruptType"))
		{
			foreach (var item in DataList)
			{
				if (item.interruptType== value)
					tempList.Add(item);
			}
		}
		return tempList;
	}
	public List<SkillAcountExcel> GetInfoByNameAndValue(string name,string value)
	{
		List<SkillAcountExcel> tempList = new List<SkillAcountExcel>();
		return tempList;
	}
}
