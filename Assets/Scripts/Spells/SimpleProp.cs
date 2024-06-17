using System.Collections;
using System.Collections.Generic;
using Spells;
using UnityEngine;

public class SimpleProp : Interactable
{
    public override void Interact(SpellType spellType)
    {
        switch (spellType)
        {
            case SpellType.BREAK:
                Destroy(gameObject);
                break;
            case SpellType.INTERACT:
                StartCoroutine(SELECT());
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SELECT()
    {
        var position = transform.position;

        for (int i = 0; i < 100; i++)
        {
            transform.Translate(new Vector3(0,0.01f,0));
            yield return new WaitForSeconds(0.01f);
        }

        for (int i = 0; i < 72; i++)
        {
            transform.Rotate(new Vector3(0,5,0));
            yield return new WaitForSeconds(0.005f);
        }
        
        for (int i = 0; i < 100; i++)
        {
            transform.Translate(new Vector3(0,-0.01f,0));
            yield return new WaitForSeconds(0.01f);
        }
        
    }
}
