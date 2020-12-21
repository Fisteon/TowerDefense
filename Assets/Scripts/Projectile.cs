using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    private Transform target;
    private Monster monster;
    private Tower tower;

    public int speed;

	// Use this for initialization
	void Start () {
        tower = transform.parent.GetComponent<Tower>();
        target = tower.currentTarget.transform;
        monster = target.GetComponent<Monster>();
	}
	
	// Update is called once per frame
	void Update () {
        if (target == null)
        {
            Destroy(this.gameObject);
            return;
        }
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target.position, step);
        transform.LookAt(target.position);
        if (Vector3.Distance(transform.position, target.position) < 0.5f)
        {
            monster.GotHit(tower.damage, tower);
            Destroy(this.gameObject);
        }
    }
}
