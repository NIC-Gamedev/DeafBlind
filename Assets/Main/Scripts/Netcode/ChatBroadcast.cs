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
    public ScrollRect scrollRect;
    public TMP_InputField playerMsg;
    private float messageHeight = 25f;

    public Image scrollbar, scrollview, chatBackground;
    public Scrollbar scroll;

    private Coroutine fadeCoroutine;
    [SerializeField]
    private float fadeDuration = 3f;
    private float visibleDuration = 5f;

    bool Ischatopen;

    private const int MaxMessages = 30;
    private readonly Queue<GameObject> _messages = new Queue<GameObject>();

    private void OnEnable()
    {
        InstanceFinder.ClientManager.RegisterBroadcast<Message>(OnMessageReceived);
        InstanceFinder.ServerManager.RegisterBroadcast<Message>(OnClientMessageReceived);
    }

    private void OnDisable()
    {
        InstanceFinder.ClientManager.UnregisterBroadcast<Message>(OnMessageReceived);
        InstanceFinder.ServerManager.UnregisterBroadcast<Message>(OnClientMessageReceived);
    }

    private void Start()
    {
        SendMessage();
    }

    #region Обработка UI
    public void SetAlpha(float alpha)
    {
        ApplyAlpha(scrollbar, alpha);
        ApplyAlpha(scroll.GetComponent<Image>(), alpha);

        if (alpha == 255)
        {
            ApplyAlpha(scrollview, 100f);
        }
        else
        {
            ApplyAlpha(scrollview, 0f);
        }

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

    private void ScrollToBottom()
    {
        StartCoroutine(ScrollToBottomCoroutine());
    }

    private IEnumerator ScrollToBottomCoroutine()
    {
        yield return null;
        scrollRect.verticalNormalizedPosition = 0f;
    }

    private void ShowChatBackground()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        ApplyAlpha(chatBackground, 1f);

        if (!Ischatopen)
            fadeCoroutine = StartCoroutine(HideChatBackgroundAfterDelay());
    }

    private IEnumerator HideChatBackgroundAfterDelay()
    {
        yield return new WaitForSeconds(visibleDuration);

        float elapsedTime = 0f;
        float startAlpha = chatBackground.color.a;

        while (elapsedTime < fadeDuration)
        {
            if (Ischatopen)
            {
                ApplyAlpha(chatBackground, 1f);
                yield break;
            }

            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / fadeDuration);
            ApplyAlpha(chatBackground, newAlpha);
            yield return null;
        }

        ApplyAlpha(chatBackground, 0f);
    }
    #endregion

    #region Логика обработки сообщений
    public void SendMessage()
    {
        if (playerMsg.gameObject.activeSelf == false)
        {
            SetAlpha(255f);
            playerMsg.gameObject.SetActive(true);
            playerMsg.ActivateInputField();
            ShowChatBackground();
            Ischatopen = true;
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

        if (playerMsg.text == "")
        {
            Ischatopen = false;
            return;
        }

        if (InstanceFinder.IsServer)
            InstanceFinder.ServerManager.Broadcast(msg);
        if (InstanceFinder.IsClient)
            InstanceFinder.ClientManager.Broadcast(msg);

        Debug.Log("Message was sent");
        Ischatopen = false;
        playerMsg.text = "";
    }

    private void OnMessageReceived(Message msg, Channel channel)
    {
        // Удаляем самое старое сообщение, если достигли максимума
        if (_messages.Count >= MaxMessages)
        {
            GameObject oldestMessage = _messages.Dequeue();
            if (oldestMessage != null)
            {
                // Получаем высоту удаляемого сообщения
                float heightToRemove = oldestMessage.GetComponent<RectTransform>().rect.height;
                // Удаляем сообщение
                Destroy(oldestMessage);
                // Уменьшаем размер chatHolder
                chatHolder.GetComponent<RectTransform>().sizeDelta -= new Vector2(0, heightToRemove);
            }
        }

        // Создаем новое сообщение
        GameObject finalMessage = Instantiate(msgElement, chatHolder);
        _messages.Enqueue(finalMessage);

        TMP_Text textComponent = finalMessage.GetComponent<TMP_Text>();
        textComponent.text = msg.username + ": " + msg.message;

        // Перестроим layout и обновим высоту
        LayoutRebuilder.ForceRebuildLayoutImmediate(finalMessage.GetComponent<RectTransform>());
        float newHeight = finalMessage.GetComponent<RectTransform>().rect.height;
        chatHolder.GetComponent<RectTransform>().sizeDelta += new Vector2(0, newHeight);

        Canvas.ForceUpdateCanvases();
        ScrollToBottom();
        ShowChatBackground();
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
        public int senderConnectionId;
    }
    #endregion
}