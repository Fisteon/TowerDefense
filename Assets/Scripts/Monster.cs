using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;
using UnityEngine.UI;

public class Monster : NetworkBehaviour {

    [Header("HP bar")]
    public Slider sliderHP;
    private RectTransform sliderPosition;
    private RectTransform canvasSize;
    private Camera camera;

    [Header("Monster stats")]
    public int bounty;
    [SyncVar]
    public int health;
    public int income;
    public int cost;

    private int dmgTaken;
    private bool invulnerable = false;

    public static bool fixingPath = false;

    [Space(10)]
    public PlayerManager playerOwner;
    public PlayerManager playerEnemey;

    public NavMeshAgent agent;
    private NavMeshPath path;

	// Use this for initialization
	void Start () {
        path = new NavMeshPath();
        //agent.CalculatePath()
        canvasSize = GameObject.Find("CanvasIngame").GetComponent<RectTransform>();
        sliderHP = Instantiate(sliderHP, transform.position, Quaternion.identity, canvasSize.transform);
        sliderPosition = sliderHP.GetComponent<RectTransform>();
        camera = FindObjectsOfType<Camera>().Where(c => c.name.StartsWith("Player_") == true).ToList()[0];
        sliderHP.maxValue = health;
        sliderHP.value = health;
    }
	
    
	// Update is called once per frame
	void Update ()
    {
        sliderHP.value = health;
        UpdateHpPosition();
        if (Time.timeScale > 1.0f && agent.hasPath)
        {
            NavMeshHit hit;
            float maxAgentTravelDistance = Time.deltaTime * agent.speed;

            //If at the end of path, stop agent.
            if (
                agent.SamplePathPosition(NavMesh.AllAreas, maxAgentTravelDistance, out hit) ||
                agent.remainingDistance <= agent.stoppingDistance
            )
            {
                Debug.Log("Stopping agent!");
                //Stop agent
            }
            //Else, move the actor and manually update the agent pos
            else
            {
                transform.position = hit.position;
                agent.nextPosition = transform.position;
            }
        }
        
        if (agent.pathStatus != NavMeshPathStatus.PathComplete)
        {
            Debug.Log(agent.pathStatus);
            if (GameMaster.Master.player1_PathAvailable && !fixingPath)
            {
                fixingPath = true;
                GameMaster.Master.player1_PathAvailable = false;
                StartCoroutine("WaitForPath");
            }
            else
            {
                if (!invulnerable)
                StartCoroutine("GetInvulnerability");
            }
        }
    }

    public void ReachedDeadZone()
    {
        CmdLoseLife();
        Destroy(this.gameObject);
    }

    public override void OnNetworkDestroy()
    {
        Destroy(sliderHP.gameObject);
    }

    [Command]
    void CmdLoseLife()
    {
        this.playerEnemey.lives--;
    }

    private IEnumerator WaitForPath()
    {
        Debug.Log("No path!");
        playerOwner.StartCoroutine("NoPath", agent);
        //playerOwner.NoPath(agent);

        invulnerable = true;
        yield return new WaitUntil(() => GameMaster.Master.player1_PathAvailable == false);
        invulnerable = false;
    }

    private IEnumerator GetInvulnerability()
    {
        invulnerable = true;
        yield return new WaitUntil(() => GameMaster.Master.player1_PathAvailable == false);
        invulnerable = false;
    }

    public void GotHit(int damage, Tower towerFired)
    {
        if (invulnerable) return;
        if (!isServer) return;

        health -= damage;
        sliderHP.value = health;
        if (health <= 0)
        {
            towerFired.PlayerOwner.AwardBounty(bounty);
            Destroy(this.gameObject);
        }
    }

    private void UpdateHpPosition()
    {
        Vector3 monsterPosition = this.transform.position;
        monsterPosition.y += 1.5f;
        Vector2 worldPosition = camera.WorldToViewportPoint(monsterPosition);
        Vector2 screenPosition = new Vector2((worldPosition.x * canvasSize.sizeDelta.x - canvasSize.sizeDelta.x * 0.5f), (worldPosition.y * canvasSize.sizeDelta.y - canvasSize.sizeDelta.y * 0.5f));
        sliderPosition.anchoredPosition = screenPosition;
    }
}
