using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class SnekRunner : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Movement speed of the snake head")]
    public float moveSpeed = 5f;
    [Tooltip("Turning speed in degrees per second")]
    public float steerSpeed = 180f;

    [Header("Snake Body Settings")]
    [Tooltip("Reference to the SpriteShapeController. It should be a child object of this Head.")]
    public SpriteShapeController spriteShapeController;
    [Tooltip("Number of segments in the snake body")]
    public int bodyLength = 20;
    [Tooltip("Distance between each body segment")]
    public float pointSpacing = 0.5f;

    [Header("Visual Settings")]
    [Tooltip("Controls the width of the body from head (0) to tail (1).")]
    public AnimationCurve bodyWidthCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 0));
    [Tooltip("Sprite to use for the head.")]
    public Sprite headSprite;
    [Tooltip("Reference to the SpriteRenderer on the head object.")]
    public SpriteRenderer headRenderer;

    // List to store the history of head positions
    private List<Vector3> pathPoints = new List<Vector3>();

    public void Setup()
    {

    }

    void Start()
    {
        // Initialize path with current position
        pathPoints.Add(transform.position);

        // Setup Head Sprite
        if (headRenderer != null && headSprite != null)
        {
            headRenderer.sprite = headSprite;
        }

        // Auto-find SpriteShapeController in children if not assigned
        if (spriteShapeController == null)
        {
            spriteShapeController = GetComponentInChildren<SpriteShapeController>();
        }

        // Ensure SpriteShapeController is assigned
        if (spriteShapeController == null)
        {
            Debug.LogError("Please assign the SpriteShapeController in the inspector or make sure it is a child of this object!");
            enabled = false;
            return;
        }

        // Force the SpriteShapeController to be at the same position as the head
        if (spriteShapeController.transform.parent == transform)
        {
            spriteShapeController.transform.localPosition = Vector3.zero;
        }

        // Initialize the spline with some dummy points if needed, or just wait for Update
    }

    void Update()
    {
        MoveHead();
        UpdateSnakeBody();
    }

    void MoveHead()
    {
        // Move forward (Up is forward in 2D usually)
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

        // Steer
        float steer = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.forward * -steer * steerSpeed * Time.deltaTime);

        // Record position
        // We add points frequently to capture the curve smoothly
        Vector3 lastPos = pathPoints[pathPoints.Count - 1];
        if (Vector3.Distance(transform.position, lastPos) > 0.05f)
        {
            pathPoints.Add(transform.position);
        }

        // Limit history size to prevent memory issues
        // We keep enough history for the full body length plus some buffer
        float requiredHistoryLength = bodyLength * pointSpacing + 2.0f;
        // Simple cleanup: if we have too many points, remove old ones.
        // A robust way would be to measure total path length, but a count limit is safer for performance.
        if (pathPoints.Count > 5000)
        {
            pathPoints.RemoveAt(0);
        }
    }

    void UpdateSnakeBody()
    {
        Spline spline = spriteShapeController.spline;
        spline.Clear();

        // We want to place 'bodyLength' points along the path, starting from the head (current pos) backwards.

        Vector3 currentPos = transform.position;
        // Add Head Point - Always at local (0,0,0) which is the Head position (since we aligned them)
        spline.InsertPointAt(0, Vector3.zero);
        spline.SetTangentMode(0, ShapeTangentMode.Continuous);
        spline.SetHeight(0, bodyWidthCurve.Evaluate(0f));

        int pointsAdded = 1;
        float distanceTravelled = 0f;
        Vector3 lastSamplePos = currentPos;

        // Iterate backwards through the path points to find segment positions
        for (int i = pathPoints.Count - 1; i >= 0; i--)
        {
            Vector3 pt = pathPoints[i];
            float dist = Vector3.Distance(lastSamplePos, pt);

            // While we have enough distance to place the next point
            while (distanceTravelled + dist >= pointSpacing)
            {
                // Calculate exact position by interpolation
                float remaining = pointSpacing - distanceTravelled;
                float t = remaining / dist;
                Vector3 newPointPos = Vector3.Lerp(lastSamplePos, pt, t);

                // Add to spline
                Vector3 localPos = spriteShapeController.transform.InverseTransformPoint(newPointPos);
                spline.InsertPointAt(pointsAdded, localPos);
                spline.SetTangentMode(pointsAdded, ShapeTangentMode.Continuous);

                float curveT = (float)pointsAdded / (float)(bodyLength - 1);
                spline.SetHeight(pointsAdded, bodyWidthCurve.Evaluate(curveT));

                pointsAdded++;
                if (pointsAdded >= bodyLength) break;

                // Update for next step
                lastSamplePos = newPointPos;
                distanceTravelled = 0f;

                // Recalculate dist from the new point to the current path point 'pt'
                // because we just "consumed" some of the segment
                dist = Vector3.Distance(lastSamplePos, pt);
            }

            if (pointsAdded >= bodyLength) break;

            distanceTravelled += dist;
            lastSamplePos = pt;
        }

        // If we ran out of history but still need points (e.g. at start), extend backwards
        while (pointsAdded < bodyLength)
        {
            // Just extend in the direction of the last segment
            Vector3 direction = (pointsAdded > 1) ?
                (spline.GetPosition(pointsAdded - 1) - spline.GetPosition(pointsAdded - 2)).normalized :
                -Vector3.up;

            if (direction == Vector3.zero) direction = -Vector3.up;

            Vector3 newPos = spline.GetPosition(pointsAdded - 1) + direction * pointSpacing;
            spline.InsertPointAt(pointsAdded, newPos);
            spline.SetTangentMode(pointsAdded, ShapeTangentMode.Continuous);

            float curveT = (float)pointsAdded / (float)(bodyLength - 1);
            spline.SetHeight(pointsAdded, bodyWidthCurve.Evaluate(curveT));

            pointsAdded++;
        }
    }
}
