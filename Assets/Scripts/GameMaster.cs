using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System;

public class GameMaster : MonoBehaviour {

    public Plane ground = new Plane(new Vector3(0f, 1f, 0f), new Vector3(0f, 0f, 0f));

    public bool player1_PathAvailable = true;
    public Dictionary<int, PlayerManager> players;

    #region GameMaster singleton
    private static GameMaster s_Master = null;
    public static GameMaster Master
    {
        get
        {
            if (s_Master == null)
            {
                s_Master = FindObjectOfType(typeof(GameMaster)) as GameMaster;
            }
            return s_Master;
        }
    }
    #endregion

    private Vector3 getRandomSpawnPoint()
    {
        Vector3 randomPoint = UnityEngine.Random.insideUnitCircle * 5;
        randomPoint.z = randomPoint.y;
        randomPoint.y = 0f;
        randomPoint = randomPoint + new Vector3(-9f, 0.5f, 0f);
        Debug.Log(randomPoint);
        return randomPoint;
    }

    private void Awake()
    {
        players = new Dictionary<int, PlayerManager>();
        players.Add(1, null);
        players.Add(2, null);
    }

    private void Start()
    {
        Application.targetFrameRate = 144;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            Debug.Log(Time.timeScale);
            Time.timeScale = 25f;
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log(Time.timeScale);
            Time.timeScale = 1f;
        }
    }

    public Vector3 GetRandomSpawnPoint()
    {
        System.Random rand = new System.Random();
        float x = ((float)rand.NextDouble() - 0.5f) * 8f;
        float z = ((float)rand.NextDouble() - 0.5f) * 16f;

        return new Vector3(x, 1f, z);
    }

    public Vector3 RoundTheLocation(Vector3 point)
    {
        float x = (float)Math.Round((point.x * 2) / 2);
        float z = (float)Math.Round((point.z * 2) / 2);
        point.x = x;
        point.y = 1f;
        point.z = z;
        return point;
    }

    public string Timer(float time)
    {
        int seconds = (int)time % 60;
        int minutes = Mathf.FloorToInt(time / 60) % 60;
        int hours = Mathf.FloorToInt(time / 3600);

        string h = hours > 9 ? hours.ToString() : (hours > 0 ? "0" + hours.ToString() : "00");
        string m = minutes > 9 ? minutes.ToString() : (minutes > 0 ? "0" + minutes.ToString() : "00");
        string s = seconds > 9 ? seconds.ToString() : (seconds > 0 ? "0" + seconds.ToString() : "00");

        return h + ":" + m + ":" + s;
    }
}