
using System.Collections.Generic;
using UnityEngine;

public class VisibilityAnalyzer : MonoBehaviour
{
    [Header("Robot Camera")]
    public Camera playerCamera;

    [Header("Environment Objects")]
    public Transform objectsRoot;

    [Header("Visibility Settings")]
    public float checkInterval = 0.1f;

    [Header("Visibility Materials")]
    public Material visibleMaterial;
    public Material hiddenMaterial;

    private class TrackedObject
    {
        public Transform target;
        public Renderer[] renderers;
        public Material[][] originalMaterials;
    }

    private readonly List<TrackedObject> trackedObjects =
        new List<TrackedObject>();

    private float timer;

    void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        if (objectsRoot == null)
        {
            Debug.LogError(
                "Assign Crates Barrels Pallets to Objects Root."
            );
            enabled = false;
            return;
        }

        FindObjectsToAnalyze();
        CheckAllObjects();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= checkInterval)
        {
            timer = 0f;
            CheckAllObjects();
        }
    }

    void FindObjectsToAnalyze()
    {
        trackedObjects.Clear();

        Transform[] allObjects =
            objectsRoot.GetComponentsInChildren<Transform>(true);

        foreach (Transform item in allObjects)
        {
            string objectName = item.name.ToLowerInvariant();

            bool isTarget =
                objectName.Contains("barrel") ||
                objectName.Contains("crate") ||
                objectName.Contains("create");

            if (!isTarget)
                continue;

            // Avoid analysing the same object twice.
            Transform parent = item.parent;
            bool alreadyInsideTarget = false;

            while (parent != null && parent != objectsRoot)
            {
                string parentName = parent.name.ToLowerInvariant();

                if (parentName.Contains("barrel") ||
                    parentName.Contains("crate") ||
                    parentName.Contains("create"))
                {
                    alreadyInsideTarget = true;
                    break;
                }

                parent = parent.parent;
            }

            if (alreadyInsideTarget)
                continue;

            Renderer[] renderers =
                item.GetComponentsInChildren<Renderer>(true);

            if (renderers.Length == 0)
                continue;

            TrackedObject tracked = new TrackedObject();
            tracked.target = item;
            tracked.renderers = renderers;
            tracked.originalMaterials = new Material[renderers.Length][];

            for (int i = 0; i < renderers.Length; i++)
            {
                tracked.originalMaterials[i] =
                    renderers[i].sharedMaterials;
            }

            trackedObjects.Add(tracked);
        }

        Debug.Log(
            "RoboSight analysing " +
            trackedObjects.Count + " barrels and crates."
        );
    }

    void CheckAllObjects()
    {
        if (playerCamera == null)
            return;

        foreach (TrackedObject item in trackedObjects)
        {
            if (item.target == null)
                continue;

            Bounds bounds = GetObjectBounds(item);

            Vector3 targetPosition = bounds.center;
            Vector3 viewport =
                playerCamera.WorldToViewportPoint(targetPosition);

            bool insideCameraView =
                viewport.z > 0 &&
                viewport.x >= 0 && viewport.x <= 1 &&
                viewport.y >= 0 && viewport.y <= 1;

            bool visible = false;

            if (insideCameraView)
            {
                Vector3 origin = playerCamera.transform.position;
                Vector3 direction = targetPosition - origin;
                float distance = direction.magnitude;

                if (distance > 0.01f)
                {
                    RaycastHit[] hits = Physics.RaycastAll(
                        origin,
                        direction.normalized,
                        distance + 0.2f
                    );

                    System.Array.Sort(
                        hits,
                        (a, b) => a.distance.CompareTo(b.distance)
                    );

                    foreach (RaycastHit hit in hits)
                    {
                        // Ignore colliders belonging to the camera
                        // or its parent player object.
                        if (hit.transform == playerCamera.transform ||
                            playerCamera.transform.IsChildOf(hit.transform))
                            continue;

                        visible =
                            hit.transform == item.target ||
                            hit.transform.IsChildOf(item.target);

                        // The first relevant collider decides visibility.
                        break;
                    }
                }
            }

            Material selectedMaterial =
                visible ? visibleMaterial : hiddenMaterial;

            if (selectedMaterial != null)
            {
                foreach (Renderer renderer in item.renderers)
                {
                    if (renderer == null)
                        continue;

                    Material[] materials =
                        renderer.sharedMaterials;

                    for (int i = 0; i < materials.Length; i++)
                        materials[i] = selectedMaterial;

                    renderer.sharedMaterials = materials;
                }
            }

            Debug.Log(
                item.target.name + " : " +
                (visible ? "VISIBLE" : "OCCLUDED / OUT OF VIEW")
            );
        }
    }

    Bounds GetObjectBounds(TrackedObject item)
    {
        Bounds bounds = new Bounds(
            item.target.position,
            Vector3.zero
        );

        bool foundBounds = false;

        foreach (Renderer renderer in item.renderers)
        {
            if (renderer == null || !renderer.enabled)
                continue;

            if (!foundBounds)
            {
                bounds = renderer.bounds;
                foundBounds = true;
            }
            else
            {
                bounds.Encapsulate(renderer.bounds);
            }
        }

        return bounds;
    }

    void OnDisable()
    {
        // Restore original materials when analysis stops.
        foreach (TrackedObject item in trackedObjects)
        {
            for (int i = 0; i < item.renderers.Length; i++)
            {
                if (item.renderers[i] != null)
                {
                    item.renderers[i].sharedMaterials =
                        item.originalMaterials[i];
                }
            }
        }
    }
}
