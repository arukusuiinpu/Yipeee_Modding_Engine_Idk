using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class CameraController : MonoBehaviour
    {
        public float zoomStrength = 400f;
        public float moveStrength = 10f;
        public Vector2 cameraVelocity = Vector2.zero;
        public float decelerrationRate = 0.91f;

        private Transform cameraRotPoint;
        private GameObject console;

        private Vector3[] pointsOfInterest =
        [
            new Vector3(0f, 0f, 0f),
            new Vector3(0f, 2500f, 0f),
            new Vector3(0f, 5000f, 0f),
            new Vector3(0f, 7500f, 0f)
        ];

        private void Start()
        {
            Camera.main.transform.localPosition = new Vector3(0f, 0f, -80f);
            cameraRotPoint = GameObject.Find("CameraRotPoint").transform;
            console = GameObject.Find("Console");
            Camera.main.allowMSAA = false;
            Camera.main.nearClipPlane = 0.03f;
        }

        private void FixedUpdate()
        {
            if (Input.GetMouseButton((int)MouseButton.Middle))
            {
                cameraVelocity.x -= Input.GetAxisRaw("Mouse X") * moveStrength * Time.deltaTime;
                cameraVelocity.y -= Input.GetAxisRaw("Mouse Y") * moveStrength * Time.deltaTime;
            }

            cameraVelocity.x *= decelerrationRate;
            cameraVelocity.y *= decelerrationRate;

            Camera.main.transform.localPosition = new Vector3(Camera.main.transform.localPosition.x + cameraVelocity.x, Camera.main.transform.localPosition.y + cameraVelocity.y, MapCameraZ(Camera.main.transform.localPosition.z, (!Interface.IsOnConsole(console) ? (Input.GetAxisRaw("Mouse ScrollWheel") * zoomStrength * Time.deltaTime / Mathf.Pow(Mathf.Abs(Camera.main.transform.localPosition.z + WorldGen.sphereRadius) / 10f / WorldGen.sphereRadius * 48f, 1.2f) * 3f) : 0f)));
            GameObject.Find("Canvas").transform.position = new Vector3(GameObject.Find("Canvas").transform.position.x, GameObject.Find("Canvas").transform.position.y, Mathf.Abs(Camera.main.transform.position.z));
        }

        public float MapCameraZ(float z, float difference)
        {
            bool dontComeTooClose = z + difference < -1f;
            bool dontComeTooFar = z + difference > -1000f;
            if (dontComeTooClose && dontComeTooFar)
            {
                return z + difference;
            }
            else return z;
        }
    }
}
