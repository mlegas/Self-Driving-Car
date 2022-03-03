using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    public GameObject agent;
    public TextMeshProUGUI gearText;
    public TextMeshProUGUI kmhText;
    public TextMeshProUGUI speedLimitText;

    private MSVehicleControllerFree vehicleController;
    private CarDrivingAgent car;
    private int clampGear;

    // Start is called before the first frame update
    void Start()
    {
        vehicleController = agent.GetComponent<MSVehicleControllerFree>();
        car = agent.GetComponent<CarDrivingAgent>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        clampGear = Mathf.Clamp(vehicleController.currentGear, -1, 1);
        if (clampGear == 0)
        {
            clampGear = 1;
        }

        gearText.text = "Gear: " + vehicleController.currentGear;
        kmhText.text = "Km/h: " + (int)(vehicleController.KMh * clampGear);
        speedLimitText.text = "Speed limit: " + car.enforcedSpeedLimit;
    }
}
