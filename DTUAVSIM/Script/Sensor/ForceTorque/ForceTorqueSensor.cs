using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimUnity.Sensor.ForceTorque
{
    public class ForceTorqueSensor : MonoBehaviour
    {
        [Header("父对象")] 
        public GameObject paraGameObject;

        [Header("子对象")] 
        public GameObject childGameObject;

        [Header("测量父对象到子对象数据")] 
        public bool getParaToChildFlag;

        [Header("3D关节类型")] 
        public bool joint3DFlag;

        public bool fixedJoint3DFlag;
        public FixedJoint fixedJoint3D;

        public bool hingeJoint3DFlag;
        public HingeJoint hingeJoint3D;

        public bool characterJoint3DFlag;
        public CharacterJoint characterJoint3D;

        public bool configurableJoint3DFlag;
        public ConfigurableJoint configurableJoint3D;

        public bool springJoint3DFlag;
        public SpringJoint springJoint3D;

        [Header("获取的3D力")] 
        public Vector3 forceValue3D;
        [Header("获取的3D力矩")]
        public Vector3 torqueValue3D;

        [Header("2D关节类型")] 
        public bool joint2DFlag;

        public bool distanceJoint2DFlag;
        public DistanceJoint2D distanceJoint2D;

        public bool fixedJoint2DFlag;
        public FixedJoint2D fixedJoint2D;

        public bool frictionJoint2DFlag;
        public FrictionJoint2D frictionJoint2D;

        public bool hingeJoint2DFlag;
        public HingeJoint2D hingeJoint2D;

        public bool relativeJoint2DFlag;
        public RelativeJoint2D relativeJoint2D;

        public bool sliderJoint2DFlag;
        public SliderJoint2D sliderJoint2D;

        public bool springJoint2DFlag;
        public SpringJoint2D springJoint2D;

        public bool targetJoint2DFlag;
        public TargetJoint2D targetJoint2D;

        public bool wheelJoint2DFlag;
        public WheelJoint2D wheelJoint2D;

        [Header("获取的2D力")] 
        public Vector2 forceValue2D;
        [Header("获取的2D力矩")] 
        public float torqueValue2D;


        

        // Start is called before the first frame update
        void Start()
        {
            if (paraGameObject.GetComponent<Rigidbody>() == null || childGameObject.GetComponent<Rigidbody>( ) == null)
            {
                Debug.LogError("对象未添加刚体模型，you need to add a rigidbody to your objet");
            }
            else
            {
                if (joint3DFlag == false && joint2DFlag == false)
                {
                    Debug.LogError("未选择关节的类型是2D还是3D");
                }

                if (joint2DFlag == true && joint3DFlag == true)
                {
                    Debug.LogError("不能同时选择关节类型为2D和3D");
                }

            }
        }


        void Update()
        {
            updtate3DForce();
        }

        public GameObject getParaObj()
        {
            return this.paraGameObject;
        }

        public GameObject getChildObj()
        {
            return this.childGameObject;
        }

        public void updtate3DForce()
        {
            if (fixedJoint3DFlag)
            {
                if (paraGameObject.GetComponent<FixedJoint>() == null)
                {
                    //关节挂在子对象那边
                    if (getParaToChildFlag)
                    {
                        
                        forceValue3D = - fixedJoint3D.currentForce;
                        torqueValue3D = - fixedJoint3D.currentTorque;

                    }
                    else
                    {
                       
                        forceValue3D = fixedJoint3D.currentForce;
                        torqueValue3D = fixedJoint3D.currentTorque;
                    }

                }
                else
                {
                    bool ispara = false;
                    
                   
                    FixedJoint[] fixedJoints =  paraGameObject.GetComponents<FixedJoint>();
                    for (int i = 0; i < fixedJoints.Length; i++)
                    {
                        if (fixedJoint3D == fixedJoints[i])
                        {
                            break;
                            ispara = true;
                        }
                    }

                    if (ispara == true)
                    {
                        //关节挂在父对象那边
                        if (getParaToChildFlag)
                        {
                            
                            forceValue3D = fixedJoint3D.currentForce;
                            torqueValue3D = fixedJoint3D.currentTorque;

                        }
                        else
                        {
                            
                            forceValue3D =  - fixedJoint3D.currentForce;
                            torqueValue3D = - fixedJoint3D.currentTorque;
                        }
                    }
                    else
                    {
                        //关节挂在子对象那边
                        if (getParaToChildFlag)
                        {
                            
                            forceValue3D =  -fixedJoint3D.currentForce;
                            torqueValue3D = -fixedJoint3D.currentTorque;

                        }
                        else
                        {
                           
                            forceValue3D =  fixedJoint3D.currentForce;
                            torqueValue3D = fixedJoint3D.currentTorque;

                        }
                    }
                   
                }
            }
            else if (hingeJoint3DFlag)
            {
                if (paraGameObject.GetComponent<HingeJoint>() == null)
                {
                    //关节挂在子对象那边
                    if (getParaToChildFlag)
                    {
                       
                        forceValue3D =  -hingeJoint3D.currentForce;
                        torqueValue3D = -hingeJoint3D.currentTorque;
                    }
                    else
                    {
                        
                        forceValue3D =  hingeJoint3D.currentForce;
                        torqueValue3D = hingeJoint3D.currentTorque;
                    }

                }
                else
                {
                    bool ispara = false;


                    HingeJoint[] hingeJoints = paraGameObject.GetComponents<HingeJoint>();
                    for (int i = 0; i < hingeJoints.Length; i++)
                    {
                        if (hingeJoint3D == hingeJoints[i])
                        {
                            break;
                            ispara = true;
                        }
                    }

                    if (ispara == true)
                    {
                        //关节挂在父对象那边
                        if (getParaToChildFlag)
                        {
                            
                            forceValue3D =  hingeJoint3D.currentForce;
                            torqueValue3D = hingeJoint3D.currentTorque;
                        }
                        else
                        {
                            
                            forceValue3D =  -hingeJoint3D.currentForce;
                            torqueValue3D = -hingeJoint3D.currentTorque;
                        }
                    }
                    else
                    {
                        //关节挂在子对象那边
                        if (getParaToChildFlag)
                        {
                            
                            forceValue3D =  -hingeJoint3D.currentForce;
                            torqueValue3D = -hingeJoint3D.currentTorque;

                        }
                        else
                        {
                           
                            forceValue3D = hingeJoint3D.currentForce;
                            torqueValue3D = hingeJoint3D.currentTorque;
                        }
                    }

                }
            }
            else if (springJoint3DFlag)
            {
                if (paraGameObject.GetComponent<SpringJoint>() == null)
                {
                    //关节挂在子对象那边
                    if (getParaToChildFlag)
                    {
                       
                        forceValue3D =  -springJoint3D.currentForce;
                        torqueValue3D = -springJoint3D.currentTorque;

                    }
                    else
                    {
                        
                        forceValue3D =  springJoint3D.currentForce;
                        torqueValue3D = springJoint3D.currentTorque;
                    }

                }
                else
                {
                    bool ispara = false;


                    SpringJoint[] springJoints = paraGameObject.GetComponents<SpringJoint>();
                    for (int i = 0; i < springJoints.Length; i++)
                    {
                        if (springJoint3D == springJoints[i])
                        {
                            break;
                            ispara = true;
                        }
                    }

                    if (ispara == true)
                    {
                        //关节挂在父对象那边
                        if (getParaToChildFlag)
                        {
                            
                            forceValue3D =  springJoint3D.currentForce;
                            torqueValue3D = springJoint3D.currentTorque;
                        }
                        else
                        {
                            
                            forceValue3D =  -springJoint3D.currentForce;
                            torqueValue3D = -springJoint3D.currentTorque;
                        }
                    }
                    else
                    {
                        //关节挂在子对象那边
                        if (getParaToChildFlag)
                        {
                            
                            forceValue3D =  -springJoint3D.currentForce;
                            torqueValue3D = -springJoint3D.currentTorque;
                        }
                        else
                        {
                           
                            forceValue3D =  springJoint3D.currentForce;
                            torqueValue3D = springJoint3D.currentTorque;
                        }
                    }

                }
            }
            else if (configurableJoint3DFlag)
            {
                if (paraGameObject.GetComponent<ConfigurableJoint>() == null)
                {
                    //关节挂在子对象那边
                    if (getParaToChildFlag)
                    {
                        
                        forceValue3D = -configurableJoint3D.currentForce;
                        torqueValue3D = -configurableJoint3D.currentTorque;
                    }
                    else
                    {
                        
                        forceValue3D =  configurableJoint3D.currentForce;
                        torqueValue3D = configurableJoint3D.currentTorque;
                    }

                }
                else
                {
                    bool ispara = false;


                   ConfigurableJoint[] configurableJoints = paraGameObject.GetComponents<ConfigurableJoint>();
                    for (int i = 0; i < configurableJoints.Length; i++)
                    {
                        if (configurableJoint3D == configurableJoints[i])
                        {
                            break;
                            ispara = true;
                        }
                    }

                    if (ispara == true)
                    {
                        //关节挂在父对象那边
                        if (getParaToChildFlag)
                        {
                           
                            forceValue3D =  configurableJoint3D.currentForce;
                            torqueValue3D = configurableJoint3D.currentTorque;

                        }
                        else
                        {
                            
                            forceValue3D =  -configurableJoint3D.currentForce;
                            torqueValue3D = -configurableJoint3D.currentTorque;
                        }
                    }
                    else
                    {
                        //关节挂在子对象那边
                        if (getParaToChildFlag)
                        {
                            
                            forceValue3D =  -configurableJoint3D.currentForce;
                            torqueValue3D = -configurableJoint3D.currentTorque;
                        }
                        else
                        {
                            
                            forceValue3D =  configurableJoint3D.currentForce;
                            torqueValue3D = configurableJoint3D.currentTorque;
                        }
                    }

                }
            }
            else if (characterJoint3DFlag)
            {
                if (paraGameObject.GetComponent<CharacterJoint>() == null)
                {
                    //关节挂在子对象那边
                    if (getParaToChildFlag)
                    {
                       
                        forceValue3D =  -characterJoint3D.currentForce;
                        torqueValue3D = -configurableJoint3D.currentTorque;
                    }
                    else
                    {
                        
                        forceValue3D =  characterJoint3D.currentForce;
                        torqueValue3D = configurableJoint3D.currentTorque;
                    }

                }
                else
                {
                    bool ispara = false;


                    CharacterJoint[] characterJoints = paraGameObject.GetComponents<CharacterJoint>();
                    for (int i = 0; i < characterJoints.Length; i++)
                    {
                        if (characterJoint3D == characterJoints[i])
                        {
                            break;
                            ispara = true;
                        }
                    }

                    if (ispara == true)
                    {
                        //关节挂在父对象那边
                        if (getParaToChildFlag)
                        {
                            
                            forceValue3D =  characterJoint3D.currentForce;
                            torqueValue3D = characterJoint3D.currentTorque;
                        }
                        else
                        {
                            
                            forceValue3D =  -characterJoint3D.currentForce;
                            torqueValue3D = -characterJoint3D.currentTorque;
                        }
                    }
                    else
                    {
                        //关节挂在子对象那边
                        if (getParaToChildFlag)
                        {
                            
                            forceValue3D =  -characterJoint3D.currentForce;
                            torqueValue3D = -characterJoint3D.currentTorque;
                        }
                        else
                        {
                            
                            forceValue3D =  characterJoint3D.currentForce;
                            torqueValue3D = characterJoint3D.currentTorque;
                        }
                    }

                }
            }
           
        }

        public void update2DForce()
        {
            if (fixedJoint2DFlag)
            {
                if (paraGameObject.GetComponent<FixedJoint2D>() == null)
                {
                    //关节挂在子对象那边
                    if (getParaToChildFlag)
                    {

                        forceValue2D = fixedJoint2D.reactionForce;
                        torqueValue2D = fixedJoint2D.reactionTorque;

                    }
                    else
                    {

                        forceValue2D = -fixedJoint2D.reactionForce;
                        torqueValue2D = -fixedJoint2D.reactionTorque;
                    }

                }
                else
                {
                    bool ispara = false;


                    FixedJoint2D[] fixedJoint2Ds = paraGameObject.GetComponents<FixedJoint2D>();
                    for (int i = 0; i < fixedJoint2Ds.Length; i++)
                    {
                        if (fixedJoint2D == fixedJoint2Ds[i])
                        {
                            break;
                            ispara = true;
                        }
                    }

                    if (ispara == true)
                    {
                        //关节挂在父对象那边
                        if (getParaToChildFlag)
                        {

                            forceValue2D = -fixedJoint2D.reactionForce;
                            torqueValue2D = -fixedJoint2D.reactionTorque;

                        }
                        else
                        {

                            forceValue2D = fixedJoint2D.reactionForce;
                            torqueValue2D = fixedJoint2D.reactionTorque;
                        }
                    }
                    else
                    {
                        //关节挂在子对象那边
                        if (getParaToChildFlag)
                        {

                            forceValue2D = fixedJoint2D.reactionForce;
                            torqueValue2D = fixedJoint2D.reactionTorque;

                        }
                        else
                        {

                            forceValue2D = -fixedJoint2D.reactionForce;
                            torqueValue2D = -fixedJoint2D.reactionTorque;

                        }
                    }

                }
            }
            else if (distanceJoint2DFlag)
            {
                if (paraGameObject.GetComponent<DistanceJoint2D>() == null)
                {
                    //关节挂在子对象那边
                    if (getParaToChildFlag)
                    {

                        forceValue2D = distanceJoint2D.reactionForce;
                        torqueValue2D = distanceJoint2D.reactionTorque;

                    }
                    else
                    {

                        forceValue2D = -distanceJoint2D.reactionForce;
                        torqueValue2D = -distanceJoint2D.reactionTorque;
                    }

                }
                else
                {
                    bool ispara = false;


                    DistanceJoint2D[] distanceJoint2Ds = paraGameObject.GetComponents<DistanceJoint2D>();
                    for (int i = 0; i < distanceJoint2Ds.Length; i++)
                    {
                        if (distanceJoint2D == distanceJoint2Ds[i])
                        {
                            break;
                            ispara = true;
                        }
                    }

                    if (ispara == true)
                    {
                        //关节挂在父对象那边
                        if (getParaToChildFlag)
                        {

                            forceValue2D = -distanceJoint2D.reactionForce;
                            torqueValue2D = -distanceJoint2D.reactionTorque;

                        }
                        else
                        {

                            forceValue2D = distanceJoint2D.reactionForce;
                            torqueValue2D = distanceJoint2D.reactionTorque;
                        }
                    }
                    else
                    {
                        //关节挂在子对象那边
                        if (getParaToChildFlag)
                        {

                            forceValue2D = distanceJoint2D.reactionForce;
                            torqueValue2D = distanceJoint2D.reactionTorque;

                        }
                        else
                        {

                            forceValue2D = -distanceJoint2D.reactionForce;
                            torqueValue2D = -distanceJoint2D.reactionTorque;

                        }
                    }

                }
            }
            else if (frictionJoint2DFlag)
            {
                if (paraGameObject.GetComponent<FrictionJoint2D>() == null)
                {
                    //关节挂在子对象那边
                    if (getParaToChildFlag)
                    {

                        forceValue2D = frictionJoint2D.reactionForce;
                        torqueValue2D = frictionJoint2D.reactionTorque;

                    }
                    else
                    {

                        forceValue2D = -frictionJoint2D.reactionForce;
                        torqueValue2D = -frictionJoint2D.reactionTorque;
                    }

                }
                else
                {
                    bool ispara = false;


                    FrictionJoint2D[] frictionJoint2Ds = paraGameObject.GetComponents<FrictionJoint2D>();
                    for (int i = 0; i < frictionJoint2Ds.Length; i++)
                    {
                        if (frictionJoint2D == frictionJoint2Ds[i])
                        {
                            break;
                            ispara = true;
                        }
                    }

                    if (ispara == true)
                    {
                        //关节挂在父对象那边
                        if (getParaToChildFlag)
                        {

                            forceValue2D = -frictionJoint2D.reactionForce;
                            torqueValue2D = -frictionJoint2D.reactionTorque;

                        }
                        else
                        {

                            forceValue2D = frictionJoint2D.reactionForce;
                            torqueValue2D = frictionJoint2D.reactionTorque;
                        }
                    }
                    else
                    {
                        //关节挂在子对象那边
                        if (getParaToChildFlag)
                        {

                            forceValue2D = frictionJoint2D.reactionForce;
                            torqueValue2D = frictionJoint2D.reactionTorque;

                        }
                        else
                        {

                            forceValue2D = -frictionJoint2D.reactionForce;
                            torqueValue2D = -frictionJoint2D.reactionTorque;

                        }
                    }

                }
            }
            else if (hingeJoint2DFlag)
            {
                if (paraGameObject.GetComponent<HingeJoint2D>() == null)
                {
                    //关节挂在子对象那边
                    if (getParaToChildFlag)
                    {

                        forceValue2D = hingeJoint2D.reactionForce;
                        torqueValue2D = hingeJoint2D.reactionTorque;

                    }
                    else
                    {

                        forceValue2D = -hingeJoint2D.reactionForce;
                        torqueValue2D = -hingeJoint2D.reactionTorque;
                    }

                }
                else
                {
                    bool ispara = false;


                    HingeJoint2D[] hingeJoint2Ds = paraGameObject.GetComponents<HingeJoint2D>();
                    for (int i = 0; i < hingeJoint2Ds.Length; i++)
                    {
                        if (hingeJoint2D == hingeJoint2Ds[i])
                        {
                            break;
                            ispara = true;
                        }
                    }

                    if (ispara == true)
                    {
                        //关节挂在父对象那边
                        if (getParaToChildFlag)
                        {

                            forceValue2D = -hingeJoint2D.reactionForce;
                            torqueValue2D = -hingeJoint2D.reactionTorque;

                        }
                        else
                        {

                            forceValue2D = hingeJoint2D.reactionForce;
                            torqueValue2D = hingeJoint2D.reactionTorque;
                        }
                    }
                    else
                    {
                        //关节挂在子对象那边
                        if (getParaToChildFlag)
                        {

                            forceValue2D = hingeJoint2D.reactionForce;
                            torqueValue2D = hingeJoint2D.reactionTorque;

                        }
                        else
                        {

                            forceValue2D = -hingeJoint2D.reactionForce;
                            torqueValue2D = -hingeJoint2D.reactionTorque;

                        }
                    }

                }
            }
            else if (relativeJoint2DFlag)
            {
                if (paraGameObject.GetComponent<RelativeJoint2D>() == null)
                {
                    //关节挂在子对象那边
                    if (getParaToChildFlag)
                    {

                        forceValue2D = relativeJoint2D.reactionForce;
                        torqueValue2D = relativeJoint2D.reactionTorque;

                    }
                    else
                    {

                        forceValue2D = -relativeJoint2D.reactionForce;
                        torqueValue2D = -relativeJoint2D.reactionTorque;
                    }

                }
                else
                {
                    bool ispara = false;


                    RelativeJoint2D[] relativeJoint2Ds = paraGameObject.GetComponents<RelativeJoint2D>();
                    for (int i = 0; i < relativeJoint2Ds.Length; i++)
                    {
                        if (relativeJoint2D == relativeJoint2Ds[i])
                        {
                            break;
                            ispara = true;
                        }
                    }

                    if (ispara == true)
                    {
                        //关节挂在父对象那边
                        if (getParaToChildFlag)
                        {

                            forceValue2D = -relativeJoint2D.reactionForce;
                            torqueValue2D = -relativeJoint2D.reactionTorque;

                        }
                        else
                        {

                            forceValue2D = relativeJoint2D.reactionForce;
                            torqueValue2D = relativeJoint2D.reactionTorque;
                        }
                    }
                    else
                    {
                        //关节挂在子对象那边
                        if (getParaToChildFlag)
                        {

                            forceValue2D = relativeJoint2D.reactionForce;
                            torqueValue2D = relativeJoint2D.reactionTorque;

                        }
                        else
                        {

                            forceValue2D = -relativeJoint2D.reactionForce;
                            torqueValue2D = -relativeJoint2D.reactionTorque;

                        }
                    }

                }
            }
            else if (sliderJoint2DFlag)
            {
                if (paraGameObject.GetComponent<SliderJoint2D>() == null)
                {
                    //关节挂在子对象那边
                    if (getParaToChildFlag)
                    {

                        forceValue2D = sliderJoint2D.reactionForce;
                        torqueValue2D = sliderJoint2D.reactionTorque;

                    }
                    else
                    {

                        forceValue2D = -sliderJoint2D.reactionForce;
                        torqueValue2D = -sliderJoint2D.reactionTorque;
                    }

                }
                else
                {
                    bool ispara = false;


                    SliderJoint2D[] sliderJoint2Ds = paraGameObject.GetComponents<SliderJoint2D>();
                    for (int i = 0; i < sliderJoint2Ds.Length; i++)
                    {
                        if (sliderJoint2D == sliderJoint2Ds[i])
                        {
                            break;
                            ispara = true;
                        }
                    }

                    if (ispara == true)
                    {
                        //关节挂在父对象那边
                        if (getParaToChildFlag)
                        {

                            forceValue2D = -sliderJoint2D.reactionForce;
                            torqueValue2D = -sliderJoint2D.reactionTorque;

                        }
                        else
                        {

                            forceValue2D = sliderJoint2D.reactionForce;
                            torqueValue2D = sliderJoint2D.reactionTorque;
                        }
                    }
                    else
                    {
                        //关节挂在子对象那边
                        if (getParaToChildFlag)
                        {

                            forceValue2D = sliderJoint2D.reactionForce;
                            torqueValue2D = sliderJoint2D.reactionTorque;

                        }
                        else
                        {

                            forceValue2D = -sliderJoint2D.reactionForce;
                            torqueValue2D = -sliderJoint2D.reactionTorque;

                        }
                    }

                }
            }
            else if (springJoint2DFlag)
            {
                if (paraGameObject.GetComponent<SpringJoint2D>() == null)
                {
                    //关节挂在子对象那边
                    if (getParaToChildFlag)
                    {

                        forceValue2D = springJoint2D.reactionForce;
                        torqueValue2D = springJoint2D.reactionTorque;

                    }
                    else
                    {

                        forceValue2D = -springJoint2D.reactionForce;
                        torqueValue2D = -springJoint2D.reactionTorque;
                    }

                }
                else
                {
                    bool ispara = false;


                    SpringJoint2D[] springJoint2Ds = paraGameObject.GetComponents<SpringJoint2D>();
                    for (int i = 0; i < springJoint2Ds.Length; i++)
                    {
                        if (springJoint2D == springJoint2Ds[i])
                        {
                            break;
                            ispara = true;
                        }
                    }

                    if (ispara == true)
                    {
                        //关节挂在父对象那边
                        if (getParaToChildFlag)
                        {

                            forceValue2D = -springJoint2D.reactionForce;
                            torqueValue2D = -springJoint2D.reactionTorque;

                        }
                        else
                        {

                            forceValue2D = springJoint2D.reactionForce;
                            torqueValue2D = springJoint2D.reactionTorque;
                        }
                    }
                    else
                    {
                        //关节挂在子对象那边
                        if (getParaToChildFlag)
                        {

                            forceValue2D = springJoint2D.reactionForce;
                            torqueValue2D = springJoint2D.reactionTorque;

                        }
                        else
                        {

                            forceValue2D = -springJoint2D.reactionForce;
                            torqueValue2D = -springJoint2D.reactionTorque;

                        }
                    }

                }
            }
            else if (targetJoint2DFlag)
            {
                if (paraGameObject.GetComponent<TargetJoint2D>() == null)
                {
                    //关节挂在子对象那边
                    if (getParaToChildFlag)
                    {

                        forceValue2D = targetJoint2D.reactionForce;
                        torqueValue2D = targetJoint2D.reactionTorque;

                    }
                    else
                    {

                        forceValue2D = -targetJoint2D.reactionForce;
                        torqueValue2D = -targetJoint2D.reactionTorque;
                    }

                }
                else
                {
                    bool ispara = false;


                    TargetJoint2D[] targetJoint2Ds = paraGameObject.GetComponents<TargetJoint2D>();
                    for (int i = 0; i < targetJoint2Ds.Length; i++)
                    {
                        if (targetJoint2D == targetJoint2Ds[i])
                        {
                            break;
                            ispara = true;
                        }
                    }

                    if (ispara == true)
                    {
                        //关节挂在父对象那边
                        if (getParaToChildFlag)
                        {

                            forceValue2D = -targetJoint2D.reactionForce;
                            torqueValue2D = -targetJoint2D.reactionTorque;

                        }
                        else
                        {

                            forceValue2D = targetJoint2D.reactionForce;
                            torqueValue2D = targetJoint2D.reactionTorque;
                        }
                    }
                    else
                    {
                        //关节挂在子对象那边
                        if (getParaToChildFlag)
                        {

                            forceValue2D = targetJoint2D.reactionForce;
                            torqueValue2D = targetJoint2D.reactionTorque;

                        }
                        else
                        {

                            forceValue2D = -targetJoint2D.reactionForce;
                            torqueValue2D = -targetJoint2D.reactionTorque;

                        }
                    }

                }
            }
            else if (wheelJoint2DFlag)
            {
                if (paraGameObject.GetComponent<WheelJoint2D>() == null)
                {
                    //关节挂在子对象那边
                    if (getParaToChildFlag)
                    {

                        forceValue2D = wheelJoint2D.reactionForce;
                        torqueValue2D = wheelJoint2D.reactionTorque;

                    }
                    else
                    {

                        forceValue2D = - wheelJoint2D.reactionForce;
                        torqueValue2D = - wheelJoint2D.reactionTorque;
                    }

                }
                else
                {
                    bool ispara = false;


                    WheelJoint2D[] wheelJoint2Ds = paraGameObject.GetComponents<WheelJoint2D>();
                    for (int i = 0; i < wheelJoint2Ds.Length; i++)
                    {
                        if (wheelJoint2D == wheelJoint2Ds[i])
                        {
                            break;
                            ispara = true;
                        }
                    }

                    if (ispara == true)
                    {
                        //关节挂在父对象那边
                        if (getParaToChildFlag)
                        {

                            forceValue2D = -wheelJoint2D.reactionForce;
                            torqueValue2D = -wheelJoint2D.reactionTorque;

                        }
                        else
                        {

                            forceValue2D = wheelJoint2D.reactionForce;
                            torqueValue2D = wheelJoint2D.reactionTorque;
                        }
                    }
                    else
                    {
                        //关节挂在子对象那边
                        if (getParaToChildFlag)
                        {

                            forceValue2D = wheelJoint2D.reactionForce;
                            torqueValue2D = wheelJoint2D.reactionTorque;

                        }
                        else
                        {

                            forceValue2D = -wheelJoint2D.reactionForce;
                            torqueValue2D = -wheelJoint2D.reactionTorque;

                        }
                    }

                }
            }

        }
       


    }

}