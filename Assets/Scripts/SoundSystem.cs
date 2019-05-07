using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class SoundSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref SoundComponent effect) =>
        {

        });
    }
}
