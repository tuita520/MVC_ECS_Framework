/*=====================================================
* - Editor by tool
* - Don't editor by handself
=====================================================*/
using System;
using UnityEngine;
using System.Collections.Generic;
using Zero.ZeroEngine.Data;


[System.Serializable]
public class MonsterRefreshExcel {
	public int id;
	public string name;
	public int refreshR;
	public int refreshT;
	public int randomBoo;
	public int[] att;
}
public class DataTableMonsterRefresh: DataBase {
	[SerializeField]
	public List<MonsterRefreshExcel> DataList = new List<MonsterRefreshExcel>();
	private Dictionary<int, MonsterRefreshExcel> _dataList = new Dictionary<int, MonsterRefreshExcel>();
	public void AddInfo(MonsterRefreshExcel _info)
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
	public Dictionary<int, MonsterRefreshExcel> GetInfo()
	{
		return _dataList;
	}
	public MonsterRefreshExcel GetInfoById(int _id)
	{
		MonsterRefreshExcel info;
		if (_dataList.TryGetValue(_id, out info))
		{
			return info;
		}
		return null;
	}
	public List<MonsterRefreshExcel> GetInfoByNameAndValue(string name,int value)
	{
		List<MonsterRefreshExcel> tempList = new List<MonsterRefreshExcel>();
		if (name.Equals("id"))
		{
			foreach (var item in DataList)
			{
				if (item.id== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("refreshR"))
		{
			foreach (var item in DataList)
			{
				if (item.refreshR== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("refreshT"))
		{
			foreach (var item in DataList)
			{
				if (item.refreshT== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("randomBoo"))
		{
			foreach (var item in DataList)
			{
				if (item.randomBoo== value)
					tempList.Add(item);
			}
		}
		return tempList;
	}
	public List<MonsterRefreshExcel> GetInfoByNameAndValue(string name,string value)
	{
		List<MonsterRefreshExcel> tempList = new List<MonsterRefreshExcel>();
		if (name.Equals("name"))
		{
			foreach (var item in DataList)
			{
				if (item.name.Equals(value))
					tempList.Add(item);
			}
		}
		return tempList;
	}
}
