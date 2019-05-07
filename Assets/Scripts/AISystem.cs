using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISystem : MonoBehaviour
{
    struct AIComponents
    {
        public AIComponent ai;
        public Transform transform;
    };

    void Update()
    {
        float dTime = Time.deltaTime;

        /*foreach(var entity in GetEntities<AIComponents>())
        {
            //do things
        }*/
        //find some way to do this without ecs package
    }
}
