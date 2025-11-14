using UnityEngine;

[CreateAssetMenu(fileName = "PlayerObject", menuName = "Scriptable Objects/PlayerObject")]
public class PlayerObject : ScriptableObject
{
    public float LaneOffset;

    public float DamageOffset;
    public float BoostOffset;

    public float MovementSpeed;
    public float JumpForce;
}
