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

    public float moveSpeed, runMultiplier, sampleTime, accuracyPerSample, damageSoundCD, collideSoundCD;

    public int samplesPerGesture, startingHP, numCalibrations;

    public bool showSamples, clearJSON, allowRecording, alwaysSprint;

    public SpellHandler.SpellType recordedType;

    public Material baseMaterial, rockMaterial;

    public AudioClip damageSound, collideSound;

    private GestureManager gestureManager;
    private SpellHandler spellHandler;
    private Health healthComp;
    private Vector3 velocity;
    private GameManager gameManager;
    private LineRenderer menuPointer;
    private AudioSource audioSource;
    private bool leftHandPointing, pointerEnabled;
    private CollisionChecker collisionChecker;
    private float toNextDamageSound, toNextCollideSound;

    void Awake()
    {
        gestureManager = new GestureManager(sampleTime, accuracyPerSample, samplesPerGesture, clearJSON, numCalibrations);
    }

    void Start()
    {
        leftHand = GetComponentInChildren<PlayerHand>();
        spellHandler = GetComponent<SpellHandler>();
        gameManager = GetComponent<GameManager>();
        menuPointer = GetComponent<LineRenderer>();
        audioSource = GetComponent<AudioSource>();
        collisionChecker = GetComponent<CollisionChecker>();

        healthComp = new Health(startingHP);

        audioSource.clip = damageSound;

        pointerEnabled = true;

        GesturePickTest.Test1();
        GesturePickTest.Test2();

        SampleConvertTest.Test1();
        SampleConvertTest.Test2();
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
                gestureManager.BeginGesture(this, true, true);
            }
        }
        else
        {
            if (leftHand.GetGrabbing())
            {
                leftHand.ToggleGrabbing();
                spellHandler.CastSpell(gestureManager.EndGesture(), gestureManager.CurrentHand());
            }
        }

        if (grabAction.GetAxis(SteamVR_Input_Sources.RightHand) >= 0.1f)
        {
            if (!rightHand.GetGrabbing())
            {
                rightHand.ToggleGrabbing();
                gestureManager.BeginGesture(this, false, true);
            }
        }
        else
        {
            if (rightHand.GetGrabbing())
            {
                rightHand.ToggleGrabbing();
                spellHandler.CastSpell(gestureManager.EndGesture(), gestureManager.CurrentHand());
            }
        }

        gestureManager.Update(this, Time.deltaTime, true);

        //Deal with sound cooldowns
        if (toNextDamageSound > 0)
            toNextDamageSound -= Time.deltaTime;

        if (toNextCollideSound > 0)
            toNextCollideSound -= Time.deltaTime;

        //Deal with player movement
        CalcMovement();

        //Remove the shield material if the shield is gone
        if(healthComp.ShieldBroke())
        {
            leftHand.GetComponentInChildren<SkinnedMeshRenderer>().material = baseMaterial;
            rightHand.GetComponentInChildren<SkinnedMeshRenderer>().material = baseMaterial;
        }

        leftHand.SetGems(healthComp.GetHealth() / (float)startingHP);
        rightHand.SetGems(healthComp.GetHealth() / (float)startingHP);
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
                gestureManager.BeginGesture(this, true, false);

                if (leftHandPointing)
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
                gestureManager.EndCalibratedGesture();
            }
        }

        if (grabAction.GetAxis(SteamVR_Input_Sources.RightHand) >= 0.1f)
        {
            if (!rightHand.GetGrabbing())
            {
                rightHand.ToggleGrabbing();
                gestureManager.BeginGesture(this, false, false);

                if (!leftHandPointing)
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
                gestureManager.EndCalibratedGesture();
            }
        }

        gestureManager.Update(this, Time.deltaTime, false);

        if (leftHandPointing && pointerEnabled)
        {
            PointAtMenu(leftHand, click);
        }
        else if (pointerEnabled)
        {
            PointAtMenu(rightHand, click);
        }
        else
        {
            menuPointer.enabled = false;
        }
    }

    private void PointAtMenu(PlayerHand hand, bool click)
    {
        RaycastHit hit;

        Physics.Raycast(hand.CenterPos(), hand.transform.forward, out hit, pointerLength);

        //Display the menu pointer
        Vector3 pointerEnd;

        if (hit.collider != null)
            pointerEnd = hit.point;
        else
            pointerEnd = hand.CenterPos() + (hand.transform.forward * pointerLength);

        menuPointer.enabled = true;
        menuPointer.SetPosition(0, hand.CenterPos());
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
        bool left = false;

        if (trackPos == Vector2.zero)
        {
            trackPos = moveAction.GetAxis(SteamVR_Input_Sources.LeftHand);
            left = true;
        }

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

        Move(new Vector3(xMove, 0, yMove).normalized, left);
    }

    public void Damage(float damage)
    {
        //play damage sound effect if enough time has passed since the last one
        if (toNextDamageSound <= 0)
        {
            audioSource.clip = damageSound;

            audioSource.Play();
            toNextDamageSound = damageSoundCD;
        }

        //If damage killed the player
        if (!healthComp.TakeDamage(damage))
        {
            Debug.Log("player died");
            gameManager.EndGame();
        }

        leftHand.SetGems(healthComp.GetHealth() / (float)startingHP);
    }

    public void ResetStuff()
    {
        healthComp = new Health(startingHP);
        leftHand.GetComponentInChildren<SkinnedMeshRenderer>().material = baseMaterial;
        rightHand.GetComponentInChildren<SkinnedMeshRenderer>().material = baseMaterial;

        leftHand.RemoveElement();
        rightHand.RemoveElement();

        leftHand.ResetGems();
        rightHand.ResetGems();

        toNextDamageSound = 0;
        toNextCollideSound = 0;
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

        leftHand.SetElement(leftElement);
        rightHand.SetElement(rightElement);
    }

    public void ToggleMenuPointer(bool enabled)
    {
        menuPointer.enabled = enabled;
    }

    public Vector3 GetPos()
    {
        return playerHead.transform.position;
    }

    public void AddShield(int shieldHp, Material shieldMaterial)
    {
        healthComp.Shield(shieldHp);

        leftHand.GetComponentInChildren<SkinnedMeshRenderer>().material = shieldMaterial;
        rightHand.GetComponentInChildren<SkinnedMeshRenderer>().material = shieldMaterial;
    }

    public GestureManager GetGestureManager()
    {
        return gestureManager;
    }

    public void SetPointerEnabled(bool pointerEnabled)
    {
        this.pointerEnabled = pointerEnabled;
    }

    private void Move(Vector3 moveDir, bool left)
    {
        float runMult = 1.0f;

        if (alwaysSprint || (!left && runAction.GetState(SteamVR_Input_Sources.RightHand)
            || left && runAction.GetState(SteamVR_Input_Sources.LeftHand)))
            runMult = runMultiplier;

        velocity = moveDir * moveSpeed * runMult;

        if (!collisionChecker.IsColliding(transform.position + velocity * Time.deltaTime, true))
            transform.position += velocity * Time.deltaTime;
        else
            Collide();
    }

    private void Collide()
    {
        if (toNextCollideSound <= 0)
        {
            audioSource.clip = collideSound;

            audioSource.Play();
            toNextCollideSound = collideSoundCD;
        }
    }
}