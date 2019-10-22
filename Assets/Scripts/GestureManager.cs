using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class GestureManager
{
    public static readonly string GESTURE_PATH = @"Data/gestures.json";

    private List<Gesture> oneHandGestures, twoHandGestures, calibratedGestures;
    private Gesture currentGesture;
    private float sampleTime, lNextSample, rNextSample, accuracyPerSample;
    private int samplesPerGesture, numCalibrations;
    private bool calibrating;
    private SpellHandler.SpellType calibrationType;
    private GestureCalibrationMenu calibrationMenu;

    // Start is called before the first frame update
    public GestureManager(float sampleTime, float accuracyPerSample, int samplesPerGesture, bool clearJSON, int numCalibrations)
    {
        oneHandGestures = new List<Gesture>();
        twoHandGestures = new List<Gesture>();
        this.sampleTime = sampleTime;

        calibratedGestures = new List<Gesture>();

        this.numCalibrations = numCalibrations;

        //Defines how close each sample must be on average to find a match
        this.accuracyPerSample = accuracyPerSample;

        //The default number of samples to convert to when comparing gestures
        this.samplesPerGesture = samplesPerGesture;

        lNextSample = -1;
        rNextSample = -1;

        if(clearJSON)
            ClearJSON();

        //Populate validGestures
        LoadJson();
    }

    // Update is called once per frame
    public void Update(Player player, float deltaTime, bool inGame)
    {
        if (!inGame && !calibrating)
            return;

        //If a left gesture is being recorded
        if (lNextSample >= 0)
        {
            lNextSample -= deltaTime;

            //If enough time has passed since the last node in the left gesture, create a new one
            if (lNextSample <= 0)
            {
                currentGesture.AddNode(player.leftHand.CenterPos() - player.playerHead.transform.position, Gesture.GestureHand.LEFT);

                lNextSample = sampleTime;
            }
        }

        //If a right gesture is being recorded
        if (rNextSample >= 0)
        {
            rNextSample -= deltaTime;

            //If enough time has passed since the last node in the right gesture, create a new one
            if (rNextSample <= 0)
            {
                currentGesture.AddNode(player.rightHand.CenterPos() - player.playerHead.transform.position, Gesture.GestureHand.RIGHT);

                rNextSample = sampleTime;
            }
        }
    }

    public void BeginGesture(Player player, bool left, bool inGame)
    {
        if (!inGame && !calibrating)
            return;

        if (left)
        {
            //If this is the first hand of the gesture
            if (rNextSample == -1)
            {
                //Create a new left gesture
                currentGesture = new Gesture(Gesture.GestureHand.LEFT);
            }
            else //If the gesture has already been started by the other hand
            {
                currentGesture.AddHand(Gesture.GestureHand.LEFT);
            }

            //Add the starting node
            currentGesture.AddStartPoint(Gesture.GestureHand.LEFT, player.leftHand.CenterPos() - player.playerHead.transform.position);

            //Set time until the next sample should be recorded
            lNextSample = sampleTime;
        }
        else
        {
            //If this is the first hand of the gesture
            if (lNextSample == -1)
            {
                //Create a new left gesture
                currentGesture = new Gesture(Gesture.GestureHand.RIGHT);
            }
            else //If the gesture has already been started by the other hand
            {
                currentGesture.AddHand(Gesture.GestureHand.RIGHT);
            }

            //Add the starting node
            currentGesture.AddStartPoint(Gesture.GestureHand.RIGHT, player.rightHand.CenterPos() - player.playerHead.transform.position);

            //Set time until the next sample should be recorded
            rNextSample = sampleTime;
        }
    }

    public SpellHandler.SpellType EndGesture()
    {
        //If the gesture was already ended
        if (lNextSample == -1 && rNextSample == -1)
            return SpellHandler.SpellType.NONE;

        lNextSample = -1;
        rNextSample = -1;

        Gesture.GestureHand hands = currentGesture.GetCurHands();

        //If there's only 1 node in one of the hands just delete the gesture and stop cause otherwise bad things happen during conversion
        if ((hands == Gesture.GestureHand.LEFT && currentGesture.NumNodes(Gesture.GestureHand.LEFT) < 2)
            || (hands == Gesture.GestureHand.RIGHT && currentGesture.NumNodes(Gesture.GestureHand.RIGHT) < 2)
            || (hands == Gesture.GestureHand.BOTH && (currentGesture.NumNodes(Gesture.GestureHand.LEFT) < 2 || currentGesture.NumNodes(Gesture.GestureHand.RIGHT) < 2)))
        {
            currentGesture = null;
            return SpellHandler.SpellType.NONE;
        }
        
        currentGesture.ChangeNumNodes(samplesPerGesture);

        return FindBestMatch(currentGesture);
    }

    public void EndCalibratedGesture()
    {
        if (!calibrating || (lNextSample == -1 && rNextSample == -1))
            return;

        lNextSample = -1;
        rNextSample = -1;

        calibrating = false;

        Gesture.GestureHand hands = currentGesture.GetCurHands();

        //If there's only 1 node in one of the hands just delete the gesture and stop cause otherwise bad things happen during conversion
        if ((hands == Gesture.GestureHand.LEFT && currentGesture.NumNodes(Gesture.GestureHand.LEFT) < 2)
            || (hands == Gesture.GestureHand.RIGHT && currentGesture.NumNodes(Gesture.GestureHand.RIGHT) < 2)
            || (hands == Gesture.GestureHand.BOTH && (currentGesture.NumNodes(Gesture.GestureHand.LEFT) < 2 || currentGesture.NumNodes(Gesture.GestureHand.RIGHT) < 2)))
        {
            currentGesture = null;
            return;
        }

        currentGesture.ChangeNumNodes(samplesPerGesture);
        currentGesture.DuplicateHand();

        if(calibratedGestures.Count == numCalibrations-1)
        {
            Debug.Log("Finishing gesture");

            //Average the gestures
            for (int i = 1; i < samplesPerGesture; i++)
            {
                Vector3 avgNode = currentGesture.NodeAt(i, Gesture.GestureHand.LEFT);
                float avgLength = avgNode.magnitude;

                for(int j = 0; j < calibratedGestures.Count; j++)
                {
                    Vector3 node = calibratedGestures[j].NodeAt(i, Gesture.GestureHand.LEFT);

                    avgNode += node;
                    avgLength += node.magnitude;
                }

                avgNode /= numCalibrations;
                avgLength /= numCalibrations;

                avgNode = avgNode.normalized * avgLength;

                currentGesture.SetNodeAt(i, Gesture.GestureHand.LEFT, avgNode);
            }

            //Save the gesture to the json
            switch(calibrationType)
            {
                case SpellHandler.SpellType.ONE_QUICK:
                case SpellHandler.SpellType.ONE_HEAVY:
                case SpellHandler.SpellType.ONE_SPECIAL:
                    SaveLastGesture(calibrationType, false);
                    break;
                case SpellHandler.SpellType.TWO_DEFENSE:
                case SpellHandler.SpellType.TWO_OFFENSE:
                    SaveLastGesture(calibrationType, true);
                    break;
            }

            //Clear the gestures
            calibratedGestures = new List<Gesture>();

            calibrationMenu.FinishedGesture();
        }
        else
        {
            Debug.Log("Ending gesture");

            //Add the gesture so it can be used once enough are recorded
            calibratedGestures.Add(currentGesture);

            calibrationMenu.DecreaseRemainingGestures();
        }
    }

    public void SaveLastGesture(SpellHandler.SpellType type, bool twoHanded)
    {
        if (currentGesture == null)
            return;

        currentGesture.SetSpell(type);

        SaveGesture(currentGesture, twoHanded);

        Debug.Log("Saved Gesture for Spell Type: " + type);
    }

    public Gesture.GestureHand CurrentHand()
    {
        if(currentGesture != null)
            return currentGesture.curHands;

        return Gesture.GestureHand.BOTH;
    }

    private void ShowSamples(Player player)
    {
        if (currentGesture.GetCurHands() == Gesture.GestureHand.LEFT || currentGesture.GetCurHands() == Gesture.GestureHand.BOTH)
        {
            for (int i = 0; i < currentGesture.NumNodes(Gesture.GestureHand.LEFT); i++)
            {
                player.CreateSample(currentGesture.NodeAt(i, Gesture.GestureHand.LEFT));
            }
        }

        if (currentGesture.GetCurHands() == Gesture.GestureHand.RIGHT || currentGesture.GetCurHands() == Gesture.GestureHand.BOTH)
        {
            for (int i = 0; i < currentGesture.NumNodes(Gesture.GestureHand.RIGHT); i++)
            {
                player.CreateSample(currentGesture.NodeAt(i, Gesture.GestureHand.RIGHT));
            }
        }
    }

    //Load Gesture data from json
    private void LoadJson()
    {
        string readText = System.IO.File.ReadAllText(GESTURE_PATH);
        List<List<Gesture>> gestures = JsonConvert.DeserializeObject<List<List<Gesture>>>(readText);

        oneHandGestures = gestures[0];
        twoHandGestures = gestures[1];

        for (int i = 0; i < oneHandGestures.Count; i++)
            oneHandGestures[i].DuplicateHand();
    }

    //Define a new gesture to be put in the json file
    private void SaveGesture(Gesture g, bool twoHanded)
    {
        //Read what's already in the json file
        string readText = System.IO.File.ReadAllText(GESTURE_PATH);
        List<List<Gesture>> gestures = JsonConvert.DeserializeObject<List<List<Gesture>>>(readText);

        int h = twoHanded ? 1 : 0;
        bool found = false;

        //Look for index of gesture with the same spell type
        for (int i = 0; i < gestures[h].Count; i++)
        {
            //If found, replace it
            if (gestures[h][i].GetSpell() == g.GetSpell())
            {
                gestures[h][i] = g;
                found = true;
                break;
            }
        }

        //If no match was found add the gesture normally
        if (!found)
            gestures[h].Add(g);

        //Serialize the gestures into json format
        string writeText = JsonConvert.SerializeObject(gestures, Formatting.Indented);

        //Write the json formatted text to the file
        System.IO.File.WriteAllText(GESTURE_PATH, writeText);

        LoadJson();
    }

    private void ClearJSON()
    {
        List<List<Gesture>> gestures = new List<List<Gesture>>();
        gestures.Add(new List<Gesture>());
        gestures.Add(new List<Gesture>());

        string text = JsonConvert.SerializeObject(gestures, Formatting.Indented);

        System.IO.File.WriteAllText(GESTURE_PATH, text);

        Debug.Log("JSON File Cleared");
    }

    //Returns the index of the best match in validGestures or -1 if none are close enough
    private SpellHandler.SpellType FindBestMatch(Gesture inputGesture)
    {
        SpellHandler.SpellType bestMatch = SpellHandler.SpellType.NONE;
        float matchDiff = 0;
        
        Gesture.GestureHand hands = inputGesture.GetCurHands();

        if((hands == Gesture.GestureHand.BOTH && (inputGesture.NumNodes(Gesture.GestureHand.LEFT) == 1 || inputGesture.NumNodes(Gesture.GestureHand.RIGHT) == 1))
            || (hands != Gesture.GestureHand.BOTH && inputGesture.NumNodes(hands) == 1))
        {
            return bestMatch;
        }
        
        if (hands == Gesture.GestureHand.BOTH)
        {
            Debug.Log("both hands involved");

            //If there's only 1 node just stop
            if (inputGesture.NumNodes(Gesture.GestureHand.LEFT) < 2 || inputGesture.NumNodes(Gesture.GestureHand.RIGHT) < 2)
                return bestMatch;
            
            foreach (Gesture g in twoHandGestures)
            {
                float diff = g.AverageDifference(inputGesture, Gesture.GestureHand.LEFT)
                    + g.AverageDifference(inputGesture, Gesture.GestureHand.RIGHT) / 2;
                
                //Debug.Log("diff = " + diff);
                
                if ((bestMatch == SpellHandler.SpellType.NONE && diff < accuracyPerSample) || diff < matchDiff)
                {
                    bestMatch = g.GetSpell();
                    matchDiff = diff;
                }
            }
        }
        else
        {
            //If there's only 1 node just stop
            if (inputGesture.NumNodes(hands) < 2)
                return bestMatch;
            
            foreach (Gesture g in oneHandGestures)
            {
                float diff = g.AverageDifference(inputGesture, hands);
                
                //Debug.Log("diff = " + diff);

                if ((bestMatch == SpellHandler.SpellType.NONE && diff < accuracyPerSample) || diff < matchDiff)
                {
                    bestMatch = g.GetSpell();
                    matchDiff = diff;
                }
            }
        }

        return bestMatch;
    }

    public void SetCalibrationMenu(GestureCalibrationMenu menu)
    {
        calibrationMenu = menu;
    }

    public void StartCalibrating()
    {
        calibrating = true;
    }

    public void ResetCalibration()
    {
        calibrating = false;

        calibratedGestures = new List<Gesture>();

        //RESET ANYTHING REGARDING CURRENT GESTURE OR SAMPLES JUST IN CASE
    }

    public void ChangeCalibrationType(SpellHandler.SpellType type)
    {
        calibrationType = type;
    }

    //private void TestStuff()
    //{
    //    //register a test motion
    //    Gesture testGesture = new Gesture(Gesture.GestureHand.RIGHT);
    //    testGesture.AddStartPoint(Gesture.GestureHand.RIGHT, new Vector3(0, 0, 0));

    //    testGesture.AddNode(new Vector3(0, 0, 1), Gesture.GestureHand.RIGHT);
    //    testGesture.AddNode(new Vector3(0, 0, 2), Gesture.GestureHand.RIGHT);
    //    testGesture.AddNode(new Vector3(0, 0, 3), Gesture.GestureHand.RIGHT);

    //    //record a motion that's turned 90 degrees
    //    Gesture checkGesture = new Gesture(Gesture.GestureHand.RIGHT);
    //    checkGesture.AddStartPoint(Gesture.GestureHand.RIGHT, new Vector3(1, 0, 0));

    //    checkGesture.AddNode(new Vector3(2, 0, 0), Gesture.GestureHand.RIGHT);
    //    checkGesture.AddNode(new Vector3(3, 0, 0), Gesture.GestureHand.RIGHT);
    //    checkGesture.AddNode(new Vector3(4, 0, 0), Gesture.GestureHand.RIGHT);

    //    //calculate the difference
    //    float diff = testGesture.AverageDifference(checkGesture, lengthAccuracyFactor, Gesture.GestureHand.RIGHT);

    //    Debug.Log("avg diff = " + diff);
    //}

    //private void TestThing(Vector3 toRotate, float rotation)
    //{
    //    float theta;

    //    theta = rotation * Mathf.Deg2Rad;

    //    //Rotate the offset as if the starting rotation of the gesture was zero
    //    float x = toRotate.x * Mathf.Cos(theta) - toRotate.z * Mathf.Sin(theta);
    //    float z = toRotate.x * Mathf.Sin(theta) + toRotate.z * Mathf.Cos(theta);


    //    Debug.Log(toRotate);
    //    Debug.Log(rotation);
    //    Debug.Log("x = " + x);
    //    Debug.Log("z = " + z);
    //}
}
