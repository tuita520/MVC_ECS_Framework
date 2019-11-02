/*=====================================================
* - Editor by tool
* - Don't editor by handself
=====================================================*/
using System;
using UnityEngine;
using System.Collections.Generic;
using Zero.ZeroEngine.Data;


[System.Serializable]
public class SkillMoveExcel {
	public int id;
	public int acountID;
	public int triggerRate;
	public int targetType;
	public int moveType;
	public int targetNum;
	public int moveAngle;
	public int moveDis;
	public int moveSpeed;
	public int moveDamageBoo;
	public int colliderRadius;
	public int objEffect;
	public int waitTime;
}
public class DataTableSkillMove: DataBase {
	[SerializeField]
	public List<SkillMoveExcel> DataList = new List<SkillMoveExcel>();
	private Dictionary<int, SkillMoveExcel> _dataList = new Dictionary<int, SkillMoveExcel>();
	public void AddInfo(SkillMoveExcel _info)
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
	public Dictionary<int, SkillMoveExcel> GetInfo()
	{
		return _dataList;
	}
	public SkillMoveExcel GetInfoById(int _id)
	{
		SkillMoveExcel info;
		if (_dataList.TryGetValue(_id, out info))
		{
			return info;
		}
		return null;
	}
	public List<SkillMoveExcel> GetInfoByNameAndValue(string name,int value)
	{
		List<SkillMoveExcel> tempList = new List<SkillMoveExcel>();
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
		if (name.Equals("targetType"))
		{
			foreach (var item in DataList)
			{
				if (item.targetType== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("moveType"))
		{
			foreach (var item in DataList)
			{
				if (item.moveType== value)
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
		if (name.Equals("moveAngle"))
		{
			foreach (var item in DataList)
			{
				if (item.moveAngle== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("moveDis"))
		{
			foreach (var item in DataList)
			{
				if (item.moveDis== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("moveSpeed"))
		{
			foreach (var item in DataList)
			{
				if (item.moveSpeed== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("moveDamageBoo"))
		{
			foreach (var item in DataList)
			{
				if (item.moveDamageBoo== value)
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
	public List<SkillMoveExcel> GetInfoByNameAndValue(string name,string value)
	{
		List<SkillMoveExcel> tempList = new List<SkillMoveExcel>();
		return tempList;
	}
}
