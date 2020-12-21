using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadZone : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Monster>() != null)
        {
            other.GetComponent<Monster>().ReachedDeadZone();
        }
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
