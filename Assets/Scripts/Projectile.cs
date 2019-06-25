using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    private Transform target;

    public float speed = 70f;
    public int damage = 50;
    public float splashRadius = 0f;
    public GameObject impactEffect;

    public void find(Transform _target)
    {
        target = _target;
    }

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 direction = target.position - transform.position;

        float distancePerFrame = speed * Time.deltaTime;

        // if the magnitude (current distance to target) of the direction is less than the distance travelled in the frame then it has hit
        if (direction.magnitude <= distancePerFrame)
        {
            HitTarget();
            return;
        }

        //translate and normalise so it move at a constant speed
        transform.Translate(direction.normalized * distancePerFrame, Space.World);

	}

    void HitTarget ()
    {
        GameObject effectinstance = (GameObject)Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(effectinstance, 2f);

        if (splashRadius > 0f)
        {
            Splash();
        }
        else
        {
            Damage(target);
        }

        Destroy(gameObject);
    }

    //apply splash area to the projectile
    void Splash()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, splashRadius);
        foreach (Collider collider in colliders)
        {
            if (collider.tag == "Enemy")
            {
                Damage(collider.transform);
            }
        }
    }

    //damage the enemy
    void Damage(Transform enemy)
    {
        Enemy e = enemy.GetComponent<Enemy>();

        if (e != null)
        {
            e.TakeDamage(damage);
            //Destroy(enemy.gameObject);
        }
   
    }

    // to check the range in the scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, splashRadius);
    }
}
