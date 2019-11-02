/*=====================================================
* - Editor by tool
* - Don't editor by handself
=====================================================*/
using System;
using UnityEngine;
using System.Collections.Generic;
using Zero.ZeroEngine.Data;


[System.Serializable]
public class ActiveSkillExcel {
	public int id;
	public int index;
	public string name;
	public int iconID;
	public int level;
	public int lvMax;
	public string desc;
	public int normalBoo;
	public int useHP;
	public int useMP;
	public int useEP;
	public int bodyType;
	public int coolTime;
	public int upAction;
	public int singType;
	public int singAction;
	public int singTime;
	public int[] singEffect;
	public int attackAction;
	public int[] attackEffect;
	public int totalTime;
	public int lockTime;
	public int delayTime;
	public int skillEntityType;
	public int entityID;
	public int targetType;
	public int releaseDis;
	public int triggerType;
	public int triggerRate;
	public int toRoleBoo;
	public int toNpcBoo;
	public int aimType;
	public int[] aimValue;
	public int viewType;
	public int[] viewValue;
	public int skillAudio;
}
public class DataTableActiveSkill: DataBase {
	[SerializeField]
	public List<ActiveSkillExcel> DataList = new List<ActiveSkillExcel>();
	private Dictionary<int, ActiveSkillExcel> _dataList = new Dictionary<int, ActiveSkillExcel>();
	public void AddInfo(ActiveSkillExcel _info)
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
	public Dictionary<int, ActiveSkillExcel> GetInfo()
	{
		return _dataList;
	}
	public ActiveSkillExcel GetInfoById(int _id)
	{
		ActiveSkillExcel info;
		if (_dataList.TryGetValue(_id, out info))
		{
			return info;
		}
		return null;
	}
	public List<ActiveSkillExcel> GetInfoByNameAndValue(string name,int value)
	{
		List<ActiveSkillExcel> tempList = new List<ActiveSkillExcel>();
		if (name.Equals("id"))
		{
			foreach (var item in DataList)
			{
				if (item.id== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("index"))
		{
			foreach (var item in DataList)
			{
				if (item.index== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("iconID"))
		{
			foreach (var item in DataList)
			{
				if (item.iconID== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("level"))
		{
			foreach (var item in DataList)
			{
				if (item.level== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("lvMax"))
		{
			foreach (var item in DataList)
			{
				if (item.lvMax== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("normalBoo"))
		{
			foreach (var item in DataList)
			{
				if (item.normalBoo== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("useHP"))
		{
			foreach (var item in DataList)
			{
				if (item.useHP== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("useMP"))
		{
			foreach (var item in DataList)
			{
				if (item.useMP== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("useEP"))
		{
			foreach (var item in DataList)
			{
				if (item.useEP== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("bodyType"))
		{
			foreach (var item in DataList)
			{
				if (item.bodyType== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("coolTime"))
		{
			foreach (var item in DataList)
			{
				if (item.coolTime== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("upAction"))
		{
			foreach (var item in DataList)
			{
				if (item.upAction== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("singType"))
		{
			foreach (var item in DataList)
			{
				if (item.singType== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("singAction"))
		{
			foreach (var item in DataList)
			{
				if (item.singAction== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("singTime"))
		{
			foreach (var item in DataList)
			{
				if (item.singTime== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("attackAction"))
		{
			foreach (var item in DataList)
			{
				if (item.attackAction== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("totalTime"))
		{
			foreach (var item in DataList)
			{
				if (item.totalTime== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("lockTime"))
		{
			foreach (var item in DataList)
			{
				if (item.lockTime== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("delayTime"))
		{
			foreach (var item in DataList)
			{
				if (item.delayTime== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("skillEntityType"))
		{
			foreach (var item in DataList)
			{
				if (item.skillEntityType== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("entityID"))
		{
			foreach (var item in DataList)
			{
				if (item.entityID== value)
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
		if (name.Equals("releaseDis"))
		{
			foreach (var item in DataList)
			{
				if (item.releaseDis== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("triggerType"))
		{
			foreach (var item in DataList)
			{
				if (item.triggerType== value)
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
		if (name.Equals("toRoleBoo"))
		{
			foreach (var item in DataList)
			{
				if (item.toRoleBoo== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("toNpcBoo"))
		{
			foreach (var item in DataList)
			{
				if (item.toNpcBoo== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("aimType"))
		{
			foreach (var item in DataList)
			{
				if (item.aimType== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("viewType"))
		{
			foreach (var item in DataList)
			{
				if (item.viewType== value)
					tempList.Add(item);
			}
		}
		if (name.Equals("skillAudio"))
		{
			foreach (var item in DataList)
			{
				if (item.skillAudio== value)
					tempList.Add(item);
			}
		}
		return tempList;
	}
	public List<ActiveSkillExcel> GetInfoByNameAndValue(string name,string value)
	{
		List<ActiveSkillExcel> tempList = new List<ActiveSkillExcel>();
		if (name.Equals("name"))
		{
			foreach (var item in DataList)
			{
				if (item.name.Equals(value))
					tempList.Add(item);
			}
		}
		if (name.Equals("desc"))
		{
			foreach (var item in DataList)
			{
				if (item.desc.Equals(value))
					tempList.Add(item);
			}
		}
		return tempList;
	}
}
