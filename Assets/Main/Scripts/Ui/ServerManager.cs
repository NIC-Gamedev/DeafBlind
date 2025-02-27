using FishNet.Managing;
using FishNet.Transporting;
using FishNet.Transporting.Tugboat;
using FishNet;
using Unity.VisualScripting;
using UnityEngine;
using System;
using Unity.Collections;
using UnityEngine.SceneManagement;
public class ServerManager : MonoBehaviour
{
    [SerializeField]
    private NetworkManager _networkManager;
    [SerializeField]
    private Tugboat _tugboat;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "ProceduralGenTest") return; // Проверяем, что загружена нужная сцена

        _networkManager = GetComponent<NetworkManager>();
        _tugboat = GetComponent<Tugboat>();

        string isServer = PlayerPrefs.GetString("IsServer");
        Debug.Log(isServer);
        if (Convert.ToBoolean(isServer))
        {
            int port = PlayerPrefs.GetInt("ServerPort", 80);
            _tugboat.SetPort((ushort)port);
            _networkManager.ServerManager.StartConnection();

            _networkManager.ClientManager.StartConnection();
        }
        else
        {
            string serverip = PlayerPrefs.GetString("ServerIp");
            string[] parts = serverip.Split(':');
            string ip = parts.Length > 0 ? parts[0] : "";
            ushort port = Convert.ToUInt16(parts.Length > 1 ? parts[1] : "80");

            _networkManager.ClientManager.StartConnection(ip, port);
        }
    }
}
