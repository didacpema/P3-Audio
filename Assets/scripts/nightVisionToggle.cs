using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nightVisionToggle : MonoBehaviour
{

    public GameObject nightvision;
    public AudioSource audioSource;
    public AudioClip nightVisionOffSound;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            if (nightvision.activeInHierarchy)
            {
                audioSource.PlayOneShot(nightVisionOffSound);
            }

            nightvision.SetActive(!nightvision.activeInHierarchy);
        }
    }
}
