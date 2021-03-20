/*
 * Author: Johan A. 
 */
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static DataCollection.Formatter;

namespace DataCollection
{
    /// <summary>
    /// Must be added to the scene. 
    /// </summary>
    public class DataHandler : MonoBehaviour
    {
        public Action<List<int>, List<int>> Display;
        
        private TickEventPublisher tickEventPublisher;
        private Collector c;
        private static List<int> _sendList1 = new List<int>();
        private static  List<int> _sendList2 = new List<int>();
        private List<string> traitNames = new List<string>
        {
            "size",
            "maxEnergy", 
            "maxHealth",
            "maxHydration",
            "maxSpeed",
            "endurance", 
            "ageLimit", 
            "temperatureResist", 
            "desirability", 
            "viewAngle", 
            "viewRadius", 
            "hearingRadius",
            "age"
        };

        /// <summary>
        /// Find the Tick Event Publisher and subscribe to collecting tick
        /// Create a new collector to handle data manipulation. 
        /// </summary>
        private void Awake()
        {
            tickEventPublisher = FindObjectOfType<global::TickEventPublisher>();
            tickEventPublisher.onCollectorUpdate += UpdateDataAndGraph;
            ButtonClick bc = FindObjectOfType<ButtonClick>();
            bc.GetListType += SetList;
            c = new Collector();
        }
        
        
        /// <summary>
        /// Called by Animal Controller to log when a new animal is started in the scene.
        /// </summary>
        /// <param name="animalModel"> Model of animal to log the traits of. </param>
        public void LogNewAnimal(AnimalModel animalModel)
        {
            c.CollectBirth(animalModel);
        }
        
        public void LogDeadAnimal(AnimalModel animalModel)
        {
            c.CollectDeath(animalModel);
        }

        private void SetList(int a, int x, int y, int z)
        {
            List<int> tmplist = new List<int>();

            switch (x)
            {
                case 0:
                    if (z == 0) tmplist = ConvertFloatListToIntList(c.rabbitStatsPerGenMean[y]);
                    if (z == 1) tmplist = ConvertFloatListToIntList((c.rabbitStatsPerGenVar[y]));
                    break;
                case 1:
                    if (z == 0) tmplist = ConvertFloatListToIntList(c.wolfStatsPerGenMean[y]);
                    if (z == 1) tmplist = ConvertFloatListToIntList((c.wolfStatsPerGenVar[y]));
                    break;      
                case 2:         
                    if (z == 0) tmplist = ConvertFloatListToIntList(c.deerStatsPerGenMean[y]);
                    if (z == 1) tmplist = ConvertFloatListToIntList((c.deerStatsPerGenVar[y]));
                    break;      
                case 3:         
                    if (z == 0) tmplist = ConvertFloatListToIntList(c.bearStatsPerGenMean[y]);
                    if (z == 1) tmplist = ConvertFloatListToIntList((c.bearStatsPerGenVar[y]));
                    break;
            }

            if (a == 0) _sendList1 = tmplist;
            
            else _sendList2 = tmplist;
        }
        
        /// <summary>
        /// Uses the Formatter to print a list in json format in /Export. name will match trait name. 
        /// </summary>
        /// <param name="listNumber"> List to be printed. See Traits class for order. 0 to 12 </param>
        /// <returns></returns>
        private async Task ExportDataToFile(int listNumber)
        {
            await WriteToFile(c.rabbitStatsPerGenMean[listNumber], traitNames[listNumber]);
        }

        /// <summary>
        /// On each tick from Event Publisher collect time series data and send to graph and write to file. 
        /// </summary>
        private void UpdateDataAndGraph()
        {
            c.Collect();
            Display?.Invoke(_sendList1, _sendList2);
            //ExportDataToFile(0);
        }
        
        /// <summary>
        /// Converts a list of floats to int by casting. Used when passing data to graph. 
        /// </summary>
        /// <param name="list"> List to be cast to int.</param>
        /// <returns></returns>
        private List<int> ConvertFloatListToIntList(List<float> list)
        {
            List<int> integerList = new List<int>();
            foreach (float f in list.ToArray())
            {
                integerList.Add((int) f);
            }

            return integerList;
        }
    }
}