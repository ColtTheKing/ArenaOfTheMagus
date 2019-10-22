using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleConvertTest
{
    public static void Test1()
    {
        Gesture testGesture = new Gesture(Gesture.GestureHand.LEFT);
        testGesture.AddStartPoint(Gesture.GestureHand.LEFT, new Vector3(0, 0, 0));
        testGesture.AddNode(new Vector3(-0.5f, 0.5f, 1.5f), Gesture.GestureHand.LEFT);
        testGesture.AddNode(new Vector3(-0.5f, 1, 3.5f), Gesture.GestureHand.LEFT);
        testGesture.AddNode(new Vector3(0, 1.5f, 5), Gesture.GestureHand.LEFT);
        testGesture.AddNode(new Vector3(0.5f, 2, 6.5f), Gesture.GestureHand.LEFT);
        testGesture.AddNode(new Vector3(1.5f, 2.5f, 7.5f), Gesture.GestureHand.LEFT);

        List<Vector3> expectedNodes = new List<Vector3>();
        expectedNodes.Add(new Vector3(0, 0, 0));
        expectedNodes.Add(new Vector3(-0.357f, 0.357f, 1.071f));
        expectedNodes.Add(new Vector3(-0.143f, 0.357f, 1.285f));
        expectedNodes.Add(new Vector3(0.071f, 0.357f, 1.357f));
        expectedNodes.Add(new Vector3(0.357f, 0.357f, 1.071f));
        expectedNodes.Add(new Vector3(0.357f, 0.357f, 1.071f));
        expectedNodes.Add(new Vector3(0.499f, 0.357f, 0.929f));
        expectedNodes.Add(new Vector3(0.716f, 0.358f, 0.716f));

        int testNumNodes = 8;
        float maxAllowedDiff = 0.001f;

        testGesture.ChangeNumNodes(testNumNodes);

        bool passed = true;

        Debug.Log("Test 1 Results:");

        for(int i = 0; i < testNumNodes; i++)
        {
            Vector3 node = testGesture.NodeAt(i, Gesture.GestureHand.LEFT);

            Debug.Log("Node " + i + " value is " + node);
            Debug.Log("Expected value is " + expectedNodes[i]);

            if ((node - expectedNodes[i]).magnitude > maxAllowedDiff)
                passed = false;
        }

        if (passed)
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
        Gesture testGesture = new Gesture(Gesture.GestureHand.LEFT);
        testGesture.AddStartPoint(Gesture.GestureHand.LEFT, new Vector3(0, 0, 0));
        testGesture.AddNode(new Vector3(-0.271f, 0.227f, 0.663f), Gesture.GestureHand.LEFT);
        testGesture.AddNode(new Vector3(-0.5f, 0.5f, 1.5f), Gesture.GestureHand.LEFT);
        testGesture.AddNode(new Vector3(-0.55f, 0.755f, 2.352f), Gesture.GestureHand.LEFT);
        testGesture.AddNode(new Vector3(-0.5f, 1, 3.333f), Gesture.GestureHand.LEFT);
        testGesture.AddNode(new Vector3(-0.361f, 1.234f, 4.1f), Gesture.GestureHand.LEFT);
        testGesture.AddNode(new Vector3(-0.081f, 1.497f, 4.899f), Gesture.GestureHand.LEFT);
        testGesture.AddNode(new Vector3(0.209f, 1.733f, 5.729f), Gesture.GestureHand.LEFT);
        testGesture.AddNode(new Vector3(0.5f, 2, 6.5f), Gesture.GestureHand.LEFT);
        testGesture.AddNode(new Vector3(0.972f, 2.247f, 7.068f), Gesture.GestureHand.LEFT);
        testGesture.AddNode(new Vector3(1.5f, 2.5f, 7.5f), Gesture.GestureHand.LEFT);

        List<Vector3> expectedNodes = new List<Vector3>();
        expectedNodes.Add(new Vector3(0, 0, 0));
        expectedNodes.Add(new Vector3(-0.369f, 0.344f, 1.022f));
        expectedNodes.Add(new Vector3(-0.174f, 0.375f, 1.208f));
        expectedNodes.Add(new Vector3(0.083f, 0.348f, 1.322f));
        expectedNodes.Add(new Vector3(0.299f, 0.355f, 1.118f));
        expectedNodes.Add(new Vector3(0.412f, 0.349f, 1.169f));
        expectedNodes.Add(new Vector3(0.519f, 0.37f, 0.985f));
        expectedNodes.Add(new Vector3(0.73f, 0.359f, 0.676f));

        int testNumNodes = 8;
        float maxAllowedDiff = 0.001f;

        testGesture.ChangeNumNodes(testNumNodes);

        bool passed = true;

        Debug.Log("Test 2 Results:");

        for (int i = 0; i < testNumNodes; i++)
        {
            Vector3 node = testGesture.NodeAt(i, Gesture.GestureHand.LEFT);

            Debug.Log("Node " + i + ": " + node);
            Debug.Log("Expected value is " + expectedNodes[i]);

            if ((node - expectedNodes[i]).magnitude > maxAllowedDiff)
                passed = false;
        }

        if (passed)
        {
            Debug.Log("Test 2 passed!");
        }
        else
        {
            Debug.Log("Test 2 failed");
        }
    }
}
