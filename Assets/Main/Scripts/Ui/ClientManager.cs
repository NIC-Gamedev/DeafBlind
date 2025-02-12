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
        // �������� ���� �� PlayerPrefs
        int port = PlayerPrefs.GetInt("ServerPort", 80); // �� ��������� ���� 80


        _tugboat.SetPort((ushort)port);
        // ������ �������
        _networkManager.ServerManager.StartConnection();

        // �����������: ��������� ������ (����)
        _networkManager.ClientManager.StartConnection();
    }
}
