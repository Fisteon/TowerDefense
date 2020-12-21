using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.Networking;


public class PlayerManager : NetworkBehaviour {

    #region Variables
    public Camera _PlayerCamera;
    [HideInInspector]
    public Camera PlayerCamera;

    [Header("Summonable")]
    public GameObject _Tower;
    public List<Monster> _Monsters;
    public TowerMask _TowerMask;

    [Header("Player stats")]
    public bool currentlyPlacingTower;

    [SyncVar]
    public string playerName;
    [SyncVar]
    public int goldAmount;
    [SyncVar]
    public int income;
    [SyncVar]
    public int lives;
    [SyncVar]
    public string timer;
    private int towerCounter;
    private int monsterCounter;

    [Header("UI elements")]
    public GameObject b_upgradeTower;
    public Canvas c_MainMenu;
    public RectTransform i_gold;
    public Text t_gold;
    public Text t_income;
    public Text t_time;
    public Text t_lives;
    public Text t_gameOverWin;
    public Text t_gameOverLose;

    public bool mouseOverButton;

    [Space(25)]
    public Transform MonsterDestination;
    public GameObject PlayerBuildZone;
    public List<Tower> selection;

    private List<Tower> towersBuilt_anticheat;
    public PlayerManager[] players;
    private TowerMask towerMask;
    private bool gameStarted;
    private float gameStartTime;

    private bool flashingGold;

    private PlayerManager EnemyPlayer;

    #endregion
    // Use this for initialization
    void Start () {
        selection = new List<Tower>();
        towersBuilt_anticheat = new List<Tower>();
        players = new PlayerManager[1];
        gameStarted = false;
        flashingGold = false;
        gameStartTime = 0;
        towerCounter = 0;
        monsterCounter = 0;
        goldAmount = 50;
        income = 10;
        lives = 30;

        i_gold = GameObject.Find("ImageGold").GetComponent<RectTransform>();
        t_gold = GameObject.Find("GoldAmount").GetComponent<Text>();
        t_income = GameObject.Find("IncomeList").GetComponent<Text>();
        t_time = GameObject.Find("Timer").GetComponent<Text>();
        t_lives = GameObject.Find("LifeAmount").GetComponent<Text>();
        t_gameOverLose = GameObject.Find("GameOverLose").GetComponent<Text>();
        t_gameOverWin = GameObject.Find("GameOverWin").GetComponent<Text>();
        c_MainMenu = GameObject.Find("CanvasMenu").GetComponent<Canvas>();
        b_upgradeTower = GameObject.Find("UpgradeTower");

        mouseOverButton = false;

        if (GameMaster.Master.players[1] == null)
        {
            this.name = "Player_1";
            GameMaster.Master.players[1] = this;
        }
        else
        {
            this.name = "Player_2";
            GameMaster.Master.players[2] = this;
        }
        
        MonsterDestination = GameObject.Find(this.name + "_MonsterDestination").transform;
        PlayerBuildZone = GameObject.Find(this.name + "_BuildZone");
        if (this.isLocalPlayer)
        {
            PlayerCamera = Instantiate(_PlayerCamera);
            PlayerCamera.name = this.name + "_Camera";
            foreach (ButtonAction ba in FindObjectsOfType<ButtonAction>())
            {
                ba.player = this;
            }
            b_upgradeTower.GetComponent<ButtonAction>().player = this;
            playerName = GameObject.Find("InputPlayerName").GetComponent<InputField>().textComponent.text;
            if (playerName == "")
            {
                playerName = "Anonymous";
            }
            CmdSyncPlayerNames(playerName);
        }
    }

	// Update is called once per frame
	void Update () {
        if (!this.isLocalPlayer)
        {
            if (players.Length != 2)
            {
                players = FindObjectsOfType<PlayerManager>();
            }
            return;
        }
        #region UI updates and Game start
        timer = GameMaster.Master.Timer(Time.time - gameStartTime);
        if (players.Length != 2)
        {
            players = FindObjectsOfType<PlayerManager>();
            return;
        }
        else
        {
            if (!gameStarted)
            {
                c_MainMenu.enabled = false;
                gameStarted = true;
                gameStartTime = Time.time;
                InvokeRepeating("AddIncomeToGold", 15f, 15f);
                foreach (PlayerManager player in players)
                {
                    if (player != this)
                    {
                        EnemyPlayer = player;
                    }
                }
            }

            t_income.text = "";
            t_lives.text = "";
            t_gold.text = this.goldAmount.ToString();

            foreach (PlayerManager player in players)
            {
                if (isServer)
                {
                    t_time.text = timer;
                }
                else if (player != this)
                {
                    t_time.text = player.timer;
                }
                t_income.text += player.playerName + " – " + player.income + "\n";
                t_lives.text += player.playerName + " – " + player.lives + "\n";
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!currentlyPlacingTower)
            {
                c_MainMenu.enabled = !c_MainMenu.enabled;
            }
        }
        #endregion

        #region Game Over check
        if (this.lives == 0 || (this.EnemyPlayer != null && this.EnemyPlayer.lives == 0))
        {
            if (isServer)
            {
                foreach(Monster m in FindObjectsOfType<Monster>())
                {
                    Destroy(m.gameObject);
                }
            }
            if (isLocalPlayer)
            {
                if (this.lives == 0)
                {
                    t_gameOverLose.enabled = true;
                }
                else
                {
                    t_gameOverWin.enabled = true;
                }
            }
            return;
        }
        #endregion

        #region Tower Placing and Selecting
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!currentlyPlacingTower)
            {
                Ray ray = PlayerCamera.ScreenPointToRay(Input.mousePosition);
                float point;
                Vector3 clickLocation = new Vector3();
                if (GameMaster.Master.ground.Raycast(ray, out point))
                {
                    clickLocation = GameMaster.Master.RoundTheLocation(ray.GetPoint(point));
                }
                InitiateTowerPlacement(clickLocation);
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (currentlyPlacingTower)
            {
                CheckTowerPlacementPosition();
            }
            else if (!mouseOverButton)
            {
                Ray ray = PlayerCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    if (hit.collider.transform.tag != "Tower")
                    {
                        ClearSelection();
                    }
                    else
                    {
                        Tower clickedTower = hit.transform.GetComponent<Tower>();
                        if (clickedTower.PlayerOwner == this)
                        {
                            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                            {
                                this.AddRemoveTowerToSelection(clickedTower);
                            }
                            else
                            {
                                this.SelectTower(clickedTower);
                            }
                        }
                    }
                }
                else
                {
                    ClearSelection();
                }
            }
        }

        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentlyPlacingTower)
            {
                Destroy(towerMask.gameObject);
                currentlyPlacingTower = false;
            }
        }
        #endregion

        #region Summoning
        if (Input.GetKeyDown(KeyCode.C))
        {
            SummonMonster(0);
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            SummonMonster(1);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            SummonMonster(2);
        }
        #endregion

        // TESTING PURPOSES ONLY
        if (Input.GetKeyDown(KeyCode.G))
        {
            CmdCHEAT();
        }
    }

    [Command]
    void CmdCHEAT()
    {
        this.goldAmount += 100;
    }

    #region PlayerName
    [Command]
    void CmdSyncPlayerNames(string pn)
    {
        playerName = pn;
    }

    [ClientRpc]
    void RpcSyncPlayerNames(string pn)
    {
        playerName = pn;
    }

#endregion

    private void AddIncomeToGold()
    {
        CmdIncomeGold();
    }

    [Command]
    void CmdIncomeGold()
    {
        RpcIncomeGold();
    }

    [ClientRpc]
    void RpcIncomeGold()
    {
        this.goldAmount += this.income;
    }

    public void AwardBounty(int bounty) {
        this.goldAmount += bounty;
    }

    public void LostLife()
    {
        if (!this.isLocalPlayer)
        {
            return;
        }
        this.lives--;
        if (lives == 0)
        {
            // Game Over
        }
    }

    private void OnDestroy()
    {
        if (PlayerCamera != null)
        {
            Destroy(PlayerCamera.gameObject);
        }
    }

    #region Tower Management

    private void InitiateTowerPlacement(Vector3 mousePosition) {
        currentlyPlacingTower = true;
        towerMask = Instantiate(_TowerMask, mousePosition, Quaternion.identity);
        towerMask.camera = PlayerCamera;
        towerMask.buildZone = PlayerBuildZone;
    }

    private void CheckTowerPlacementPosition()
    {
        if (towerMask.outOfBounds)
        {
            return;
        }
        Collider[] obstacles = Physics.OverlapBox(towerMask.transform.position, towerMask.transform.localScale / 2, Quaternion.identity, LayerMask.GetMask("Default"));
        if (obstacles.Length == 0)
        {
            PlaceTower();
        }
        else
        {
            foreach (Collider c in obstacles)
            {
                if (!c.isTrigger)
                {
                    return;
                }
            }
            PlaceTower();
        }
    }

    public void UpgradeSelectedTowers()
    {
        foreach(Tower t in selection)
        {
            if (t.towerLevel < 2)
            {
                if (this.goldAmount >= t.upgradeCost)
                {
                    CmdUpgradeTower(t.gameObject);
                }
            }
        }
    }

    [Command]
    void CmdUpgradeTower(GameObject t)
    {
        RpcUpgradeTower(t);
    }

    [ClientRpc]
    void RpcUpgradeTower(GameObject tower)
    {
        Tower t = tower.GetComponent<Tower>();
        t.TowerLevelModels[t.towerLevel].SetActive(false);
        t.towerLevel++;
        t.TowerLevelModels[t.towerLevel].SetActive(true);
        t.Upgrade();
    }

    private void PlaceTower()
    {
        if (this.goldAmount < _Tower.GetComponent<Tower>().cost)
        {
            StartCoroutine("FlashGold");
            return;
        }
        // Anticheat is a list of all towers built
        // We add a tower to it whenever it is spawned, and in case of no available path
        // towers are removed LIFO style until the path is available again
        CmdSpawnTower(towerMask.transform.position);
        Destroy(towerMask.gameObject);
    }

    private IEnumerator FlashGold()
    {
        if (!flashingGold)
        {
            flashingGold = true;
            while (i_gold.sizeDelta.x < 70)
            {
                i_gold.sizeDelta = new Vector2(i_gold.sizeDelta.x + 1, i_gold.sizeDelta.y + 1);
                yield return new WaitForSeconds(0.015f);
            }
            while (i_gold.sizeDelta.x > 50)
            {
                i_gold.sizeDelta = new Vector2(i_gold.sizeDelta.x - 1, i_gold.sizeDelta.y - 1);
                yield return new WaitForSeconds(0.015f);
            }
        }
        flashingGold = false;
    }

    [Command]
    public void CmdSpawnTower(Vector3 location)
    {
        GameObject tower = Instantiate(_Tower, location, Quaternion.identity);
        NetworkServer.Spawn(tower);

        RpcSetupTower(location, tower);
    }

    [ClientRpc]
    void RpcSetupTower(Vector3 location, GameObject t)
    {
        Tower tower = t.GetComponent<Tower>();
        
        tower.name = this.name + "_Tower_" + this.towerCounter;
        tower.PlayerOwner = this;
        this.towersBuilt_anticheat.Add(tower);
        this.currentlyPlacingTower = false;
        this.towerCounter++;
        this.goldAmount -= tower.cost;
    }

    public void SelectTower(Tower tower)
    {
        ClearSelection();
        selection.Add(tower);
        tower.IsSelected = true;
        if (tower.towerLevel < 3)
        {
            //b_upgradeTower.SetActive(true);
        }
    }

    private void ClearSelection()
    {
        foreach(Tower t in selection)
        {
            t.IsSelected = false;
        }
        selection.Clear();
        //b_upgradeTower.SetActive(false);
    }

    public void AddRemoveTowerToSelection(Tower tower)
    {
        if (selection.Contains(tower))
        {
            selection.Remove(tower);
            tower.IsSelected = false;
        }
        else
        {
            selection.Add(tower);
            tower.IsSelected = true;
        }
    }

    public void DestroyTower(Tower t)
    {
        if (t.PlayerOwner != this)
        {
            return;
        }
        towersBuilt_anticheat.Remove(t);
        selection.Remove(t);
        CmdDestroyTower(t.gameObject);
    }

    [Command]
    void CmdDestroyTower(GameObject t)
    {
        NetworkServer.Destroy(t.gameObject);
    }

    #endregion

    #region Monster Management

    public void SummonMonster(int type)
    {
        CmdSpawnMonster(type);
    }

    [Command]
    void CmdSpawnMonster(int type)
    {
        if (this.goldAmount < _Monsters[type].cost)
        {
            return;
        }

        Monster m = Instantiate(this._Monsters[type], GameMaster.Master.GetRandomSpawnPoint(), Quaternion.identity);
        NetworkServer.Spawn(m.gameObject);

        RpcSetupMonster(m.gameObject);
    }

    [ClientRpc]
    void RpcSetupMonster(GameObject m)
    {
        Monster monster = m.GetComponent<Monster>();

        monster.name = "Monster_" + this.monsterCounter;
        monsterCounter++;
        monster.playerOwner = this;
        foreach (PlayerManager p in this.players)
        {
            if (p != this)
            {
                monster.playerEnemey = p;
            }
        }
        monster.agent = monster.GetComponent<NavMeshAgent>();
        monster.agent.destination = this.MonsterDestination.position;

        this.goldAmount -= monster.cost;
        this.income += monster.income;
    }

    public IEnumerator NoPath(NavMeshAgent monster)
    {
        int counter = 1;
        //NavMeshPath path = new NavMeshPath();
        do
        {
            Tower block = towersBuilt_anticheat[towersBuilt_anticheat.Count - 1];
            towersBuilt_anticheat.Remove(block);
            Destroy(block.gameObject);
            Debug.Log("Tower deletion - " + counter++);
            
            monster.ResetPath();
            monster.SetDestination(MonsterDestination.position);
            // Wait for the next frame for calculation
            yield return new WaitForSeconds(0.3f);

            // If monster reached the deadzone while coroutine was waiting
            if (monster == null) break;
        } while (monster.pathStatus != NavMeshPathStatus.PathComplete);

        GameMaster.Master.player1_PathAvailable = true;
        Monster.fixingPath = false;
    }

    #endregion
}
