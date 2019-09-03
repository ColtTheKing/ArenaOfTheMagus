using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionChecker : MonoBehaviour
{
    public List<Obstacle> obstacles;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsColliding(Vector3 point, bool player)
    {
        foreach(Obstacle o in obstacles)
        {
            if (o.onlyPlayer && !player)
                continue;

            //If the point is within the bounds of this obstacle it is colliding
            if (point.x > o.transform.position.x - o.radius && point.x < o.transform.position.x + o.radius
                && point.z > o.transform.position.z - o.radius && point.z < o.transform.position.z + o.radius)
                return true;
        }

        return false;
    }
}
