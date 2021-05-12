using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public List<NodeTuples> neighbours = new List<NodeTuples>();

    public Node prevNode;
    public float distance = 999999;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;

        foreach (NodeTuples t in neighbours)
        {
            Gizmos.DrawLine(this.transform.position, t.node.transform.position);
        }
    }
}