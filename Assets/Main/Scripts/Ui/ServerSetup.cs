using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ServerSetup : MonoBehaviour
{
    public TMPro.TMP_InputField portInputField; // ���� ����� ������ �����
    public void SaveSettingsAndStart()
    {
        if (int.TryParse(portInputField.text, out int port))
        {
            PlayerPrefs.SetString("IsServer", "true");
            PlayerPrefs.SetInt("ServerPort", port);
            SceneManager.LoadScene("ProceduralGenTest"); // ������� �� ����� �������
        }
        else
        {
            Debug.LogError("Incrorrect port type . Enter correct port format:"+portInputField.text);
            // ������������� ����� ��������� ������������ ����� UI �� ������ �����
        }
    }

    public void SetBlind()
    {
        PlayerPrefs.SetString("IsBlind", "true");
    }

    public void SetDeaf()
    {
        PlayerPrefs.SetString("IsBlind", "false");
    }


}
