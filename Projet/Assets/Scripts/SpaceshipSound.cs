using UnityEngine;
using UnityEngine.Serialization;

public class EngineSound : MonoBehaviour
{
    public AudioSource sound;
    public float minPitch = 0;
    public float maxPitch = 2;
    
    private SpaceShip _spaceShip;

    void Start()
    {
        _spaceShip = GameObject.FindGameObjectWithTag("SpaceShip").GetComponent<SpaceShip>();
    }

    void Update()
    {
        UpdatePitch(_spaceShip.throttle);
    }
    
    /// <summary>
    /// Met à jour le pitch du son du vaisseau selon son pourcentage de la vitesse de la lumière
    /// </summary>
    private void UpdatePitch(float throttle)
    {
        sound.pitch = throttle * (maxPitch - minPitch) + minPitch;
    }
}
