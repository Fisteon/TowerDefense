using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MyNetworkManager : NetworkManager
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnStartHost()
    {
        base.OnStartHost();
    }

    public void ButtonHostGameClicked()
    {
        StartHost();
    }

    public void ButtonJoinGameClicked()
    {
        StartClient();
    }

    public void ButtonDisconnectClicked()
    {
        if (NetworkServer.active)
        {
            if (this.IsClientConnected())
            {
                this.StopHost();
            }
            else
            {
                this.StopServer();
            }
        }
    }
}
