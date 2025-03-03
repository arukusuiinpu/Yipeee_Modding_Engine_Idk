using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.Universal.TemporalAA;

namespace Core
{
    public class CameraController : MonoBehaviour
    {
        public float zoomStrength = 600f;
        public float moveStrength = 10f;
        public Vector2 cameraVelocity = Vector2.zero;
        public float decelerrationRate = 0.91f;

        private Transform cameraRotPoint;
        private GameObject console;

        private RectTransform debugRect;
        private TMP_Text debugText;

        public float renderDistanceMultiplyer = 1f;

        private void Start()
        {
            Camera.main.transform.localPosition = new Vector3(0f, 0f, -80f);
            cameraRotPoint = GameObject.Find("CameraRotPoint").transform;
            console = GameObject.Find("Console");
            Camera.main.allowMSAA = false;
            Camera.main.nearClipPlane = 0.03f;

            //GameObject debugInfo = Instantiate(GameObject.Find("Toggle").transform.Find("Background").transform.Find("Text").gameObject, GameObject.Find("Canvas").transform);
            //debugInfo.name = "Debug Info";
            //debugRect = debugInfo.GetComponent<RectTransform>();
            //
            //debugText = debugRect.GetComponent<TMP_Text>();
            //
            //Yipeee.Logger.LogError(debugText.ToString());
        }

        private void Update()
        {
            //debugRect.localPosition = new Vector3(0f, 0f, 0f);
            //debugRect.sizeDelta = new Vector2(160f, 30f);

            //debugText.text = "test";
            //debugText.color = Color.white;
            //debugText.fontSize = 40;

            if (Input.GetMouseButtonDown((int)MouseButton.Right))
            {
                renderDistanceMultiplyer += 0.1f;
            }

            void For()
            {
                float xyDist = 1f;
                float scale = 1f;
                xyDist *= scale;
                float xDist = xyDist * WorldGen.chunkSize.y;
                float yDist = xyDist * WorldGen.chunkSize.x;
                Vector3 localWorldSize = new Vector3(WorldGen.worldSize.x * WorldGen.mainWorld.size, WorldGen.worldSize.y * WorldGen.mainWorld.size, WorldGen.worldSize.z);
                for (float y = -localWorldSize.y; y < localWorldSize.y; y += yDist)
                {
                    for (float x = -localWorldSize.x; x < localWorldSize.x; x += xDist)
                    {
                        Vector2 position = new Vector2(x, y);
                        if (WorldGen.GetChunk(position) == null && Mathf.Pow(x, 2f) + Mathf.Pow(y, 2f) <= Mathf.Pow(WorldGen.renderDistance * renderDistanceMultiplyer, 2f))
                        {
                            Chunk chunk = WorldGen.SpawnChunk(position, WorldGen.mainWorld.size, WorldGen.mainWorld.seed, WorldGen.mainWorld.settings, WorldGen.mainWorld.terrains, WorldGen.mainWorld.rules);
                            WorldGen.RegisterChunk(position, chunk);
                            chunk.self.transform.position += WorldGen.mainWorld.location;
                            return;
                        }
                    }
                }
            }
            For();
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
