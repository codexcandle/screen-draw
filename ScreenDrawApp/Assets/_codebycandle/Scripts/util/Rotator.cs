using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float xRate = 0f;    // degrees / sec
    public float yRate = 20f;
    public float zRate = 0f;

    public bool isResetting;
    private bool doSpin;

    float speed = 0.1f;

    public void Reset()
    {
        isResetting = true;
    }

    void Start()
    {
        doSpin = true;
    }

    void Update()
    {
        if(isResetting)
        {
           transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, Time.time * speed);

            if(transform.rotation == Quaternion.identity)
            {
                isResetting = false;

                doSpin = false;
            }
        }
        else if(doSpin)
        {
            transform.Rotate(new Vector3(xRate, yRate, zRate) * Time.deltaTime);
        }
    }
}