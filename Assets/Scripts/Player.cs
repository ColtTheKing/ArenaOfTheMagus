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
    public SteamVR_Action_Boolean runAction;

    public PlayerHand leftHand, rightHand;
    public GameObject playerHead;

    public float pointerLength;

    //Temporary visualization of the gesture
    public GameObject sample;

    public float moveSpeed, runMultiplier, sampleTime, accuracyPerSample;

    public int samplesPerGesture, startingHP;

    public bool showSamples, clearJSON;

    public SpellHandler.SpellType recordedType;

    private GestureManager gestureManager;
    private SpellHandler spellHandler;
    private Health healthComp;
    private Vector3 velocity;
    private GameManager gameManager;
    private LineRenderer menuPointer;
    private bool leftHandPointing;

    void Start()
    {
        leftHand = GetComponentInChildren<PlayerHand>();

        gestureManager = new GestureManager(sampleTime, accuracyPerSample, samplesPerGesture, clearJSON);

        spellHandler = GetComponent<SpellHandler>();

        gameManager = GetComponent<GameManager>();

        healthComp = new Health(startingHP);

        menuPointer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (gameManager.InGame())
            UpdateGame();
        else
            UpdateMenu();
    }

    private void UpdateGame()
    {
        //Deal with gesture starting and stopping and hand grabbing
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
                spellHandler.CastSpell(gestureManager.EndGesture(this), gestureManager.CurrentHand());
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
                spellHandler.CastSpell(gestureManager.EndGesture(this), gestureManager.CurrentHand());
                Application.Quit();
            }
        }

        gestureManager.Update(this, Time.deltaTime);

        //Deal with saving gestures WILL BE REMOVED LATER
        if (recordAction.GetStateDown(SteamVR_Input_Sources.LeftHand) || recordAction.GetStateDown(SteamVR_Input_Sources.RightHand))
            gestureManager.SaveLastGesture(this, recordedType);

        //Deal with player movement
        CalcMovement();
    }

    private void UpdateMenu()
    {
        bool click = false;

        //Deal with clicking and hand grabbing
        if (grabAction.GetAxis(SteamVR_Input_Sources.LeftHand) >= 0.1f)
        {
            if (!leftHand.GetGrabbing())
            {
                leftHand.ToggleGrabbing();

                if(leftHandPointing)
                    click = true;
                else
                    leftHandPointing = true;
            }
        }
        else
        {
            if (leftHand.GetGrabbing())
            {
                leftHand.ToggleGrabbing();
            }
        }

        if (grabAction.GetAxis(SteamVR_Input_Sources.RightHand) >= 0.1f)
        {
            if (!rightHand.GetGrabbing())
            {
                rightHand.ToggleGrabbing();

                if(!leftHandPointing)
                    click = true;
                else
                    leftHandPointing = false;
            }
        }
        else
        {
            if (rightHand.GetGrabbing())
            {
                rightHand.ToggleGrabbing();
            }
        }

        if (leftHandPointing)
        {
            PointAtMenu(leftHand, click);
        }
        else
        {
            PointAtMenu(rightHand, click);
        }
    }

    private void PointAtMenu(PlayerHand hand, bool click)
    {
        RaycastHit hit;

        Physics.Raycast(hand.transform.position, hand.transform.forward, out hit, pointerLength);

        //Display the menu pointer
        Vector3 pointerEnd;

        if (hit.collider != null)
            pointerEnd = hit.point;
        else
            pointerEnd = hand.transform.position + (hand.transform.forward * pointerLength);

        menuPointer.SetPosition(0, hand.transform.position);
        menuPointer.SetPosition(1, pointerEnd);

        //If the player pressed the trigger on a button activate the button
        if (click && hit.collider != null)
        {
            MenuButton button = hit.transform.gameObject.GetComponent<MenuButton>();

            if (button != null)
                gameManager.PressMenuButton(button.buttonId);
        }
    }

    private void CalcMovement()
    {
        Vector2 trackPos = moveAction.GetAxis(SteamVR_Input_Sources.RightHand);
        float playerRot = playerHead.transform.eulerAngles.y * Mathf.Deg2Rad; //in radians

        if (trackPos.x == 0.0f && trackPos.y == 0.0f)
            return;

        float runMult = 1.0f;
        
        if (runAction.GetState(SteamVR_Input_Sources.RightHand))
            runMult = runMultiplier;

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

        velocity = new Vector3(xMove, 0, yMove).normalized * moveSpeed * runMult;

        transform.position = transform.position + (velocity * Time.deltaTime);
    }

    public void Damage(int damage)
    {
        //maybe add damaged sound effect
        Debug.Log("ouch");

        //If damage killed the player
        if (!healthComp.TakeDamage(damage))
        {
            Debug.Log("player died");
            gameManager.EndGame();
        }
    }

    public void ResetHealth()
    {
        healthComp = new Health(startingHP);
    }

    public void CreateSample(Vector3 pos)
    {
        if (!showSamples)
            return;

        GameObject instNode = Instantiate(sample);
        instNode.transform.position = playerHead.transform.position + pos;
    }

    public Vector3 GetVelocity()
    {
        return velocity;
    }

    public void SetElements(SpellHandler.SpellElement leftElement, SpellHandler.SpellElement rightElement)
    {
        spellHandler.elementLeft = leftElement;
        spellHandler.elementRight = rightElement;
    }

    public void ToggleMenuPointer(bool enabled)
    {
        menuPointer.enabled = enabled;
    }

    public Vector3 GetPos()
    {
        return playerHead.transform.position;
    }
}
