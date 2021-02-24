/*
 * Author: Johan A.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace DataCollection
{
    public static class Formatter
    {
        public static async Task WriteToFile<T>(List<T> data)
        {
            string fileName = "Assets/Scripts/DataCollection/test.json";
            string path = Path.Combine(Directory.GetCurrentDirectory(), fileName);

            if (!File.Exists(path))
            {
                // Create a file to write to.
                string heading = "File to store data";
                File.WriteAllText(path, heading);
            }
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.list = data;
            string toWrite = JsonUtility.ToJson(wrapper, true);
            using (StreamWriter outputFile = new StreamWriter(path) )
            {
                await outputFile.WriteAsync(toWrite);
            }
        }
        
        [Serializable]
        public class Wrapper<T>
        {
            public List<T> list;
        }
    }
    
}