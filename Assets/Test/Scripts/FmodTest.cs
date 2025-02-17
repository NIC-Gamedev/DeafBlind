using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class FmodTest : MonoBehaviour
{
    public bool makeSound;
    public string soundPath;
    public EventReference refer;

    private void Start()
    {
        if (RuntimeManager.HaveAllBanksLoaded)
            Debug.Log("FMOD банки загружены!");
        else
            Debug.LogError("FMOD банки НЕ загружены!");
    }
    private void Update()
    {
        if (makeSound)
        {
            RuntimeManager.PlayOneShot(refer);
            makeSound = false;
        }
    }
}
