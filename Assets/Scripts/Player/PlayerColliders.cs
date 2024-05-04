using GameMap.Generator;
using UnityEngine;

public class PlayerColliders : MonoBehaviour
{
    Collider playerCollider;

    void Awake()
    {
        playerCollider = GetComponent<Collider>();
        playerCollider.enabled = false;
    }
}
