using UnityEngine;
using URDev.GameFramework;

public class Test : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Managers.Input.KeyAction += () => { Debug.Log("KeyAction"); };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
