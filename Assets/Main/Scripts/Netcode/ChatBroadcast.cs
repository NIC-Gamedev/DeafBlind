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
    public ScrollRect scrollRect; // Scroll View для прокрутки
    public TMP_InputField playerMsg;
    private float messageHeight = 25f; // Высота одного сообщения

    public Image  scrollbar , scrollview;
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
    public void SetAlpha(float alpha)
    {
        ApplyAlpha(scrollbar, alpha);
        ApplyAlpha(scroll.GetComponent<Image>(), alpha);
        Debug.Log(alpha);
        if(alpha == 255)
        {
            ApplyAlpha(scrollview, 100f);

        }
        else
        {
            ApplyAlpha(scrollview, 0f);


        }
        Debug.Log("Disabling");
        if (scrollbar != null)
        {
            ColorBlock cb = scroll.colors;
            cb.selectedColor = new Color(cb.selectedColor.r, cb.selectedColor.g, cb.selectedColor.b, alpha);
            cb.normalColor = new Color(cb.normalColor.r, cb.normalColor.g, cb.normalColor.b, alpha);
            cb.disabledColor = new Color(cb.normalColor.r, cb.normalColor.g, cb.normalColor.b, alpha);
            cb.highlightedColor = new Color(cb.normalColor.r, cb.normalColor.g, cb.normalColor.b, alpha);
            cb.pressedColor = new Color(cb.normalColor.r, cb.normalColor.g, cb.normalColor.b, alpha);
            scroll.colors = cb;
        }

    }

    private void ApplyAlpha(Image img, float alpha)
    {
        if (img != null)
        {
            img.canvasRenderer.SetAlpha(alpha);
        }
    }
    public void SendMessage()
    {
        if(playerMsg.gameObject.activeSelf == false)
        {
            SetAlpha(255f);
            playerMsg.gameObject.SetActive(true);
            playerMsg.ActivateInputField();
            return;
        }
   
        Message msg = new Message()
        {
            username = PlayerPrefs.GetString("Username"),
            message = playerMsg.text,
            senderConnectionId = InstanceFinder.ClientManager.Connection.ClientId

        };
        playerMsg.gameObject.SetActive(false);
        SetAlpha(0f);

        if (InstanceFinder.IsServer)
            InstanceFinder.ServerManager.Broadcast(msg);
        if (InstanceFinder.IsClient)
            InstanceFinder.ClientManager.Broadcast(msg);

        Debug.Log("Message was sent");
      
        if (playerMsg.text == "")
            return;
        playerMsg.text = ""; // Очистка после отправки
    }

    private void OnMessageReceived(Message msg, Channel channel)
    {


        GameObject finalMessage = Instantiate(msgElement, chatHolder);
        chatHolder.GetComponent<RectTransform>().sizeDelta += new Vector2(0, messageHeight);

        finalMessage.GetComponent<TMP_Text>().text = msg.username + ": " + msg.message;

        Canvas.ForceUpdateCanvases(); // Обновляем UI
        ScrollToBottom();
    }

    private void ScrollToBottom()
    {
        StartCoroutine(ScrollToBottomCoroutine());
    }

    private IEnumerator ScrollToBottomCoroutine()
    {
        yield return null; // Ждем один кадр, чтобы UI успел обновиться
        scrollRect.verticalNormalizedPosition = 0f; // Прокрутка вниз
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
        public int senderConnectionId; // Добавляем идентификатор соединения отправителя

    }
   
}
