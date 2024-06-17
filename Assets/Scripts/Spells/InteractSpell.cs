using System.Collections;
using System.Collections.Generic;
using Spells;
using UnityEngine;

public class InteractSpell : Spell
{
    public override void CollisionAction(GameObject target)
    {
        if (target.TryGetComponent(out Interactable interactable))
        {
            interactable.Interact(SpellType.INTERACT);
        }
    }
}
