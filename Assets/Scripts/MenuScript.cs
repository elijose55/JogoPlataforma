using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{

    public AudioClip Click;
    private AudioSource audioSource;

    void Start(){
        audioSource = GetComponent<AudioSource>();
    }
    public void StartGame(){
        audioSource.PlayOneShot(Click);
        Application.LoadLevel("Main");
    }
    public void HomePage(){
        audioSource.PlayOneShot(Click);
        Application.LoadLevel("Intro");
    }
}
