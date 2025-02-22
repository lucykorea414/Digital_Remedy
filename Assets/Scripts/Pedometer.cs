using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
#pragma warning disable CS0618

// https://blog.birost.com/a?ID=01650-cc0220d7-0c42-4688-afab-40faca7a671e
public class Pedometer : MonoBehaviour
{
    // public TextMeshProUGUI statusText, stepsText;
    public float lowLimit = 0.005f;//slow
    public float highLimit = 0.1f;//peaks and valleys when walking
    public float vertHighLimit = 0.25f;//The peak and valley when jumping
    private bool isHigh = false;//status
    private float filterCurrent = 10.0f;//filter parameters to get the fitted value
    private float filterAverage = 0.1f;//Filter parameters to get the average
    private float accelerationCurrent = 0f;//fitting value
    private float accelerationAverage = 0f;//Average
    private int oldSteps;
    private float deltaTime = 0f;//Timer
    private int jumpCount = 0;//jump count
    private int oldjumpCount = 0;

    public int steps = 0;//number of steps
    public bool isWalking = false;
    public bool isJumping = false;

    private bool startTimer = false;//Start timing

    void Awake()
    {
        accelerationAverage = Input.acceleration.magnitude;
        oldSteps = steps;
        oldjumpCount = jumpCount;
    }

    void Update()
    {
        checkWalkingAndJumping();//Check whether to walk
    }

    void FixedUpdate()
    {

        //Filter Input.acceleration.magnitude (acceleration scalar sum) through Lerp
        //The linear interpolation formula used here is exactly the EMA one-time exponential filtering y[i]=y[i-1]+(x[i]-y[i])*k=(1-k)*y[i] +kx[i]
        accelerationCurrent = Mathf.Lerp(accelerationCurrent, Input.acceleration.magnitude, Time.deltaTime * filterCurrent);
        accelerationAverage = Mathf.Lerp(accelerationAverage, Input.acceleration.magnitude, Time.deltaTime * filterAverage);
        float delta = accelerationCurrent - accelerationAverage;//Get the difference, that is, the slope

        if (!isHigh)
        {
            if (delta > highLimit)//Go high
            {
                isHigh = true;
                steps++;
                //stepsText.text = "Number of steps: " + steps + "\nNumber of jumps: " + jumpCount;
            }
            if (delta > vertHighLimit)
            {
                isHigh = true;
                jumpCount++;
                //stepsText.text = "Number of steps: " + steps + "\nNumber of jumps:" + jumpCount;
            }
        }
        else
        {
            if (delta < lowLimit)//lower
            {
                isHigh = false;
            }
        }
    }


    private void checkWalkingAndJumping()
    {
        if ((steps != oldSteps) || (oldjumpCount != jumpCount))
        {
            startTimer = true;
            deltaTime = 0f;
        }

        if (startTimer)//Timer, make it slower to update the UI, because you can��t walk in only one frame QAQ
        {
            deltaTime += Time.deltaTime;

            if (deltaTime != 0)
            {
                if (oldjumpCount != jumpCount)//Check if it is a jump
                    isJumping = true;
                else
                    isWalking = true;

            }
            if (deltaTime > 2)
            {
                deltaTime = 0F;
                startTimer = false;
            }
        }
        else if (!startTimer)
        {
            isWalking = false;
            isJumping = false;
        }
        oldSteps = steps;
        oldjumpCount = jumpCount;
    }
}
