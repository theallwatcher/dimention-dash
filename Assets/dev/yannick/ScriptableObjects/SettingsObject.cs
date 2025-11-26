using UnityEngine;

[CreateAssetMenu(fileName = "Settings", menuName = "Scriptable Objects/Settings")]
public class SettingsObject : ScriptableObject
{
    [Header("General")]
    public bool IsPaused;

    [Header("Audio")]
    public float Volume;

    [Header("Other")]
    public int FrameRate;
}
