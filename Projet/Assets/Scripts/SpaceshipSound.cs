using UnityEngine;

public class EngineSound : MonoBehaviour
{
    public AudioSource SpaceshipSound;
    private SpaceShip _spaceShip;

    /// <summary>
    /// Met à jour le pitch du son du vaisseau selon son pourcentage de la vitesse de la lumière
    /// </summary>
    public void UpdatePitch(float throttle)
    {
        SpaceshipSound.pitch = Mathf.Lerp(1f, 3f, Mathf.Abs(throttle));
    }

    void Start()
    {
        _spaceShip = GameObject.FindGameObjectWithTag("SpaceShip").GetComponent<SpaceShip>();
    }

    void Update()
    {

        UpdatePitch(_spaceShip.throttle);
    }
}
