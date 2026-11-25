
using System.Collections;
using UnityEngine;
using Unity.AI.Navigation;

public class DynamicObstacleSpawner : MonoBehaviour
{
    public GameObject obstaclePrefab;
    public Camera playerCamera;
    public NavMeshSurface navMeshSurface;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            SpawnObstacle();

        if (Input.GetMouseButtonDown(1))
            RemoveObstacle();
    }

    void SpawnObstacle()
    {
        if (obstaclePrefab == null || playerCamera == null)
            return;

        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f,
            Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            float height = obstaclePrefab.transform.localScale.y / 2f;
            Vector3 position = hit.point + Vector3.up * height;

            Instantiate(obstaclePrefab, position, Quaternion.identity);

            StartCoroutine(UpdateSurface());
        }
    }

    void RemoveObstacle()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f,
            Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            UnityEngine.AI.NavMeshObstacle obstacle =
                hit.collider.GetComponent<UnityEngine.AI.NavMeshObstacle>();

            if (obstacle != null)
            {
                Destroy(obstacle.gameObject);
                StartCoroutine(UpdateSurface());
            }
        }
    }

    IEnumerator UpdateSurface()
    {
        yield return null;

        if (navMeshSurface != null &&
            navMeshSurface.navMeshData != null)
        {
            yield return navMeshSurface.UpdateNavMesh(
                navMeshSurface.navMeshData
            );
        }
    }
}
