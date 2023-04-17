using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class CarPhysics : MonoBehaviour
{

    private Rigidbody carRigidBody;

    public GameObject Tire;
    public GameObject carTransfort;
    public AnimationCurve speedCurve;
    public GameObject cloneWheel;
    public float airspeed = 12.5f;

    [Header("Suspension")]
    public float RestDistance; //Довжина пружини у якій вона хоче перебувати
    public float Strength; //Наскільки жорстка пружина
    public float Damper; //Сила дампінгу

    [Header("Steering")]
    public bool FR;
    public bool FL;
    public bool RR;
    public bool RL;
    public float tireGripFactor;
    public float tireMass;
    public float wheelBase, TurnRadius, rearTrack;

    [Header("Acceleration")]
    public float Speed;
    public float carTopSpeed;
    public bool IsBrakingWheel;

    [Header("Tire Visualization")]
    public float tireSize;
    public float maxRestPos;
    public float minRestPos;

    // All variables
    private float accelInput;
    private float WheelRight, WheelLeft;
    private float wheelAngle, steerAngle, steerInput;
    private float offset;
    private float vel;
    private float force;
    private float steeringVel;
    private float desiredVelChange;
    private float desiredAccel;
    private float pushValue;
    private float carSpeed;
    private float normilizedSpeed;
    private float availableTorque;
    private float airInput;
    private bool rayDidHit;
    private bool BreakInput;


    private Vector3 tireBasePos;
    private Vector3 rotationAmount;
    private Vector3 rVelo;
    private Vector3 basePos;
    private Vector3 wheelVelocity;
    private Vector3 steeringDir;
    private Vector3 springDir;
    private Vector3 tireWorldVel; 
    private Vector3 wheelsSuspensionPos;
    private Vector3 accelDir;
    private Vector3 tr;
    private Quaternion baseRot;


    private Ray ray;
    private RaycastHit rayHit;



    void Start()
    {
        carRigidBody = transform.root.GetComponent<Rigidbody>();
        tireBasePos = Tire.transform.localPosition;
        baseRot = carTransfort.transform.rotation;

        accelInput = Input.GetAxis("Vertical");
        steerInput = Input.GetAxis("Horizontal");
        airInput = Input.GetAxis("AirUpDown");
    }


    void Update()
    {
        rVelo = carTransfort.transform.InverseTransformDirection(new Vector3(carRigidBody.velocity.x, 0, carRigidBody.velocity.z));
        rotationAmount = Vector3.right * (rVelo.z * 1.6f * Time.deltaTime * Mathf.Deg2Rad) * 8000f;
        cloneWheel.transform.Rotate(rotationAmount);

        if (FR)
            steerAngle = WheelRight;
        if (FL)
            steerAngle = WheelLeft;
    }

    void FixedUpdate()
    {
        ray.origin = transform.position;
        ray.direction = -transform.up;
        rayDidHit = Physics.Raycast(ray, out rayHit, RestDistance + 0.05f);

        if (rayDidHit)
        {
            ////////////////////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////// Управління підвіскою //////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////////////////////
            springDir = Tire.transform.up;

            tireWorldVel = carRigidBody.GetPointVelocity(Tire.transform.position);

            offset = RestDistance - rayHit.distance;

            vel = Vector3.Dot(springDir, tireWorldVel);

            force = (offset * Strength) - (vel * Damper);

            wheelVelocity = transform.InverseTransformDirection(carRigidBody.GetPointVelocity(rayHit.point));


            ////////////////////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////// Управління рулем //////////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////////////////////

            steerInput = Input.GetAxis("Horizontal");

            if (steerInput > 0) // Поворот у право
            {
                WheelRight = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (TurnRadius - (rearTrack / 2))) * steerInput;
                WheelLeft = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (TurnRadius + (rearTrack / 2))) * steerInput;
            }

            else if (steerInput < 0) // Поворот у ліво
            {
                WheelRight = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (TurnRadius + (rearTrack / 2))) * steerInput;
                WheelLeft = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (TurnRadius - (rearTrack / 2))) * steerInput;
            }

            else
            {
                WheelLeft = 0;
                WheelRight = 0;
            }


            wheelAngle = Mathf.Lerp(wheelAngle, steerAngle, 4 /*Час повороту*/ * Time.deltaTime);

            Tire.transform.localRotation = Quaternion.Euler(Vector3.up * wheelAngle);

            steeringDir = Tire.transform.right;

            steeringVel = Vector3.Dot(steeringDir, tireWorldVel);

            desiredVelChange = -steeringVel * tireGripFactor;

            desiredAccel = desiredVelChange / Time.fixedDeltaTime;

            carRigidBody.AddForceAtPosition((springDir * force) + (steeringDir * tireMass * desiredAccel), Tire.transform.position);


            ////////////////////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////// Розгін та тормозіння //////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////////////////////

            accelInput = Input.GetAxis("Vertical");
            BreakInput = Input.GetButton("Brake");



            accelDir = Tire.transform.forward * Speed;

            if (accelInput > 0.0f)
            {
                carSpeed = Vector3.Dot(carTransfort.transform.forward, carRigidBody.velocity);

                normilizedSpeed = Mathf.Clamp01(Mathf.Abs(carSpeed) / carTopSpeed);

                availableTorque = speedCurve.Evaluate(normilizedSpeed) * accelInput;

                carRigidBody.AddForceAtPosition(accelDir * availableTorque, Tire.transform.position);

                if (carRigidBody.velocity.magnitude > carTopSpeed)
                {
                    carRigidBody.velocity = carRigidBody.velocity.normalized * carTopSpeed;
                }

                rVelo = carTransfort.transform.InverseTransformDirection(new Vector3(carRigidBody.velocity.x, 0, carRigidBody.velocity.z));
                rotationAmount = Vector3.right * (rVelo.z * 1.6f * Time.deltaTime * Mathf.Deg2Rad) * 8000f;
                cloneWheel.transform.Rotate(rotationAmount);
            }
            else if (accelInput < 0.0f)
            {
                carRigidBody.AddForceAtPosition(accelDir * accelInput, Tire.transform.position);

                if (carRigidBody.velocity.magnitude > (carTopSpeed / 1.5f))
                    carRigidBody.velocity = carRigidBody.velocity.normalized * (carTopSpeed / 1.5f);

            }
            else
                carRigidBody.velocity /= 1.0015f;

            if (BreakInput && IsBrakingWheel)
                carRigidBody.velocity /= 1.0095f;



            ////////////////////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////// Візуалізація //////////////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////////////////////

            pushValue = -(rayHit.distance - tireSize);
            basePos = Tire.transform.position;
            wheelsSuspensionPos = new Vector3(basePos.x, transform.position.y + pushValue, basePos.z);

            Tire.transform.position = Vector3.MoveTowards(basePos, wheelsSuspensionPos, 0.02f);
            Tire.transform.localPosition = new Vector3(tireBasePos.x, Tire.transform.localPosition.y, tireBasePos.z);

            if (Tire.transform.localPosition.y < minRestPos) Tire.transform.localPosition = new Vector3(tireBasePos.x, minRestPos, tireBasePos.z);
            if (Tire.transform.localPosition.y > maxRestPos) Tire.transform.localPosition = new Vector3(tireBasePos.x, maxRestPos, tireBasePos.z);

            cloneWheel.transform.position = Tire.transform.position;


        }
        else
        {
            Tire.transform.localPosition = Vector3.MoveTowards(Tire.transform.localPosition, tireBasePos, 0.02f);


            if (Input.GetButtonDown("Reset") && carRigidBody.velocity.magnitude < 2f)
            {
                Debug.Log("True");
                carTransfort.transform.rotation = Quaternion.Euler(carTransfort.transform.rotation.eulerAngles.x, carTransfort.transform.rotation.eulerAngles.y, 0);
                carTransfort.transform.position = new Vector3(carTransfort.transform.position.x, carTransfort.transform.position.y+1, carTransfort.transform.position.z);
            }
        }

        tr = carTransfort.transform.position;


        

    }
}
