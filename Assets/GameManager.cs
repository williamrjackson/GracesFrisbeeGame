using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Transform frisbee;
    public AudioClip hahaClip;
    public UnityAction OnGameOver;
    public bool GameOver = false;
    private AudioSource audioSource;
    CatcherThrower[] teamInOrder;
    void Awake ()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        audioSource = this.EnsureComponent<AudioSource>();
    }

    void Caught()
    {

    }
    // Start is called before the first frame update
    public void Hit()
    {
        GameOver = true;
        if (OnGameOver != null)
        {
            OnGameOver();
        }
        if (audioSource != null && hahaClip != null)
        {
            audioSource.PlayOneShot(hahaClip);
        }
    }

}
