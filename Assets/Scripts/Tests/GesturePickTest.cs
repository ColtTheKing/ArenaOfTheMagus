using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GesturePickTest
{
    public static void Test1()
    {
        Gesture baseGesture = new Gesture(Gesture.GestureHand.LEFT);
        baseGesture.AddStartPoint(Gesture.GestureHand.LEFT, new Vector3(0, 0, 0));
        baseGesture.AddNode(new Vector3(-0.5f, 0.5f, 1.5f), Gesture.GestureHand.LEFT);
        baseGesture.AddNode(new Vector3(-0.5f, 1, 3.5f), Gesture.GestureHand.LEFT);
        baseGesture.AddNode(new Vector3(0, 1.5f, 5), Gesture.GestureHand.LEFT);
        baseGesture.AddNode(new Vector3(0.5f, 2, 6.5f), Gesture.GestureHand.LEFT);
        baseGesture.AddNode(new Vector3(1.5f, 2.5f, 7.5f), Gesture.GestureHand.LEFT);

        Gesture testGesture = new Gesture(Gesture.GestureHand.LEFT);
        testGesture.AddStartPoint(Gesture.GestureHand.LEFT, new Vector3(0, 0, 0));
        testGesture.AddNode(new Vector3(-0.5f, 0.5f, 1.5f), Gesture.GestureHand.LEFT);
        testGesture.AddNode(new Vector3(-1, 1, 3), Gesture.GestureHand.LEFT);
        testGesture.AddNode(new Vector3(-0.5f, 1.5f, 5), Gesture.GestureHand.LEFT);
        testGesture.AddNode(new Vector3(0.5f, 2, 6.5f), Gesture.GestureHand.LEFT);
        testGesture.AddNode(new Vector3(2, 2.5f, 7), Gesture.GestureHand.LEFT);

        float maxAllowedDiff = 0.6f;
        float expectedDiff = 0.036f;

        float diff = baseGesture.AverageDifference(testGesture, Gesture.GestureHand.LEFT);

        Debug.Log("Difference value for Test 1 is " + diff);
        Debug.Log("The expected difference is " + expectedDiff);
        Debug.Log("It should be less than " + maxAllowedDiff + " and therefore match the base gesture");

        if (diff < maxAllowedDiff && Mathf.Abs(expectedDiff - diff) < 0.001f)
        {
            Debug.Log("Test 1 passed!");
        }
        else
        {
            Debug.Log("Test 1 failed");
        }
    }

    public static void Test2()
    {
        Gesture baseGesture = new Gesture(Gesture.GestureHand.LEFT);
        baseGesture.AddStartPoint(Gesture.GestureHand.LEFT, new Vector3(0, 0, 0));
        baseGesture.AddNode(new Vector3(-0.5f, 0.5f, 1.5f), Gesture.GestureHand.LEFT);
        baseGesture.AddNode(new Vector3(-0.5f, 1, 3.5f), Gesture.GestureHand.LEFT);
        baseGesture.AddNode(new Vector3(0, 1.5f, 5), Gesture.GestureHand.LEFT);
        baseGesture.AddNode(new Vector3(0.5f, 2, 6.5f), Gesture.GestureHand.LEFT);
        baseGesture.AddNode(new Vector3(1.5f, 2.5f, 7.5f), Gesture.GestureHand.LEFT);

        Gesture testGesture = new Gesture(Gesture.GestureHand.LEFT);
        testGesture.AddStartPoint(Gesture.GestureHand.LEFT, new Vector3(0, 0, 0));
        testGesture.AddNode(new Vector3(-0.5f, -0.5f, -1.5f), Gesture.GestureHand.LEFT);
        testGesture.AddNode(new Vector3(-1, -1, -3), Gesture.GestureHand.LEFT);
        testGesture.AddNode(new Vector3(-0.5f, -1.5f, -5), Gesture.GestureHand.LEFT);
        testGesture.AddNode(new Vector3(0.5f, -2, -6.5f), Gesture.GestureHand.LEFT);
        testGesture.AddNode(new Vector3(2, -2.5f, -7), Gesture.GestureHand.LEFT);

        float maxAllowedDiff = 0.6f;
        float expectedDiff = 1.588f;

        float diff = baseGesture.AverageDifference(testGesture, Gesture.GestureHand.LEFT);

        Debug.Log("Difference value for Test 2 is " + diff);
        Debug.Log("The expected difference is " + expectedDiff);
        Debug.Log("It should be greater than " + maxAllowedDiff + " and therefore not match the base gesture");

        if (diff > maxAllowedDiff)
        {
            Debug.Log("Test 2 passed!");
        }
        else
        {
            Debug.Log("Test 2 failed");
        }
    }
}
