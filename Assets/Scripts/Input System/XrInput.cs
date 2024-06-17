using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class XrInput : MonoBehaviour
{
    // Start is called before the first frame update
    [FormerlySerializedAs("Debug")] public  bool DEBUG;
    [Header("Left Controller")] 
    public InputActionProperty LeftTrigger;
    public InputActionProperty LeftGrip;
    public InputActionProperty LeftPrimary;
    public InputActionProperty LeftSecondary;
    public InputActionProperty LeftJoyStick;
    
    [Space]
    [Header("Right Controller")] 
    public InputActionProperty RightTrigger;
    public InputActionProperty RightGrip;
    public InputActionProperty RightPrimary;
    public InputActionProperty RightSecondary;
    public InputActionProperty RightJoyStick;


    public static InputAction Left_Trigger;
    public static InputAction Left_Grip;
    public static InputAction Left_Primary;
    public static InputAction Left_Secondary;
    public static InputAction Left_Joy_Stick;
    
    public static InputAction Right_Trigger;
    public static InputAction Right_Grip;
    public static InputAction Right_Primary;
    public static InputAction Right_Secondary;
    public static InputAction Right_Joy_Stick;


    private void OnEnable()
    {
        Left_Primary = LeftPrimary.action;
        Left_Secondary = LeftSecondary.action;
        Left_Grip = LeftGrip.action;
        Left_Trigger = LeftTrigger.action;
        Left_Joy_Stick = LeftJoyStick.action;
        
        Right_Primary = RightPrimary.action;
        Right_Secondary = RightSecondary.action;
        Right_Grip = RightGrip.action;
        Right_Trigger = RightTrigger.action;
        Right_Joy_Stick = RightJoyStick.action;
        
        
    }


    // Update is called once per frame
    void Update()
    {
        if (DEBUG)
        {
            
            //Left Controls Debugger
            
            if (LeftTrigger.action.IsPressed())
            {
                Debug.Log("Left Trigger Pressed");
            }

            if (LeftPrimary.action.IsPressed())
            {
                Debug.Log("left Primary Pressed");
            }

            if (LeftSecondary.action.IsPressed())
            {
                Debug.Log("left Secondary Pressed");
            }

            if (LeftJoyStick.action.WasPerformedThisFrame())
            {
                Debug.Log($"Left Joystick in action: {LeftJoyStick.action.ReadValue<Vector2>()}");
            }

            if (LeftGrip.action.IsPressed())
            {
                Debug.Log("left Grip Pressed");
            }
            
            
            //Right Controls Debugger
            
            if (RightTrigger.action.IsPressed())
            {
                Debug.Log("Right Trigger Pressed");
            }

            if (RightPrimary.action.IsPressed())
            {
                Debug.Log("Right Primary Pressed");
            }

            if (RightSecondary.action.IsPressed())
            {
                Debug.Log("Right Secondary Pressed");
            }

            if (RightJoyStick.action.WasPerformedThisFrame())
            {
                Debug.Log($"Right Joystick in action: {RightJoyStick.action.ReadValue<Vector2>()}");
            }

            if (RightGrip.action.IsPressed())
            {
                Debug.Log("Right Grip Pressed");
            }
        }
    }
    
    
}



