/*=====================================================
* - Editor by tool
* - Don't editor by handself
=====================================================*/
using System;
using UnityEngine;
using System.Collections.Generic;
using Zero.ZeroEngine.Data;


[System.Serializable]
public class TransferExcel {
	public int id;
	public string name;
	public int mapId;
	public int toMapId;
	public int model;
}
public class DataTableTransfer: DataBase {
	[SerializeField]
	public List<TransferExcel> DataList = new List<TransferExcel>();
	private Dictionary<int, TransferExcel> _dataList = new Dictionary<int, TransferExcel>();
	public void AddInfo(TransferExcel _info)
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
	public Dictionary<int, TransferExcel> GetInfo()
	{
		return _dataList;
	}
	public TransferExcel GetInfoById(int _id)
	{
		TransferExcel info;
		if (_dataList.TryGetValue(_id, out info))
		{
			return info;
		}
		return null;
	}
	public List<TransferExcel> GetInfoByNameAndValue(string name,int value)
	{
		List<TransferExcel> tempList = new List<TransferExcel>();
		if (name.Equals("id"))
		{
			foreach (var item in DataList)
			{
				if (item.id== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("mapId"))
		{
			foreach (var item in DataList)
			{
				if (item.mapId== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("toMapId"))
		{
			foreach (var item in DataList)
			{
				if (item.toMapId== value)
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
		return tempList;
	}
	public List<TransferExcel> GetInfoByNameAndValue(string name,string value)
	{
		List<TransferExcel> tempList = new List<TransferExcel>();
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
