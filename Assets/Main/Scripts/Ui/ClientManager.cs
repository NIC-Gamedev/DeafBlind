using FishNet.Managing;
using FishNet.Transporting.Tugboat;
using UnityEngine;

public class ClientManager : MonoBehaviour
{
    private NetworkManager _networkManager;
    private Tugboat _tugboat;



    private void OnLevelWasLoaded(int level)
    {
        _networkManager = GetComponent<NetworkManager>();
        _tugboat = GetComponent<Tugboat>();
        Debug.Log("Server port is set: " + PlayerPrefs.GetInt("ServerPort", 80));
        // Получаем порт из PlayerPrefs
        int port = PlayerPrefs.GetInt("ServerPort", 80); // По умолчанию порт 80


        _tugboat.SetPort((ushort)port);
        // Запуск сервера
        _networkManager.ServerManager.StartConnection();

        // Опционально: запустить клиент (хост)
        _networkManager.ClientManager.StartConnection();
    }
}
