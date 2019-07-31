using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class GestureManager
{
    public static readonly string GESTURE_PATH = @"Data/gestures.json";

    private List<Gesture> oneHandGestures, twoHandGestures;
    private Gesture currentGesture;
    private float sampleTime, lNextSample, rNextSample, accuracyPerSample, lengthAccuracyFactor;
    private int samplesPerGesture;

    // Start is called before the first frame update
    public GestureManager(float sampleTime, float accuracyPerSample, float lengthAccuracyFactor, int samplesPerGesture, bool clearJSON)
    {
        oneHandGestures = new List<Gesture>();
        twoHandGestures = new List<Gesture>();
        this.sampleTime = sampleTime;

        //Defines how close each sample must be on average to find a match
        this.accuracyPerSample = accuracyPerSample;

        //The default number of samples to convert to when comparing gestures
        this.samplesPerGesture = samplesPerGesture;

        //The amount that gesture length is considered when comparing gestures
        this.lengthAccuracyFactor = lengthAccuracyFactor;

        lNextSample = -1;
        rNextSample = -1;

        if(clearJSON)
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
            currentGesture.AddStartPoint(Gesture.GestureHand.LEFT, player.leftHand.transform.position - player.playerHead.transform.position);

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
            currentGesture.AddStartPoint(Gesture.GestureHand.RIGHT, player.rightHand.transform.position - player.playerHead.transform.position);

            //Set time until the next sample should be recorded
            rNextSample = sampleTime;
        }
    }

    public SpellHandler.SpellType EndGesture(Player player)
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

    public void SaveLastGesture(Player player, SpellHandler.SpellType type)
    {
        if (currentGesture == null)
            return;

        //Show the visual representation of what was recorded
        ShowSamples(player);

        currentGesture.SetSpell(type);

        SaveGesture(currentGesture);

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
            //If there's only 1 node just stop
            if (inputGesture.NumNodes(Gesture.GestureHand.LEFT) < 2 || inputGesture.NumNodes(Gesture.GestureHand.RIGHT) < 2)
                return bestMatch;
            
            foreach (Gesture g in twoHandGestures)
            {
                float diff = g.AverageDifference(inputGesture, lengthAccuracyFactor, Gesture.GestureHand.LEFT)
                    + g.AverageDifference(inputGesture, lengthAccuracyFactor, Gesture.GestureHand.RIGHT) / 2;
                
                Debug.Log("diff = " + diff);
                
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
                float diff = g.AverageDifference(inputGesture, lengthAccuracyFactor, hands);
                
                Debug.Log("diff = " + diff);

                if ((bestMatch == SpellHandler.SpellType.NONE && diff < accuracyPerSample) || diff < matchDiff)
                {
                    bestMatch = g.GetSpell();
                    matchDiff = diff;
                }
            }
        }

        return bestMatch;
    }
}
