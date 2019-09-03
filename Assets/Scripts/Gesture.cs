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
    public SpellHandler.SpellType spell;

    private Vector3 leftLastNodePos, rightLastNodePos;
    private float leftRotation, rightRotation;

    public Gesture(GestureHand startingHand)
    {
        switch (startingHand)
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

        curHands = startingHand;
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

    public void AddStartPoint(GestureHand hand, Vector3 startPos)
    {
        float tempRot;

        float h = Mathf.Sqrt(startPos.x * startPos.x + startPos.z * startPos.z);

        //Determine the rotation of this node relative to the head
        if (h == 0)
        {
            tempRot = 0;
        }
        else
        {
            if(startPos.x >= 0)
                tempRot = Mathf.Acos(startPos.z / h);
            else
                tempRot = 2.0f * Mathf.PI - Mathf.Acos(startPos.z / h);
        }

        //If this node is the first then it should be the starting position
        if (hand == GestureHand.LEFT && leftNodes.Count == 0)
        {
            leftNodes.Add(startPos);
            leftLastNodePos = startPos;

            leftRotation = tempRot;

            //Debug.Log("Starting left");
            //Debug.Log("left rot = " + leftRotation);
        }
        else if (hand == GestureHand.RIGHT && rightNodes.Count == 0)
        {
            rightNodes.Add(startPos);
            rightLastNodePos = startPos;

            rightRotation = tempRot;

            //Debug.Log("Starting right");
            //Debug.Log("right rot = " + rightRotation);
        }
    }

    public void AddNode(Vector3 node, GestureHand hand)
    {
        Vector3 offset;
        float theta;

        //Add a new node equal to the offset of the last node to this one and update the last node variable
        switch (hand)
        {
            case GestureHand.LEFT:
                if (leftLastNodePos != null)
                {
                    offset = node - leftLastNodePos;
                    leftLastNodePos = node;
                    theta = leftRotation;
                }
                else
                    throw new System.Exception("The start point for this hand has not been added yet");
                break;
            case GestureHand.RIGHT:
                if (rightLastNodePos != null)
                {
                    offset = node - rightLastNodePos;
                    rightLastNodePos = node;
                    theta = rightRotation;
                }
                else
                    throw new System.Exception("The start point for this hand has not been added yet");
                break;
            default:
                throw new System.Exception("You shouldn't be adding a node to both hands at once");
        }

        //Rotate the offset as if the starting rotation of the gesture was zero
        float x = offset.x * Mathf.Cos(theta) - offset.z * Mathf.Sin(theta);
        float z = offset.x * Mathf.Sin(theta) + offset.z * Mathf.Cos(theta);

        offset.x = x;
        offset.z = z;

        //Debug.Log("x = " + x);
        //Debug.Log("z = " + z);

        //Add the offset to the gesture
        switch (hand)
        {
            case GestureHand.LEFT:
                if(leftNodes != null)
                    leftNodes.Add(offset);
                else
                    throw new System.Exception("This hand does not exist in this gesture");
                break;
            case GestureHand.RIGHT:
                if (rightNodes != null)
                    rightNodes.Add(offset);
                else
                    throw new System.Exception("This hand does not exist in this gesture");
                break;
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

    public void SetNodeAt(int index, GestureHand hand, Vector3 value)
    {
        switch (hand)
        {
            case GestureHand.LEFT:
                if (leftNodes == null)
                    throw new System.Exception("This hand does not exist in this gesture");
                else if (index >= leftNodes.Count)
                    throw new System.Exception("Node index is too high. Count is " + leftNodes.Count + ", but index is " + index);
                else
                    leftNodes[index] = value;
                break;
            case GestureHand.RIGHT:
                if (rightNodes == null)
                    throw new System.Exception("This hand does not exist in this gesture");
                else if (index >= rightNodes.Count)
                    throw new System.Exception("Node index is too high. Count is " + rightNodes.Count + ", but index is " + index);
                else
                    rightNodes[index] = value;
                break;
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

            //Keeps track of the actual position each point on the gesture is at
            Vector3 lastNodePosition = NodeAt(0, GestureHand.LEFT);
            int lastNodeIndex = 0;

            Vector3 lastNodeConvertedPosition = lastNodePosition;

            for (int i = 1; i < numNodes - 1; i++)
            {
                //How far along the gesture the current segment is
                float curSegment = segment * i;

                //If we passed any nodes increment the index and add on that node's offset
                while(Mathf.FloorToInt(curSegment) > lastNodeIndex)
                {
                    lastNodePosition += NodeAt(++lastNodeIndex, GestureHand.LEFT);
                }

                //Calculate the portion of the next node's distance to move
                Vector3 partOfNext = NodeAt(lastNodeIndex+1, GestureHand.LEFT) * (curSegment % 1);

                //Calculate the actual position of the converted node
                Vector3 convertedNodePos = lastNodePosition + partOfNext;

                //Add the offset of the converted node from the last converted node
                nodesConverted.Add(convertedNodePos - lastNodeConvertedPosition);

                //Update the last converted node's position
                lastNodeConvertedPosition = convertedNodePos;
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

            //Keeps track of the actual position each point on the gesture is at
            Vector3 lastNodePosition = NodeAt(0, GestureHand.RIGHT);
            int lastNodeIndex = 0;

            Vector3 lastNodeConvertedPosition = lastNodePosition;

            for (int i = 1; i < numNodes - 1; i++)
            {
                //How far along the gesture the current segment is
                float curSegment = segment * i;

                //If we passed any nodes increment the index and add on that node's offset
                while (Mathf.FloorToInt(curSegment) > lastNodeIndex)
                {
                    lastNodePosition += NodeAt(++lastNodeIndex, GestureHand.RIGHT);
                }

                //Calculate the portion of the next node's distance to move
                Vector3 partOfNext = NodeAt(lastNodeIndex + 1, GestureHand.RIGHT) * (curSegment % 1);

                //Calculate the actual position of the converted node
                Vector3 convertedNodePos = lastNodePosition + partOfNext;

                //Add the offset of the converted node from the last converted node
                nodesConverted.Add(convertedNodePos - lastNodeConvertedPosition);

                //Update the last converted node's position
                lastNodeConvertedPosition = convertedNodePos;
            }

            //The last node will be the same
            nodesConverted.Add(NodeAt(NumNodes(GestureHand.RIGHT) - 1, GestureHand.RIGHT));

            //Actually set the nodes to the new converted ones
            rightNodes = nodesConverted;
        }
    }

    public float GetRotation(GestureHand hand)
    {
        switch(hand)
        {
            case GestureHand.LEFT:
                return leftRotation;
            case GestureHand.RIGHT:
                return rightRotation;
            default:
                return leftRotation + rightRotation / 2;
        }
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

    public float AverageDifference(Gesture toCompare, float lengthFactor, GestureHand hand)
    {
        float diff = 0;
        int numNodes = NumNodes(hand);

        for (int i = 1; i < numNodes; i++)
        {
            Vector3 node1 = NodeAt(i, hand);
            Vector3 node2 = toCompare.NodeAt(i, hand);
            
            float angleDifference = 1.0f - Vector3.Dot(node1.normalized, node2.normalized);

            float lengthDifference = Mathf.Abs(node1.magnitude - node2.magnitude) / node1.magnitude;

            diff += angleDifference + lengthDifference * lengthFactor;
        }

        return diff / (numNodes-1);
    }

    public void DuplicateHand()
    {
        if (curHands == GestureHand.BOTH)
            return;

        if (rightNodes == null)
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

            rightRotation = 2.0f * Mathf.PI - leftRotation;
        }
        else if (leftNodes == null)
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

            leftRotation = 2.0f * Mathf.PI - rightRotation;
        }
    }
}
