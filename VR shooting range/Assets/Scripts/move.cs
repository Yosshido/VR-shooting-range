﻿using UnityEngine;
using System.Collections;
using Valve.VR;

public class move : MonoBehaviour
{
    private float timer = 0.0f; 
    public float bobbingSpeed = 0.18f;
    public float bobbingAmount = 0.2f; // For head_bob() - See function
    public float midpoint = 2.0f;

    SteamVR_TrackedObject trackedObj; //The tracked object
    SteamVR_Controller.Device controller; //The controller
    public GameObject cameraRig; //The Camera Rig
    public GameObject look; //The object considered "forward"
    public float speed = 2.0f;

    Vector2 touchpad; //Where the user's finger is on the touchpad
    Vector3 currentLocation; //The current location of the camera rig
    Vector3 nextLocation; //Where it should move to


    // First event function
    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>(); //get and set required component
    }

    void head_bob() // Bob head based on http://answers.unity3d.com/questions/283086/headbobber-script-in-c.html
    {
        Vector3 cSharpConversion = cameraRig.transform.localPosition;
        float waveslice = 0.0f;

        Debug.Log("BOB!");
        waveslice = Mathf.Sin(timer);
        timer = timer + bobbingSpeed;

        if (timer > Mathf.PI * 2)
        {
            timer = timer - (Mathf.PI * 2);
        }
        
        if (waveslice != 0)
        {
            float translateChange = waveslice * bobbingAmount;
            float totalAxes = Mathf.Abs(cSharpConversion[0]) + Mathf.Abs(cSharpConversion[2]);
            totalAxes = Mathf.Clamp(totalAxes, 0.0f, 1.0f);
            translateChange = totalAxes * translateChange;
            cSharpConversion.y = midpoint + translateChange;
        }

        else
        {
            cSharpConversion.y = midpoint;
        }

        cameraRig.transform.localPosition = cSharpConversion;
    }

    // Called on a physics step - FixedUpdate timestep changed to 90fps (1/90) Change to update flags
    void FixedUpdate()
    {
        controller = SteamVR_Controller.Input((int)trackedObj.index); //Create Controller variable and use it to store inputs
        currentLocation = cameraRig.transform.position; // Get position of rig

        if (controller.GetPress(SteamVR_Controller.ButtonMask.Touchpad)) //On touchpad top press
        {
            Debug.Log("Touchpad top was clicked to move"); //Log action 
            touchpad = controller.GetAxis(EVRButtonId.k_EButton_SteamVR_Touchpad);

            if (touchpad.y > 0.15f)
            {
                Debug.Log("forward"); //Log action 
                nextLocation = currentLocation; //Next location begins at current
                nextLocation += look.transform.forward * speed * Time.deltaTime; // moves on all axes
                nextLocation[1] = 0; //revert Y
                cameraRig.transform.position = nextLocation; //move rig
            }

            else
            {
                Debug.Log("backward");
                nextLocation = currentLocation; //Next location begins at current
                nextLocation -= look.transform.forward * speed * Time.deltaTime; // moves on all axes
                nextLocation[1] = 0; //revert Y
                cameraRig.transform.position = nextLocation; //move rig
            }

        }

        else
            timer = 0;

        head_bob(); //animate

    }


}