using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointChecker : MonoBehaviour
{
    public CarController theCar;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            //  Debug.Log("hit cp " + other.GetComponent<CheckPoint>().cpNumber);

            theCar.CheckPointHit(other.GetComponent<CheckPoint>().cpNumber);
        }
    }
}
