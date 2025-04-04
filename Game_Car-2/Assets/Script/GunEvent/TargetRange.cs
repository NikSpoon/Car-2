
using UnityEngine;

public class TargetRange : MonoBehaviour
{
    public bool isInRange { get; private set; } = false;
    private int playerCounter = 0;
    [SerializeField] private GameObject[] _targets;

    private void OnTriggerEnter(Collider other)
    {
        if (IsTarget(other))
        {
            playerCounter++;
            isInRange = playerCounter > 0;
            Debug.Log($"Player ENTERED: Counter = {playerCounter}, isInRange = {isInRange}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsTarget(other))
        {
            playerCounter--;
            isInRange = playerCounter > 0;
            Debug.Log($"Player EXITED: Counter = {playerCounter}, isInRange = {isInRange}");
        }
    }
    private bool IsTarget(Collider other)
    {
        
        return other.CompareTag("Player") || System.Array.Exists(_targets, target => target == other.gameObject);
    }
    void OnDrawGizmos()
    {
        Gizmos.color = isInRange ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, GetComponent<SphereCollider>().radius);
    }
}
