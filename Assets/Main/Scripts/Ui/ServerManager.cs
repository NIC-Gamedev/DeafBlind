using FishNet.Managing;
using FishNet.Transporting;
using FishNet.Transporting.Tugboat;
using Unity.VisualScripting;
using UnityEngine;
using System;
public class ServerManager : MonoBehaviour
{
    public NetworkManager networkManager;
    private Tugboat tugboat;

    void Start()
    {
        
    }

    private void OnLevelWasLoaded(int level)
    {
        networkManager = GetComponent<NetworkManager>();
        tugboat = GetComponent<Tugboat>();
        Debug.Log(PlayerPrefs.GetInt("ServerPort", 80));
        // Получаем порт из PlayerPrefs
        int port = PlayerPrefs.GetInt("ServerPort", 80); // По умолчанию порт 80


        tugboat.SetPort((ushort)port);
        // Запуск сервера
        networkManager.ServerManager.StartConnection();

        // Опционально: запустить клиент (хост)
        networkManager.ClientManager.StartConnection();
    }
}
