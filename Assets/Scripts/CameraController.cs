using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public CarController target = null;
    private Vector3 offSetDir;

    public float minDistance, maxDistance;
    private float activeDistance;

    [SerializeField] Transform startTargerOffset = null;


    void Start()
    {
        offSetDir = transform.position - startTargerOffset.position;

        activeDistance = minDistance;

        offSetDir.Normalize();
        
    }

    // Update is called once per frame
    void Update()
    {

        activeDistance = minDistance + ((maxDistance - minDistance) * (target.theRB.velocity.magnitude / target.maxSpeed));

        transform.position = target.transform.position + (offSetDir * activeDistance);
        
    }
}
