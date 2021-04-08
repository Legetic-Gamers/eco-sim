/*
 * Author: Johan A. 
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Model;
using UnityEngine;
using static DataCollection.Formatter;
using Debug = UnityEngine.Debug;

namespace DataCollection
{
    /// <summary>
    /// Must be added to the scene. 
    /// </summary>
    public class DataHandler : MonoBehaviour
    {
        public Action<List<float>, List<float>> Display;
        
        [SerializeField]
        private bool ShowFrameRate;
        public int granularity = 5; // how many frames to wait until you re-calculate the FPS
        List<float> times;
        private List<float> framerate;
        private int counter;
        
        private TickEventPublisher tickEventPublisher;
        public Collector c;
        private List<float> sendList1 = new List<float>();
        private List<float> sendList2 = new List<float>();
        
        private static int _speciesNumber1 = 0;
        private static int _traitNumber1 = 0;
        private static int _dataTypeNumber1 = 0;
        private static int _speciesNumber2 = 0;
        private static int _traitNumber2 = 0;
        private static int _dataTypeNumber2 = 0;


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
        public void Awake()
        {
            tickEventPublisher = FindObjectOfType<global::TickEventPublisher>();
            // Subscribe to Tick Event publisher update data and graph 
            tickEventPublisher.onCollectorUpdate += UpdateDataAndGraph;
            //ButtonClick bc = FindObjectOfType<ButtonClick>();
            //bc.GetListType += SetList;
            // Make a collector to handle data
            c = new Collector();
            // Prepare for frame rate collection
            times = new List<float>(0);
            framerate = new List<float>(10);
            counter = 5;
        }
            
        /// <summary>
        /// Calculates the average frame rate over *granularity* frames and adds to the *framerate* list. 
        /// </summary>
        public void Update()
        {
            if (ShowFrameRate)
            {
                if (counter <= 0)
                {
                    float sum = 0;
                    foreach (float framerate in times) sum += framerate;

                    float average = sum / times.Count;
                    float fps = 1 / average;
                    framerate.Add(fps);
                    counter = granularity;
                }

                times.Add(Time.deltaTime);
                counter--;
            }
        }
        
        /// <summary>
        /// Called by Animal Controller to log when a new animal is started in the scene.
        /// </summary>
        /// <param name="animalModel"> Model of animal to log the traits of. </param>
        public void LogNewAnimal(AnimalModel animalModel)
        {
            c.CollectBirth(animalModel);
        }
        
        /// <summary>
        /// Called when an animal dies. 
        /// </summary>
        /// <param name="animalModel"> The model of the dead animal </param>
        /// <param name="causeOfDeath"> The Cause that made the animal call on dead state </param>
        public void LogDeadAnimal(AnimalModel animalModel, AnimalModel.CauseOfDeath causeOfDeath, float dist)
        {
            c.CollectDeath(animalModel, causeOfDeath, dist);
        }
        
        /// <summary>
        /// Called when plants are activated
        /// </summary>
        /// <param name="plantModel"> The model of the plant </param>
        public void LogNewPlant(PlantModel plantModel)
        {
            c.CollectNewFood(plantModel);
        }
        
        /// <summary>
        /// Called when a plant is eaten
        /// </summary>
        /// <param name="plantModel"> The model of the eaten plant </param>
        public void LogDeadPlant(PlantModel plantModel)
        {
            c.CollectDeadFood(plantModel);
        }

        private void SetList(int a, int x, int y, int z)
        {
            List<float> tmplist = new List<float>();
            switch (x)
            {
                case 0:
                    if (z == 0) tmplist = c.rabbitStatsPerGenMean[y];
                    if (z == 1) tmplist = c.rabbitStatsPerGenVar[y];
                    break;
                case 1:
                    if (z == 0) tmplist = c.wolfStatsPerGenMean[y];
                    if (z == 1) tmplist = c.wolfStatsPerGenVar[y];
                    break;      
                case 2:         
                    if (z == 0) tmplist = c.deerStatsPerGenMean[y];
                    if (z == 1) tmplist = c.deerStatsPerGenVar[y];
                    break;      
                case 3:         
                    if (z == 0) tmplist = c.bearStatsPerGenMean[y];
                    if (z == 1) tmplist = c.bearStatsPerGenVar[y];
                    break;
            }

            if (a == 0)
            {
                sendList1 = tmplist;
                _speciesNumber1 = x;
                _traitNumber1 = y;
                _dataTypeNumber1 = z;
            }

            else
            {
                sendList2 = tmplist;
                _speciesNumber2 = x;
                _traitNumber2 = y;
                _dataTypeNumber2 = z;
            }
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

        // ReSharper disable Unity.PerformanceAnalysis
        /// <summary>
        /// On each tick from Event Publisher collect time series data and send to graph and write to file. 
        /// </summary>
        private void UpdateDataAndGraph()
        {
            //c.Collect();

            Debug.Log(c.totalAnimalsAlive);
            SetList(0,_speciesNumber1,_traitNumber1,_dataTypeNumber1);
            SetList(1,_speciesNumber2,_traitNumber2,_dataTypeNumber2);
            Display?.Invoke(sendList1, sendList2);
            //if (ShowFrameRate) Display(ConvertFloatListToIntList(framerate));
            //ExportDataToFile(0);
            Debug.Log(c.totalAnimalsAlive);
        }
        
        /// <summary>
        /// Converts a list of floats to int by casting. Used when passing data to graph. 
        /// </summary>
        /// <param name="list"> List to be cast to int.</param>
        /// <returns></returns>
        private List<int> ConvertFloatListToIntList(List<float> list)
        {
            List<int> integerList = new List<int>();

            foreach (float f in list.ToArray()) integerList.Add((int) f);
            return integerList;
        }
        
    }
}