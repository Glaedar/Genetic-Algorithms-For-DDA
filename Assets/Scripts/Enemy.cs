using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour {

    //set up variable the enemy uses
    public float startSpeed;
    public float speed;

    public float startHealth;
    public float health;

    public float armour;
    private float maxArmour = 40.0f;

    private float maxSlow = 45.0f;
    public float slowResistance;

    private Transform target;
    private int wavePointIndex = 0;

    Vector3 lastPosition;
    private float distanceTraveled;
    float finalDistanceTraveled;


    public Image healthbar;

    private WaveSpawner waveSpawner;

    private int enemyNum;
    private int speedData;
    private int healthData;
    private int armourData;
    private int slowData;

    private int speedArmourSame;
    private int slowHealthSame;

    void Awake()
    {
  
    }

	// Use this for initialization
	void Start ()
    {
        //gain access to the wavespawner script
        GameObject waveSpawnController = GameObject.Find("WaveSpawnController");
        waveSpawner = waveSpawnController.GetComponent<WaveSpawner>();

        //get the number of this enemy the wavespawner has dedicated it
        enemyNum = waveSpawner.enemyNumber;
       
        //use enemy number to get the stats created by the genetic algorithm
        speedData = waveSpawner.speed[enemyNum];
        healthData = waveSpawner.health[enemyNum];
        armourData = waveSpawner.armour[enemyNum];
        slowData = waveSpawner.slowResist[enemyNum];

        //convert the values of the stats to floats and apply to the enemy
        startSpeed = (float)speedData;
        startHealth = (float)healthData;
        armour = (float)armourData;
        slowResistance = (float)slowData;


        
        if (startSpeed < 4)
        {
            startSpeed = 5;
        }
        if (startHealth < 4)
        {
            startHealth = 5;
        }

        //if corresponding values are the same then add 1 to a random variable
        if(startHealth == slowResistance)
        {
            slowHealthSame = Random.Range(0, 2);
            if (slowHealthSame == 0)
            {
                startHealth = startHealth + 1;
            }
            if (slowHealthSame == 1)
            {
                slowResistance = slowResistance + 1;
            }
        }

        if (startHealth == slowResistance)
        {
            speedArmourSame = Random.Range(0, 2);
            if (speedArmourSame == 0)
            {
                startSpeed = startSpeed + 1;
            }
            if (speedArmourSame == 1)
            {
                armour = armour + 1;
            }
        }

        // divide corresponding values depending on which is higher
        if (startSpeed > armour && startSpeed > 22)
        {
            armour = armour / 2;
        }

        if (armour > startSpeed)
        {
            startSpeed = startSpeed / 2;
        }

        if (slowResistance > startHealth)
        {
            startHealth = startHealth / 2;
        }

        if (startHealth > slowResistance)
        {
            slowResistance = slowResistance / 2;
        }

        //set the first waypoint as where it needs to move to
        target = Waypoints.wayPoints[0];
        health = startHealth;
        speed = startSpeed;
        lastPosition = transform.position;

    }
	
    //set up the percent the moveing is slowed to
    public void moveSlow(float percent)
    {
        
        float resistPercent = slowResistance / maxSlow;
        if (percent >= resistPercent)
        {
            speed = startSpeed * (1f - (percent - resistPercent));
        }
    }

    //if the amount of damage taken is lower than the armour then take damage
    public void TakeDamage (float amount)
    {
        float resistDamage = armour / maxArmour;
        health -= amount * (1f - resistDamage);
       

        healthbar.fillAmount = health / startHealth;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        //get the distance travelled for the fitness function
        waveSpawner.distanceEnemyreached[enemyNum] = distanceTraveled;
        Destroy(gameObject);
    }

    public float finalDistance()
    {
        finalDistanceTraveled = distanceTraveled;
        return finalDistanceTraveled;
    }



	// Update is called once per frame
	void Update ()
    {

        distanceTraveled += Vector3.Distance(transform.position, lastPosition);
        lastPosition = transform.position;

        // set drection the enemy is faceing towards the next waypoint
        Vector3 direction = target.position - transform.position;
        // Move the enemy towards the next waypoint
        transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);

  

        // if the enemy reaches the next waypoint then get the next one
        if (Vector3.Distance(transform.position, target.position) <= 0.4f)
        {
        
            GetNextWaypoint();
        }

        speed = startSpeed;


    }

    void GetNextWaypoint()
    {
        if(wavePointIndex >= Waypoints.wayPoints.Length -1)
        {
            reachEnd();
            return;
        }
        // Get the next waypoint in the list
        wavePointIndex++;
        // Set the new target to the next waypoint
        target = Waypoints.wayPoints[wavePointIndex];


    }

    void reachEnd()
    {
        PlayerStats.Lives--;
        Die();
    }
}
