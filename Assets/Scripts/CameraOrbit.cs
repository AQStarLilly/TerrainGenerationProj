using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    [Header("Orbit Settings")]
    public Transform target;      // Center point of the terrain
    public float radius = 50f;    // Distance from the center
    public float height = 30f;    // Height above the terrain
    public float rotationSpeed = 10f; // Degrees per second

    private float angle = 0f;

    void Update()
    {
        if (target == null) return;

        // Increase angle over time
        angle += rotationSpeed * Time.deltaTime;

        // Convert degrees - radians
        float rad = angle * Mathf.Deg2Rad;

        // Calculate position on circle
        float x = Mathf.Cos(rad) * radius;
        float z = Mathf.Sin(rad) * radius;

        // Move camera
        transform.position = new Vector3(
            target.position.x + x,
            height,
            target.position.z + z
        );

        // Always look at the terrain center
        transform.LookAt(target.position);
    }
}
