using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;

public class Navigation : MonoBehaviour
{
    public Movement movement;
    
    public Node startNode;
    public Node endNode;
    private Node currentNode;

    public List<Node> path;
    List<Node> unvisitedNodes = new List<Node>();
    List<Node> visitedNodes = new List<Node>();

    int loopCount = 0;

    private void Start()
    {
        CalculatePath();
    }

    private void CalculatePath()
    {
        //setup
        startNode.distance = 0;
        unvisitedNodes.Add(startNode);
        float endNodeDistance = Vector3.Distance(startNode.transform.position, endNode.transform.position);

        //loop
        while (unvisitedNodes.Count > 0 && loopCount < 1000)
        {
            currentNode = unvisitedNodes[0];

            if (unvisitedNodes[0] == endNode)
            {
                visitedNodes.Add(currentNode);
                unvisitedNodes.Remove(currentNode);
                continue;
            }

            List<NodeTuples> neighbours = currentNode.neighbours;

            foreach (NodeTuples tuple in neighbours)
            {
                if (visitedNodes.Contains(tuple.node))
                    continue;

                float heuristic = Vector3.Distance(currentNode.transform.position, endNode.transform.position);

                if (endNodeDistance < heuristic)
                    continue;

                float dist = currentNode.distance + tuple.weight + heuristic;

                if (tuple.node.distance > dist)
                {
                    tuple.node.distance = dist;
                    tuple.node.prevNode = currentNode;
                }

                if (!unvisitedNodes.Contains(tuple.node))
                    unvisitedNodes.Add(tuple.node);
            }

            visitedNodes.Add(currentNode);
            unvisitedNodes.Remove(currentNode);

            unvisitedNodes.OrderBy(n => n.distance);
            loopCount++;
        }
        loopCount = 0;
        
        Node nodeCheck = endNode;

        while (nodeCheck.prevNode != null && loopCount < 100)
        {
            path.Add(nodeCheck);
            nodeCheck = nodeCheck.prevNode;
            loopCount++;
        }
        path.Add(startNode);
        loopCount = 0;
        movement.enabled = true;
    }
}