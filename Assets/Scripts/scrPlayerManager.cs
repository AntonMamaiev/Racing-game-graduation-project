using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrPlayerManager : MonoBehaviour
{
    //public GameObject carTransfort;
    //private Rigidbody carRigidBody;
    //private Vector3 tr;
    //private float airInput;
    //private float steerInput;
    //public float airspeed;
    //private float airspeed2;

    //private RaycastHit hit;
    //private Vector3 dir = -Vector3.up;
    //public float raylength;

    //void Start()
    //{
    //    carRigidBody = transform.root.GetComponent<Rigidbody>();
    //    airspeed2 = airspeed / 10;
    //}
    //void FixedUpdate()
    //{
    //    ////////////////////////////////////////////////////////////////////////////////////////////
    //    //////////////////////////////////// Поворот у повітрі /////////////////////////////////////
    //    ////////////////////////////////////////////////////////////////////////////////////////////


    //    Debug.DrawRay(transform.position, dir * raylength, Color.red);

    //    if (Physics.Raycast(transform.position, dir * raylength, out hit))
    //    {
    //        Debug.DrawRay(transform.position, dir * raylength, Color.green);
    //        steerInput = Input.GetAxis("Horizontal");
    //        if (steerInput > 0) // Поворот у право
    //        {
    //            carTransfort.transform.Rotate(new Vector3(0, 0, -airspeed * -steerInput));
    //            airspeed2 = -airspeed / 10;
    //        }
    //        else if (steerInput < 0) // Поворот у ліво
    //        {
    //            carTransfort.transform.Rotate(new Vector3(0, 0, airspeed * steerInput));
    //            airspeed2 = airspeed / 10;
    //        }
    //        else
    //        {
    //            carTransfort.transform.Rotate(new Vector3(0, 0, airspeed2));
    //        }

    //        //airInput = Input.GetAxis("AirUpDown");
    //        //if (airInput > 0) // Газ
    //        //{
    //        //    carTransfort.transform.Rotate(new Vector3(airspeed, 0, 0));
    //        //}
    //        //else if (airInput < 0) // Тормоз
    //        //{
    //        //    carTransfort.transform.Rotate(new Vector3(-airspeed, 0, 0));
    //        //}
    //        //else
    //        //{
    //        //    carTransfort.transform.Rotate(new Vector3(airspeed2, 0, 0));
    //        //}
            
    //    }
    //}
}
