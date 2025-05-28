using UnityEngine;

public class TargetFinder : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private float detectionRadius = 30f;   
    public Transform CurrentTarget { get; private set; }

    void Update()
    {
        FindClosestTarget();
    }

    private void FindClosestTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, targetLayer);

        float closestDistance = Mathf.Infinity;
        Transform closestTarget = null;

        foreach (var hit in hits)
        {
            float distance = Vector3.Distance(transform.position, hit.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = hit.transform;
            }
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
