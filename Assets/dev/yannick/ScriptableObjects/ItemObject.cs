using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ItemObject", menuName = "Scriptable Objects/ItemObject")]
public class ItemObject : ScriptableObject
{
    public Sprite UI_sprite;
    public GameObject Prefab;

    public enum ItemType
    {
         Bomb,
         Spikes,
         Boost,
         LaneSwitch,
         InvertControls,
         Shield,
         PositionSwitch,
         Coins
    }

    public ItemType Type;

}
