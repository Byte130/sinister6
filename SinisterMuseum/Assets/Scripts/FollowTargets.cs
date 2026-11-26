using UnityEngine;
using System.Collections.Generic;

public class FollowTargets : MonoBehaviour
{

    public List<Transform> targets;

    public Vector3 offset;
    public float smoothTime = 0.5f;

    public Vector3 velocity;

    public float minZoom = 40f;
    public float maxZoom = 10f;

    private void LateUpdate()
    {
        if (targets.Count == 0)
            return;

        Zoom();

        Vector3 centerPoint = GetCenterPoint();

        Vector3 newPosition = centerPoint + offset;

        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);

        Vector3 GetCenterPoint()
            {
            if (targets.Count == 1)
            {
                return targets[0].position;
            }
            var bounds = new Bounds(targets[0].position, Vector3.zero);
            for (int i = 0; i < targets.Count; i++)
            {
                bounds.Encapsulate(targets[i].position);
            }
            return bounds.center;
        }
    }

    void Zoom()
    {
        float newZoom = Mathf.Lerp(maxZoom, minZoom, GetGreatestDistance() / 50f);
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, newZoom, Time.deltaTime);
    }

    float GetGreatestDistance()
    {
        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }
        return bounds.size.x;
    }
    public void AddTarget(Transform target)
    {
        if (!targets.Contains(target))
        {
            targets.Add(target);
        }
    }
} 