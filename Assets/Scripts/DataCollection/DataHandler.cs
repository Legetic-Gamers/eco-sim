/*
 * Author: Johan A. 
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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
        public Action<List<int>> Display;
        
        [SerializeField]
        private bool ShowFrameRate;
        public int granularity = 5; // how many frames to wait until you re-calculate the FPS
        List<float> times;
        private List<float> framerate;
        private int counter;
        
        private TickEventPublisher tickEventPublisher;
        private Collector c;
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
        
        public void LogDeadAnimal(AnimalModel animalModel, AnimalController.CauseOfDeath causeOfDeath)
        {
            c.CollectDeath(animalModel, causeOfDeath);
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
            c.Collect();
            //Display(ConvertFloatListToIntList(c.rabbitStatsPerGenMean[0]));
            //if (ShowFrameRate) Display(ConvertFloatListToIntList(framerate));
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
            foreach (float f in list.ToList())
            {
                integerList.Add((int) f);
            }

            return integerList;
        }
    }
}