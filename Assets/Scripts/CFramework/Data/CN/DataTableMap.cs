/*=====================================================
* - Editor by tool
* - Don't editor by handself
=====================================================*/
using System;
using UnityEngine;
using System.Collections.Generic;
using Zero.ZeroEngine.Data;


[System.Serializable]
public class MapExcel {
	public int map_id;
	public int mapType;
	public int fbType;
	public string name;
	public int mapResId;
	public string musicBg;
	public string mapDesc;
	public string mapIcon;
	public int mapPKModel;
	public int openLv;
	public int reliveType;
}
public class DataTableMap: DataBase {
	[SerializeField]
	public List<MapExcel> DataList = new List<MapExcel>();
	private Dictionary<int, MapExcel> _dataList = new Dictionary<int, MapExcel>();
	public void AddInfo(MapExcel _info)
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
			_dataList.Add(info.map_id, info);
		}
	}
	public Dictionary<int, MapExcel> GetInfo()
	{
		return _dataList;
	}
	public MapExcel GetInfoById(int _id)
	{
		MapExcel info;
		if (_dataList.TryGetValue(_id, out info))
		{
			return info;
		}
		return null;
	}
	public List<MapExcel> GetInfoByNameAndValue(string name,int value)
	{
		List<MapExcel> tempList = new List<MapExcel>();
		if (name.Equals("map_id"))
		{
			foreach (var item in DataList)
			{
				if (item.map_id== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("mapType"))
		{
			foreach (var item in DataList)
			{
				if (item.mapType== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("fbType"))
		{
			foreach (var item in DataList)
			{
				if (item.fbType== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("mapResId"))
		{
			foreach (var item in DataList)
			{
				if (item.mapResId== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("mapPKModel"))
		{
			foreach (var item in DataList)
			{
				if (item.mapPKModel== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("openLv"))
		{
			foreach (var item in DataList)
			{
				if (item.openLv== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("reliveType"))
		{
			foreach (var item in DataList)
			{
				if (item.reliveType== value)
					tempList.Add(item);
			}
		}
		return tempList;
	}
	public List<MapExcel> GetInfoByNameAndValue(string name,string value)
	{
		List<MapExcel> tempList = new List<MapExcel>();
		if (name.Equals("name"))
		{
			foreach (var item in DataList)
			{
				if (item.name.Equals(value))
					tempList.Add(item);
			}
		}
		if (name.Equals("musicBg"))
		{
			foreach (var item in DataList)
			{
				if (item.musicBg.Equals(value))
					tempList.Add(item);
			}
		}
		if (name.Equals("mapDesc"))
		{
			foreach (var item in DataList)
			{
				if (item.mapDesc.Equals(value))
					tempList.Add(item);
			}
		}
		if (name.Equals("mapIcon"))
		{
			foreach (var item in DataList)
			{
				if (item.mapIcon.Equals(value))
					tempList.Add(item);
			}
		}
		return tempList;
	}
}
