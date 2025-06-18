using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using static System.IO.StreamWriter;
using TMPro;
using SFB;
using UnityEngine.UI;

public class FileWriter : MonoBehaviour
{
    void Start()
    {   
    }

    public static void WriteFile(string path, string objContent, string mtlContent, string voxContent){//name

        if(!Directory.Exists("Exported_Models")){
            DirectoryInfo dirInfo = new DirectoryInfo("Exported_Models");
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }
        }
        string name = path.Split("\\")[path.Split("\\").Length-1];
        string[] a = objContent.Split("model1");
        objContent = a[0] + name + a[1];

        System.IO.StreamWriter obj = new System.IO.StreamWriter(path+".obj");//"Exported_Models/"+name
        obj.Write(objContent);
        obj.Close();

        System.IO.StreamWriter mtl = new System.IO.StreamWriter(path+".mtl");
        mtl.Write(mtlContent);
        mtl.Close();

        System.IO.StreamWriter vox = new System.IO.StreamWriter(path+".vox");
        vox.Write(voxContent);
        vox.Close();
    }





    public static void SaveState(string voxContent)
    {
        string dirPath = Path.Combine(Application.dataPath, "SavedState");
        string filePath = Path.Combine(dirPath, "SavedState.vox");

        GameObject.Find("Console1").GetComponent<TextMeshProUGUI>().text = filePath;

        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }

        using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
        using (var bs = new BufferedStream(fs, 16384))
        using (var sw = new StreamWriter(bs))
        {
            sw.Write(voxContent);
        }
    }
}


