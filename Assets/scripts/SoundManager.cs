using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance {get; set;}

    // Sound
    public AudioSource gun;
    public AudioClip gunShotSound;
    public AudioClip gunNoAmmoSound;
    public AudioClip gunReloadBSound;
    public AudioClip gunReloadASound;

    private void Awake()
    {
        if(Instance != null && Instance != this){
            Destroy(gameObject);
        }
        else{
            Instance=this;
        }
    }
}
