using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoldingAnimationTest : MonoBehaviour
{
    /*This will be a test. This just going to loop over and object just to see how it looks.
     We can probably change it up about, but this is the idea that I had for the hold note.
     
     First of all, we hand to make sure the rotation start horizontally, and that it moves up and down.
     
     Next, the scale should be 0, and should make it all the way to 1.
     
     Next comes the hard part; decide the revolutions of the rotation.
     We don't want the rotation to be 360 at the way; we want to be able to calculate
     how many revolutions the rotation will do as it scales up to one based on the music speed or tempo.
     
     This part will require a lot of math, but for now, we will not worry about it. We just need to scale and rotate, then figure
     out the revolutions per seconds (which is what we'll use). Ands heads up; at scale 1, rotation should have completed, so I know
     for sure some division will be involved to get the average revolution while still getting the full circle in. That's a challenge I'm willing to take.*/

    //Reference to our fill, which is an image
    [SerializeField]
    private Image fillImage;

    //The scaling will be divided by the max value, simulation
    //an initial note point and the end note point of a approach circle
    [Range(1000f, 10000f), SerializeField]
    private float maxValue;

    //The rotation speed
    [Range(10f, 1000f), SerializeField]
    private float rotationSpeed;

    //The current value
    private float currentValue;

    //And then our reset constant
    private const float RESET = 0f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(HoldSequence());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// The animation of holding a key.
    /// </summary>
    /// <returns></returns>
    IEnumerator HoldSequence()
    {
        while(true)
        {
            fillImage.transform.localScale = new Vector3(1f / GetPercentage(), 1f/ GetPercentage());
            fillImage.fillAmount += rotationSpeed * Time.deltaTime;

            //If scale is full size, complete the hold
            if (GetPercentage() >= 0.99f)
                Complete();

            //If the fill covers the whole key, reset it
            if (fillImage.fillAmount >= 0.99f)
                fillImage.fillAmount = RESET;

            yield return null;
        }
    }

    /// <summary>
    /// Complete a full hold
    /// </summary>
    void Complete()
    {
        currentValue = RESET;
    }

    /// <summary>
    /// The percentage of current value and maxValue
    /// </summary>
    float GetPercentage() => currentValue / maxValue;
}
