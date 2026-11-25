
using UnityEngine;

public class DetectionZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GuardPatrol guard =
                GetComponentInParent<GuardPatrol>();

            if (guard != null)
                guard.StartChase(other.transform);
        }
    }
}
