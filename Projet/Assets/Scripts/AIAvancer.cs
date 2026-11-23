using UnityEngine;

public class AIAvancer : MonoBehaviour
{
    private AudioSource audioSource;
    float temps = 8f;

    void Start()
    {
        audioSource = GameObject.FindGameObjectWithTag("AudioAvancer").GetComponent<AudioSource>(); ;
    }

    void Update()
    {
        temps -= Time.deltaTime;
        if (temps < 0)
        {
            audioSource.Play();
            enabled = false; // arrête Update pour ne pas relancer le son à chaque frame
        }
    }
}