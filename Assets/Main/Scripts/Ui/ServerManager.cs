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
        // �������� ���� �� PlayerPrefs
        int port = PlayerPrefs.GetInt("ServerPort", 80); // �� ��������� ���� 80


        tugboat.SetPort((ushort)port);
        // ������ �������
        networkManager.ServerManager.StartConnection();

        // �����������: ��������� ������ (����)
        networkManager.ClientManager.StartConnection();
    }
}
