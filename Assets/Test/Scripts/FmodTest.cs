using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class FmodTest : MonoBehaviour
{
    public bool makeSound;
    public string soundPath;
    public EventReference refer;
    public float vol;
    private void Update()
    {
        if (makeSound)
        {
            AudioManager.instance.Play(refer,vol,1,false);
            makeSound = false;
        }
    }
}
