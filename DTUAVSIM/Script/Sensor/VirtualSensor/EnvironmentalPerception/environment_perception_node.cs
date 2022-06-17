using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DTUAVSIM.Sensor.EnvironmentalPerception
{
    public class environment_perception_node : MonoBehaviour
    {
        public LayerMask Mask;                              //The Environment Layer of Perception Sensor
        public float PerceptionRange;                       //The Range of Environmental Perception Sensor Work
        public float PerceptionAngleStart;                  //The Start Angle of Environment Perception Sensor
        public float PerceptionAngleEnd;                    //The End Angle of Environment Perception Sensor
        public float PerceptionAngleInc;                    //The Increment Angle of Environment Perception Sensor
        private environment_perception _enviromentPerception;
        // Start is called before the first frame update
        void Start()
        {
            _enviromentPerception = new environment_perception(Mask, PerceptionRange,PerceptionAngleStart,PerceptionAngleEnd, PerceptionAngleInc);
        }

        // Update is called once per frame
        void Update()
        {
           _enviromentPerception.UpdateSensor(transform.position);
            Debug.Log("minValue: " + _enviromentPerception.GetMinDistance());
            Debug.Log("minValue_direction: " + _enviromentPerception.GetMinDistanceDirection());
        }
    }
}