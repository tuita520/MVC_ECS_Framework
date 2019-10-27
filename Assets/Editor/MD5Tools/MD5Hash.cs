//=====================================================
// - FileName:      MD5Hash.cs
// - Created:       #AuthorName#
// - UserName:      #CreateTime#
// - Email:         1023276156@qq.com
// - Description:   
// -  (C) Copyright 2018 - 2019
// -  All Rights Reserved.
//======================================================
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

public class MD5Hash{

    public static string Get(FileStream fs)
    {
        try
        {
            int len = (int)fs.Length;
            byte[] data = new byte[len];
            fs.Read(data, 0, len);
            fs.Close();
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] result = md5.ComputeHash(data);
            string fileMD5 = "";
            foreach(byte b in result)
            {
                fileMD5 += Convert.ToString(b, 16);
            }
            return fileMD5;
        }
        catch(FileNotFoundException e)
        {
            Console.WriteLine(e.Message);
            return "";
        }
    }
}
