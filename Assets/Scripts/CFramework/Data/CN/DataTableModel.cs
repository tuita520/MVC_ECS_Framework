/*=====================================================
* - Editor by tool
* - Don't editor by handself
=====================================================*/
using System;
using UnityEngine;
using System.Collections.Generic;
using Zero.ZeroEngine.Data;


[System.Serializable]
public class ModelExcel {
	public int id;
	public string name;
	public string path;
}
public class DataTableModel: DataBase {
	[SerializeField]
	public List<ModelExcel> DataList = new List<ModelExcel>();
	private Dictionary<int, ModelExcel> _dataList = new Dictionary<int, ModelExcel>();
	public void AddInfo(ModelExcel _info)
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
	public Dictionary<int, ModelExcel> GetInfo()
	{
		return _dataList;
	}
	public ModelExcel GetInfoById(int _id)
	{
		ModelExcel info;
		if (_dataList.TryGetValue(_id, out info))
		{
			return info;
		}
		return null;
	}
	public List<ModelExcel> GetInfoByNameAndValue(string name,int value)
	{
		List<ModelExcel> tempList = new List<ModelExcel>();
		if (name.Equals("id"))
		{
			foreach (var item in DataList)
			{
				if (item.id== value)
					tempList.Add(item);
			}
		}
		return tempList;
	}
	public List<ModelExcel> GetInfoByNameAndValue(string name,string value)
	{
		List<ModelExcel> tempList = new List<ModelExcel>();
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
