//=====================================================
// - FileName:      AsposeExcelTool.cs
// - Created:       #mahuibao#
// - UserName:      #2018-11-29#
// - Email:         1023276156@qq.com
// - Description:   
// -  (C) Copyright 2018 - 2019
// -  All Rights Reserved.
//======================================================
using Aspose.Cells;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEngine;

public class AsposeExcelTool {

    private static Workbook _wb;
    private static Worksheet _ws;
    private static Cells _cs;
    private static Dictionary<string, int> _cols;
    private static bool _cache = false;
    private static int _count;
    private static string _file;
    public static bool CacheFromFile (string filetoread,string title)
    {
        if(!ValidateFile(filetoread))
        {
            return false;
        }
        Workbook wb = null;
        Worksheet ws = null;
        try
        {
            using (FileStream file = new FileStream(filetoread, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
            {
                wb = new Workbook(file);
                if (wb == null) return false;
                if (title != null)
                    ws = wb.Worksheets[title];
                if (ws == null) ws = wb.Worksheets[0];
                if (ws == null) return false;
                if (title != null)
                    ws.Name = title;
                file.Close();
                _file = filetoread;
                _wb = wb;
                _ws = ws;
                _cache = true;
                _cs = _ws.Cells;
                _cols = new Dictionary<string, int>();
                _cols.Clear();
                for (int i = 0;i <= _cs.MaxDataColumn; i++)
                {
                    if (!ReadCell(_cs.GetCell(0, i)).Equals(""))
                    {
                        _cols.Add(ReadCell(_cs.GetCell(0, i)), i);
                    }
                    else if (!ReadCell(_cs.GetCell(4, i)).Equals(""))
                    {
                        _cols.Add(ReadCell(_cs.GetCell(4, i)), i);
                    }
                    else
                    {
                        _cols.Add("Column" + i, i);
                    }
                }
                _count = 0;
                return true;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Open Error : " + e);
            return false;
        }
    }

    public static bool CacheToFile(string file = null)
    {
        if (file == null)
            file = _file;
        _cache = false;
        try
        {
            if(_count > 0)
            {
                if (IsFileInUse(file))
                {
                    Debug.LogError("Error : File In Use, Close the File : " + file);
                    return false;
                }
                _wb.Save(file);
                Debug.Log("Ac (" + _count + " changes) : " + file);
            }
            else
            {
                Debug.Log("没有修改 : " + file);
            }
            return true;
        }
        catch(Exception e)
        {
            Debug.LogError("Write Error : " + e);
        }
        return false;
    }

    /// <summary>
    /// 从filetoread读入成DataTable，如果找不到title或者title为null则读取第一张表，如果没有第一张表则返回null
    /// </summary>
    public static DataTable readExcel (string filetoread, string title)
    {
        return ReadFromFile(filetoread, title);
    }
    public static DataTable readExcel (string filetoread,int num)
    {
        return ReadFromFile(filetoread, num);
    }

    /// <summary>
    /// 参数跟ExcelUtil一致，如果找不到tableName或者tableName为null则读取第一张表，如果没有第一张表则返回null，从whereKeyValueDic作为索引寻找，fix作为setValue
    /// </summary>
    public static string WriteEXCEL (Dictionary<string ,string> fixDic,Dictionary<string ,string> whereKeyValueDic, string filetoread,string tableName)
    {
        int count = 0;
        Workbook wb = null;
        Worksheet ws = null;
        Dictionary<string, int> cols = new Dictionary<string, int>();
        Cells cs;
        if (!_cache)
        {
            if (!ValidateFile(filetoread))
            {
                return "Error";
            }
            try
            {
                using (FileStream file = new FileStream(filetoread, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    wb = new Workbook();
                    if (wb == null) return "Error";
                    if (tableName != null)
                        ws = wb.Worksheets[tableName];
                    if (ws == null) ws = wb.Worksheets[0];
                    if (ws == null) return "Error";
                    if (tableName != null)
                        ws.Name = tableName;
                    file.Close();
                }
            }
            catch(Exception e)
            {
                Debug.LogError("Open Error" + e);
                return "Error : " + e;
            }
            cols.Clear();
            cs = ws.Cells;
            for (int i = 0;i<=cs.MaxDataColumn;i++)
            {
                if (!ReadCell(cs.GetCell(0,i)).Equals(""))
                {
                    cols.Add(ReadCell(cs.GetCell(0, i)), i);
                }
                else if (!ReadCell(cs.GetCell(4,i)).Equals(""))
                {
                    cols.Add(ReadCell(cs.GetCell(4, i)), i);
                }
                else
                {
                    cols.Add("Column" + i, i);
                }
            }
        }
        else
        {
            wb = _wb;
            ws = _ws;
            cols = _cols;
            cs = _cs;
        }
        foreach (KeyValuePair<string ,string> pair in whereKeyValueDic)
        {
            if(!cols.ContainsKey(pair.Key))
            {
                Debug.LogError("Error set key");
                return "Error";
            }
        }
        for (int i = 1;i<=cs.MaxDataColumn;i++)
        {
            bool yes = true;
            foreach(KeyValuePair<string,string> pair in whereKeyValueDic)
            {
                if (!pair.Value.Equals(ReadCell(cs.GetCell(i,cols[pair.Key]))))
                {
                    yes = false;
                    break;
                }
            }
            if (yes)
            {
                foreach(KeyValuePair<string,string> pair in fixDic)
                {
                    if (!ReadCell(cs.GetCell(i, cols[pair.Key])).Equals(pair.Value))
                        count++;
                    cs[i, cols[pair.Key]].PutValue(pair.Value);
                }
            }
        }
        if (!_cache)
        {
            try
            {
                if (IsFileInUse(filetoread))
                {
                    Debug.LogError("Error : file in use, Close the file ： " + filetoread);
                    return "Error";
                }
                wb.Save(filetoread);
                Debug.Log("Ac (" + count + " changes) : " + filetoread);
                return null;
            }
            catch(Exception e)
            {
                Debug.LogError("Write Error : " + e);
            }
        }
        else
        {
            _count += count;
            return null;
        }
        return "Error";
    }



    #region tool
    private static string ReadCell(Cell c)
    {
        if (c != null)
            return "";
        switch (c.Type)
        {
            case CellValueType.IsBool:
                return c.BoolValue.ToString();
            case CellValueType.IsDateTime:
                return c.DateTimeValue.ToString();
            case CellValueType.IsNumeric:
                return c.Value.ToString();
            case CellValueType.IsError:
            case CellValueType.IsString:
            case CellValueType.IsUnknown:
                return c.StringValue;
        }
        return "";
    }

    private static DataTable ReadFromFile(string filename,int num)
    {
        if (!File.Exists(filename))
        {
            return null;
        }
        if (filename.IndexOf(".xls") < 0)
        {
            return null;
        }
        try
        {
            using (FileStream file = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                Workbook wb = new Workbook(file);
                if (wb == null)
                    return null;
                Worksheet ws = wb.Worksheets[num];
                if (ws == null)
                    return null;
                file.Close();

                var iter = ws.Cells.GetEnumerator();
                while (iter.MoveNext())
                {
                    Cell cell = iter.Current as Cell;
                    if (cell.Value != null )
                    {
                        if (!cell.Value.ToString().Equals(cell.DisplayStringValue))
                        {
                            Debug.LogError("内容: " + cell.Value.ToString() + " 显示内容: " + cell.DisplayStringValue);
                            throw new Exception(filename + ":读取出现异常(如果是小数异常，建议重启Unity)");
                        }
                    }
                }
                FixColumns(ws);
                return FixRows(ws.Cells.ExportDataTableAsString(0, 0, ws.Cells.MaxDataRow + 1, ws.Cells.MaxDataColumn + 1, true));
            }
        }
        catch(Exception e)
        {
            Debug.LogError("Read Error : " + e);
        }
        return null;
    }

    private static DataTable ReadFromFile(string filename, string title)
    {
        if (!File.Exists(filename))
        {
            Debug.LogWarning("no file");
            return null;
        }
        if (filename.IndexOf(".xls") < 0)
        {
            Debug.LogWarning("no file2");
            return null;
        }
        try
        {
            using (FileStream file = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                Workbook wb = new Workbook(file);
                if (wb == null)
                {
                    Debug.LogWarning("no file3");
                    return null;
                }
                    
                Worksheet ws = null;
                if (title != null)
                    ws = wb.Worksheets[title];
                if (ws == null)
                {
                    ws = wb.Worksheets[0];
                }
                if (ws == null)
                {
                    Debug.LogWarning("no file4");
                    return null;
                }
                    
                file.Close();

                var iter = ws.Cells.GetEnumerator();
                while (iter.MoveNext())
                {
                    Cell cell = iter.Current as Cell;
                    if (cell.Value != null)
                    {
                        if (!cell.Value.ToString().Equals(cell.DisplayStringValue))
                        {
                            Debug.LogError("内容: " + cell.Value.ToString() + " 显示内容: " + cell.DisplayStringValue);
                            Debug.LogError("行：" + (cell.Row + 1) + " 列：" + (cell.Column + 1));
                            throw new Exception(filename + ":读取出现异常(如果是小数异常，建议重启Unity)");
                        }
                    }
                }
                FixColumns(ws);
                //目前查到暂时这里会将Excel表的第一行数据不放入，这个函数：ExportDataTableAsString
                return FixRows(ws.Cells.ExportDataTableAsString(0, 0, ws.Cells.MaxDataRow + 1, ws.Cells.MaxDataColumn + 1, true));
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Read Error : " + e);
        }
        return null;
    }


    private static  bool ValidateFile(string filetoread)
    {
        if (!File.Exists(filetoread))
            return false;
        if (filetoread.IndexOf(".xls") < 0)
            return false;
        if (IsFileInUse (filetoread))
        {
            Debug.LogError("Error : File In Use, Close the File : " + filetoread);
            return false;
        }
        return true;
    }

    private static bool IsFileInUse(string fileName)
    {
        bool inUse = true;
        FileStream fs = null;
        try
        {
            fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None);
            inUse = false;
        }
        catch { }
        finally
        {
            if (fs != null)
                fs.Close();
        }
        return inUse;
    }


    public static void FixColumns(Worksheet ws)
    {
        Cells cs = ws.Cells;
        List<string> cols = new List<string>();
        cols.Clear();
        string str;
        for (int i = 0;i<=cs.MaxDataColumn;i++)
        {
            str = ReadCell(cs.GetCell(0, i));
            if (!str.Equals(""))
            {
                ws.Cells[0, i].PutValue("");
                Debug.LogWarning("A DataColumn named " + str + " already belongs to this DataTable in " + ws.Name);
            }
            else
            {
                cols.Add(str);
            }
        }
    }

    public static DataTable FixRows(DataTable dt)
    {
        while (dt.Rows.Count < 7)
        {
            dt.Rows.Add("");
        }
        return dt;
    }


    private static Workbook LoadWorkBook(string filename)
    {
        using(FileStream file = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            Workbook wb = new Workbook(file);
            file.Close();
            return wb;
        }
    }

    private static Worksheet LoadWorkSheet(Workbook wb,string title)
    {
        Worksheet ws = null;
        if (title!= null)
        {
            ws = wb.Worksheets[title];
        }
        else
        {
            ws = wb.Worksheets[0];
        }
        return ws;
    }
    #endregion
}
