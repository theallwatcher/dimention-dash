using UnityEngine;

public class test_script : MonoBehaviour
{
    void Start()
    {
        // Voorbeeld van het aanroepen van een deeltje bij het starten van het spel.
        ParticleManager.Instance.SpawnParticle("test", this.transform.position);
        AudioManager.Instance.Play("test");
    }
}
