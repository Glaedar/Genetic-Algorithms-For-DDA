using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class WaveSpawner : MonoBehaviour {

    public Transform enemyPrefab;

    public Transform spawnPoint;

    public float waveTimer = 5f;
    private float countdown = 2f;

    public Text waveCountdownText;

    public int waveNumber = 0;

    private Transform calculateDistance;

    public GeneticAlgorithm geneticAlgorithm;
    public List<int> fittestStats;

    //public GeneticAlgorithm geneticAlgorithm;

    public List<float> enemyDistance;

    public Enemy enemy;
   // List<int> stats;


    public float[] distanceEnemyreached;

    //set up arrays the GA will put values into
    public int[] speed;
    public int[] health;
    public int[] armour;
    public int[] slowResist;

    public int statArrayNum = 0;

    private int waypointIndex = 0;
    private int waypointNum = 0;

    float totalDistance;

    public int enemyNumber = 0;
    private int populationNum = 0;
   
    // Use this for initialization
    void Start ()
    {

        calculateDistance = Waypoints.wayPoints[0];

        GameObject[] waypoint = GameObject.FindGameObjectsWithTag("Waypoint");

        while (waypointIndex < Waypoints.wayPoints.Length - 1)
        {
            float addDistance = Vector3.Distance(waypoint[waypointNum].transform.position, calculateDistance.position);
            totalDistance += addDistance;
            waypointNum++;
            waypointIndex++;
            calculateDistance = Waypoints.wayPoints[waypointIndex];
        }
        // totalDistance = totalDistance - 42.3244f;
        totalDistance = 279.7f;
        Debug.Log("total distance = " + totalDistance);

        distanceEnemyreached = new float[20];
        speed = new int[20];
        health = new int[20];
        armour = new int[20];
        slowResist = new int[20];

        //enemy = new Enemy();
        //enemy.waveSpawner = this;
        geneticAlgorithm = new GeneticAlgorithm();
        geneticAlgorithm.waveSpawner = this;

        geneticAlgorithm.Run();
    }

    public float calculateFitness(List<int> stats)
    {
        //calculate the fitness score and send it back to the genetic algorithm
        float fitnessScore = 0;

        fitnessScore = distanceEnemyreached[populationNum] / totalDistance;

        populationNum++;
        if (populationNum > 19)
        {
            populationNum = 0;
        }

        return fitnessScore;

    }



    // Update is called once per frame
    void Update ()
    {
        if (GameObject.Find("Enemy(Clone)") == null)
        {
            //decrease timer every second
            countdown -= Time.deltaTime;
            //if timer has reached 0 spawn wave and reset timer
            if (countdown <= 0f)
            {

                enemyNumber = 0;
                //set true to make ga run
                if (waveNumber > 0)
                {
                    clearArrays();
                    geneticAlgorithm.busy = true;

                }
                
                StartCoroutine(SpawnWave());
                countdown = waveTimer;
            }
        }

        waveCountdownText.text = "Next Wave In: " + Mathf.Round(countdown).ToString();
	}

    void clearArrays ()
    {
        speed = new int[20];
        health = new int[20];
        armour = new int[20];
        slowResist = new int[20];
    }
    

    IEnumerator SpawnWave ()
    {
        //run epoch of genetic algorithm
        if (geneticAlgorithm.busy) geneticAlgorithm.Epoch();


        distanceEnemyreached = new float[20];
        waveNumber++;
        //PlayerStats.wave = waveNumber;

        for (int i = 0; i < 19; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(0.2f);
        }

        geneticAlgorithm.busy = false;

    }



    public void EnemyStatsIntoArrays(List<int> stats)
    {
        for(int statsIndex = 0; statsIndex < stats.Count; statsIndex++)
        {
            //Debug.Log(stats.Count);
            if (statsIndex == 0)
            {          
                speed[statArrayNum] = stats[statsIndex];
                //Debug.Log(speed[statArrayNum]);
            }
            if (statsIndex == 1)
            {      
                health[statArrayNum] = stats[statsIndex];
                //Debug.Log(health[statArrayNum]);
            }
            if (statsIndex == 2)
            {
                armour[statArrayNum] = stats[statsIndex];
            }
            if (statsIndex == 3)
            {
                slowResist[statArrayNum] = stats[statsIndex];
            }
            
            //Debug.Log("Speed " + speed.Length + " health " + health.Length + " Armour " + armour.Length + " SlowResist " + slowResist.Length);
        }

        //Debug.Log(health[10]);
        if (statArrayNum < 19)
        {
            statArrayNum++;
        }
        else
        {
            statArrayNum = 0;
        }
    
    }

    void SpawnEnemy()
    {
        //put the enemy game object into the level
        Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        if (enemyNumber < 19)
        {
            enemyNumber++;
        }


    }
}
