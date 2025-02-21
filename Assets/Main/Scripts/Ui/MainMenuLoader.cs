using FishNet;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
public class MainMenuLoader : MonoBehaviour
{
    public void LoadMainMenu()
    {
        Destroy(InstanceFinder.NetworkManager.GameObject());
        SceneManager.LoadScene("UI");
    }
}
