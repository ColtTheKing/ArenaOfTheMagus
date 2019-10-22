using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellEffect : MonoBehaviour
{
    public bool destroyOnDeath;

    private Enemy target;
    private float lifespan;
    private bool targetSet;

    // Start is called before the first frame update
    public void Awake()
    {
        lifespan = 1000;
    }

    // Update is called once per frame
    public void Update()
    {
        //Update it's remaining lifespan
        lifespan -= Time.deltaTime;

        Debug.Log("lifespan = " + lifespan);

        if (lifespan <= 0)
            Destroy(gameObject);

        if(target != null)
        {
            //Move the spell
            Vector3 targetPos = target.transform.position;
            targetPos.y = transform.position.y;

            transform.position = targetPos;
            transform.eulerAngles = target.transform.eulerAngles;
        }
        else if(targetSet && destroyOnDeath)
        {
            Destroy(gameObject);
        }
    }

    public void EffectTarget(Enemy enemy, float effectDuration)
    {
        target = enemy;

        Vector3 targetPos = target.transform.position;
        targetPos.y = transform.position.y;

        transform.position = targetPos;

        targetSet = true;
        lifespan = effectDuration;
        Debug.Log("effected");
    }
}
