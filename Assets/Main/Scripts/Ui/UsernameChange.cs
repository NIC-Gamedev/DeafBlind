using TMPro;
using UnityEngine;

public class UsernameChange : MonoBehaviour
{
    public GameObject usernameInput;
    public GameObject usernamePanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        if (PlayerPrefs.HasKey("Username"))
        {
            Debug.Log(PlayerPrefs.GetString("Username"));
        }
        else
        {

            Debug.Log("Enter username");

            usernamePanel.SetActive(true);
        }
    }
    private void OnLevelWasLoaded(int level)
    {

        if (PlayerPrefs.HasKey("Username"))
        {
            Debug.Log(PlayerPrefs.GetString("Username"));
        }
        else
        {
            Debug.Log("theres no username");

            usernamePanel.SetActive(true);
        }
    }
    // Update is called once per frame
    void Update()
    {
       
    }



    public void ChangeUsername()
    {
        if (usernameInput == null)
        {
            Debug.LogError("usernameInput is null");
            return;
        }

        TMP_InputField inputField = usernameInput.GetComponent<TMP_InputField>();
        if (inputField == null)
        {
            Debug.LogError("TMP_InputField component is missing on usernameInput");
            return;
        }

        string username = inputField.text;
        if (string.IsNullOrEmpty(username)) return;

        PlayerPrefs.SetString("Username", username);
        Debug.Log("Username changed to: " + username);
    }
}
