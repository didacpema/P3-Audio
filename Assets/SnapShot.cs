using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SnapShot : MonoBehaviour
{
    public AudioMixer mixer;
    public bool nvgOn=false;
    public bool bigRoom=false;
    public bool snapOn=false;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.N)){
            nvgOn = !nvgOn;
            if(snapOn){
                if(nvgOn && bigRoom){
                mixer.FindSnapshot("grande_nvg").TransitionTo(1f);
                }
                else if(nvgOn && !bigRoom){
                    mixer.FindSnapshot("pequeña_nvg").TransitionTo(1f);
                }
                else if(!nvgOn && bigRoom){
                    mixer.FindSnapshot("grande").TransitionTo(1f);
                }
                else if(!nvgOn && !bigRoom){
                    mixer.FindSnapshot("pequeña").TransitionTo(1f);
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        snapOn = true;
        if(nvgOn && bigRoom){
            mixer.FindSnapshot("grande_nvg").TransitionTo(1f);
        }
        else if(nvgOn && !bigRoom){
            mixer.FindSnapshot("pequeña_nvg").TransitionTo(1f);
        }
        else if(!nvgOn && bigRoom){
            mixer.FindSnapshot("grande").TransitionTo(1f);
        }
        else if(!nvgOn && !bigRoom){
            mixer.FindSnapshot("pequeña").TransitionTo(1f);
        }
    }

    void OnTriggerExit(Collider other)
    {
        snapOn = false;
        if(nvgOn && bigRoom){
            mixer.FindSnapshot("grande_nvg").TransitionTo(1f);
        }
        else if(nvgOn && !bigRoom){
            mixer.FindSnapshot("pequeña_nvg").TransitionTo(1f);
        }
        else if(!nvgOn && bigRoom){
            mixer.FindSnapshot("grande").TransitionTo(1f);
        }
        else if(!nvgOn && !bigRoom){
            mixer.FindSnapshot("pequeña").TransitionTo(1f);
        }
    }
}

