using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// My Smart Camera
/// </summary>
public class SmartCamera : MonoBehaviour
{
    private Camera cam;

    public class SmartCameraPoint
    {
        public Vector2 anchor;
        public Vector2 offset;
        public float distance;
        public float fov;
        public float weight;

        public SmartCameraPoint(Vector2 anchor, Vector2 offset, float distance, float fov)
        {
            this.anchor = anchor;
            this.offset = offset;
            this.distance = distance;
            this.fov = fov;
            this.weight = 0;
        }
    }

    private Vector2 PPPos { get => PonPo.ponPo.transform.position; }

    public static List<SmartCameraPoint> cameraPoints = new List<SmartCameraPoint>();

    private void React(Vector2 direction)
    {
        transform.Translate(direction * GameSystem.TheMatrix.PonPoSetting.reactDistance);
    }

    private void Awake()
    {
        cameraPoints.Clear();
        cam = GetComponent<Camera>();
    }

    private void Start()
    {
        PonPo.ponPo.onShoot.AddListener(React);
    }

    private void Update()
    {
        float fullWeight = 0;
        for (int i = 0; i < cameraPoints.Count; i++)
        {
            float d = Vector2.Distance(PPPos, cameraPoints[i].anchor);
            float weight = 1.0f / (d + 0.001f);
            cameraPoints[i].weight = weight;
            fullWeight += weight;
        }
        Vector2 target2D = PPPos;
        float targetZ = 0;
        float targetFov = 0;
        for (int i = 0; i < cameraPoints.Count; i++)
        {
            float factor = cameraPoints[i].weight / fullWeight;
            target2D += cameraPoints[i].offset * factor;
            targetZ += cameraPoints[i].distance * factor;
            targetFov += cameraPoints[i].fov * factor;
        }
        Vector3 target3D = new Vector3(target2D.x, target2D.y, targetZ);

        float t = 1.0f - Mathf.Pow(1.0f - GameSystem.TheMatrix.PonPoSetting.movingRate, Time.deltaTime / Time.timeScale);
        transform.position = Vector3.Lerp(transform.position, target3D, t);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFov, t);
    }
}
