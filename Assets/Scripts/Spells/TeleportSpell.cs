using System.Collections;
using System.Collections.Generic;
using Spells;
using Unity.XR.CoreUtils;
using UnityEngine;

public class TeleportSpell : Spell
{
    
    public override void CollisionAction(GameObject Target)
    {
        if (Target.CompareTag("Teleportation Area"))
        {
            FindObjectOfType<XROrigin>().transform.position = transform.position; 
        }
        
    }
}
