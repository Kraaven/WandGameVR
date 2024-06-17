using System;
using System.Collections.Generic;
using UnityEngine;

namespace Spells
{
    public class Spell : MonoBehaviour
    {
        // Start is called before the first frame update
        private Vector3 StartPosition;
        void Start()
        {
            StartPosition = transform.position;
            var rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.velocity = transform.forward * 16.5f;
        
        }

        // Update is called once per frame
        void Update()
        {
            if (Vector3.Distance(StartPosition, transform.position) > 20)
            {
                Destroy(gameObject);
            }
        }

        // private void FixedUpdate()
        // {
        //     transform.Translate(transform.forward*0.5f);
        // }

        public virtual void CollisionAction(GameObject target)
        {
            
        }

        private void OnCollisionEnter(Collision other)
        {
            CollisionAction(other.gameObject);
            Destroy(gameObject);
        }
    }


    [RequireComponent(typeof(Collider))]
    public class Interactable : MonoBehaviour
    {

        public virtual void Interact(SpellType spellType)
        {
            
        }

    }

    public enum SpellType
    {
        TELEPORT,
        INTERACT,
        BREAK
    }
}
