/*=====================================================
* - Editor by tool
* - Don't editor by handself
=====================================================*/
using System;
using UnityEngine;
using System.Collections.Generic;
using Zero.ZeroEngine.Data;


[System.Serializable]
public class NpcExcel {
	public int id;
	public int type;
	public string name;
	public int inScene;
	public string speak;
	public int speakT;
	public int model;
	public string headIcon;
	public int field;
	public int interactive;
	public string dialog;
}
public class DataTableNpc: DataBase {
	[SerializeField]
	public List<NpcExcel> DataList = new List<NpcExcel>();
	private Dictionary<int, NpcExcel> _dataList = new Dictionary<int, NpcExcel>();
	public void AddInfo(NpcExcel _info)
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
	public Dictionary<int, NpcExcel> GetInfo()
	{
		return _dataList;
	}
	public NpcExcel GetInfoById(int _id)
	{
		NpcExcel info;
		if (_dataList.TryGetValue(_id, out info))
		{
			return info;
		}
		return null;
	}
	public List<NpcExcel> GetInfoByNameAndValue(string name,int value)
	{
		List<NpcExcel> tempList = new List<NpcExcel>();
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
		if (name.Equals("inScene"))
		{
			foreach (var item in DataList)
			{
				if (item.inScene== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("speakT"))
		{
			foreach (var item in DataList)
			{
				if (item.speakT== value)
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
		if (name.Equals("field"))
		{
			foreach (var item in DataList)
			{
				if (item.field== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("interactive"))
		{
			foreach (var item in DataList)
			{
				if (item.interactive== value)
					tempList.Add(item);
			}
		}
		return tempList;
	}
	public List<NpcExcel> GetInfoByNameAndValue(string name,string value)
	{
		List<NpcExcel> tempList = new List<NpcExcel>();
		if (name.Equals("name"))
		{
			foreach (var item in DataList)
			{
				if (item.name.Equals(value))
					tempList.Add(item);
			}
		}
		if (name.Equals("speak"))
		{
			foreach (var item in DataList)
			{
				if (item.speak.Equals(value))
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
		if (name.Equals("dialog"))
		{
			foreach (var item in DataList)
			{
				if (item.dialog.Equals(value))
					tempList.Add(item);
			}
		}
		return tempList;
	}
}
