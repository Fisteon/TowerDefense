using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Tower : NetworkBehaviour {

    [Header("Prefabs")]
    public PlayerManager PlayerOwner;
    public GameObject projectile;
    public GameObject selectionLine;
    [Space(5)]
    public List<GameObject> TowerLevelModels;

    [Header("Tower info")]
    public GameObject currentTarget;

    [SyncVar]
    public float fireRate;
    [SyncVar]
    public int damage;
    [SyncVar]
    public int towerLevel;
    
    public bool isSelected;

    [Space(8)]
    public int cost;
    public int upgradeCost;
    [Space(8)]

    public Material material_up_one;
    public Material material_up_two;

    // Use this for initialization
    void Start() {
        damage = 25;
        upgradeCost = 10;
        InvokeRepeating("Fire", 0f, fireRate);
        towerLevel = 0;
        selectionLine = transform.GetChild(1).gameObject;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            if (isSelected)
            {
                PlayerOwner.DestroyTower(this);
            }
        }
	}

    public void Upgrade()
    {
        CancelInvoke();
        this.damage *= 2;
        this.fireRate *= 0.85f;
        InvokeRepeating("Fire", 0f, fireRate);
    }

    private void Fire()
    {
        if (currentTarget != null)
        {
            Instantiate(projectile, transform);
        }
        else
        {
            CancelInvoke();
            InvokeRepeating("Fire", fireRate, fireRate);
        }
    }
    
    private void SetLineActiveState(bool selected)
    {
        selectionLine.SetActive(selected);
    }

    public bool IsSelected
    {
        get
        {
            return this.isSelected;
        }
        set
        {
            this.isSelected = value;
            SetLineActiveState(isSelected);
        }
    }
}
