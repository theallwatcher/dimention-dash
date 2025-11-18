using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ItemObject", menuName = "Scriptable Objects/ItemObject")]
public class ItemObject : ScriptableObject
{
    public Sprite UI_sprite;
    public GameObject Prefab;
}
