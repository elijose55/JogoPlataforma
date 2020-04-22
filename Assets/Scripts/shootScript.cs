using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shootScript : MonoBehaviour
{

    private Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();
        StartCoroutine(Timer());
    }

    IEnumerator Timer() {
        while(true){
            yield return new WaitForSeconds(2f);
            anim.SetTrigger("Animate");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
