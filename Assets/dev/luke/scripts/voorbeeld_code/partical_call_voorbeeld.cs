using UnityEngine;

public class EffectSpawner : MonoBehaviour
{
    public void Explode()
    {
        // Roep "Explosion" effect aan op de locatie van dit object.
        ParticleManager.Instance.SpawnParticle("Explosion", this.transform.position);
    }

    public void Jump()
    {
        // Roep "Dust" effect aan, maar 0.1 eenheden lager dan de pivot.
        Vector3 groundOffset = new Vector3(0f, -0.1f, 0f);
        
        // Let op de 'this.transform.position' om de positie van het aanroepende object te gebruiken.
        ParticleManager.Instance.SpawnParticle("Dust", this.transform.position, groundOffset);
    }
}