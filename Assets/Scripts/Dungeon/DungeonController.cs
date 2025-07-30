using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonController : MonoBehaviour
{
    public GameObject gate;

    [SerializeField] private Collider2D gateCollider;
    //[SerializeField] private Animation 

    private void Start()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

}
