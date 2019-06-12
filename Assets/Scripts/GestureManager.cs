using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class GestureManager
{
    public static readonly string GESTURE_PATH = @"Data/gestures.json";

    private List<Gesture> oneHandGestures, twoHandGestures;
    private Gesture currentGesture;
    private float sampleTime, lNextSample, rNextSample, accuracyPerSample;
    private int samplesPerGesture;

    // Start is called before the first frame update
    public GestureManager(float sampleTime, float accuracyPerSample, int samplesPerGesture)
    {
        oneHandGestures = new List<Gesture>();
        twoHandGestures = new List<Gesture>();
        this.sampleTime = sampleTime;

        //Defines how close each sample must be on average to find a match
        this.accuracyPerSample = accuracyPerSample;

        //The default number of samples to convert to when comparing gestures
        this.samplesPerGesture = samplesPerGesture;

        lNextSample = -1;
        rNextSample = -1;

        //Uncomment this to clear the json
        ClearJSON();

        //Populate validGestures
        LoadJson();
    }

    // Update is called once per frame
    public void Update(Player player, float deltaTime)
    {
        //If a left gesture is being recorded
        if (lNextSample >= 0)
        {
            lNextSample -= deltaTime;

            //If enough time has passed since the last node in the left gesture, create a new one
            if (lNextSample <= 0)
            {
                currentGesture.AddNode(player.leftHand.transform.position - player.playerHead.transform.position, Gesture.GestureHand.LEFT);

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
                currentGesture.AddNode(player.rightHand.transform.position - player.playerHead.transform.position, Gesture.GestureHand.RIGHT);

                rNextSample = sampleTime;
            }
        }
    }

    public void BeginGesture(Player player, bool left)
    {
        if(left)
        {
            //If this is the first hand of the gesture
            if (rNextSample == -1)
            {
                //Create a new left gesture
                currentGesture = new Gesture(player.playerHead.transform.eulerAngles.y, Gesture.GestureHand.LEFT);
            }
            else //If the gesture has already been started by the other hand
            {
                currentGesture.AddHand(Gesture.GestureHand.LEFT);
            }

            //Add the first node
            currentGesture.AddNode(player.leftHand.transform.position - player.playerHead.transform.position, Gesture.GestureHand.LEFT);

            //Set time until the next sample should be recorded
            lNextSample = sampleTime;
        }
        else
        {
            //If this is the first hand of the gesture
            if (lNextSample == -1)
            {
                //Create a new left gesture
                currentGesture = new Gesture(player.playerHead.transform.eulerAngles.y, Gesture.GestureHand.RIGHT);
            }
            else //If the gesture has already been started by the other hand
            {
                currentGesture.AddHand(Gesture.GestureHand.RIGHT);
            }

            //Add the first node
            currentGesture.AddNode(player.rightHand.transform.position - player.playerHead.transform.position, Gesture.GestureHand.RIGHT);

            //Set time until the next sample should be recorded
            rNextSample = sampleTime;
        }
    }

    public SpellHandler.SpellType EndGesture(Player player)
    {
        //Show the visual representation of what was recorded
        //ShowSamples(player);

        lNextSample = -1;
        rNextSample = -1;

        return FindBestMatch(currentGesture);
    }

    private void ShowSamples(Player player)
    {
        if (lNextSample >= 0)
        {
            for (int i = 0; i < currentGesture.NumNodes(Gesture.GestureHand.LEFT); i++)
            {
                player.CreateSample(currentGesture.NodeAt(i, Gesture.GestureHand.LEFT));
            }
        }

        if (rNextSample >= 0)
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

        //If there isn't anything in the file or something is wrong it's probably best to just clear the json
        //if(gestures.Count < 2)
        //{
        //    ClearJSON();

        //    readText = System.IO.File.ReadAllText(GESTURE_PATH);
        //    gestures = JsonConvert.DeserializeObject<List<List<Gesture>>>(readText);
        //}

        oneHandGestures = gestures[0];
        twoHandGestures = gestures[1];
    }

    //Define a new gesture to be put in the json file
    private void SaveGesture(Gesture g)
    {
        //Read what's already in the json file
        string readText = System.IO.File.ReadAllText(GESTURE_PATH);
        List<List<Gesture>> gestures = JsonConvert.DeserializeObject<List<List<Gesture>>>(readText);

        int h = g.GetCurHands() == Gesture.GestureHand.BOTH ? 1 : 0;
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

        //Add gesture to list of valid gestures (do after in case of hand duplication)
        if (g.GetCurHands() == Gesture.GestureHand.BOTH)
        {
            twoHandGestures.Add(g);
        }
        else
        {
            g.DuplicateHand(g.GetCurHands());
            oneHandGestures.Add(g);
        }
    }

    private void ClearJSON()
    {
        List<List<Gesture>> gestures = new List<List<Gesture>>();
        gestures.Add(new List<Gesture>());
        gestures.Add(new List<Gesture>());

        string text = JsonConvert.SerializeObject(gestures, Formatting.Indented);

        System.IO.File.WriteAllText(GESTURE_PATH, text);
    }

    //Returns the index of the best match in validGestures or -1 if none are close enough
    private SpellHandler.SpellType FindBestMatch(Gesture inputGesture)
    {
        SpellHandler.SpellType bestMatch = SpellHandler.SpellType.NONE;
        float matchDiff = 0;

        Gesture.GestureHand hands = inputGesture.GetCurHands();

        if (hands == Gesture.GestureHand.BOTH)
        {
            //Make sure the number of samples is correct for the left hand
            List<Vector3> leftSamples = ConvertSamples(inputGesture, Gesture.GestureHand.LEFT);

            //Make sure the number of samples is correct for the right hand
            List<Vector3> rightSamples = ConvertSamples(inputGesture, Gesture.GestureHand.RIGHT);

            foreach(Gesture g in twoHandGestures)
            {
                float diff = g.AverageDifference(leftSamples, Gesture.GestureHand.LEFT)
                    + g.AverageDifference(rightSamples, Gesture.GestureHand.RIGHT) / 2;

                if ((bestMatch == SpellHandler.SpellType.NONE && diff < accuracyPerSample) || diff < matchDiff)
                {
                    bestMatch = g.GetSpell();
                    matchDiff = diff;
                }
            }
        }
        else
        {
            //Make sure the number of samples is correct for the hand being used
            List<Vector3> samples = ConvertSamples(inputGesture, hands);

            foreach (Gesture g in oneHandGestures)
            {
                float diff = g.AverageDifference(samples, hands);

                if((bestMatch == SpellHandler.SpellType.NONE && diff < accuracyPerSample) || diff < matchDiff)
                {
                    bestMatch = g.GetSpell();
                    matchDiff = diff;
                }
            }
        }

        return bestMatch;
    }

    //Converts given samples into a number of new samples equal to samplesPerGesture
    private List<Vector3> ConvertSamples(Gesture samples, Gesture.GestureHand hand)
    {
        List<Vector3> samplesConverted = new List<Vector3>();

        //First node will be the same
        samplesConverted.Add(samples.NodeAt(0, hand));

        //The relative length of the segments between the samples
        float segment = (float)(samples.NumNodes(hand) - 1) / (samplesPerGesture - 1);

        for (int i = 1; i < samplesPerGesture - 1; i++)
        {
            float curSegment = segment * i;
            Vector3 curSample = samples.NodeAt(Mathf.FloorToInt(curSegment), hand);
            float percentAlong = curSegment % 1;

            Vector3 changeVec = samples.NodeAt(Mathf.FloorToInt(curSegment) + 1, hand) - curSample;
            changeVec *= percentAlong;

            samplesConverted.Add(curSample + changeVec);
        }

        //The last node will be the same
        samplesConverted.Add(samples.NodeAt(samples.NumNodes(hand) - 1, hand));

        return samplesConverted;
    }
}
