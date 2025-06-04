using UnityEngine;

public class TargetFinder : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private float detectionRadius = 30f;
    [SerializeField, Range(0, 180)] private float detectionAngle = 90f; // Половина угла обзора вперед

    public Transform CurrentTarget { get; private set; }
    private Transform lastTarget = null;
    public Transform curent;

    void Update()
    {
        FindClosestTarget();
        curent = CurrentTarget;
    }

    private void FindClosestTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, targetLayer);

        float closestDistance = Mathf.Infinity;
        Transform closestTarget = null;

        Vector3 forward = transform.forward;

        foreach (var hit in hits)
        {
            if (hit.transform == transform) continue;

            Vector3 directionToTarget = (hit.transform.position - transform.position).normalized;
            float angle = Vector3.SignedAngle(forward, directionToTarget, Vector3.up);

            // Проверяем, что цель находится в пределах ±detectionAngle
            if (angle < -detectionAngle || angle > detectionAngle)
                continue;

            float distance = Vector3.Distance(transform.position, hit.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = hit.transform;
            }
        }

        if (closestTarget != lastTarget)
        {
            lastTarget = closestTarget;
        }
        CurrentTarget = closestTarget;
    }

    private void OnDrawGizmosSelected()
    {
        // Визуализация радиуса обнаружения
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Визуализация сектора обзора (передняя полусфера ±detectionAngle)
        Vector3 forward = transform.forward;

        // Левая граница сектора
        Vector3 leftBoundary = Quaternion.Euler(0, -detectionAngle, 0) * forward * detectionRadius;
        // Правая граница сектора
        Vector3 rightBoundary = Quaternion.Euler(0, detectionAngle, 0) * forward * detectionRadius;

        Gizmos.color = new Color(0, 1, 0, 0.3f);
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);

        // Нарисуем заполненный сектор с помощью нескольких линий
        int segments = 20;
        float angleStep = detectionAngle * 2 / segments;
        Vector3 previousPoint = transform.position + leftBoundary;
        for (int i = 1; i <= segments; i++)
        {
            float currentAngle = -detectionAngle + angleStep * i;
            Vector3 nextPoint = transform.position + Quaternion.Euler(0, currentAngle, 0) * forward * detectionRadius;
            Gizmos.DrawLine(previousPoint, nextPoint);
            previousPoint = nextPoint;
        }
    }
}
