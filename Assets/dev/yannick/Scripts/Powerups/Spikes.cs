using UnityEngine;

public class Spikes : BasePowerup
{
    [SerializeField] private float damageAmount;
    [SerializeField] private GameObject damageEffect;
    protected override void Activate(PlayerMovement player)
    {
        player.MovePlayerZ(damageAmount);
        Instantiate(damageEffect);
        Destroy(gameObject);
    }
}
