using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEditor.VersionControl;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public Navigation navigation;
    
    public Vector3 velocity;
    public Vector3 steering;
    public Vector3 targetVelocity;

    public float mass;
    public float targetDistance;
    public Node target;
    public int nodesInPath;

    void Update()
    {

        if (navigation.charState == CharState.Chase)
        {
            targetVelocity = Vector3.Normalize(navigation.enemy.transform.position - this.transform.position) * 0.01f;
            targetDistance = Vector3.Distance(this.transform.position, navigation.enemy.transform.position);
        }
        else
        {
            target = navigation.path[nodesInPath];
            targetVelocity = Vector3.Normalize(target.transform.position - this.transform.position) * 0.01f;
            targetDistance = Vector3.Distance(this.transform.position, target.transform.position);
        }

        steering = targetVelocity - velocity;

        steering = steering / mass;

        velocity = velocity + steering;
        this.transform.position = this.transform.position + velocity;

        if (navigation.charState != CharState.Chase)
        {
            if (targetDistance < 0.5 && nodesInPath != 0)
            {
                nodesInPath--;
            }
            else if (targetDistance < 0.5)
            {
                navigation.finalNodeFound = true;
            }
        }
    }
}