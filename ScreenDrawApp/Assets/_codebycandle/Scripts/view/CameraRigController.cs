using UnityEngine;

public class CameraRigController:MonoBehaviour
{
    public float xRate = 0f;    // degrees / sec
    public float yRate = 20f;
    public float zRate = 0f;

    private bool doSpin;
    private bool isResetting;

    float speed = 0.01f;

    private Quaternion baseRotation;

    public void Reset()
    {
        isResetting = true;
    }

    void Start()
    {
        Init();
    }

    void Update()
    {
        if (isResetting)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, baseRotation, Time.time * speed);

            if (transform.rotation == Quaternion.identity)
            {
                isResetting = false;

                doSpin = false;
            }
        }
        else if (doSpin)
        {
            transform.Rotate(new Vector3(xRate, yRate, zRate) * Time.deltaTime);
        }
    }

    private void Init()
    {
        baseRotation = transform.rotation;

        doSpin = true;
    }
}