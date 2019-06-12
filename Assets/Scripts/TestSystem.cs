using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

public class TestSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        float dTime = Time.deltaTime;

        Entities.ForEach((ref Translation translation, ref TestComponent ai) =>
        {
            
        });
    }
}
