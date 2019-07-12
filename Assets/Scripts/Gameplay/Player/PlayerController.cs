using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

//This class should yield with the player input. It returns the conclusions of the player input (jump, movement direction, etc), not the keys.
//Input for debug mode should be dealt in DebugMode class. Input for menus should be dealt in LevelManager class.
public class PlayerController : MonoBehaviour {
    Player m_player;
    GameManager gameManager;
    public Joystick joystickMove;
    public Joystick joystickCam;

    // Use this for initialization
    void Start() {
        m_player = GetComponent<Player>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        //joystickMove = GameObject.Find("JoystickMove").GetComponent<Joystick> ();
        //joystickCam = GameObject.Find("JoystickCam").GetComponent<Joystick>();
    }

    // Method to be called in order to deal with input from player
    public void GetDirections(ref float axisHorizontal, ref float axisVertical, ref float camHorizontal, ref float camVertical) {
        /*
        axisHorizontal = joystickMove.Horizontal;
        axisVertical = joystickMove.Vertical;

        camHorizontal = joystickCam.Horizontal;
        camVertical = joystickCam.Vertical;*/
        axisHorizontal = Input.GetAxis("Horizontal");
        axisVertical = Input.GetAxis("Vertical");

        camHorizontal = Input.GetAxis("Mouse X");
        camVertical = Input.GetAxis("Mouse Y");
    }

    public void GetButtons(ref bool jump, ref bool pickObject, ref bool aimObjects, ref bool returnCam) {
        if (Input.GetButtonDown("Jump")) {
            jump = true;
        }

        if (Input.GetButtonDown("Change  Camera")) {
            gameManager.ChangeCamera();
        }
        /* if (CrossPlatformInputManager.GetButtonDown("Jump")) {
             print("Salto2");
             jump = true;
         }*/
        /*if (CrossPlatformInputManager.GetButtonDown("PickObjects"))
            pickObject = true;

        if (CrossPlatformInputManager.GetButtonDown("AimObjects"))
            aimObjects = true;
        if (CrossPlatformInputManager.GetButtonDown("ReturnCam"))
            returnCam = true;*/

    }
}