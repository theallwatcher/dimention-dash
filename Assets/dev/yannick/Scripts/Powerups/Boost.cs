using UnityEngine;

public class Boost : BasePowerup
{
    [SerializeField] float boostAmount;
    protected override void Activate(PlayerMovement player)
    {
        player.DamagePlayer(-boostAmount);
    }
}
