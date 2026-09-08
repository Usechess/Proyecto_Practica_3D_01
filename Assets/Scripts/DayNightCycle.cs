using System;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [SerializeField] private Light sourceLight;

    private enum DayState
    {
        Dawn,
        Day,
        Sunset,
        Night
    }
    private DayState currentState;

    [SerializeField] private float dawnSpeed;
    [SerializeField] private float daySpeed;
    [SerializeField] private float sunsetSpeed;
    [SerializeField] private float nightSpeed;
    [SerializeField] private float cycleSpeed;

    private float rotationX;

    private void Start()
    {
        rotationX = 15.186f;

        sourceLight.transform.rotation = Quaternion.Euler(rotationX, -25.968f, 5.252f);

        currentState = DayState.Day;
    }

    private void Update()
    {
        if (sourceLight != null)
        {
            if (currentState == DayState.Dawn)
            {
                rotationX += dawnSpeed * cycleSpeed * Time.deltaTime;

                sourceLight.transform.rotation = Quaternion.Euler(rotationX, -25.968f, 5.252f);

                if (rotationX >= 5)
                {
                    rotationX = 5;
                    currentState = DayState.Day;

                    Debug.Log("Day has started");
                }

            }
            else if (currentState == DayState.Day)
            {
                rotationX += daySpeed * cycleSpeed * Time.deltaTime;

                sourceLight.transform.rotation = Quaternion.Euler(rotationX,-25.968f, 5.252f);

                if (rotationX >= 180f)
                {
                    rotationX = 180f;
                    currentState = DayState.Sunset;

                    Debug.Log("No longer Day");
                }
            }
            else if (currentState == DayState.Sunset) 
            {
                rotationX += sunsetSpeed * cycleSpeed * Time.deltaTime;

                sourceLight.transform.rotation = Quaternion.Euler(rotationX, -25.968f, 5.252f);

                if (rotationX >= 190)
                {
                    rotationX = 190;
                    currentState = DayState.Night;
                    Debug.Log("Sunset has ended");
                    Debug.Log("Night has started");
                }
            }
            else if (currentState == DayState.Night)
            {
                rotationX += nightSpeed * cycleSpeed * Time.deltaTime;

                sourceLight.transform.rotation = Quaternion.Euler(rotationX, -25.968f, 5.252f);

                if (rotationX >= 345)
                {
                    rotationX = -15f;

                    currentState = DayState.Dawn;

                    Debug.Log("Night has ended");
                    Debug.Log("Dawn has started");


                }
            }
        }
    }
}