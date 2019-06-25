using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour {

    public static int Lives;
    public int startLives = 1;
    private int wave;
    private WaveSpawner waveSpawner;

    // Use this for initialization
    void Start ()
    {
        Lives = startLives;
        GameObject waveSpawnController = GameObject.Find("WaveSpawnController");
        waveSpawner = waveSpawnController.GetComponent<WaveSpawner>();
    }
	
	// Update is called once per frame
	void Update ()
    {
		if (Lives <= 0)
        {
            wave = waveSpawner.waveNumber;
            Debug.Log("Wave Number" + wave);
            PlayerPrefs.SetInt("WaveNumber", wave);
            SceneManager.LoadScene("MainMenu");
        }
	}
}
