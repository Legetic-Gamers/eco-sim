/*
 * Author: Johan A.
 */
using AnimalsV2;
using UnityEngine;

namespace DataCollection
{
    public class JsonFormatter
    {
        public void WriteDataToFile(Animal[] animals)
        {
            string json = JsonUtility.ToJson(animals.ToString(), true);
            // TODO fix path
            System.IO.File.WriteAllText(@"D:\path.txt", json);
        }
    }
}