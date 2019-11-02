/*=====================================================
* - Editor by tool
* - Don't editor by handself
=====================================================*/
using System;
using UnityEngine;
using System.Collections.Generic;
using Zero.ZeroEngine.Data;


[System.Serializable]
public class SkillSummonExcel {
	public int id;
	public int acountID;
	public int triggerRate;
	public int summonType;
	public int wayType;
	public int roleType;
	public int roleID;
	public int number;
	public int radius;
	public int lifeTime;
	public int objEffect;
	public int waitTime;
}
public class DataTableSkillSummon: DataBase {
	[SerializeField]
	public List<SkillSummonExcel> DataList = new List<SkillSummonExcel>();
	private Dictionary<int, SkillSummonExcel> _dataList = new Dictionary<int, SkillSummonExcel>();
	public void AddInfo(SkillSummonExcel _info)
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
	public Dictionary<int, SkillSummonExcel> GetInfo()
	{
		return _dataList;
	}
	public SkillSummonExcel GetInfoById(int _id)
	{
		SkillSummonExcel info;
		if (_dataList.TryGetValue(_id, out info))
		{
			return info;
		}
		return null;
	}
	public List<SkillSummonExcel> GetInfoByNameAndValue(string name,int value)
	{
		List<SkillSummonExcel> tempList = new List<SkillSummonExcel>();
		if (name.Equals("id"))
		{
			foreach (var item in DataList)
			{
				if (item.id== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("acountID"))
		{
			foreach (var item in DataList)
			{
				if (item.acountID== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("triggerRate"))
		{
			foreach (var item in DataList)
			{
				if (item.triggerRate== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("summonType"))
		{
			foreach (var item in DataList)
			{
				if (item.summonType== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("wayType"))
		{
			foreach (var item in DataList)
			{
				if (item.wayType== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("roleType"))
		{
			foreach (var item in DataList)
			{
				if (item.roleType== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("roleID"))
		{
			foreach (var item in DataList)
			{
				if (item.roleID== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("number"))
		{
			foreach (var item in DataList)
			{
				if (item.number== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("radius"))
		{
			foreach (var item in DataList)
			{
				if (item.radius== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("lifeTime"))
		{
			foreach (var item in DataList)
			{
				if (item.lifeTime== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("objEffect"))
		{
			foreach (var item in DataList)
			{
				if (item.objEffect== value)
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
		return tempList;
	}
	public List<SkillSummonExcel> GetInfoByNameAndValue(string name,string value)
	{
		List<SkillSummonExcel> tempList = new List<SkillSummonExcel>();
		return tempList;
	}
}
