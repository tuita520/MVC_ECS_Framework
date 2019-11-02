/*=====================================================
* - Editor by tool
* - Don't editor by handself
=====================================================*/
using System;
using UnityEngine;
using System.Collections.Generic;
using Zero.ZeroEngine.Data;


[System.Serializable]
public class SkillRangeExcel {
	public int id;
	public int acountID;
	public int triggerRate;
	public int areaType;
	public int shapeType;
	public int[] rangeParam1;
	public int rangeParam2;
	public int targetNum;
	public int intervaTime;
	public int lifeTime;
	public int objEffect;
	public int loopBoo;
	public int waitTime;
}
public class DataTableSkillRange: DataBase {
	[SerializeField]
	public List<SkillRangeExcel> DataList = new List<SkillRangeExcel>();
	private Dictionary<int, SkillRangeExcel> _dataList = new Dictionary<int, SkillRangeExcel>();
	public void AddInfo(SkillRangeExcel _info)
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
	public Dictionary<int, SkillRangeExcel> GetInfo()
	{
		return _dataList;
	}
	public SkillRangeExcel GetInfoById(int _id)
	{
		SkillRangeExcel info;
		if (_dataList.TryGetValue(_id, out info))
		{
			return info;
		}
		return null;
	}
	public List<SkillRangeExcel> GetInfoByNameAndValue(string name,int value)
	{
		List<SkillRangeExcel> tempList = new List<SkillRangeExcel>();
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
		if (name.Equals("areaType"))
		{
			foreach (var item in DataList)
			{
				if (item.areaType== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("shapeType"))
		{
			foreach (var item in DataList)
			{
				if (item.shapeType== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("rangeParam2"))
		{
			foreach (var item in DataList)
			{
				if (item.rangeParam2== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("targetNum"))
		{
			foreach (var item in DataList)
			{
				if (item.targetNum== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("intervaTime"))
		{
			foreach (var item in DataList)
			{
				if (item.intervaTime== value)
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
		if (name.Equals("loopBoo"))
		{
			foreach (var item in DataList)
			{
				if (item.loopBoo== value)
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
	public List<SkillRangeExcel> GetInfoByNameAndValue(string name,string value)
	{
		List<SkillRangeExcel> tempList = new List<SkillRangeExcel>();
		return tempList;
	}
}
