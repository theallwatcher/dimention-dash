using UnityEngine;

public class Spikes : BasePowerup
{
    [SerializeField] private float damageAmount;
    [SerializeField] private GameObject damageEffect;
    protected override void Activate(PlayerMovement player)
    {
        audioManagerSam.Instance.Play(audioManagerSam.SoundType.Break);
        player.MovePlayerZ(damageAmount);
        Instantiate(damageEffect);
        Destroy(gameObject);
    }
}
