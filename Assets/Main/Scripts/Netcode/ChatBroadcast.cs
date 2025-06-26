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

    // Переменные что мы переводим в прозрачный режим
    public Image  scrollbar , scrollview, chatBackground;
    public Scrollbar scroll;

    private Coroutine fadeCoroutine;
    [SerializeField]
    private float fadeDuration = 3f; // Время исчезновения
    private float visibleDuration = 5f; // Время перед исчезновением

    bool Ischatopen;

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
        Debug.Log(alpha);
        if(alpha == 255)
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
        yield return null; // Ждем один кадр, чтобы UI успел обновиться
        scrollRect.verticalNormalizedPosition = 0f; // Прокрутка вниз
    }
    private void ShowChatBackground()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        ApplyAlpha(chatBackground, 1f); // Делаем фон видимым

        if (!Ischatopen) // Запускаем скрытие только если чат закрыт
            fadeCoroutine = StartCoroutine(HideChatBackgroundAfterDelay());
    }

    private IEnumerator HideChatBackgroundAfterDelay()
    {
        yield return new WaitForSeconds(visibleDuration); // Ждем 5 секунд

        float elapsedTime = 0f;
        float startAlpha = chatBackground.color.a;

        while (elapsedTime < fadeDuration)
        {
            if (Ischatopen) // Если чат открыт — выходим из корутины
            {
                ApplyAlpha(chatBackground, 1f);
                yield break;
            }

            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / fadeDuration);
            ApplyAlpha(chatBackground, newAlpha);
            yield return null;
        }

        ApplyAlpha(chatBackground, 0f); // Полностью скрываем фон
    }


    #endregion

    #region Логика обработки сообщений
    public void SendMessage()
    {
        if(playerMsg.gameObject.activeSelf == false)
        {
            SetAlpha(255f);
            playerMsg.gameObject.SetActive(true);
            playerMsg.ActivateInputField();
            ShowChatBackground(); // Делаем фон непрозрачным
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


        playerMsg.text = ""; // Очистка после отправки
    }

    private void OnMessageReceived(Message msg, Channel channel)
    {


        GameObject finalMessage = Instantiate(msgElement, chatHolder);
        chatHolder.GetComponent<RectTransform>().sizeDelta += new Vector2(0, messageHeight);

        finalMessage.GetComponent<TMP_Text>().text = msg.username + ": " + msg.message;

        Canvas.ForceUpdateCanvases(); // Обновляем UI
        ScrollToBottom();
        ShowChatBackground(); // Делаем фон непрозрачным

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

    #endregion

}
