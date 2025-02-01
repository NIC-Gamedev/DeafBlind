using FishNet.Managing;
using UnityEngine;

public class Netcodetest : MonoBehaviour
{
    private NetworkManager _networkManager;

    public void StartHost()
    {
        _networkManager = FindObjectOfType<NetworkManager>();

        // Запускаем сервер (хост)
        _networkManager.ServerManager.StartConnection();

        // Подключаем локального клиента к себе
        _networkManager.ClientManager.StartConnection();

        Debug.Log("Хост запущен!");
    }
}
