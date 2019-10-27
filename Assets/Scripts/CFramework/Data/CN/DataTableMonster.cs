/*=====================================================
* - Editor by tool
* - Don't editor by handself
=====================================================*/
using System;
using UnityEngine;
using System.Collections.Generic;
using Zero.ZeroEngine.Data;


[System.Serializable]
public class MonsterExcel {
	public int id;
	public string name;
	public int type;
	public int nameBoo;
	public int lv;
	public int speed;
	public int hp;
	public int p_attack;
	public int m_attack;
	public int model;
	public int hitBR;
	public int modelScale;
	public string headIcon;
	public int fireS;
	public int beFireS;
	public int dieS;
}
public class DataTableMonster: DataBase {
	[SerializeField]
	public List<MonsterExcel> DataList = new List<MonsterExcel>();
	private Dictionary<int, MonsterExcel> _dataList = new Dictionary<int, MonsterExcel>();
	public void AddInfo(MonsterExcel _info)
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
	public Dictionary<int, MonsterExcel> GetInfo()
	{
		return _dataList;
	}
	public MonsterExcel GetInfoById(int _id)
	{
		MonsterExcel info;
		if (_dataList.TryGetValue(_id, out info))
		{
			return info;
		}
		return null;
	}
	public List<MonsterExcel> GetInfoByNameAndValue(string name,int value)
	{
		List<MonsterExcel> tempList = new List<MonsterExcel>();
		if (name.Equals("id"))
		{
			foreach (var item in DataList)
			{
				if (item.id== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("type"))
		{
			foreach (var item in DataList)
			{
				if (item.type== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("nameBoo"))
		{
			foreach (var item in DataList)
			{
				if (item.nameBoo== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("lv"))
		{
			foreach (var item in DataList)
			{
				if (item.lv== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("speed"))
		{
			foreach (var item in DataList)
			{
				if (item.speed== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("hp"))
		{
			foreach (var item in DataList)
			{
				if (item.hp== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("p_attack"))
		{
			foreach (var item in DataList)
			{
				if (item.p_attack== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("m_attack"))
		{
			foreach (var item in DataList)
			{
				if (item.m_attack== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("model"))
		{
			foreach (var item in DataList)
			{
				if (item.model== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("hitBR"))
		{
			foreach (var item in DataList)
			{
				if (item.hitBR== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("modelScale"))
		{
			foreach (var item in DataList)
			{
				if (item.modelScale== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("fireS"))
		{
			foreach (var item in DataList)
			{
				if (item.fireS== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("beFireS"))
		{
			foreach (var item in DataList)
			{
				if (item.beFireS== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("dieS"))
		{
			foreach (var item in DataList)
			{
				if (item.dieS== value)
					tempList.Add(item);
			}
		}
		return tempList;
	}
	public List<MonsterExcel> GetInfoByNameAndValue(string name,string value)
	{
		List<MonsterExcel> tempList = new List<MonsterExcel>();
		if (name.Equals("name"))
		{
			foreach (var item in DataList)
			{
				if (item.name.Equals(value))
					tempList.Add(item);
			}
		}
		if (name.Equals("headIcon"))
		{
			foreach (var item in DataList)
			{
				if (item.headIcon.Equals(value))
					tempList.Add(item);
			}
		}
		return tempList;
	}
}
