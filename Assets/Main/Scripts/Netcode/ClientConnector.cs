using FishNet.Managing;
using UnityEngine;

public class ClientConnector : MonoBehaviour
{
    private NetworkManager _networkManager;
    public string HostIP = "127.0.0.1"; // IP ����� (��� ���������� �����)

    public void StartClient()
    {
        _networkManager = FindObjectOfType<NetworkManager>();

        // ������������ � �����
        _networkManager.ClientManager.StartConnection(HostIP, _networkManager.TransportManager.Transport.GetPort());

        Debug.Log("������� ����������� � " + HostIP);
    }
}
