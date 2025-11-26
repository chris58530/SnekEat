using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.U2D;

public class SnekObjectView : MonoBehaviour, IHittable
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
    [Tooltip("Reference to the SpriteRenderer on the head object.")]
    public SpriteRenderer headRenderer;

    // --- [New] 新增出洞相關設定 ---
    [Header("Portal Exit Settings")]
    [Tooltip("出洞時，寬度從0變到1的過渡距離。數值越小切口越銳利，數值越大切口越平滑")]
    public float exitTransitionLength = 1.0f;
    [Tooltip("螺旋出洞的半徑增長係數 (b in r = a + b*theta)")]
    public float spiralGrowthFactor = 0.3f;
    [Tooltip("螺旋出洞的起始半徑")]
    public float spiralStartRadius = 0.1f;

    private bool isExitingPortal = false;
    private Vector3 exitPoint;
    private float exitCurrentDistance = 0f;
    private float currentSpiralAngle = 0f;
    private Action onExitComplete;
    private Vector3 savedEntryPosition;
    private Vector3 savedEntryDirection;

    // -----------------------------

    [Header("Skill Settings")]
    [SerializeField] private Transform shootRoot;
    [SerializeField] private BulletObjectView bulletPrefab;
    [SerializeField] private float dashDuration = 0.5f;
    [SerializeField] private float dashSpeedMultiplier = 2f;

    // List to store the history of head positions
    private List<Vector3> pathPoints = new List<Vector3>();

    public bool canMove = false;

    // (未使用到的變數，若無需要可移除)
    // private Transform portalTransform; 

    public Action<ScoreObjectView> onGetScore;
    public Action<Transform, Quaternion> onEnterPortal;
    public Action<Transform> onStartEnterPortal;
    public Action onHit;

    private Tween moveTween;

    public void OnHit(int damage)
    {
        onHit?.Invoke();
    }

    public void Setup(SnekkiesAsset skinAsset, Action completeCallback)
    {
        Debug.Log($"{nameof(SnekObjectView)}: Setup called with skinAsset: {skinAsset}");

        if (skinAsset == null)
        {
            Debug.LogError("SnekObjectView: skinAsset is null!");
            return;
        }

        if (headRenderer == null)
        {
            headRenderer = GetComponent<SpriteRenderer>();
            if (headRenderer == null)
            {
                headRenderer = GetComponentInChildren<SpriteRenderer>();
            }
        }

        if (headRenderer != null)
        {
            headRenderer.sprite = skinAsset.head;
        }
        else
        {
            Debug.LogError("SnekObjectView: headRenderer is missing!");
        }

        if (spriteShapeController == null)
        {
            spriteShapeController = GetComponentInChildren<SpriteShapeController>();
        }

        if (spriteShapeController != null)
        {
            if (spriteShapeController.spriteShape != null && spriteShapeController.spriteShape.angleRanges.Count > 0)
            {
                spriteShapeController.spriteShape.angleRanges[0].sprites = new List<Sprite> { skinAsset.body };
            }
            else
            {
                Debug.LogWarning("SnekObjectView: SpriteShape profile or angle ranges are missing/empty.");
            }
        }
        else
        {
            Debug.LogError("SnekObjectView: spriteShapeController is missing!");
        }

        completeCallback?.Invoke();
    }

    public void SetBodyLength(int length)
    {
        bodyLength = length;
    }

    public void SetSpeed(int newSpeed)
    {
        moveSpeed = newSpeed;
    }

    public void EnabledMove(bool canMove)
    {
        this.canMove = canMove;

        if (!canMove)
            return;
        moveTween?.Kill();
    }

    public void GoOutPortal(Vector3 startPosition, Vector3 direction)
    {
        // 1. 重置移動狀態與出洞狀態
        canMove = true;
        isExitingPortal = true;
        exitCurrentDistance = 0f; // 剛開始鑽出的距離為0

        // 2. 設定蛇頭的位置與方向
        transform.position = startPosition;

        // 根據出洞方向設定旋轉 (Up is forward)
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, targetAngle);

        // 3. 重置路徑紀錄，只保留起始點
        pathPoints.Clear();
        pathPoints.Add(startPosition);

        // 4. 強制更新一次身體 (此時因為 exitCurrentDistance 為 0，身體應全為寬度 0)
        UpdateSnakeBody();
    }
    // ----------------------------------------------

    void Start()
    {
        // Initialize path with current position
        pathPoints.Add(transform.position);

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
    }

    public void ManualUpdate(Vector2 moveDirection)
    {
        if (!canMove)
            return;

        MoveHead(moveDirection);
        UpdateSnakeBody();
    }

    void MoveHead(Vector2 moveDirection)
    {
        float moveStep = moveSpeed * Time.deltaTime;

        if (isExitingPortal)
        {
            // Spiral Logic: r = a + b * theta
            // Calculate angular speed to maintain constant linear speed
            // v = r * omega => omega = v / r
            float currentRadius = spiralStartRadius + spiralGrowthFactor * currentSpiralAngle;

            // Avoid division by zero
            if (currentRadius < 0.01f) currentRadius = 0.01f;

            float angularSpeed = moveSpeed / currentRadius;
            float deltaAngle = angularSpeed * Time.deltaTime;

            currentSpiralAngle += deltaAngle;

            // Recalculate radius with new angle
            float newRadius = spiralStartRadius + spiralGrowthFactor * currentSpiralAngle;

            float newX = newRadius * Mathf.Cos(currentSpiralAngle);
            float newY = newRadius * Mathf.Sin(currentSpiralAngle);

            Vector3 newPos = new Vector3(newX, newY, 0f);

            // Calculate rotation to face tangent
            // Tangent vector (-sin, cos)
            // Or just look at new position from old position
            Vector3 direction = (newPos - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
                transform.rotation = Quaternion.Euler(0, 0, targetAngle);
            }

            transform.position = newPos;

            exitCurrentDistance += moveStep;

            // Check completion
            float totalSnakeLength = bodyLength * pointSpacing;
            if (exitCurrentDistance >= totalSnakeLength + exitTransitionLength)
            {
                isExitingPortal = false;
                onExitComplete?.Invoke();
                onExitComplete = null;
            }
        }
        else
        {
            // Normal Movement
            transform.Translate(Vector3.up * moveStep);

            // Steer towards input direction
            if (moveDirection.sqrMagnitude > 0.01f)
            {
                float targetAngle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg - 90f;
                Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, steerSpeed * Time.deltaTime);
            }
        }

        RecordPath();
    }

    void RecordPath()
    {
        if (pathPoints.Count == 0)
        {
            pathPoints.Add(transform.position);
            return;
        }

        Vector3 lastPos = pathPoints[pathPoints.Count - 1];
        if (Vector3.Distance(transform.position, lastPos) > 0.05f)
        {
            pathPoints.Add(transform.position);
        }

        if (pathPoints.Count > 5000)
        {
            pathPoints.RemoveAt(0);
        }
    }

    void UpdateSnakeBody()
    {
        Spline spline = spriteShapeController.spline;
        spline.Clear();

        Vector3 currentPos = transform.position;

        // Add Head Point
        spline.InsertPointAt(0, Vector3.zero);
        spline.SetTangentMode(0, ShapeTangentMode.Continuous);

        float headWidth = bodyWidthCurve.Evaluate(0f);
        if (isExitingPortal)
        {
            // 如果正在出洞，蛇頭也要受距離影響 (0 -> 1)
            float headScale = Mathf.Clamp01(exitCurrentDistance / exitTransitionLength);
            headWidth *= headScale;
        }
        spline.SetHeight(0, headWidth);

        int pointsAdded = 1;
        float distanceTravelled = 0f;
        Vector3 lastSamplePos = currentPos;

        // Iterate backwards through the path points
        for (int i = pathPoints.Count - 1; i >= 0; i--)
        {
            Vector3 pt = pathPoints[i];
            float dist = Vector3.Distance(lastSamplePos, pt);

            while (distanceTravelled + dist >= pointSpacing)
            {
                float remaining = pointSpacing - distanceTravelled;
                float t = remaining / dist;
                Vector3 newPointPos = Vector3.Lerp(lastSamplePos, pt, t);

                Vector3 localPos = spriteShapeController.transform.InverseTransformPoint(newPointPos);
                spline.InsertPointAt(pointsAdded, localPos);
                spline.SetTangentMode(pointsAdded, ShapeTangentMode.Continuous);

                float curveT = (float)pointsAdded / (float)(bodyLength - 1);
                float originalWidth = bodyWidthCurve.Evaluate(curveT);

                // --- [Modified] 身體節點寬度動態縮放 ---
                float finalWidth = originalWidth;
                if (isExitingPortal)
                {
                    // 計算此節點距離蛇頭的總距離 (pointsAdded * pointSpacing)
                    float distFromHead = pointsAdded * pointSpacing;

                    // 計算此節點距離「出洞點」還有多遠
                    // 如果 exitCurrentDistance = 5, distFromHead = 3 -> 顯示 (差值 2)
                    // 如果 exitCurrentDistance = 5, distFromHead = 6 -> 消失 (差值 -1)
                    float distFromExitPoint = exitCurrentDistance - distFromHead;

                    // 計算縮放比例 (0~1)
                    float exitScale = Mathf.Clamp01(distFromExitPoint / exitTransitionLength);
                    finalWidth *= exitScale;
                }
                spline.SetHeight(pointsAdded, finalWidth);
                // -------------------------------------

                pointsAdded++;
                if (pointsAdded >= bodyLength) break;

                lastSamplePos = newPointPos;
                distanceTravelled = 0f;
                dist = Vector3.Distance(lastSamplePos, pt);
            }

            if (pointsAdded >= bodyLength) break;

            distanceTravelled += dist;
            lastSamplePos = pt;
        }

        // Extend backwards if ran out of history
        while (pointsAdded < bodyLength)
        {
            Vector3 direction = (pointsAdded > 1) ?
                (spline.GetPosition(pointsAdded - 1) - spline.GetPosition(pointsAdded - 2)).normalized :
                -Vector3.up;

            if (direction == Vector3.zero) direction = -Vector3.up;

            Vector3 newPos = spline.GetPosition(pointsAdded - 1) + direction * pointSpacing;
            spline.InsertPointAt(pointsAdded, newPos);
            spline.SetTangentMode(pointsAdded, ShapeTangentMode.Continuous);

            float curveT = (float)pointsAdded / (float)(bodyLength - 1);
            float originalWidth = bodyWidthCurve.Evaluate(curveT);

            // --- [Modified] 補點寬度動態縮放 ---
            float finalWidth = originalWidth;
            if (isExitingPortal)
            {
                float distFromHead = pointsAdded * pointSpacing;
                float distFromExitPoint = exitCurrentDistance - distFromHead;
                float exitScale = Mathf.Clamp01(distFromExitPoint / exitTransitionLength);
                finalWidth *= exitScale;
            }
            spline.SetHeight(pointsAdded, finalWidth);
            // ---------------------------------

            pointsAdded++;
        }
    }

    private void GoIntoPortal(Transform portalTransform)
    {
        canMove = false;
        Vector3 direction = (portalTransform.position - transform.position).normalized;

        // Save entry info for exit
        savedEntryPosition = portalTransform.position;
        savedEntryDirection = direction;

        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, targetAngle);

        onStartEnterPortal?.Invoke(portalTransform);

        float distance = (bodyLength * pointSpacing) + 5f;
        float duration = (distance / moveSpeed) / 2;

        Vector3 targetPos = transform.position + direction * distance;

        moveTween?.Kill();
        moveTween = transform.DOMove(targetPos, duration)
            .SetEase(Ease.Linear)
            .OnUpdate(() =>
            {
                RecordPath();
                UpdateSnakeBody();
            })
            .OnComplete(() =>
            {
                onEnterPortal?.Invoke(portalTransform, transform.rotation);
            });
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<ScoreObjectView>(out var scoreObj))
        {
            onGetScore?.Invoke(scoreObj);
        }
        if (collision.gameObject.TryGetComponent<PortalObjectView>(out var portalObj))
        {
            if (portalObj.isFake) return;
            GoIntoPortal(portalObj.transform);
        }
    }

    public void Shoot()
    {
        if (shootRoot != null && bulletPrefab != null)
        {
            BulletObjectView bullet = Instantiate(bulletPrefab, shootRoot.position, shootRoot.rotation * Quaternion.Euler(0, 0, 180f));
            bullet.Initialize(-shootRoot.up, BulletTarget.Boss);
        }
    }

    public void Dash()
    {
        StartCoroutine(DashRoutine());
    }

    private System.Collections.IEnumerator DashRoutine()
    {
        float originalSpeed = moveSpeed;
        float originalSteer = steerSpeed;

        moveSpeed *= dashSpeedMultiplier;
        steerSpeed *= dashSpeedMultiplier;

        yield return new WaitForSeconds(dashDuration);

        moveSpeed = originalSpeed;
        steerSpeed = originalSteer;
    }

    private void Awake()
    {
        gameObject.tag = "Player";
    }

    public void ExitPortal(Action onComplete)
    {
        canMove = true; // Allow ManualUpdate to run
        isExitingPortal = true;
        onExitComplete = onComplete;

        // Reset spiral state
        currentSpiralAngle = 0f;
        exitCurrentDistance = 0f;

        // Start at (0,0,0)
        Vector3 startPos = Vector3.zero;
        transform.position = startPos;
        exitPoint = startPos;

        // Reset path
        pathPoints.Clear();
        pathPoints.Add(startPos);

        // Initial rotation (arbitrary, will be updated by spiral movement)
        transform.rotation = Quaternion.identity;

        // Force update body once
        UpdateSnakeBody();
    }
}