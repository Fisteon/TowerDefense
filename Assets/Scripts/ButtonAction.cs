using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonAction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    Button button;
    public PlayerManager player;

	// Use this for initialization
	void Start () {
        button = this.GetComponent<Button>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SummonMonster()
    {
        int monsterType = (int.Parse(transform.name.Split('_')[1])  - 1);
        player.SummonMonster(monsterType);
    }

    public void UpgradeTower()
    {
        Debug.Log("Upgrading tower...");
        player.UpgradeSelectedTowers();
    }

    public void OnPointerEnter(PointerEventData data)
    {
        player.mouseOverButton = true;
        Debug.Log("Mouse is over the button!");
    }

    public void OnPointerExit(PointerEventData data)
    {
        player.mouseOverButton = false;
        Debug.Log("Mouse is not over the button!");
    }
}
