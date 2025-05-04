using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TVScript : MonoBehaviour
{
    //When player trigger TV collider and F is pressed, the TV will turn on and off
    public GameObject TVScreenOff;
    public GameObject TVScreenOn;
    public AudioSource switchSFX;
    private bool isOff = true;
    private bool onRange = false;

    // Update is called once per frame
    void Update()
    {
        if (onRange)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (isOff)
                {
                    PlayFX();
                    TVScreenOff.SetActive(false);
                    TVScreenOn.SetActive(true);
                    isOff = false;
                }
                else
                {
                    PlayFX();
                    TVScreenOff.SetActive(true);
                    TVScreenOn.SetActive(false);
                    isOff = true;
                }
            }
        }


    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onRange = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onRange = false;
        }
    }
    private void PlayFX()
    {
        if (switchSFX != null)
        {
            switchSFX.Play();
        }
    }
}
