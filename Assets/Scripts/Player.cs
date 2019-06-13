using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Player : MonoBehaviour
{
    //Action Types
    public SteamVR_Action_Single grabAction;
    public SteamVR_Action_Vector2 moveAction;
    public SteamVR_Action_Boolean recordAction;

    public PlayerHand leftHand, rightHand;
    public GameObject playerHead;

    //Temporary visualization of the gesture
    public GameObject sample;

    public SpellHandler.SpellElement spellElement;

    public float moveSpeed, sampleTime, accuracyPerSample;

    public int samplesPerGesture;

    public bool showSamples, clearJSON;

    public SpellHandler.SpellType recordedType;

    private GestureManager gestureManager;
    private SpellHandler spellHandler;

    private void Awake()
    {
        
    }

    void Start()
    {
        leftHand = GetComponentInChildren<PlayerHand>();

        gestureManager = new GestureManager(sampleTime, accuracyPerSample, samplesPerGesture, clearJSON);

        spellHandler = new SpellHandler(spellElement);
    }

    void Update()
    {
        if (grabAction.GetAxis(SteamVR_Input_Sources.LeftHand) >= 0.1f)
        {
            if (!leftHand.GetGrabbing())
            {
                leftHand.ToggleGrabbing();
                gestureManager.BeginGesture(this, true);
            }
        }
        else
        {
            if (leftHand.GetGrabbing())
            {
                leftHand.ToggleGrabbing();
                spellHandler.CastSpell(gestureManager.EndGesture(this));
            }
        }

        if (grabAction.GetAxis(SteamVR_Input_Sources.RightHand) >= 0.1f)
        {
            if (!rightHand.GetGrabbing())
            {
                rightHand.ToggleGrabbing();
                gestureManager.BeginGesture(this, false);
            }
        }
        else
        {
            if (rightHand.GetGrabbing())
            {
                rightHand.ToggleGrabbing();
                spellHandler.CastSpell(gestureManager.EndGesture(this));
            }
        }

        gestureManager.Update(this, Time.deltaTime);

        if (recordAction.GetStateDown(SteamVR_Input_Sources.RightHand))
            gestureManager.SaveLastGesture(this, recordedType);

        //Deal with player movement
        CalcMovement();
    }

    private void CalcMovement()
    {
        Vector2 trackPos = moveAction.GetAxis(SteamVR_Input_Sources.RightHand);
        float playerRot = playerHead.transform.eulerAngles.y * Mathf.Deg2Rad; //in radians

        if (trackPos.x == 0.0f && trackPos.y == 0.0f)
            return;

        float h, trackRot, theta, xMove, yMove;

        //length from middle of trackpad to touched position
        h = Mathf.Sqrt(trackPos.x * trackPos.x + trackPos.y * trackPos.y);

        //rotation of touched position relative to up on trackpad
        trackRot = Mathf.Acos(trackPos.y / h);

        //corrects for cosine only going up to 180
        if (trackPos.x < 0)
            trackRot = 2.0f * Mathf.PI - trackRot;

        //angle of the direction in which to actually move
        theta = playerRot + trackRot;

        if (theta >= 2.0f * Mathf.PI)
            theta -= 2.0f * Mathf.PI;

        //theta must be normalized to be used in the trigonometric functions
        //this also determines which quadrant and thus the sign of the x and y
        if (theta > 3.0f * Mathf.PI / 2.0f)
        {
            theta = 2.0f * Mathf.PI - theta;
            xMove = -1;
            yMove = 1;
        }
        else if (theta > Mathf.PI)
        {
            theta = theta - Mathf.PI;
            xMove = -1;
            yMove = -1;
        }
        else if (theta > Mathf.PI / 2.0f)
        {
            theta = Mathf.PI - theta;
            xMove = 1;
            yMove = -1;
        }
        else
        {
            xMove = 1;
            yMove = 1;
        }

        xMove *= h * Mathf.Sin(theta);
        yMove *= h * Mathf.Cos(theta);

        transform.position = transform.position + new Vector3(xMove * moveSpeed, 0, yMove * moveSpeed);
    }

    public void CreateSample(Vector3 pos)
    {
        if (!showSamples)
            return;

        GameObject instNode = Instantiate(sample);
        instNode.transform.position = playerHead.transform.position + pos;
    }
}
