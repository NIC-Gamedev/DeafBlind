using UnityEngine;
using FishNet;
using FishNet.Managing;
using FishNet.Transporting;
using FishNet.Managing.Client;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using Cinemachine;
using FishNet.Object;
public class ErrorCatcher : MonoBehaviour
{
    private NetworkManager networkManager;
    public GameObject UnableToConnectPanel;
    public GameObject ServerClosedWithErrorPanel;
    public GameObject ServerConnectedStoppedPanel;
    public GameObject ServerLoadingPanel;
    private void Awake()
    {
        // Получаем ссылку на NetworkManager
        networkManager = InstanceFinder.NetworkManager;

    }
   
    private void OnEnable()
    {
        // Подписываемся на событие изменения состояния подключения клиента
        networkManager.ClientManager.OnClientConnectionState += OnClientConnectionStateChanged;
      
    }

    private void OnDisable()
    {
        // Отписываемся от события
        networkManager.ClientManager.OnClientConnectionState -= OnClientConnectionStateChanged;
    }
    
    private void OnClientConnectionStateChanged(ClientConnectionStateArgs args)
    {
        switch (args.ConnectionState)
        {
            case LocalConnectionState.Stopped:
                // Состояние "Stopped" может означать, что соединение вообще не было установлено.
                Debug.Log("Подключение не удалось. Состояние: Stopped");
                UnableToConnectPanel.SetActive(true);
                break;

            case LocalConnectionState.Stopping:
                // Состояние "Stopping" – процесс корректного завершения соединения.
                Debug.Log("Соединение закрывается. Состояние: Stopping");
                break;

            case LocalConnectionState.Starting:
                // Состояние "Starting" – попытка установить соединение.
                Debug.Log("Попытка подключения... Состояние: Starting");
                // Здесь можно показать индикатор подключения (например, спиннер)
                ServerLoadingPanel.SetActive(true);
                break;

            case LocalConnectionState.Started:
                // Соединение успешно установлено.
                Debug.Log("Соединение установлено. Состояние: Started");
                // Здесь можно скрыть все панели ошибок/индикаторы подключения.
                ServerLoadingPanel.SetActive(false);
                break;

            case LocalConnectionState.StoppedError:
                // Ошибка при установлении или во время соединения.
                Debug.Log("Подключение завершилось с ошибкой. Состояние: StoppedError");
                ServerClosedWithErrorPanel.SetActive(true);
                break;

            case LocalConnectionState.StoppedClosed:
                // Соединение было закрыто (например, сервер больше не принимает подключения).
                Debug.Log("Соединение закрыто сервером. Состояние: StoppedClosed");
                // Здесь можно вызвать соответствующий UI для уведомления о закрытии соединения.
                ServerConnectedStoppedPanel.SetActive(true);
                break;
        }
    }
}
