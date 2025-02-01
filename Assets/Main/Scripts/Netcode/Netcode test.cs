using FishNet.Managing;
using UnityEngine;

public class Netcodetest : MonoBehaviour
{
    private NetworkManager _networkManager;

    public void StartHost()
    {
        _networkManager = FindObjectOfType<NetworkManager>();

        // ��������� ������ (����)
        _networkManager.ServerManager.StartConnection();

        // ���������� ���������� ������� � ����
        _networkManager.ClientManager.StartConnection();

        Debug.Log("���� �������!");
    }
}
