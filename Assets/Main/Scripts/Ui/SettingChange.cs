using System;
using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.UI;

public class SettingChange : MonoBehaviour
{
    [SerializeField]
    private TMP_Dropdown _screenResolution;

    [SerializeField]
    private Toggle _fullScreenToggle;

    public string ScreenResolution;
    public bool _isFullscreen;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeFullScreen()
    {
        if (string.IsNullOrEmpty(ScreenResolution))
        {
            Debug.LogError("ScreenResolution не задана!");
            return;
        }

        string[] parts = ScreenResolution.Split('×');

        if (parts.Length != 2 || !int.TryParse(parts[0], out int width) || !int.TryParse(parts[1], out int height))
        {
            Debug.LogError("Ошибка парсинга ScreenResolution: " + ScreenResolution);
            return;
        }

        Screen.SetResolution(width, height, _isFullscreen);
    }

    public void ChangeScreenRes()
    {
        

        string selectedResolution = _screenResolution.options[_screenResolution.value].text;

        string[] parts = selectedResolution.Split('×');

        if (parts.Length != 2 || !int.TryParse(parts[0], out int width) || !int.TryParse(parts[1], out int height))
        {
            Debug.LogError("Ошибка парсинга выбранного разрешения: " + selectedResolution);
            return;
        }
        ScreenResolution = selectedResolution;
        Screen.SetResolution(width, height, _isFullscreen);
        Debug.Log("Разрешение установлено");
    }
}
