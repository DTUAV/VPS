using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTUAVCARS.UAV
{
    public class PropellorRotate : MonoBehaviour
    {
        [Header("螺旋桨的旋转方向true:顺时针，false：逆时针")] public bool RotationDec;

        [Header("是否启动电机")] public bool IsStart;

        [Header("螺旋桨对象")] public GameObject PropellorObject;

        [Header("旋转速度/度每秒")] public float RotationAngleRate;

        private int _rotationDec;
      // Start is called before the first frame update
     //   void Start()
      //  {
      //  }
      void FixedUpdate()
        {
            if (IsStart)
            {
                _rotationDec = RotationDec ? 1 : -1;//设置无人机的运动方向
                PropellorObject.transform.Rotate(new Vector3(0, 1, 0), RotationAngleRate*Time.fixedDeltaTime * _rotationDec, Space.Self);
            }

        }
    }
}
