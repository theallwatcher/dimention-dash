using UnityEngine;

public class Boost : BasePowerup
{
    [SerializeField] float boostAmount;
    protected override void Activate(PlayerMovement player)
    {
        player.MovePlayerZ(-boostAmount);
        Destroy(gameObject.transform.parent.gameObject);
    }
}
