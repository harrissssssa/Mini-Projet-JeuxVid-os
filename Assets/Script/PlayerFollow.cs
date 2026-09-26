using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // glisse ton joueur ici dans l'Inspector
    public float smoothSpeed = 5f;
    public Vector3 offset = new Vector3(0f, 0f, -10f); // -10 pour rester devant la scène en 2D

    public bool useBounds = false;
    public Vector2 minBounds;
    public Vector2 maxBounds;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        if (useBounds)
        {
            smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, minBounds.x, maxBounds.x);
            smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, minBounds.y, maxBounds.y);
        }

        transform.position = smoothedPosition;
    }
}