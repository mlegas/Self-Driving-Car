using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

[Serializable]
public class AgentCarControls
{
    public float horizontal = 0.0f;
    public float vertical = 0.0f;
}

public class CarDrivingAgent : Agent
{
    public bool stoppedAtStopSign;
    public bool stopSignReward;
    public bool stopSignPenalty;

    public float kmh;
    
    public float enforcedSpeedLimit;

    private Vector3 initialPosition;
    private Rigidbody rb;
    private bool finishCrash;
    private bool badCrash;
    private float horizontal;
    private float vertical;
    public AgentCarControls carControls;

    private MSVehicleControllerFree vehicleController;
    private int clampGear;

    private Transform leftWheel;
    private Transform rightWheel;

    private List<GameObject> raycastOrigins;
    public List<bool> raycastHits;

    public override void InitializeAgent()
    {
        stoppedAtStopSign = false;
        stopSignReward = false;
        stopSignPenalty = false;

        vehicleController = GetComponent<MSVehicleControllerFree>();

        initialPosition = gameObject.transform.position;

        leftWheel = gameObject.transform.Find("Car Components").Find("WheelMeshes").Find("_LeftFrontWheel");
        rightWheel = gameObject.transform.Find("Car Components").Find("WheelMeshes").Find("_RightFrontWheel");

        Transform raycastContainer = gameObject.transform.Find("Raycast Origins");

        raycastOrigins = new List<GameObject>();
        raycastHits = new List<bool>();

        for (int i = 0; i < raycastContainer.childCount; i++)
        {
            raycastOrigins.Add(raycastContainer.GetChild(i).gameObject);
            raycastHits.Add(false);
        }

        finishCrash = false;
        badCrash = false;
        kmh = 0.0f;
        horizontal = 0.0f;
        vertical = 0.0f;

        enforcedSpeedLimit = 200.0f;

        rb = gameObject.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        clampGear = Mathf.Clamp(vehicleController.currentGear, -1, 1);

        if (clampGear == 0)
        {
            clampGear = 1;
        }
        
        kmh = vehicleController.KMh * (float)clampGear;

        RaycastHit hit;

        int i = 0;

        foreach (GameObject raycastOrigin in raycastOrigins)
        {
            if (Physics.Raycast(raycastOrigin.transform.position, raycastOrigin.transform.TransformDirection(Vector3.forward), out hit, 15.0f))
            {
                if (hit.collider.tag == "Road")
                {
                    Debug.DrawRay(raycastOrigin.transform.position, raycastOrigin.transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                    raycastHits[i] = true;
                }

                else
                {
                    Debug.DrawRay(raycastOrigin.transform.position, raycastOrigin.transform.TransformDirection(Vector3.forward) * hit.distance, Color.red);
                    raycastHits[i] = false;
                }
            }

            else
            {
                Debug.DrawRay(raycastOrigin.transform.position, raycastOrigin.transform.TransformDirection(Vector3.forward) * 15.0f, Color.blue);
                raycastHits[i] = false;
            }

            i++;
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "WrongWay")
        {
            badCrash = true;
        }
    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.tag == "Stop")
        {
            if (kmh < 5)
            {
                stoppedAtStopSign = true;
            }
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.tag == "20")
        {
            enforcedSpeedLimit = 20;
        }

        else if (collider.tag == "30")
        {
            enforcedSpeedLimit = 30;
        }

        else if (collider.tag == "50")
        {
            enforcedSpeedLimit = 50;
        }

        else if (collider.tag == "60")
        {
            enforcedSpeedLimit = 60;
        }

        else if (collider.tag == "70")
        {
            enforcedSpeedLimit = 70;
        }

        else if (collider.tag == "80")
        {
            enforcedSpeedLimit = 80;
        }

        else if (collider.tag == "100")
        {
            enforcedSpeedLimit = 100;
        }

        else if (collider.tag == "120")
        {
            enforcedSpeedLimit = 120;
        }

        else if (collider.tag == "200")
        {
            enforcedSpeedLimit = 200;
        }

        else if (collider.tag == "Stop")
        {
            if (!stoppedAtStopSign)
            {
                stopSignPenalty = true;
            }

            else
            {
                stopSignReward = true;
                stoppedAtStopSign = false;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "End")
        {
            finishCrash = true;
        }

        else if (collision.collider.tag == "TrafficSign" || collision.collider.tag == "WrongWay")
        {
            badCrash = true;
        }
    }

    public override void CollectObservations()
    {
        AddVectorObs(kmh);
        AddVectorObs(leftWheel.rotation.y);
        AddVectorObs(rightWheel.rotation.y);

        foreach (bool hit in raycastHits)
        {
            AddVectorObs(hit);
        }
    }

    public override void AgentAction(float[] vectorAction)
    {
        ActionSpaceTypeControls(vectorAction);

        float reward;

        if (kmh < 0.0f)
        {
            reward = Vector3.Dot(rb.velocity, gameObject.transform.forward) / 5.0f;
        }

        else if (kmh > enforcedSpeedLimit)
        {
            float differenceInSpeed = kmh - enforcedSpeedLimit;
            reward = -differenceInSpeed / 5.0f;
        }

        else
        {
            reward = Vector3.Dot(rb.velocity, gameObject.transform.forward) / 20.0f;
        }

        AddReward(reward);

        if (gameObject.transform.position.y < -0.2f)
        {
            Done();
            SetReward(-50.0f);
        }

        else if (badCrash)
        {
            badCrash = false;
            Done();
            SetReward(-50.0f);
        }

        else if (finishCrash)
        {
            finishCrash = false;
            Done();
            SetReward(100.0f);
        }

        else if (stopSignPenalty)
        {
            stopSignPenalty = false;
            SetReward(-kmh / 5.0f);
        }

        else if (stopSignReward)
        {
            stopSignReward = false;
            SetReward(50.0f);
        }
    }

    private void ActionSpaceTypeControls(float[] vectorAction)
    {
        if (gameObject.GetComponent<BehaviorParameters>().brainParameters.vectorActionSpaceType == SpaceType.Continuous)
        {
            horizontal = Mathf.Clamp(vectorAction[0], -1.0f, 1.0f);
            vertical = Mathf.Clamp(vectorAction[1], -1.0f, 1.0f);
        }

        else if (gameObject.GetComponent<BehaviorParameters>().brainParameters.vectorActionSpaceType == SpaceType.Discrete)
        {
            if (vectorAction[0] == 0)
            {
                horizontal = -1.0f;
            }

            else if (vectorAction[0] == 1)
            {
                horizontal = 0.0f;
            }

            else if (vectorAction[0] == 2)
            {
                horizontal = 1.0f;
            }

            if (vectorAction[1] == 0)
            {
                vertical = -1.0f;
            }

            else if (vectorAction[1] == 1)
            {
                vertical = 0.0f;
            }

            else if (vectorAction[1] == 2)
            {
                vertical = 1.0f;
            }
        }

        carControls.horizontal = horizontal;
        carControls.vertical = vertical;
    }

    public override void AgentReset()
    {
        gameObject.transform.rotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
        Vector3 newPosition = initialPosition;
        newPosition.x = newPosition.x + UnityEngine.Random.Range(-5.0f, 5.0f);
        gameObject.transform.position = newPosition;
        rb.velocity = new Vector3(0.0f, 0.0f, 0.0f);
    }

    public override float[] Heuristic()
    {
        float[] inputs = new float[2];

        inputs[0] = Input.GetAxis("Horizontal");
        inputs[1] = Input.GetAxis("Vertical");
        
        return inputs;
    }
}
