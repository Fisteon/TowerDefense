using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerRange : MonoBehaviour {

    List<Collider> potentialTargets;
    Tower tower;

    // Use this for initialization
    void Start () {
        tower = this.transform.parent.GetComponent<Tower>();
        potentialTargets = new List<Collider>();
	}
	
	// Update is called once per frame
	void Update () {
        if (tower.currentTarget == null)
        {
            AcquireNewTarget();
        }
    }

    private void OnTriggerEnter(Collider target)
    {
        if (target.GetComponent<Monster>().playerOwner == tower.PlayerOwner)
        {
            return;
        }

        potentialTargets.Add(target);
        if (tower.currentTarget != null)
        {
            return;
        }
        tower.currentTarget = target.gameObject;
    }

    private void OnTriggerExit(Collider target)
    {
        if (tower.currentTarget == target.gameObject)
        {
            potentialTargets.Remove(target);
            tower.currentTarget = null;
        }
        AcquireNewTarget();
    }

    private void AcquireNewTarget()
    {
        Collider newTarget = null;
        float lowestDistance = Mathf.Infinity;

        foreach (Collider c in potentialTargets)
        {
            if (c == null)
            {
                continue;
            }
            float distance = Vector3.Distance(c.transform.position, this.transform.position);
            if (distance < lowestDistance)
            {
                lowestDistance = distance;
                newTarget = c;
            }
        }
        tower.currentTarget = newTarget == null ? null : newTarget.gameObject;
    }
}
