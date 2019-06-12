using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sample : MonoBehaviour
{
    public float LIFE_SPAN;
    private float remainingLife;

    // Start is called before the first frame update
    void Start()
    {
        remainingLife = LIFE_SPAN;
    }

    // Update is called once per frame
    void Update()
    {
        remainingLife -= Time.deltaTime;

        if (remainingLife <= 0)
            Destroy(gameObject);
    }
}
