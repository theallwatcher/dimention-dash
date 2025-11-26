using UnityEngine;
using UnityEngine.InputSystem;

public class audiotest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.tKey.wasPressedThisFrame) {

            audioManagerSam.Instance.Play(audioManagerSam.SoundType.Jump);
        }
        
    }
}
