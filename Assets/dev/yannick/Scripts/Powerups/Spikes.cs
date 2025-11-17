using UnityEngine;

public class Spikes : BasePowerup
{
    [SerializeField] private float damageAmount;
    protected override void Activate(PlayerMovement player)
    {
        player.MovePlayerZ(damageAmount);
    }
}
