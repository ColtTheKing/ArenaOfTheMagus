using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class HandSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref HandComponent hand) =>
        {

        });
    }
}
