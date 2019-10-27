/*=====================================================
* - Editor by tool
* - Don't editor by handself
=====================================================*/
using System;
using UnityEngine;
using System.Collections.Generic;
using Zero.ZeroEngine.Data;


[System.Serializable]
public class UiExcel {
	public int id;
	public string desc;
	public string name;
	public string path;
	public int holdBoo;
}
public class DataTableUi: DataBase {
	[SerializeField]
	public List<UiExcel> DataList = new List<UiExcel>();
	private Dictionary<int, UiExcel> _dataList = new Dictionary<int, UiExcel>();
	public void AddInfo(UiExcel _info)
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
	public Dictionary<int, UiExcel> GetInfo()
	{
		return _dataList;
	}
	public UiExcel GetInfoById(int _id)
	{
		UiExcel info;
		if (_dataList.TryGetValue(_id, out info))
		{
			return info;
		}
		return null;
	}
	public List<UiExcel> GetInfoByNameAndValue(string name,int value)
	{
		List<UiExcel> tempList = new List<UiExcel>();
		if (name.Equals("id"))
		{
			foreach (var item in DataList)
			{
				if (item.id== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("holdBoo"))
		{
			foreach (var item in DataList)
			{
				if (item.holdBoo== value)
					tempList.Add(item);
			}
		}
		return tempList;
	}
	public List<UiExcel> GetInfoByNameAndValue(string name,string value)
	{
		List<UiExcel> tempList = new List<UiExcel>();
		if (name.Equals("desc"))
		{
			foreach (var item in DataList)
			{
				if (item.desc.Equals(value))
					tempList.Add(item);
			}
		}
		if (name.Equals("name"))
		{
			foreach (var item in DataList)
			{
				if (item.name.Equals(value))
					tempList.Add(item);
			}
		}
		if (name.Equals("path"))
		{
			foreach (var item in DataList)
			{
				if (item.path.Equals(value))
					tempList.Add(item);
			}
		}
		return tempList;
	}
}
