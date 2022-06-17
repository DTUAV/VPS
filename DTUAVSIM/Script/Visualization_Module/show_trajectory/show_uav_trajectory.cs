/*
 * Author: Yuanlin Yang
 * Date: 2022-1-3
 * Location: Guangdong University of Technology
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
using UnityEngine;
using DTUAVCARS.DTVisualization;
public class show_uav_trajectory : MonoBehaviour
{
    private show_trajectory _showTrajectory;

    public Rigidbody UavRb;
    public Material Mat;
    public Color LineColor;
    public float WidthX;
    public float WidthY;
    public bool IsPlot;
    public float Duration;
    public bool IsAuto = true;

    public bool SetIsAuto(bool auto)
    {
        return (_showTrajectory.SetIsAuto(auto));
    }
    public bool ClearTrajectory()
    {
        _showTrajectory.ClearAllLine();
        return true;
    }
    // Start is called before the first frame update
    void Start()
    {
        _showTrajectory = new show_trajectory(Mat, LineColor, WidthX, WidthY, UavRb.position, IsPlot, Duration,IsAuto);
    }

    // Update is called once per frame
    [System.Obsolete]
    void Update()
    {
        _showTrajectory.SetCurrentPosition(UavRb.position);
        _showTrajectory.DrawLine();
    }


}
