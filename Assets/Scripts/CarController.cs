using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class CarController : MonoBehaviour
{
#region variables

    public float maxSpeed = 0f;
    public float forwardAcceleration = 10f, reverseAcceleration = 5f;
    public float turnStrength = 180f;


    private float speedInput = 0f;
    private float turnInput = 0f;

    private float groundRayLength = 0.75f;

    private float dragOnGround = 0f;
    public float gravityMod = 10f;

    private float maxWheelTurn = 25f;

    public float maxEmission = 25f, emissionFadeSpeed = 20f;
    private float emissionRate = 0f;

    private float skidFadeSpeed = 2f;

    public int nextCheckpoint = 0;
    public int currentLap = 0;

    private float currentLapTime, bestLapTime;

    private float resetCounter = 0f;
    public float resetCooldown = 2f;

    private bool isGrounded;

   
    

    #endregion

#region otherVariables

    public Rigidbody theRB;

    [SerializeField] Transform groundRayPoint = null , groundRayPoint2 = null;

    [SerializeField] Transform leftFrontWheel = null, rightFrontWheel = null;

    [SerializeField] LayerMask whatIsGround = default;

    [SerializeField] ParticleSystem[] dustTrail = null;

    [SerializeField] AudioSource engineSound = null, skidSound = null;



    #endregion

    #region AI Variable
    public bool isAI;
    public int currentTarget = 0;
    public float aiAccelerateSpeed = 1f, aiTurnSpeed = 0.8f, aiReachPointRange = 5f, aiPointVariance = 3f,aiMaxTurn = 15f;
    private float aiSpeedInput = 1f, aiSpeedMod = 0f;
    private Vector3 targetPoint;

    #endregion

    
   


    void Start()
    {
        dragOnGround = theRB.drag;
        theRB.transform.parent = null;
        UIManager.instance.lapCounterText.text = currentLap + "/" + RaceManager.instance.totalLaps;
        resetCounter = resetCooldown;

        if (isAI)
        {
            targetPoint = RaceManager.instance.allCheckpoints[currentTarget].transform.position;
            RandomiseAITarget();
            aiSpeedMod = Random.Range(0.8f, 1.1f);
        }
    }


    void Update()
    {
        if (!RaceManager.instance.isStarting)
        {
            LapTimer();
            CheckMovement();
            TurnWheels();
            DustTrail();
            CarSound();
        }

        



    }
    private void FixedUpdate() // physics related functions in fixed update
    {
        ApplyMovement();
        CheckGround();


        transform.position = theRB.position;
    }

    void CheckMovement()
    {
        if (!isAI)
        {
            speedInput = 0f;
            if (CrossPlatformInputManager.GetAxis("Vertical") > 0 || Input.GetAxis("Vertical")>0 ) //forward movement
            {
                speedInput = CrossPlatformInputManager.GetAxis("Vertical") * forwardAcceleration; 
               // speedInput = Input.GetAxis("Vertical") * forwardAcceleration;
               
            }
            else if (CrossPlatformInputManager.GetAxis("Vertical") < 0 || Input.GetAxis("Vertical") < 0) //reverse movement
            {
                 speedInput = CrossPlatformInputManager.GetAxis("Vertical") * reverseAcceleration;
               // speedInput = Input.GetAxis("Vertical") * reverseAcceleration;
            }

            turnInput = CrossPlatformInputManager.GetAxis("Horizontal");
           // turnInput = Input.GetAxis("Horizontal");

            if (resetCounter > 0)
            {
                resetCounter -= Time.deltaTime;
            }
            if (Input.GetKeyDown(KeyCode.R) && resetCounter <= 0 )
            {
                ResetToTrack();
            }
        }
        else // ai movement
        {
            AIMovement();
        }  
    }

    void ApplyMovement() 
    {
        if (isGrounded) //accelerates car
        {
            theRB.AddForce(transform.forward * speedInput * 1000f);
            theRB.drag = dragOnGround;
        }
        else //disable movement in air
        {
            theRB.drag = 0.1f;
            theRB.AddForce(-Vector3.up * gravityMod * 100f);
        }

        if(theRB.velocity.magnitude > maxSpeed) //cap car speed 
        {
            theRB.velocity = theRB.velocity.normalized * maxSpeed;
        }

        if (speedInput != 0 && isGrounded) // steering
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turnInput * turnStrength * Time.deltaTime * Mathf.Sign(speedInput) * (theRB.velocity.magnitude / maxSpeed), 0f));

           
        }
    }

    void CheckGround()
    {
        isGrounded = false;

        RaycastHit hit;
        Vector3 normalTarget = Vector3.zero;

        //checking if grounded
        if (Physics.Raycast(groundRayPoint.position, -transform.up, out hit, groundRayLength, whatIsGround))
        {
            isGrounded = true;
            normalTarget = hit.normal;
        }

        //rotating car on ramp
        if(Physics.Raycast(groundRayPoint2.position, -transform.up,out hit, groundRayLength, whatIsGround))
        {
            isGrounded = true;

            normalTarget = (normalTarget + hit.normal) / 2f; 
        }

        //when on ground rotate to match the normal
        if (isGrounded)
        {
            transform.rotation = Quaternion.FromToRotation(transform.up, normalTarget) * transform.rotation;
        }


    }

    void TurnWheels()
    {
        leftFrontWheel.localRotation  = Quaternion.Euler(leftFrontWheel.localRotation.eulerAngles.x, (maxWheelTurn * turnInput) - 180f, leftFrontWheel.localRotation.eulerAngles.z);
        rightFrontWheel.localRotation = Quaternion.Euler(rightFrontWheel.localRotation.eulerAngles.x, (turnInput * maxWheelTurn), rightFrontWheel.localRotation.eulerAngles.z);
    }

    void DustTrail()
    {
        emissionRate = Mathf.MoveTowards(emissionRate, 0f, emissionFadeSpeed * Time.deltaTime);

        if(isGrounded && (Mathf.Abs(turnInput) > 0.5f  || (theRB.velocity.magnitude < maxSpeed * 0.5f && theRB.velocity.magnitude != 0)))
        {
            emissionRate = maxEmission;
        }

        if(theRB.velocity.magnitude <= 0.5f)
        {
            emissionRate = 0f;
        }

        for (int i = 0; i < dustTrail.Length; i++)
        {
            var emissionModule = dustTrail[i].emission;

            emissionModule.rateOverTime = emissionRate;
        }
    }

    void CarSound()
    {
        if(engineSound != null)
        {
            engineSound.pitch = 1f + ((theRB.velocity.magnitude / maxSpeed) * 2f);
        }

        if(skidSound != null)
        {
            if(Mathf.Abs(turnInput) > 0.5f)
            {
                skidSound.volume = 1f;

            }
            else
            {
                skidSound.volume = Mathf.MoveTowards(skidSound.volume, 0f, skidFadeSpeed * Time.deltaTime);
            }
        }
    }

    public void CheckPointHit(int cpNumber)
    {
        if(cpNumber == nextCheckpoint)
        {
            nextCheckpoint++;

            if (nextCheckpoint == RaceManager.instance.allCheckpoints.Length)
            {
                nextCheckpoint = 0;
                LapCompleted();
            }
        }

        if (isAI)
        {
            if(cpNumber == currentTarget)
            {
                SetNextAITarget();
            }
        }
    }

    public void LapCompleted()
    {
        currentLap++;

        if (currentLapTime < bestLapTime || bestLapTime == 0)
        {
            bestLapTime = currentLapTime;
        }
        if (currentLap <= RaceManager.instance.totalLaps)
        {
            currentLapTime = 0f;

            if (!isAI)
            {
                var ts = System.TimeSpan.FromSeconds(bestLapTime);
                UIManager.instance.bestLapTimeText.text = string.Format("{0:00}m{1:00}.{2:000}s", ts.Minutes, ts.Seconds, ts.Milliseconds);

                UIManager.instance.lapCounterText.text = currentLap + "/" + RaceManager.instance.totalLaps;
            }
        }
        else
        {
            if (!isAI)
            {
                isAI = true;
                aiSpeedMod = 1f;

                targetPoint = RaceManager.instance.allCheckpoints[currentTarget].transform.position;
                RandomiseAITarget();

                var ts = System.TimeSpan.FromSeconds(bestLapTime);
                UIManager.instance.bestLapTimeText.text = string.Format("{0:00}m{1:00}.{2:000}s", ts.Minutes, ts.Seconds, ts.Milliseconds);

                RaceManager.instance.FinishRace();

            }
        }
    }

    void LapTimer()
    {
        currentLapTime += Time.deltaTime; //keep track of current lap time 
        if (!isAI)
        {
            var ts = System.TimeSpan.FromSeconds(currentLapTime);
            UIManager.instance.currentLapTimeText.text = string.Format("{0:00}m{1:00}.{2:000}s", ts.Minutes, ts.Seconds, ts.Milliseconds);
        }
    }
   
    public void RandomiseAITarget()
    {
        targetPoint += new Vector3(Random.Range(-aiPointVariance, aiPointVariance), 0f, Random.Range(-aiPointVariance, aiPointVariance));
    }

    void SetNextAITarget()
    {
        currentTarget++;
        if (currentTarget >= RaceManager.instance.allCheckpoints.Length)
            currentTarget = 0;

        targetPoint = RaceManager.instance.allCheckpoints[currentTarget].transform.position;
        RandomiseAITarget();
    }
    
    void AIMovement()
    {
        targetPoint.y = transform.position.y;

        if (Vector3.Distance(transform.position, targetPoint) < aiReachPointRange)
        {
            SetNextAITarget();

        }

        Vector3 targetDir = targetPoint - transform.position;
        float angle = Vector3.Angle(targetDir, transform.forward);

        Vector3 localPos = transform.InverseTransformPoint(targetPoint);
        if (localPos.x < 0f)
        {
            angle = -angle;
        }
        turnInput = Mathf.Clamp(angle / aiMaxTurn, -1f, 1f);

        if (Mathf.Abs(angle) < aiMaxTurn)
        {
            aiSpeedInput = Mathf.MoveTowards(aiSpeedInput, 1f, aiAccelerateSpeed);
        }
        else
        {
            aiSpeedInput = Mathf.MoveTowards(aiSpeedInput, aiTurnSpeed, aiAccelerateSpeed);
        }

        speedInput = aiSpeedInput * forwardAcceleration * aiSpeedMod;
    }

    void ResetToTrack()
    {
        int pointToGoTo = nextCheckpoint - 1;
        if(pointToGoTo < 0)
        {
            pointToGoTo = RaceManager.instance.allCheckpoints.Length - 1;
        }
        transform.position = RaceManager.instance.allCheckpoints[pointToGoTo].transform.position;

        theRB.transform.position = transform.position;
        theRB.velocity = Vector3.zero;
        speedInput = 0f;
        turnInput = 0f;

        resetCounter = resetCooldown;
    }
}
