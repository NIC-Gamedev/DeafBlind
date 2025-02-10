using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServerSetup : MonoBehaviour
{
    public TMPro.TMP_InputField portInputField; // ���� ����� ������ �����

    public void SaveSettingsAndStart()
    {
        if (int.TryParse(portInputField.text, out int port))
        {
            PlayerPrefs.SetInt("ProceduralGenTest", port);
            SceneManager.LoadScene("ProceduralGenTest"); // ������� �� ����� �������
        }
        else
        {
            Debug.LogError("������������ ������ �����. ������� ����� �����."+portInputField.text);
            // ������������� ����� ��������� ������������ ����� UI �� ������ �����
        }
    }
}
