/*
 * Author: Johan A.
 */

using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace DataCollection
{
    public class JsonFormatter : MonoBehaviour
    {
        /*private void Awake()
        {
            /*string json = JsonUtility.ToJson(animals.ToString(), true);#1#
            /*System.IO.File.WriteAllText(@"D:\path.txt", json);#1#
            string fileName = "test.txt";
            string path = Path.GetFullPath(fileName);
            Debug.Log(path);
            
            if (!File.Exists(path))
            {
                // Create a file to write to.
                string[] createText = { "Hello", "And", "Welcome" };
                Debug.Log("Writing2?");
                File.WriteAllLines(path, createText);
            }
            
            
            string appendText = "This is extra text" + Environment.NewLine;
            File.AppendAllText(path, appendText);
            
        }

        /*public static async Task<> ExampleAsync()
        {
            using StreamWriter file = new("WriteLines2.txt", append: true);
            await file.WriteLineAsync("Fourth line");
        }#1#*/
    }
}