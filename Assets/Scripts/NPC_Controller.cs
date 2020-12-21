using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Controller : MonoBehaviour
{
    public GameObject tower;
    int counter = 0;
    List<Vector3> towerPositions;

    // Start is called before the first frame update
    void Start()
    {
        towerPositions = new List<Vector3>();
        CalculateTowerPositions();
        Debug.Log(towerPositions[0]);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void CalculateTowerPositions()
    {
        bool left = true;
        for (int i = 0; i < 10; i += 2)
        {
            if (left)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (j == 9)
                    {
                        continue;
                    }
                    towerPositions.Add(new Vector3(((i * (-2)) - 8), 1, (j * 2) - 9));
                    //Instantiate(this.tower, new Vector3(((i * (-2)) - 8), 1, (j * 2) - 9), Quaternion.identity);
                }
            }
            else
            {
                Debug.Log("TEST");
                for (int j = 9; j >= 0; j--)
                {
                    if (j == 0)
                    {
                        continue;
                    }
                    towerPositions.Add(new Vector3(((i * (-2)) - 8), 1, (j * 2) - 9));
                }
            }
            left = !left;
        }
    }
}
