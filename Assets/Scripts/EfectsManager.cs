using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EfectsManager : MonoBehaviour
{
    public AudioSource pasosSource;
    public AudioSource efectosSource;
    public AudioClip Pasos;
    public AudioClip Grito;

    public void SetGrito()
    {
        efectosSource.clip = Grito;
        efectosSource.loop = false;
        efectosSource.Play();
    }

    public void SetPasos()
    {
        pasosSource.clip = Pasos;
        pasosSource.loop = true;
        pasosSource.pitch = 2.5f;
        pasosSource.Play();
    }
}
