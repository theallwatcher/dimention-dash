using UnityEngine;
using System;

// Dit zorgt ervoor dat de manager gemakkelijk kan worden aangeroepen met ParticleManager.Instance.
public class ParticleManager : MonoBehaviour
{
    // De static referentie (Singleton)
    public static ParticleManager Instance;

    [Header("Deeltjes Lijst (Prefabs)")]
    // Array om al je ParticleSystem prefabs te organiseren.
    public ParticleEntry[] particles;

    void Awake()
    {
        // --- Singleton Implementatie ---
        if (Instance == null)
        {
            Instance = this;
            // Zorgt ervoor dat de manager blijft bestaan bij het laden van nieuwe scènes.
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Vernietig dubbele managers.
            Destroy(gameObject);
            return;
        }
    }

    /// <summary>
    /// Spawnt een Particle System op een specifieke locatie met een optionele offset.
    /// Roep aan met: ParticleManager.Instance.SpawnParticle("Dust", this.transform.position, new Vector3(0, 0.5f, 0));
    /// </summary>
    /// <param name="particleName">De naam van het Particle System dat moet worden afgespeeld.</param>
    /// <param name="spawnPosition">De wereldpositie waar de deeltjes moeten spawnen.</param>
    /// <param name="offset">De lokale offset ten opzichte van de spawnPosition (default is Vector3.zero).</param>
    /// <returns>De instantie van het gespawnde Particle System (of null als niet gevonden).</returns>
    public ParticleSystem SpawnParticle(string particleName, Vector3 spawnPosition, Vector3 offset = default)
    {
        // 1. Zoek de Particle Entry op naam.
        ParticleEntry entry = Array.Find(particles, p => p.name == particleName);

        if (entry.prefab == null)
        {
            Debug.LogWarning($"Particle: '{particleName}' is niet gevonden of heeft geen prefab ingesteld in de Manager!");
            return null;
        }

        // 2. Bereken de uiteindelijke positie.
        Vector3 finalPosition = spawnPosition + offset;

        // 3. Instantieer het Particle System.
        // Let op: 'entry.prefab' moet een prefab van een ParticleSystem zijn.
        ParticleSystem newParticles = Instantiate(entry.prefab, finalPosition, Quaternion.identity);

        // 4. Start het systeem en bepaal de vernietigingstijd.
        newParticles.Play();

        float destroyTime;
        
        // Gebruik de customDuration als deze is ingesteld (groter dan 0).
        if (entry.customDuration > 0f)
        {
            destroyTime = entry.customDuration;
        }
        else
        {
            // Val terug op de ingebouwde duur van het ParticleSystem.
            // Dit zorgt ervoor dat het object pas wordt vernietigd nadat de laatste deeltjes zijn verdwenen.
            destroyTime = newParticles.main.duration + newParticles.main.startLifetime.constantMax;
        }
        
        // Vernietig het object na de berekende tijd.
        Destroy(newParticles.gameObject, destroyTime);

        return newParticles;
    }
}

// Een serialiseerbare struct om de prefabs, namen en aangepaste duur te organiseren in de Inspector.
[System.Serializable]
public struct ParticleEntry
{
    public string name;                 // De unieke naam om het effect mee aan te roepen (b.v. "DustCloud").
    public ParticleSystem prefab;       // Het Particle System Prefab dat moet spawnen.
    [Tooltip("Als deze waarde > 0 is, wordt de deeltjesinstantie na dit aantal seconden vernietigd.")]
    public float customDuration;        // De aangepaste tijd voor vernietiging (in seconden). Laat op 0 voor automatische vernietiging.
}