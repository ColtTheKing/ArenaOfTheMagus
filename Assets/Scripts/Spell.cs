using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour
{
    public float speed, lifespan;
    public int damage;
    public SpellHandler.SpellElement element;
    public bool deleteOnContact;

    private Vector3 direction;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Move the spell if necessary
        transform.position = transform.position + (direction * speed);

        //Check if the spell is effecting something


        //Update it's remaining lifespan
        lifespan -= Time.deltaTime;

        if (lifespan <= 0)
            Destroy(gameObject);
    }

    public void SetDirection(Vector3 direction)
    {
        this.direction = direction;
    }

    void OnCollisionEnter(Collision collision)
    {
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();

        if(enemy != null)
        {
            enemy.Damage(damage);

            if (element == SpellHandler.SpellElement.FIRE)
                enemy.Ignite();

            if (deleteOnContact)
                Destroy(gameObject);
        }
    }
}
