using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

public class AISystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        float dTime = Time.deltaTime;

        Entities.ForEach((ref Translation translation, ref AIComponent ai) =>
        {
            
        });
    }
}
