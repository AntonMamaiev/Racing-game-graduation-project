using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrCarController : MonoBehaviour
{
    public scrWheel[] wheels;

    [Header("Car Specs")]
    public float wheelBase;
    public float rearTrack;
    public float TurnRadius;

    [Header("Inputs")]
    public float steerInput;

    private float ackerAngleWheelRight;
    private float ackerAngleWheelLeft;

    void Update()
    {
        steerInput = Input.GetAxis("Horizontal");
        if (steerInput > 0) // Поворот у право
        {
            ackerAngleWheelRight = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (TurnRadius - (rearTrack / 2))) * steerInput;
            ackerAngleWheelLeft  = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (TurnRadius + (rearTrack / 2))) * steerInput;
        }

        else if (steerInput < 0) // Поворот у ліво
        {
            ackerAngleWheelRight = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (TurnRadius + (rearTrack / 2))) * steerInput;
            ackerAngleWheelLeft  = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (TurnRadius - (rearTrack / 2))) * steerInput;
        }

        else
        {
            ackerAngleWheelLeft = 0;
            ackerAngleWheelRight = 0;
        }

        foreach (scrWheel w in wheels)
        {
            if (w.FR)
                w.steerAngle = ackerAngleWheelRight;
            if (w.FL)
                w.steerAngle = ackerAngleWheelLeft;
        }
    }
}
