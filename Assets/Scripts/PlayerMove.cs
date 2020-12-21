using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

public class PlayerMove : NetworkBehaviour {

    public NavMeshAgent agent;
    public GameObject aoe;

    // Use this for initialization
    void Start () {
        agent = this.GetComponent<NavMeshAgent>();
    }
	
	// Update is called once per frame
	void Update () {
        if (!this.isLocalPlayer)
        {
            return;
        }
        if (Input.GetMouseButtonDown(1) || Input.GetMouseButton(1))
        {
            StopAllCoroutines();
            SetDestination();   
        }
    }

    private void SetDestination()
    {
        Ray ray = this.GetComponent<PlayerManager>().PlayerCamera.ScreenPointToRay(Input.mousePosition);
        float point;
        if (GameMaster.Master.ground.Raycast(ray, out point))
        {
            agent.destination = ray.GetPoint(point);
        }
    }

    public void CastAoe()
    {
        Ray ray = this.GetComponent<PlayerManager>().PlayerCamera.ScreenPointToRay(Input.mousePosition);
        float point;
        Vector3 clickLocation = new Vector3();
        if (GameMaster.Master.ground.Raycast(ray, out point))
        {
            clickLocation = ray.GetPoint(point);
        }

        if (Vector3.Distance(transform.position, clickLocation) > 3f)
        {
            StartCoroutine("moveuntil", clickLocation);
        }
        else
        {
            Instantiate(aoe, clickLocation, Quaternion.identity);
        }
    }

    private IEnumerator moveuntil(Vector3 point)
    {
        agent.SetDestination(point);
        yield return new WaitUntil(() => Vector3.Distance(this.transform.position, point) <= 3);
        agent.SetDestination(transform.position);
        Instantiate(aoe, point, Quaternion.identity);
    }
}
