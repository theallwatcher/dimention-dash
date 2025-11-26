using UnityEngine;

[CreateAssetMenu(fileName = "PlayerObject", menuName = "Scriptable Objects/PlayerObject")]
public class PlayerObject : ScriptableObject
{
    public float LaneOffset;

    public float MovementSpeed;
    public float JumpHeight;

    public float SlideDuration;
    public float slideHeight;

}
