using UnityEngine;

public class CheckpointManager : MonoBehaviour
{

    [SerializeField] private Transform _track;
    public Transform[] CheckpointsWithStart { get; private set; }
    public Transform[] Checkpoints { get; private set; }
    public int Len { get; private set; }
    private void Awake()
    {
        
        Len = _track.childCount;
        Checkpoints = new Transform[Len];

        for (int i = 0; i < Len; i++)
        {
            Transform checkpoint = _track.GetChild(i);
            if (checkpoint != null)
            {
                Checkpoints[i] = checkpoint;
                Debug.Log("Checkpoint " + i + " is set.");
            }
            else
            {
                Debug.LogError("Checkpoint " + i + " is null!");
            }
        }
      
    }
    void Start()
    {
        CheckpointsWithStart = new Transform[Len + 1];

        CheckpointsWithStart[0] = transform; 

        for (int i = 0; i < Len; i++)
        {
            CheckpointsWithStart[i + 1] = Checkpoints[i];
        }

    }
}
