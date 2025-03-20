using UnityEngine;
using FishNet.Broadcast;
using FishNet.Connection;
using FishNet;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using FishNet.Transporting;
public class ChatBroadcast : MonoBehaviour
{
    public Transform chatHolder;
    public GameObject msgElement;
    public ScrollRect scrollRect; // Scroll View ��� ���������
    public TMP_InputField  playerMsg;

   

    private void Update()
    {
       
    }

    public void SendMessage()
    {
        Message msg = new Message()
        {
            username = PlayerPrefs.GetString("Username"),
            message = playerMsg.text
        };
        
        if(InstanceFinder.IsServer)
            InstanceFinder.ServerManager.Broadcast(msg);
        if(InstanceFinder.IsClient)
            InstanceFinder.ClientManager.Broadcast(msg);


        playerMsg.text = ""; // ������� ����� ��������
    }

    private void OnMessageReceived(Message msg, Channel channel)
    {
        GameObject finalMessage = Instantiate(msgElement, chatHolder);
        finalMessage.GetComponent<TMP_Text>().text = msg.username + ": " + msg.message;

        Canvas.ForceUpdateCanvases(); // ��������� UI
        ScrollToBottom();
    }

    private void ScrollToBottom()
    {
        StartCoroutine(ScrollToBottomCoroutine());
    }

    private IEnumerator ScrollToBottomCoroutine()
    {
        yield return null; // ���� ���� ����, ����� UI ����� ����������
        scrollRect.verticalNormalizedPosition = 0f; // ��������� ����
    }

    private void OnClientMessageReceived(NetworkConnection connection, Message msg, Channel channel)
    {
        InstanceFinder.ServerManager.Broadcast(msg);
    }

    public struct Message : IBroadcast
    {
        public string username;
        public string message;
    }
}
