using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DNA {

    public List<int> bits;
    public float fitness;
    int bitValue;
    int getBestvalue = 0;

    public DNA()
    {
        Initialize();
    }

    public DNA(int numBits)
    {
        Initialize();

        for (int i = 0; i < numBits; i++)
        {
            //get either a 1 or a 0 and add it to the list of bits
            bitValue = Random.Range(0, 2);

            if(getBestvalue == 0)
            {
                bitValue = 0;
            }
            if (getBestvalue == 3)
            {
                getBestvalue = 0;
            }

            bits.Add(bitValue);
            getBestvalue++;
        }
    }

    private void Initialize()
    {
        fitness = 0;
        bits = new List<int>();
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
