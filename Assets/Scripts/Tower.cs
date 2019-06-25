using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour {

    public Transform target;
    private Enemy targetEnemy;

    
    public float range = 10f;

    public float fireRate = 1f;
    private float fireCountdown = 0f;
    public GameObject projectilePrefab;

    public string enemyTag = "Enemy";

    public bool useSlow = false;
    public LineRenderer lineRenderer;

    public float slowPercent = 0.8f;

    public Transform firePoint;

	// Use this for initialization
	void Start ()
    {
        InvokeRepeating("UpdateTarget", 0f, 0.5f);
	}
	
    void UpdateTarget()
    {
        //find all of the enemies and put them in an array
        GameObject[] enimies = GameObject.FindGameObjectsWithTag(enemyTag);

        //store shortest distance to enemy that has been found so far
        float shortestDistance = Mathf.Infinity;

        // store the closest enemy in it's own temporary variable
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enimies)
        {
            //find the distance to each enemy
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                //set shorest distance to enemy found
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }


        //check if and enemy has been found and if it is in range
        if (nearestEnemy != null && shortestDistance <= range)
        {
            target = nearestEnemy.transform;
            targetEnemy = nearestEnemy.GetComponent<Enemy>();
        }
        else
        {
            target = null;
        }

    }
	// Update is called once per frame
	void Update ()
    {
		// if a target has not been found then return.
        if (target == null)
        {
            if (useSlow)
            {
                if (lineRenderer.enabled)
                {
                    lineRenderer.enabled = false;
                }
            }
            return;
        }
    
        if(useSlow)
        {
            Slow();
        }
        else
        {

            if (fireCountdown <= 0f)
            {
                Shoot();
                //determines how often a projectile will fire
                fireCountdown = 1f / fireRate;
            }

            fireCountdown -= Time.deltaTime;
        }

	}

    void Slow()
    {
        targetEnemy.moveSlow(slowPercent);
        if(!lineRenderer.enabled)
        {
            lineRenderer.enabled = true;
        }
        lineRenderer.SetPosition(0, firePoint.position);
        lineRenderer.SetPosition(1, target.position);

    }

    void Shoot()
    {
        //temp variable to instantiate the projectile game object
        GameObject madeProjectile = (GameObject)Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        //find the component of the projectile
        Projectile projectile = madeProjectile.GetComponent<Projectile>();

        if (projectile != null)
        {
            projectile.find(target);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
