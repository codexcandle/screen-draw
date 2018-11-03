using UnityEngine;

namespace Codebycandle.ScreenDrawApp
{
    public class CameraRigController:MonoBehaviour
    {
        private float rotateSpeed = 100F;

        private Camera cam;

        public bool keyActive
        {
            get;
            set;
        }

        public void RotateVertical(bool up)
        {
            Rotate((up ? rotateSpeed : -rotateSpeed), 0, 0);
        }

        public void RotateHorizontal(bool left)
        {
            Rotate(0, (left ? rotateSpeed : -rotateSpeed), 0);
        }

        public void SetCamProjectionMode(bool orthographic)
        {
            cam.orthographic = orthographic;
        }

        void Start()
        {
            Init();
        }

        void Update()
        {
            if (!keyActive) return;

            // arrow-keys
            if (Input.GetKey(KeyCode.UpArrow))
            {
                RotateVertical(true);
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                RotateVertical(false);
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                RotateHorizontal(true);
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                RotateHorizontal(false);
            }

            // space-bar
            if(Input.GetKeyDown(KeyCode.Space))
            {
                SetCamProjectionMode(!cam.orthographic);
            }
        }

        private void Init()
        {
            cam = Camera.main;
        }

        private void Rotate(float xSpeed, float ySpeed, float zSpeed)
        {
            Vector3 temp = transform.rotation.eulerAngles;
            temp.x += (xSpeed * Time.deltaTime);
            temp.y += (ySpeed * Time.deltaTime);
            temp.z += (zSpeed * Time.deltaTime);

            transform.rotation = Quaternion.Euler(temp);
        }
    }
}