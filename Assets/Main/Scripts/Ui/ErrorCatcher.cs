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
        // �������� ������ �� NetworkManager
        networkManager = InstanceFinder.NetworkManager;

    }
   
    private void OnEnable()
    {
        // ������������� �� ������� ��������� ��������� ����������� �������
        networkManager.ClientManager.OnClientConnectionState += OnClientConnectionStateChanged;
      
    }

    private void OnDisable()
    {
        // ������������ �� �������
        networkManager.ClientManager.OnClientConnectionState -= OnClientConnectionStateChanged;
    }
    
    private void OnClientConnectionStateChanged(ClientConnectionStateArgs args)
    {
        switch (args.ConnectionState)
        {
            case LocalConnectionState.Stopped:
                // ��������� "Stopped" ����� ��������, ��� ���������� ������ �� ���� �����������.
                Debug.Log("����������� �� �������. ���������: Stopped");
                UnableToConnectPanel.SetActive(true);
                break;

            case LocalConnectionState.Stopping:
                // ��������� "Stopping" � ������� ����������� ���������� ����������.
                Debug.Log("���������� �����������. ���������: Stopping");
                break;

            case LocalConnectionState.Starting:
                // ��������� "Starting" � ������� ���������� ����������.
                Debug.Log("������� �����������... ���������: Starting");
                // ����� ����� �������� ��������� ����������� (��������, �������)
                ServerLoadingPanel.SetActive(true);
                break;

            case LocalConnectionState.Started:
                // ���������� ������� �����������.
                Debug.Log("���������� �����������. ���������: Started");
                // ����� ����� ������ ��� ������ ������/���������� �����������.
                ServerLoadingPanel.SetActive(false);
                break;

            case LocalConnectionState.StoppedError:
                // ������ ��� ������������ ��� �� ����� ����������.
                Debug.Log("����������� ����������� � �������. ���������: StoppedError");
                ServerClosedWithErrorPanel.SetActive(true);
                break;

            case LocalConnectionState.StoppedClosed:
                // ���������� ���� ������� (��������, ������ ������ �� ��������� �����������).
                Debug.Log("���������� ������� ��������. ���������: StoppedClosed");
                // ����� ����� ������� ��������������� UI ��� ����������� � �������� ����������.
                ServerConnectedStoppedPanel.SetActive(true);
                break;
        }
    }
}
