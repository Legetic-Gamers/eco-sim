using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ViewController.Senses
{
    public class Senses : MonoBehaviour
    {
        [Range(0, 360)]
        private float angle;
        private float radius;
        
        private float hearingRadius;
        private float viewRadius;

        [SerializeField]
        private LayerMask targetMask;
        [SerializeField]
        private LayerMask obstacleMask;

        [SerializeField] public bool useConstantTickInterval;
        
        private AnimalController animalController;

        private Transform thisTransform;
        
        public Action onSenseTick;
        

        /* \/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/ */

        // reduce clutter in FindTargets()
        private void ClearLists()
        {
            animalController.visibleHostileTargets.Clear();
            animalController.visibleFriendlyTargets.Clear();
            animalController.visibleFoodTargets.Clear();
            animalController.visibleWaterTargets.Clear();
            
            animalController.heardHostileTargets.Clear();
            animalController.heardFriendlyTargets.Clear();
            animalController.heardPreyTargets.Clear();
        }

        public void FindTargets()
        {
            // prevent adding duplicates
            ClearLists();

            if (thisTransform == null)
            {
                thisTransform = transform;
            }

            // add targets in list when they enter the sphere
            Collider[] targetsInRadius = Physics.OverlapSphere(thisTransform.position, radius, targetMask);

            // loop through targets within the entire circle to determine whether to add to a Targets list
            for (int i = 0; i < targetsInRadius.Length; i++)
            {
                GameObject target = targetsInRadius[i].gameObject;
                Transform targetTransform = target.transform;

                // don't add self
                if (target == gameObject) continue;
                
                //If target is an animal, try to find its center transform, otherwise target transform is at the feet.
                if (target.gameObject.CompareTag("Animal"))
                {
                    if(target.TryGetComponent(out AnimalController targetController) && targetController.centerTransform != null)
                    {
                        targetTransform = targetController.centerTransform;
                    }
                    
                }
                
                //If target is a plant, try to find its center transform, otherwise target transform is at the bottom.
                if (target.gameObject.CompareTag("Plant"))
                {
                    if(target.TryGetComponent(out PlantController plantController) && plantController.centerTransform != null && plantController.plantModel != null && plantController.plantModel.isRegrowing)
                    {
                        targetTransform = plantController.centerTransform;
                    }
                    
                }
                
                
                Vector3 dirToTarget = (targetTransform.position - transform.position).normalized;

                float distToTarget = Vector3.Distance(thisTransform.position, targetTransform.position);

                if (distToTarget <= hearingRadius && 
                    target.gameObject.CompareTag("Animal")) HandleHeardAnimalTarget(target);
                
                if (Vector3.Angle(transform.forward, dirToTarget) < angle / 2)
                {
                    if (distToTarget <= viewRadius)
                    {
                        // if target is not obscured
                        if (!Physics.Raycast(thisTransform.position, dirToTarget, distToTarget, obstacleMask))
                        {
                            if (target.gameObject.CompareTag("Plant"))
                            {
                                HandlePlantTarget(target);
                            }
                            else if (target.gameObject.CompareTag("Animal"))
                            {
                                HandleSeenAnimalTarget(target);
                            } 
                            else if (target.gameObject.CompareTag("Water"))
                            {
                                HandleWaterTarget(target);
                            }
                        }
                        
                    }
                }
            }
        }

        
        /* \/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/ */
        
        private void HandleHeardAnimalTarget(GameObject target)
        {
            AnimalController targetAnimalController = target.GetComponent<AnimalController>();

            //if the targets animalModel can eat this animalModel: add to visibleHostileTargets
            if (targetAnimalController.animalModel.CanEat(animalController.animalModel) && targetAnimalController.animalModel.IsAlive)
            {
                animalController.heardHostileTargets.Add(target);
                animalController.actionPerceivedHostile?.Invoke(target);
            }
            //if the target is of same species: add to visibleFriendlyTargets
            else if (animalController.animalModel.IsSameSpecies(targetAnimalController.animalModel) && targetAnimalController.animalModel.IsAlive)
            {
                animalController.heardFriendlyTargets.Add(target);
            }
            //if this animalModel can eat the targets animalModel: add to visibleFoodTargets
            else if (animalController.animalModel.CanEat(targetAnimalController.animalModel) )
            {
                animalController.heardPreyTargets.Add(target);
            }
        }
        
        private void HandleSeenAnimalTarget(GameObject target)
        {
            AnimalController targetAnimalController = target.GetComponent<AnimalController>();

            //if the targets animalModel can eat this animalModel: add to visibleHostileTargets
            if (targetAnimalController.animalModel.CanEat(animalController.animalModel) && targetAnimalController.animalModel.IsAlive)
            {
                animalController.visibleHostileTargets.Add(target);
                animalController.actionPerceivedHostile?.Invoke(target);

            }  
            //if this animalModel can the targets animalModel: add to visibleFoodTargets
            else if (animalController.animalModel.CanEat(targetAnimalController.animalModel))
            {
                animalController.visibleFoodTargets.Add(target);
            }
            //if the target is of same species: add to visibleFriendlyTargets
            else if (animalController.animalModel.IsSameSpecies(targetAnimalController.animalModel) && targetAnimalController.animalModel.IsAlive)
            {
                animalController.visibleFriendlyTargets.Add(target);
            }
        }
        
        private void HandleWaterTarget(GameObject target)
        {
            animalController.visibleWaterTargets.Add(target);
        }

        private void HandlePlantTarget(GameObject target)
        {
            PlantController targetPlantController = target.GetComponent<PlantController>();
            if (animalController.animalModel.CanEat(targetPlantController.plantModel) && !targetPlantController.plantModel.isRegrowing)
            {
                animalController.visibleFoodTargets.Add(target);
            }
        }
        
        /* \/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/ */
        private void Start()
        {
            Init();
        }

        public void Init()
        {

            animalController = GetComponent<AnimalController>();
            
            hearingRadius = animalController.animalModel.traits.hearingRadius; 
            viewRadius = animalController.animalModel.traits.viewRadius;
            
            // use the greater of the two radii for overlapSphere
            radius = Mathf.Max(hearingRadius,viewRadius);
            
            angle = animalController.animalModel.traits.viewAngle;

            thisTransform = animalController.eyesTransform;
            if (thisTransform == null)
            {
                Debug.LogWarning("NO EYES FOUND, SENSING FROM FEET!");
                thisTransform = transform;
            }

            //Used in ML
            if (useConstantTickInterval)
            {
                Debug.Log("Using tickEventPublisher for senses");
                StartCoroutine(ConstantSenseLoop());
            }
            else
            {
                StartCoroutine(RandomSensesLoop());
            }
        }

        private IEnumerator ConstantSenseLoop()
        {
            while (true)
            {
                FindTargets();
                onSenseTick?.Invoke();
                yield return new WaitForSeconds(0.5f/Time.timeScale);
            
            }
        }
        private IEnumerator RandomSensesLoop()
        {
            while (true)
            {
                FindTargets();
                onSenseTick?.Invoke();
                yield return new WaitForSeconds(Random.Range(0.5f, 1f));
            
            }
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}