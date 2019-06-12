using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gesture
{
    public enum GestureHand
    {
        LEFT,
        RIGHT,
        BOTH
    };

    public List<Vector3> leftNodes, rightNodes;
    public GestureHand curHands;
    public float yRotation; //starting rotation of the gesture in degrees
    public SpellHandler.SpellType spell;

    public Gesture(float yRotation, GestureHand hand)
    {
        this.yRotation = yRotation;

        switch(hand)
        {
            case GestureHand.LEFT:
                leftNodes = new List<Vector3>();
                rightNodes = null;
                break;
            case GestureHand.RIGHT:
                leftNodes = null;
                rightNodes = new List<Vector3>();
                break;
            case GestureHand.BOTH:
                leftNodes = new List<Vector3>();
                rightNodes = new List<Vector3>();
                break;
        }

        curHands = hand;
        spell = SpellHandler.SpellType.NONE;
    }

    public void AddHand(GestureHand hand)
    {
        switch (hand)
        {
            case GestureHand.LEFT:
                if (curHands == GestureHand.RIGHT)
                {
                    leftNodes = new List<Vector3>();
                    curHands = GestureHand.BOTH;
                }
                else
                    throw new System.Exception("Unable to add this hand");
                break;
            case GestureHand.RIGHT:
                if (curHands == GestureHand.LEFT)
                {
                    rightNodes = new List<Vector3>();
                    curHands = GestureHand.BOTH;
                }
                else
                    throw new System.Exception("Unable to add this hand");
                break;
            case GestureHand.BOTH:
                throw new System.Exception("Should not add both hands here");
        }
    }

    public void AddNode(Vector3 node, GestureHand hand)
    {
        //Fix node based on rotation of head
        float h, nodeRot, theta;
        Vector3 rotNode;

        //Distance from the headset to the node
        h = Mathf.Sqrt(node.x * node.x + node.z * node.z);

        //If the node is exactly above or below the head
        if (h != 0)
        {
            //Rotation of the node relative to the forward direction of the gesture
            nodeRot = Mathf.Acos(node.z / h);

            //Corrects for cosine only going up to 180
            if (node.x < 0)
                nodeRot = 2.0f * Mathf.PI - nodeRot;

            //Angle of the node when the gesture is rotated back to zero
            theta = nodeRot - (yRotation * Mathf.Deg2Rad);

            if (theta >= 2.0f * Mathf.PI)
                theta -= 2.0f * Mathf.PI;
            else if (theta < 0.0f)
                theta += 2.0f * Mathf.PI;

            //theta must be normalized to be used in the trigonometric functions
            //this also determines which quadrant and thus the sign of the x and y
            if (theta > 3.0f * Mathf.PI / 2.0f)
            {
                theta = 2.0f * Mathf.PI - theta;
                rotNode = new Vector3(-1, node.y, 1);
            }
            else if (theta > Mathf.PI)
            {
                theta = theta - Mathf.PI;
                rotNode = new Vector3(-1, node.y, -1);
            }
            else if (theta > Mathf.PI / 2.0f)
            {
                theta = Mathf.PI - theta;
                rotNode = new Vector3(1, node.y, -1);
            }
            else
            {
                rotNode = new Vector3(1, node.y, 1);
            }

            rotNode.x *= h * Mathf.Sin(theta);
            rotNode.z *= h * Mathf.Cos(theta);
        }
        else
        {
            rotNode = new Vector3(0, node.y, 0);
        }

        switch (hand)
        {
            case GestureHand.LEFT:
                if(leftNodes != null)
                    leftNodes.Add(rotNode);
                else
                    throw new System.Exception("This hand does not exist in this gesture");
                break;
            case GestureHand.RIGHT:
                if (rightNodes != null)
                    rightNodes.Add(rotNode);
                else
                    throw new System.Exception("This hand does not exist in this gesture");
                break;
            case GestureHand.BOTH:
                throw new System.Exception("Should not add the same node to both hands");
        }
    }

    public Vector3 NodeAt(int index, GestureHand hand)
    {
        switch (hand)
        {
            case GestureHand.LEFT:
                if (leftNodes != null)
                    return leftNodes[index];
                else
                    throw new System.Exception("This hand does not exist in this gesture");
            case GestureHand.RIGHT:
                if (rightNodes != null)
                    return rightNodes[index];
                else
                    throw new System.Exception("This hand does not exist in this gesture");
            default:
                throw new System.Exception("Unable to get the node at more than one hand");
        }
    }

    public int NumNodes(GestureHand hand)
    {
        switch (hand)
        {
            case GestureHand.LEFT:
                if (leftNodes != null)
                    return leftNodes.Count;
                else
                    throw new System.Exception("This hand does not exist in this gesture");
            case GestureHand.RIGHT:
                if (rightNodes != null)
                    return rightNodes.Count;
                else
                    throw new System.Exception("This hand does not exist in this gesture");
            default:
                throw new System.Exception("Unable to get the number of nodes for more than one hand");
        }
    }

    //Don't remember what this was for lol
    public void ChangeNumNodes(GestureHand hand)
    {
        switch (hand)
        {
            case GestureHand.LEFT:
                
                break;
            case GestureHand.RIGHT:
                
                break;
            case GestureHand.BOTH:
                
                break;
        }
    }

    public float GetRotation()
    {
        return yRotation;
    }

    public GestureHand GetCurHands()
    {
        return curHands;
    }

    public void SetSpell(SpellHandler.SpellType spell)
    {
        this.spell = spell;
    }

    public SpellHandler.SpellType GetSpell()
    {
        return spell;
    }

    public float AverageDifference(List<Vector3> samples, GestureHand hand)
    {
        float diff = 0;
        int numNodes = NumNodes(hand);

        for (int i = 0; i < numNodes; i++)
        {
            Vector3 diffVec = NodeAt(i, hand) - samples[i];

            diff += diffVec.magnitude;
        }

        return diff / numNodes;
    }

    public void DuplicateHand(GestureHand hand)
    {
        if (hand == GestureHand.LEFT)
        {
            //Add a right hand to copy onto
            AddHand(GestureHand.RIGHT);

            for (int i = 0; i < leftNodes.Count; i++)
            {
                //Flip the x to the other side and call it a day
                Vector3 temp = leftNodes[i];
                temp.x *= -1;

                rightNodes.Add(temp);
            }
        }
        else if (hand == GestureHand.RIGHT)
        {
            //Add a left hand to copy onto
            AddHand(GestureHand.LEFT);

            for (int i = 0; i < rightNodes.Count; i++)
            {
                //Flip the x to the other side and call it a day
                Vector3 temp = rightNodes[i];
                temp.x *= -1;

                leftNodes.Add(temp);
            }
        }
    }
}
