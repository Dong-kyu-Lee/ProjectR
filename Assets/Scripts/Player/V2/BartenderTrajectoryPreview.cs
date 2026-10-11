using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// 술병을 생성하거나 소모하지 않고 현재 월드의 충돌체를 기준으로 경로를 예측한다.
public class BartenderTrajectoryPreview : MonoBehaviour
{
    [SerializeField] private Material lineMaterial;
    [SerializeField, Min(0.01f)] private float lineWidth = 0.045f;
    [SerializeField] private Color primaryColor = new Color(1f, 0.85f, 0.45f, 0.8f);
    [SerializeField] private Color secondaryColor = new Color(1f, 0.85f, 0.45f, 0.4f);

    private LineRenderer primaryLine;
    private LineRenderer secondaryLine;
    private Vector3[] points;
    private readonly List<RaycastHit2D> hits = new List<RaycastHit2D>(16);

    public void Show(ProjectileV2 projectile, Vector3 origin, Vector2 direction,
        bool showSecondThrow, SpriteRenderer playerSprite)
    {
        if (projectile == null || lineMaterial == null)
        {
            Hide();
            return;
        }

        if (primaryLine == null)
            primaryLine = CreateLine("Bottle trajectory", primaryColor);

        Draw(primaryLine, projectile, origin, direction, playerSprite);

        if (showSecondThrow)
        {
            if (secondaryLine == null)
                secondaryLine = CreateLine("Second bottle trajectory", secondaryColor);

            Draw(secondaryLine, projectile, origin,
                ProjectileV2.GetSecondThrowDirection(direction), playerSprite);
        }
        else if (secondaryLine != null)
        {
            secondaryLine.enabled = false;
        }
    }

    public void Hide()
    {
        if (primaryLine != null) primaryLine.enabled = false;
        if (secondaryLine != null) secondaryLine.enabled = false;
    }

    private LineRenderer CreateLine(string objectName, Color color)
    {
        GameObject lineObject = new GameObject(objectName);
        lineObject.transform.SetParent(transform, false);
        lineObject.layer = gameObject.layer;

        LineRenderer line = lineObject.AddComponent<LineRenderer>();
        line.sharedMaterial = lineMaterial;
        line.useWorldSpace = true;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth * 0.5f;
        line.startColor = color;
        line.endColor = new Color(color.r, color.g, color.b, color.a * 0.3f);
        line.numCapVertices = 2;
        line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        line.receiveShadows = false;
        line.enabled = false;
        return line;
    }

    private void Draw(LineRenderer line, ProjectileV2 projectile, Vector3 origin,
        Vector2 direction, SpriteRenderer playerSprite)
    {
        int capacity = Mathf.CeilToInt(projectile.Lifetime / Time.fixedDeltaTime) + 1;
        if (points == null || points.Length != capacity)
            points = new Vector3[capacity];

        int count = CalculatePath(gameObject.scene.GetPhysicsScene2D(), projectile,
            origin, direction, points);

        if (playerSprite != null)
        {
            line.sortingLayerID = playerSprite.sortingLayerID;
            line.sortingOrder = playerSprite.sortingOrder + 1;
        }

        line.positionCount = count;
        for (int i = 0; i < count; i++)
            line.SetPosition(i, points[i]);
        line.enabled = count > 1;
    }

    // 각 물리 프레임마다 속도에 중력을 더한 뒤 위치를 옮긴다.
    // 연속 포물선 공식만 사용했을 때 생기는 실제 Rigidbody2D와의 오차를 줄인다.
    // 움직이는 적/발판의 미래 위치까지 예측하지는 않는다.
    public int CalculatePath(PhysicsScene2D physicsScene, ProjectileV2 projectile,
        Vector3 origin, Vector2 direction, Vector3[] result)
    {
        if (result == null || result.Length == 0) return 0;

        CircleCollider2D circle = projectile.GetComponent<CircleCollider2D>();
        Vector3 scale = projectile.transform.lossyScale;
        float radius = circle != null
            ? circle.radius * Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.y)) : 0f;
        Vector2 offset = circle != null ? Vector2.Scale(circle.offset, (Vector2)scale) : Vector2.zero;
        int projectileLayer = projectile.gameObject.layer;
        float drag = projectile.projectileRigidbody.drag;
        float step = Time.fixedDeltaTime;
        int steps = Mathf.CeilToInt(projectile.Lifetime / step);
        Vector2 position = origin;
        Vector2 velocity = projectile.GetLaunchVelocity(direction);
        result[0] = origin;
        int count = 1;

        // Effector의 Collider Mask가 전역 레이어 설정을 대체할 수 있어,
        // 먼저 모든 비트리거 충돌체를 조회한 뒤 실제 충돌 조건을 판별한다.
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(Physics2D.AllLayers);
        filter.useTriggers = false;

        for (int i = 0; i < steps && count < result.Length; i++)
        {
            velocity += projectile.Gravity * step;
            velocity *= Mathf.Clamp01(1f - drag * step);
            Vector2 displacement = velocity * step;
            float distance = displacement.magnitude;
            if (distance > Physics2D.maxTranslationSpeed * step)
            {
                displacement = displacement.normalized * Physics2D.maxTranslationSpeed * step;
                velocity = displacement / step;
                distance = displacement.magnitude;
            }

            bool blocked = false;
            Vector2 next = position + displacement;
            if (distance > 0.00001f)
            {
                physicsScene.CircleCast(position + offset, radius,
                    displacement / distance, distance, filter, hits);

                float nearest = float.PositiveInfinity;
                foreach (RaycastHit2D hit in hits)
                {
                    if (hit.distance >= nearest || !BlocksThrow(hit, projectileLayer, velocity))
                        continue;

                    nearest = hit.distance;
                    next = hit.centroid - offset;
                    blocked = true;
                }
            }

            result[count++] = new Vector3(next.x, next.y, origin.z);
            if (blocked) break;
            position = next;
        }

        return count;
    }

    private bool BlocksThrow(RaycastHit2D hit, int projectileLayer, Vector2 velocity)
    {
        Collider2D collider = hit.collider;
        if (collider == null || collider.transform.IsChildOf(transform)) return false;

        PlatformEffector2D platform = null;
        if (collider.usedByEffector)
        {
            platform = collider.GetComponent<PlatformEffector2D>();
            if (platform == null && collider.attachedRigidbody != null)
                platform = collider.attachedRigidbody.GetComponent<PlatformEffector2D>();
        }

        if (platform != null && platform.isActiveAndEnabled)
        {
            bool canCollide = platform.useColliderMask
                ? (platform.colliderMask & (1 << projectileLayer)) != 0
                : !Physics2D.GetIgnoreLayerCollision(projectileLayer, collider.gameObject.layer);
            if (!canCollide) return false;

            if (platform.useOneWay)
            {
                Vector2 surfaceUp = Quaternion.Euler(0f, 0f, platform.rotationalOffset)
                    * platform.transform.up;
                Vector2 relativeVelocity = velocity;
                if (collider.attachedRigidbody != null)
                    relativeVelocity -= collider.attachedRigidbody.GetPointVelocity(hit.point);

                // 아래에서 통과하는 병과 표면 허용 각도 밖의 접촉을 제외한다.
                if (Vector2.Dot(relativeVelocity, surfaceUp) >= 0f) return false;
                if (Vector2.Angle(hit.normal, surfaceUp) > platform.surfaceArc * 0.5f)
                    return false;
            }

            return true;
        }

        return !Physics2D.GetIgnoreLayerCollision(projectileLayer, collider.gameObject.layer);
    }

    private void OnDisable()
    {
        Hide();
    }
}
