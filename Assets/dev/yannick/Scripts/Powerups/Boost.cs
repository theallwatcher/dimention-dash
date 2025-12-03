using UnityEngine;

public class Boost : BasePowerup
{
    [SerializeField] float boostAmount;
    protected override void Activate(PlayerMovement player)
    {
        audioManagerSam.Instance.Play(audioManagerSam.SoundType.Boost);
        player.MovePlayerZ(-boostAmount);
        Destroy(gameObject.transform.parent.gameObject);
    }
}
