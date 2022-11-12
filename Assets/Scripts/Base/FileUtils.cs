using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class FileUtils {

    public static T[] GetAllResoursesOfTypeAtPath<T>(string path) {
        ArrayList al = new ArrayList();
        string[] fileEntries = Directory.GetFiles(Application.dataPath + "/Resources/" + path);
        foreach (string fileName in fileEntries) {
            string tempName = fileName.Replace("\\", "/");

            int index = tempName.LastIndexOf(".");
            if (index == -1)
                continue;
            else if (index != -1)
                tempName = tempName.Substring(0, index);
            index = tempName.LastIndexOf("/Resources/");
            if (index != -1)
                tempName = tempName.Substring(index + "/Resources/".Length);
            
            Object t = Resources.Load(tempName);

            if ((t != null) &&
                    (t.GetType() == typeof(T))){
                al.Add(t);
            }
        }
        T[] result = new T[al.Count];
        for (int i = 0; i < al.Count; i++)
            result[i] = (T)al[i];
        return result;
    }


}