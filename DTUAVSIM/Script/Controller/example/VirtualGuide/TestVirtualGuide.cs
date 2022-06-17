using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DTUAVSIM.Sensor.EnvironmentalPerception;
using DTUAVCARS.Controller;
public class TestVirtualGuide : MonoBehaviour
{
    public LayerMask Mask;                              //The Environment Layer of Perception Sensor
    public float PerceptionRange;                       //The Range of Environmental Perception Sensor Work
    public float PerceptionAngleStart;                  //The Start Angle of Environment Perception Sensor
    public float PerceptionAngleEnd;                    //The End Angle of Environment Perception Sensor
    public float PerceptionAngleInc;                    //The Increment Angle of Environment Perception Sensor
    private environment_perception _enviromentPerception;
    public int predictedSteps;
    public List<float> omiga;
    public float safeRange;
    public List<Vector3> PredictedPosition;
    private VirtualGuide _virtualGuide;
    private Vector3 templatePosition;
    // Start is called before the first frame update
    void Start()
    {
        _enviromentPerception = new environment_perception(Mask, PerceptionRange, PerceptionAngleStart, PerceptionAngleEnd, PerceptionAngleInc);
        _virtualGuide = new VirtualGuide(predictedSteps, omiga, safeRange, _enviromentPerception);

    }

    // Update is called once per frame
    void Update()
    {
        _virtualGuide.UpdateController(PredictedPosition, transform.position);
        Debug.Log("Controller_output: " + _virtualGuide.GetControllerOutput());
        for (int i = 0; i < PredictedPosition.Count; i++)
        {
            templatePosition.x = transform.position.x + 0.9f * (i + 1);
            templatePosition.y = transform.position.y;
            templatePosition.z = transform.position.z + 0.9f * (i + 1);
            PredictedPosition[i] = templatePosition;
        }

    }
}
