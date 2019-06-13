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
                if (leftNodes == null)
                    throw new System.Exception("This hand does not exist in this gesture");
                else if (index >= leftNodes.Count)
                    throw new System.Exception("Node index is too high. Count is " + leftNodes.Count + ", but index is " + index);
                else
                    return leftNodes[index];
            case GestureHand.RIGHT:
                if (rightNodes == null)
                    throw new System.Exception("This hand does not exist in this gesture");
                else if (index >= rightNodes.Count)
                    throw new System.Exception("Node index is too high. Count is " + rightNodes.Count + ", but index is " + index);
                else
                    return rightNodes[index];
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

    //Converts the nodes on each hand into a number of new nodes equal to numNodes
    public void ChangeNumNodes(int numNodes)
    {
        if (leftNodes != null)
        {
            List<Vector3> nodesConverted = new List<Vector3>();

            //First node will be the same
            nodesConverted.Add(NodeAt(0, GestureHand.LEFT));

            //The relative length of the segments between the samples
            float segment = (float)(NumNodes(GestureHand.LEFT) - 1) / (numNodes - 1);

            for (int i = 1; i < numNodes - 1; i++)
            {
                float curSegment = segment * i;

                Vector3 curSample = NodeAt(Mathf.FloorToInt(curSegment), GestureHand.LEFT);
                float percentAlong = curSegment % 1;

                Vector3 changeVec = NodeAt(Mathf.FloorToInt(curSegment) + 1, GestureHand.LEFT) - curSample;
                changeVec *= percentAlong;

                nodesConverted.Add(curSample + changeVec);
            }

            //The last node will be the same
            nodesConverted.Add(NodeAt(NumNodes(GestureHand.LEFT) - 1, GestureHand.LEFT));

            //Actually set the nodes to the new converted ones
            leftNodes = nodesConverted;
        }

        if (rightNodes != null)
        {
            List<Vector3> nodesConverted = new List<Vector3>();

            //First node will be the same
            nodesConverted.Add(NodeAt(0, GestureHand.RIGHT));

            //The relative length of the segments between the samples
            float segment = (float)(NumNodes(GestureHand.RIGHT) - 1) / (numNodes - 1);

            for (int i = 1; i < numNodes - 1; i++)
            {
                float curSegment = segment * i;

                Vector3 curSample = NodeAt(Mathf.FloorToInt(curSegment), GestureHand.RIGHT);
                float percentAlong = curSegment % 1;

                Vector3 changeVec = NodeAt(Mathf.FloorToInt(curSegment) + 1, GestureHand.RIGHT) - curSample;
                changeVec *= percentAlong;

                nodesConverted.Add(curSample + changeVec);
            }

            //The last node will be the same
            nodesConverted.Add(NodeAt(NumNodes(GestureHand.RIGHT) - 1, GestureHand.RIGHT));

            //Actually set the nodes to the new converted ones
            rightNodes = nodesConverted;
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

    public float AverageDifference(Gesture toCompare, GestureHand hand)
    {
        float diff = 0;
        int numNodes = NumNodes(hand);

        for (int i = 0; i < numNodes; i++)
        {
            Vector3 diffVec = NodeAt(i, hand) - toCompare.NodeAt(i, hand);

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
