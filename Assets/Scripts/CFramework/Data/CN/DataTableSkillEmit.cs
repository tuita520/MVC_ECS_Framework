/*=====================================================
* - Editor by tool
* - Don't editor by handself
=====================================================*/
using System;
using UnityEngine;
using System.Collections.Generic;
using Zero.ZeroEngine.Data;


[System.Serializable]
public class SkillEmitExcel {
	public int id;
	public int acountID;
	public int triggerRate;
	public int emitType;
	public int angleType;
	public int targetNum;
	public int flySpeed;
	public int flyDelay;
	public int flyParam1;
	public int flyParam2;
	public string flyParam3;
	public int dapBoo;
	public int lifeTime;
	public int colliderRadius;
	public int objNum;
	public int objEffect;
	public int waitTime;
}
public class DataTableSkillEmit: DataBase {
	[SerializeField]
	public List<SkillEmitExcel> DataList = new List<SkillEmitExcel>();
	private Dictionary<int, SkillEmitExcel> _dataList = new Dictionary<int, SkillEmitExcel>();
	public void AddInfo(SkillEmitExcel _info)
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
	public Dictionary<int, SkillEmitExcel> GetInfo()
	{
		return _dataList;
	}
	public SkillEmitExcel GetInfoById(int _id)
	{
		SkillEmitExcel info;
		if (_dataList.TryGetValue(_id, out info))
		{
			return info;
		}
		return null;
	}
	public List<SkillEmitExcel> GetInfoByNameAndValue(string name,int value)
	{
		List<SkillEmitExcel> tempList = new List<SkillEmitExcel>();
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
		if (name.Equals("emitType"))
		{
			foreach (var item in DataList)
			{
				if (item.emitType== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("angleType"))
		{
			foreach (var item in DataList)
			{
				if (item.angleType== value)
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
		if (name.Equals("flySpeed"))
		{
			foreach (var item in DataList)
			{
				if (item.flySpeed== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("flyDelay"))
		{
			foreach (var item in DataList)
			{
				if (item.flyDelay== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("flyParam1"))
		{
			foreach (var item in DataList)
			{
				if (item.flyParam1== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("flyParam2"))
		{
			foreach (var item in DataList)
			{
				if (item.flyParam2== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("dapBoo"))
		{
			foreach (var item in DataList)
			{
				if (item.dapBoo== value)
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
		if (name.Equals("colliderRadius"))
		{
			foreach (var item in DataList)
			{
				if (item.colliderRadius== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("objNum"))
		{
			foreach (var item in DataList)
			{
				if (item.objNum== value)
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
	public List<SkillEmitExcel> GetInfoByNameAndValue(string name,string value)
	{
		List<SkillEmitExcel> tempList = new List<SkillEmitExcel>();
		if (name.Equals("flyParam3"))
		{
			foreach (var item in DataList)
			{
				if (item.flyParam3.Equals(value))
					tempList.Add(item);
			}
		}
		return tempList;
	}
}
