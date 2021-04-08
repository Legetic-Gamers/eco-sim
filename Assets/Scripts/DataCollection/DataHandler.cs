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

        private static int _listNumber = 1; 
        private static int _speciesNumberPopulation = 0; 
        private static int _speciesNumberBirthRate = 0; 
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
            tickEventPublisher.onDataHandlerUpdate += UpdateDataAndGraph;
            tickEventPublisher.onCollectorUpdate += CollectBirthRate;
            //ButtonClick bc = FindObjectOfType<ButtonClick>();
            //bc.GetListTrait += SetTrait;
            //bc.GetListPopulation += SetPopulation;
            //bc.GetListBirthRate += SetBirthRate;
            //bc.GetListFoodAvailable += SetFoodAvailable;
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
        public void LogDeadAnimal(AnimalModel animalModel, AnimalModel.CauseOfDeath causeOfDeath)
        {
            c.CollectDeath(animalModel, causeOfDeath);
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

        private void SetPopulation(int speciesNumberPopulation)
        {
            List<float> tmplist = new List<float>();
            switch (speciesNumberPopulation)
            {
                case 0:
                    tmplist = ConvertIntListToFloatList(c.totalAnimalsAlivePerGeneration);
                    break;
                case 1:
                    tmplist = c.rabbitTotalAlivePerGen;
                    break;
                case 2:
                    tmplist = c.wolfTotalAlivePerGen;
                    break;
                case 3:
                    tmplist = c.deerTotalAlivePerGen;
                    break;
                case 4:
                    tmplist = c.bearTotalAlivePerGen;
                    break;
                
            }

            sendList1 = tmplist;
            _speciesNumberPopulation = speciesNumberPopulation;
            _listNumber = 0;
        }

        private void SetTrait(int listNumber, int speciesNumber, int traitNumber, int dataType)
        {
            List<float> tmplist = new List<float>();
            switch (speciesNumber)
            {
                case 0:
                    if (dataType == 0) tmplist = c.rabbitStatsPerGenMean[traitNumber];
                    if (dataType == 1) tmplist = c.rabbitStatsPerGenVar[traitNumber];
                    break;
                case 1:
                    if (dataType == 0) tmplist = c.wolfStatsPerGenMean[traitNumber];
                    if (dataType == 1) tmplist = c.wolfStatsPerGenVar[traitNumber];
                    break;      
                case 2:         
                    if (dataType == 0) tmplist = c.deerStatsPerGenMean[traitNumber];
                    if (dataType == 1) tmplist = c.deerStatsPerGenVar[traitNumber];
                    break;      
                case 3:         
                    if (dataType == 0) tmplist = c.bearStatsPerGenMean[traitNumber];
                    if (dataType == 1) tmplist = c.bearStatsPerGenVar[traitNumber];
                    break;
            }

            _listNumber = 1;

            if (listNumber == 0)
            {
                sendList1 = tmplist;
                _speciesNumber1 = speciesNumber;
                _traitNumber1 = traitNumber;
                _dataTypeNumber1 = dataType;
            }

            else
            {
                sendList2 = tmplist;
                _speciesNumber2 = speciesNumber;
                _traitNumber2 = traitNumber;
                _dataTypeNumber2 = dataType;
            }
        }
        
        private void SetBirthRate(int speciesNumberBirthRate)
        {
            List<float> tmplist = new List<float>();
            switch (speciesNumberBirthRate)
            {
                case 0:
                    tmplist = c.birthRatePerMinute[0];
                    break;
                case 1:
                    tmplist = c.birthRatePerMinute[1];
                    break;
                case 2:
                    tmplist = c.birthRatePerMinute[2];
                    break;
                case 3:
                    tmplist = c.birthRatePerMinute[3];
                    break;
            }

            sendList1 = tmplist;
            _speciesNumberPopulation = speciesNumberBirthRate;
            _listNumber = 2;
        }

        private void SetFoodAvailable()
        {
            sendList1 = ConvertIntListToFloatList(c.foodActivePerMinute);
            _listNumber = 3;
        }

        private void Updatelist(int listNumber)
        {
            switch (listNumber)
            {
                case 0: SetPopulation(_speciesNumberPopulation);
                    break;
                case 1: SetTrait(0,_speciesNumber1, _traitNumber1, 0);
                    break;
                case 2: SetBirthRate(_speciesNumberBirthRate);
                    break;
                case 3: SetFoodAvailable();
                    break;
                
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

            Updatelist(_listNumber);
            //SetTrait(0,_speciesNumber1,_traitNumber1,_dataTypeNumber1);
            //SetList(1,_speciesNumber2,_traitNumber2,_dataTypeNumber2);

            Display?.Invoke(sendList1, sendList2);
            //if (ShowFrameRate) Display(ConvertFloatListToIntList(framerate));
            //ExportDataToFile(0);
        }

        private void CollectBirthRate()
        {
            c.Collect();

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
        

        private List<float> ConvertIntListToFloatList(List<int> list)
        {
            List<float> floatList = new List<float>();

            foreach (int f in list.ToArray()) floatList.Add((float) f);
            return floatList;
        }

    }
}