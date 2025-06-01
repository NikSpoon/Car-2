using UnityEngine;

public class TargetFinder : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private float detectionRadius = 30f;   
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

        foreach (var hit in hits)
        {
            if (hit.transform == transform) continue;

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
    }
}
