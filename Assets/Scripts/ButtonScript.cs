using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class ButtonScript : MonoBehaviour {

    public Text waveNumber;

    void Start()
    {
        waveNumber.text = "Wave Reached : " + PlayerPrefs.GetInt("WaveNumber").ToString();
    }

    public void NewGameButton(string newGameLevel)
    {
        SceneManager.LoadScene(newGameLevel);
    }

    public void ExitBtn()
    {
        Application.Quit();
    }
	// Use this for initialization
	
	
	// Update is called once per frame
	void Update () {
		
	}
}
