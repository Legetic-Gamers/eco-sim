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
        /// <summary>
        /// Writes a list to a file given the name of the file and field to print, asynchronously.
        /// </summary>
        /// <param name="data"> Data to write </param>
        /// <param name="filename"> Name of the file </param>
        /// <typeparam name="T"> Generic </typeparam>
        /// <returns> returns when the thread is finished writing. </returns>
        public static async Task WriteToFile<T>(List<T> data, string filename)
        {
            string pathToFile = Path.Combine("Assets/Scripts/DataCollection/Export/", filename + ".json");
            string path = Path.Combine(Directory.GetCurrentDirectory(), pathToFile);
            
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
        
        /// <summary>
        /// Wrapper class so Unitys build in Json utility can be used. 
        /// </summary>
        /// <typeparam name="T"> Generic </typeparam>
        [Serializable]
        public class Wrapper<T>
        {
            public List<T> list;
        }
    }
    
}