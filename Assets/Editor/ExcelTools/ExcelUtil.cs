//=====================================================
// - FileName:      ExcelUtil.cs
// - Created:       #mahuibao#
// - UserName:      #2018-11-26#
// - Email:         1023276156@qq.com
// - Description:   
// -  (C) Copyright 2018 - 2019
// -  All Rights Reserved.
//======================================================
using Excel;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.IO;
using UnityEngine;

public class ExcelUtil
{

    public static DataTable readExcel(string filetoread, string title)
    {
#if !ASPOSE_READ && ASPOSE
            OdbcConnection oCon = null;
            OdbcDataReader rData = null;
            try 
            {
                DataTable dtYourData = new DataTable();
                string con = "Driver={Microsoft Excel Driver (*.xls, *.xlsx, *.xlsm, *.xlsb)};Dbq=" + filetoread + ";";
                string yourQuery = "SELECT * FROM [" + title + "$]";
                oCon = new OdbcConnection(con);
                OdbcCommand oCmd = new OdbcCommand(yourQuery, oCon);
                oCon.Open();
                rData = oCmd.ExecuteReader();
                dtYourData.Load(rData);
                rData.Close();
                oCon.Close();
                return dtYourData;
            }
            catch(System.Exception ex)
            {
                if(null != rData)
                {
                    rData.Close();
                }
                if(null != oCon)
                {
                    oCon.Close();
                }
                Debug.LogError(filetoread + "读取错误" + ex.Message);
                return null;
            }
#else
        Debug.Log("readExcel");
        return AsposeExcelTool.readExcel(filetoread, title);
#endif
    }

}