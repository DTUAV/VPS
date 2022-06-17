using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DTUAVCARS.DTMission;
using DTUAVCARS.DTPlanning;

public class init_interface : MonoBehaviour
{
    public velocity_command VecCommand;

    public eight_track_uav EightTrackUav;

    public square_track_uav SquareTrackUav;

    public triangle_track_uav TriangleTrackUav;
    // Start is called before the first frame update
    void Awake()
    {
        VecCommand.K = PlayerPrefs.GetFloat("k", -0.3f);
        VecCommand.MaxVelocityX = PlayerPrefs.GetFloat("maxVelocity", 1);
        VecCommand.MaxVelocityY = PlayerPrefs.GetFloat("maxVelocity", 1);
        VecCommand.MaxVelocityZ = PlayerPrefs.GetFloat("maxVelocity", 1);
        int track_id = PlayerPrefs.GetInt("track_id", 0);
        switch (track_id)
        {
            case 0:
            {
                SquareTrackUav.Heigh = PlayerPrefs.GetFloat("track_heigh", 2);
                SquareTrackUav.Lendth = PlayerPrefs.GetFloat("track_width", 2);
                SquareTrackUav.Width = PlayerPrefs.GetFloat("track_length", 2);
            }
                break;
            case 1:
            {
                TriangleTrackUav.Heigh = PlayerPrefs.GetFloat("track_heigh", 2);
                TriangleTrackUav.Lendth = PlayerPrefs.GetFloat("track_width", 2);
                TriangleTrackUav.Width = PlayerPrefs.GetFloat("track_length", 2);
            }
                break;
            case 2:
            {
                EightTrackUav.Heigh = PlayerPrefs.GetFloat("track_heigh", 2);
                EightTrackUav.Lendth = PlayerPrefs.GetFloat("track_width", 2);
                EightTrackUav.Width = PlayerPrefs.GetFloat("track_length", 2);
            }
                break;
            default:
            {

            }
                break;
        }
    }
}
