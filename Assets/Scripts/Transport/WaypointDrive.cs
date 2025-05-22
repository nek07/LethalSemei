using System.Collections.Generic;
using UnityEngine;

namespace Transports
{
    public class WaypointDrive : MonoBehaviour
    {
        [Header("Waypoints")] 
        [SerializeField] private List<Transform> waypoints;

        [Header("Transport Settings")]
        [SerializeField] private float motorTorque = 1200f;
        [SerializeField] private float brakeForce = 2500f;
        [SerializeField] private float maxSteerAngle = 20f;

        [Header("Wheel Colliders")] 
        [SerializeField] private WheelCollider[] driveWheels;
        [SerializeField] private WheelCollider[] steerWheels;

        private int currentWaypointIndex = 0;


        void FixedUpdate()
        {
            if (currentWaypointIndex >= waypoints.Count) return;

            Transform target = waypoints[currentWaypointIndex];
            Vector3 localTarget = transform.InverseTransformPoint(target.position);
            float steer = Mathf.Clamp(localTarget.x / localTarget.magnitude, -1f, 1f);
            float steerAngle = steer * maxSteerAngle;

            foreach (WheelCollider wheel in steerWheels)
            {
                wheel.steerAngle = steerAngle;
            }

            float distance = Vector3.Distance(transform.position, target.position);
            bool shouldBrake = distance < 4f;

            foreach (WheelCollider wheel in driveWheels)
            {
                wheel.motorTorque = shouldBrake ? 0f : motorTorque;
                wheel.brakeTorque = shouldBrake ? brakeForce : 0f;
            }

            if (distance < 4f)
            {
                currentWaypointIndex++;
            }
        }
    }
}