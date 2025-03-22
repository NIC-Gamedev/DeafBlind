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
    public TMP_InputField playerMsg;
    private float messageHeight = 25f; // ������ ������ ���������

    public Image viewport, scrollbar;
    public Scrollbar scroll;

    private void OnEnable()
    {
        InstanceFinder.ClientManager.RegisterBroadcast<Message>(OnMessageReceived);
        InstanceFinder.ServerManager.RegisterBroadcast<Message>(OnClientMessageReceived);

    }

    private void OnDisable()
    {
        InstanceFinder.ClientManager.RegisterBroadcast<Message>(OnMessageReceived);
        InstanceFinder.ServerManager.RegisterBroadcast<Message>(OnClientMessageReceived);

    }

    private void Update()
    {

    }

    public void SendMessage()
    {
        Message msg = new Message()
        {
            username = PlayerPrefs.GetString("Username"),
            message = playerMsg.text,
            senderConnectionId = InstanceFinder.ClientManager.Connection.ClientId

        };

        if (InstanceFinder.IsServer)
            InstanceFinder.ServerManager.Broadcast(msg);
        if (InstanceFinder.IsClient)
            InstanceFinder.ClientManager.Broadcast(msg);

        Debug.Log("Message was sent");

        //Color color = viewport.color;
        //color.a = 0f; // ������������� ����� � 0 (��������� ����������)
        //viewport.color = color;
        //scrollbar.color = color;

        playerMsg.text = ""; // ������� ����� ��������
    }

    private void OnMessageReceived(Message msg, Channel channel)
    {


        GameObject finalMessage = Instantiate(msgElement, chatHolder);
        chatHolder.GetComponent<RectTransform>().sizeDelta += new Vector2(0, messageHeight);

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
        if (msg.senderConnectionId == InstanceFinder.ClientManager.Connection.ClientId)
            return;
        InstanceFinder.ServerManager.Broadcast(msg);
    }

    public struct Message : IBroadcast
    {
        public string username;
        public string message;
        public int senderConnectionId; // ��������� ������������� ���������� �����������

    }
    //public void SetAlphaToZero(bool isReverse)
    //{
    //    Color color = viewport.color;
    //    color.a = isReverse ? 1f : 0f; // 1f = ��������� �����, 0f = ���������
    //    viewport.color = color;

    //    ColorBlock colorBlock = scroll.colors;
    //    colorBlock.normalColor = color;
    //    colorBlock.highlightedColor = color;
    //    colorBlock.pressedColor = color;
    //    colorBlock.selectedColor = color;

    //    // ������ disabledColor.a �� 128 (� ��������� 0-1 ��� 128/255)
    //    Color disabledColor = color;
    //    disabledColor.a = 128f / 255f;
    //    colorBlock.disabledColor = disabledColor;

    //    msgElement.SetActive(isReverse);
        


    //}

}
