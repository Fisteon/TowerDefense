using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class NetworkHUD : MonoBehaviour
{
    public Button b_Host;
    public Button b_Join;
    public Button b_Disconnect;

    public Text t_connectionStatus;

    private NetworkManager manager;

    // Start is called before the first frame update
    void Start()
    {
        manager = GetComponent<NetworkManager>();
    }

    // Update is called once per frame
    void Update()
    {
        bool noConnection = (manager.client == null || manager.client.connection == null || manager.client.connection.connectionId == -1);
        if (!manager.IsClientConnected() && !NetworkServer.active && manager.matchMaker == null)
        {
            if (!noConnection)
            {
                t_connectionStatus.text = "Connecting...";
            }
        }
        else
        {
            t_connectionStatus.text = "Connected!";
        }
    }

    public void ButtonHostGameClicked()
    {
        manager.StartHost();
        ChangeButtons();
    }

    public void ButtonJoinGameClicked()
    {
        manager.StartClient();
        ChangeButtons();
    }

    public void ButtonDisconnectClicked()
    { 
        if (manager.IsClientConnected())
        {
            manager.StopHost();
            ChangeButtons();
        }
        else
        {
            manager.StopServer();
            ChangeButtons();
        }
        
    }

    public void ButtonExitGameClicked()
    {
        Application.Quit();
    }

    void ChangeButtons()
    {
        b_Host.gameObject.SetActive(!b_Host.gameObject.activeSelf);
        b_Join.gameObject.SetActive(!b_Join.gameObject.activeSelf);
        b_Disconnect.gameObject.SetActive(!b_Disconnect.gameObject.activeSelf);
        t_connectionStatus.gameObject.SetActive(!t_connectionStatus.gameObject.activeSelf);
    }
}
