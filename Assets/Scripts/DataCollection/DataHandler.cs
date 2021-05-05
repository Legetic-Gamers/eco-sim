/*
 * Author: Johan A. 
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
            // Make a collector to handle data
            c = new Collector();
            // Prepare for frame rate collection
            times = new List<float>(0);
            framerate = new List<float>(10);
            counter = 5;
        }

        public void Start()
        {
            ButtonClick bc = FindObjectOfType<ButtonClick>();
            if (bc)
            {
                bc.GetListTrait += SetTrait;
                bc.GetListPopulationPerGeneration += SetPopulationGeneration;
                bc.GetListPopulationPerMinute += SetPopulationMinute;
                bc.GetListBirthRate += SetBirthRate;
                bc.GetListFoodAvailable += SetFoodAvailable;    
            }
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
        public void LogNewPlant()
        {
            c.CollectNewFood();
        }
        
        /// <summary>
        /// Called when a plant is eaten
        /// </summary>
        public void LogDeadPlant()
        {
            c.CollectDeadFood();
        }

        private void SetPopulationGeneration(int speciesNumberPopulation)
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
            Display?.Invoke(sendList1, sendList2);

        }

        private void SetPopulationMinute(int speciesNumberPopulation)
        {
            List<float> tmplist = new List<float>();
            switch (speciesNumberPopulation)
            {
                case 0:
                    tmplist = c.animalsAlivePerSpecies[0];
                    break;
                case 1:
                    tmplist = c.animalsAlivePerSpecies[1];
                    break;
                case 2:
                    tmplist = c.animalsAlivePerSpecies[2];
                    break;
                case 3:
                    tmplist = c.animalsAlivePerSpecies[3];
                    break;
                
            }

            sendList1 = tmplist;
            _speciesNumberPopulation = speciesNumberPopulation;
            _listNumber = 4;
            Display?.Invoke(sendList1, sendList2);
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
            Display?.Invoke(sendList1, sendList2);

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
            _speciesNumberBirthRate = speciesNumberBirthRate;
            _listNumber = 2;
            Display?.Invoke(sendList1, sendList2);

        }

        private void SetFoodAvailable()
        {
            sendList1 = ConvertIntListToFloatList(c.foodActivePerMinute);
            _listNumber = 3;
            Display?.Invoke(sendList1, sendList2);
        }

        private void Updatelist(int listNumber)
        {
            switch (listNumber)
            {
                case 0: SetPopulationGeneration(_speciesNumberPopulation);
                    break;
                case 1: SetTrait(0,_speciesNumber1, _traitNumber1, 0);
                    break;
                case 2: SetBirthRate(_speciesNumberBirthRate);
                    break;
                case 3: SetFoodAvailable();
                    break;
                case 4: SetPopulationMinute(_speciesNumberPopulation);
                    break;
                
            }
        }

        
        /// <summary>
        /// Uses the Formatter to print a list in json format in /Export. name will match trait name. 
        /// </summary>
        /// <param name="listNumber"> List to be printed. See Traits class for order. 0 to 12 </param>
        /// <returns></returns>
        public async Task ExportDataToFile(string dirName)
        {
            
            string basePath = "simulations/";
/*
            List<int> toPrint = new List<int>();
            toPrint.Add(c.causeOfDeath[AnimalModel.CauseOfDeath.Hunger]);
            toPrint.Add(c.causeOfDeath[AnimalModel.CauseOfDeath.Hydration]);
            toPrint.Add(c.causeOfDeath[AnimalModel.CauseOfDeath.Age]);
            toPrint.Add(c.causeOfDeath[AnimalModel.CauseOfDeath.Energy]);
            toPrint.Add(c.causeOfDeath[AnimalModel.CauseOfDeath.Health]);
            toPrint.Add(c.causeOfDeath[AnimalModel.CauseOfDeath.Eaten]);
            */

            string dirPath = basePath + dirName;
            dirPath = ProcessDirectoryPath(dirPath);
            
            if (dirPath == null)
            {
                Debug.LogError("Could not find available path");
            }
            
            Debug.Log("Trying to export data to available path: " + dirName);

            
            //create  files for rabbits
            string rabbitDirPath = dirPath + "/" + "Rabbits";
            await WriteToFile(c.animalsAlivePerSpecies[0], "rPopulation", rabbitDirPath);
            await WriteToFile(c.birthRatePerMinute[0], "rBirthRate", rabbitDirPath);
            await WriteToFile(c.rabbitStatsPerGenMean[0], "rSize", rabbitDirPath);
            await WriteToFile(c.rabbitStatsPerGenMean[1], "rMaxEnergy", rabbitDirPath);
            await WriteToFile(c.rabbitStatsPerGenMean[2], "rMaxHydration", rabbitDirPath);
            await WriteToFile(c.rabbitStatsPerGenMean[3], "rMaxSpeed", rabbitDirPath);
            await WriteToFile(c.rabbitStatsPerGenMean[4], "rMaxReproductiveUrge", rabbitDirPath);
            await WriteToFile(c.rabbitStatsPerGenMean[5], "rAgeLimit", rabbitDirPath);
            await WriteToFile(c.rabbitStatsPerGenMean[6], "rViewAngle", rabbitDirPath);
            await WriteToFile(c.rabbitStatsPerGenMean[7], "rViewRadius", rabbitDirPath);
            await WriteToFile(c.rabbitStatsPerGenMean[8], "rHearingRadius", rabbitDirPath);
            
            //create  files for wolfs
            string wolfDirPath = dirPath + "/" + "Wolfs";
            await WriteToFile(c.animalsAlivePerSpecies[1], "wPopulation", wolfDirPath);
            await WriteToFile(c.birthRatePerMinute[1], "wBirthRate", wolfDirPath);
            await WriteToFile(c.wolfStatsPerGenMean[0], "wSize", wolfDirPath);
            await WriteToFile(c.wolfStatsPerGenMean[1], "wMaxEnergy", wolfDirPath);
            await WriteToFile(c.wolfStatsPerGenMean[2], "wMaxHydration", wolfDirPath);
            await WriteToFile(c.wolfStatsPerGenMean[3], "wMaxSpeed", wolfDirPath);
            await WriteToFile(c.wolfStatsPerGenMean[4], "wMaxReproductiveUrge", wolfDirPath);
            await WriteToFile(c.wolfStatsPerGenMean[5], "wAgeLimit", wolfDirPath);
            await WriteToFile(c.wolfStatsPerGenMean[6], "wViewAngle", wolfDirPath);
            await WriteToFile(c.wolfStatsPerGenMean[7], "wViewRadius", wolfDirPath);
            await WriteToFile(c.wolfStatsPerGenMean[8], "wHearingRadius", wolfDirPath);
            
            //create  files for deers
            string deerDirPath = dirPath + "/" + "Deers";
            await WriteToFile(c.animalsAlivePerSpecies[2], "dPopulation", deerDirPath);
            await WriteToFile(c.birthRatePerMinute[2], "dBirthRate", deerDirPath);
            await WriteToFile(c.deerStatsPerGenMean[0], "dSize", deerDirPath);
            await WriteToFile(c.deerStatsPerGenMean[1], "dMaxEnergy", deerDirPath);
            await WriteToFile(c.deerStatsPerGenMean[2], "dMaxHydration", deerDirPath);
            await WriteToFile(c.deerStatsPerGenMean[3], "dMaxSpeed", deerDirPath);
            await WriteToFile(c.deerStatsPerGenMean[4], "dMaxReproductiveUrge", deerDirPath);
            await WriteToFile(c.deerStatsPerGenMean[5], "dAgeLimit", deerDirPath);
            await WriteToFile(c.deerStatsPerGenMean[6], "dViewAngle", deerDirPath);
            await WriteToFile(c.deerStatsPerGenMean[7], "dViewRadius", deerDirPath);
            await WriteToFile(c.deerStatsPerGenMean[8], "dHearingRadius", deerDirPath);
            
            //create  files for bears
            string bearDirPath = dirPath + "/" + "Bears";
            await WriteToFile(c.animalsAlivePerSpecies[3], "bPopulation", bearDirPath);
            await WriteToFile(c.birthRatePerMinute[3], "bBirthRate", bearDirPath);
            await WriteToFile(c.bearStatsPerGenMean[0], "bSize", bearDirPath);
            await WriteToFile(c.bearStatsPerGenMean[1], "bMaxEnergy", bearDirPath);
            await WriteToFile(c.bearStatsPerGenMean[2], "bMaxHydration", bearDirPath);
            await WriteToFile(c.bearStatsPerGenMean[3], "bMaxSpeed", bearDirPath);
            await WriteToFile(c.bearStatsPerGenMean[4], "bMaxReproductiveUrge", bearDirPath);
            await WriteToFile(c.bearStatsPerGenMean[5], "bAgeLimit", bearDirPath);
            await WriteToFile(c.bearStatsPerGenMean[6], "bViewAngle", bearDirPath);
            await WriteToFile(c.bearStatsPerGenMean[7], "bViewRadius", bearDirPath);
            await WriteToFile(c.bearStatsPerGenMean[8], "bHearingRadius", bearDirPath);
            
            //create  files for others (plants etc.)
            string plantDirPath = dirPath + "/" + "Plants";
            await WriteToFile(c.foodActivePerMinute,"fActive", plantDirPath);

        }

        private string ProcessDirectoryPath(string dirPath)
        {
            //If directory does not exist create it
            if (Directory.Exists(dirPath)) {
                Debug.Log("Path is occupied: " + dirPath);

                //set foldername + (1)
                for (int i = 1; i <= 10; i++)
                {

                    dirPath = dirPath + " " + "(" + i + ")";
                    Debug.Log("Trying : " + dirPath);
                    if (!Directory.Exists(dirPath))
                    {
                        return dirPath;
                    }
                }
                Debug.LogError("Could not create path: " + dirPath);
                return null;
            }
            return dirPath;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        /// <summary>
        /// On each tick from Event Publisher collect time series data and send to graph and write to file. 
        /// </summary>
        private void UpdateDataAndGraph()
        {
            // only call display if graph is activated from ShowGraphManager
            if (Window_Graph.IsGraphOne)
            {
                Updatelist(_listNumber);
                //Display?.Invoke(sendList1, sendList2);
            }
                
            //if (ShowFrameRate) Display(ConvertFloatListToIntList(framerate));
            //ExportDataToFile(0); Now prints cause of death
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