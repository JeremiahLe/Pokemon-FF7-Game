using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class LauncherScript : MonoBehaviourPunCallbacks
{
    MainMenuManager m_MainMenuManager;

    public void Awake()
    {
        m_MainMenuManager = GetComponent<MainMenuManager>();    
    }

    public void ConnectToMasterServer()
    {
        Debug.Log("Connecting to Master!", this);
        PhotonNetwork.ConnectUsingSettings(); // connects to master server with user settings
    }

    public override void OnConnectedToMaster()
    {   
        Debug.Log("Connected to Master!", this);
        PhotonNetwork.JoinLobby(); // Lobbys are where you find and create rooms
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby!", this);
        //sceneButtonManager.Connected();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected!", this);
    }
}
