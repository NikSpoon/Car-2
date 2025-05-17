using UnityEngine;

public class TargetFinder : MonoBehaviour
{
    public Transform CurrentTarget { get; private set; }

    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float searchInterval = 1f;
    private float _nextSearchTime = 0f;

    void Update()
    {
        if (Time.time >= _nextSearchTime)
        {
            FindClosestTarget();
            _nextSearchTime = Time.time + searchInterval;
        }
    }

    private void FindClosestTarget()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag(playerTag);
        Transform closest = null;
        float minDistance = Mathf.Infinity;

        foreach (var player in players)
        {
            float dist = Vector3.Distance(transform.position, player.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = player.transform;
            }
        }

        CurrentTarget = closest;
    }
}
