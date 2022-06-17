/*
© Guandong Univesity of Technology , 2022
Author: Yuanlin Yang (yongwang0808@163.com)

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at
<http://www.apache.org/licenses/LICENSE-2.0>.
Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace DTUAV.ModelLearn.MotionModel
{
    public class GenerateTargetPosition
    {
        private Vector3 _upperPositionLimit;
        private Vector3 _lowerPositionLimit;
        private Vector3 _targetPosition;
        private Random _random;

        public bool SetUpperPositionLimit(Vector3 upperPositionLimit)
        {
            _upperPositionLimit = upperPositionLimit;
            return true;
        }

        public bool SetLowerPositionLimit(Vector3 lowerPositionLimit)
        {
            _lowerPositionLimit = lowerPositionLimit;
            return true;
        }
        public Vector3 GetLowerPositionLimit()
        {
            return _lowerPositionLimit;

        }
        public Vector3 GetUpperPositionLimit()
        {
            return _upperPositionLimit;
        }
        private float randData(float minRange, float maxRange)
        {
            return (float)(_random.NextDouble() * (maxRange - minRange) + minRange);
        }

        public Vector3 GetTargetPosition()
        {
            _targetPosition.x = randData(_lowerPositionLimit.x, _upperPositionLimit.x);
            _targetPosition.y = randData(_lowerPositionLimit.y, _upperPositionLimit.y);
            _targetPosition.z = randData(_lowerPositionLimit.z, _upperPositionLimit.z);
            return _targetPosition;
        }

        public Vector3 GetTargetPosition(Vector3 upperPositionLimit, Vector3 lowerPositionLimit)
        {
            _upperPositionLimit = upperPositionLimit;
            _lowerPositionLimit = lowerPositionLimit;
            _targetPosition.x = randData(_lowerPositionLimit.x, _upperPositionLimit.x);
            _targetPosition.y = randData(_lowerPositionLimit.y, _upperPositionLimit.y);
            _targetPosition.z = randData(_lowerPositionLimit.z, _upperPositionLimit.z);
            return _targetPosition;
        }
        public GenerateTargetPosition(Vector3 upperPositionLimit,Vector3 lowerPositionLimit)
        {
            _upperPositionLimit = upperPositionLimit;
            _lowerPositionLimit = lowerPositionLimit;
            _targetPosition = new Vector3();
            _random = new Random();
        }
    }
}
