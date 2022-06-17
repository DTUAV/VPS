using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class virtual_reality_synchronization_init : MonoBehaviour
{
    private string k = "-0.3";
    private string track_length = "2.0";
    private string track_width = "2.0";
    private string track_heigh = "2.0";
    private string maxVelocity = "1.0";

    public string eigth_track_scene = "virtual_reality_synchronization_eigth_track";

    public string square_track_scene = "virtual_reality_synchronization_square_track";

    public string triangle_track_scene = "virtual_reality_synchronization_triangle_track";
    // Start is called before the first frame update

    void OnGUI()
    {
        GUI.Label(new Rect(20, 20, 20, 20), "k: ");
        k = GUI.TextField(new Rect(40, 20, 50, 20), k);
        GUI.Label(new Rect(100, 20, 80, 20), "maxVelocity: ");
        maxVelocity = GUI.TextField(new Rect(180, 20, 50, 20), maxVelocity);
        GUI.Label(new Rect(20, 40, 80, 20), "track_length: ");
        track_length = GUI.TextField(new Rect(100, 40, 50, 20), track_length);
        GUI.Label(new Rect(160, 40, 80, 20), "track_width: ");
        track_width = GUI.TextField(new Rect(240, 40, 50, 20), track_width);
        GUI.Label(new Rect(300, 40, 80, 20), "track_heigh: ");
        track_heigh = GUI.TextField(new Rect(380, 40, 50, 20), track_heigh);

        if (GUI.Button(new Rect(80, 80, 150, 50), "eigth_track"))
        {
            SceneManager.LoadScene(eigth_track_scene);
            PlayerPrefs.SetFloat("k", Convert.ToSingle(k));
            PlayerPrefs.SetInt("track_id", 2);
            PlayerPrefs.SetFloat("maxVelocity", Convert.ToSingle(maxVelocity));
            PlayerPrefs.SetFloat("track_length", Convert.ToSingle(track_length));
            PlayerPrefs.SetFloat("track_width", Convert.ToSingle(track_width));
            PlayerPrefs.SetFloat("track_heigh", Convert.ToSingle(track_heigh));
        }

        if (GUI.Button(new Rect(80, 170, 150, 50), "square_track"))
        {
            SceneManager.LoadScene(square_track_scene);
            PlayerPrefs.SetFloat("k", Convert.ToSingle(k));
            PlayerPrefs.SetInt("track_id", 0);
            PlayerPrefs.SetFloat("maxVelocity", Convert.ToSingle(maxVelocity));
            PlayerPrefs.SetFloat("track_length", Convert.ToSingle(track_length));
            PlayerPrefs.SetFloat("track_width", Convert.ToSingle(track_width));
            PlayerPrefs.SetFloat("track_heigh", Convert.ToSingle(track_heigh));
        }

        if (GUI.Button(new Rect(80, 250, 150, 50), "triangle_track"))
        {
            SceneManager.LoadScene(triangle_track_scene);
            PlayerPrefs.SetFloat("k", Convert.ToSingle(k));
            PlayerPrefs.SetInt("track_id", 1);
            PlayerPrefs.SetFloat("maxVelocity", Convert.ToSingle(maxVelocity));
            PlayerPrefs.SetFloat("track_length", Convert.ToSingle(track_length));
            PlayerPrefs.SetFloat("track_width", Convert.ToSingle(track_width));
            PlayerPrefs.SetFloat("track_heigh", Convert.ToSingle(track_heigh));
        }
    }
}