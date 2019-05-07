using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class SpellSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref SpellComponent spell) =>
        {

        });
    }
}
