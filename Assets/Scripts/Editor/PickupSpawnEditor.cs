using UnityEditor;
using UnityEngine;

namespace solobranch.ggj2025.Editor
{
    [CustomEditor(typeof(PickupManager))]
    public class PickupSpawnEditor : UnityEditor.Editor
    {
        private bool isPlacingSpawnPoints = false;
        private GameObject previewObject;

        private void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;

            if (previewObject != null)
            {
                DestroyImmediate(previewObject);
            }
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            PickupManager manager = (PickupManager)target;

            if (GUILayout.Button(isPlacingSpawnPoints ? "Stop Placing Spawn Points" : "Place Spawn Points"))
            {
                isPlacingSpawnPoints = !isPlacingSpawnPoints;

                if (isPlacingSpawnPoints)
                {
                    EditorUtility.DisplayDialog("Placement Mode",
                        "Click on the Scene View to place spawn points. Press Esc to exit placement mode.", "OK");
                    CreatePreviewObject();
                }
                else
                {
                    DestroyImmediate(previewObject); // Remove preview when stopping
                }

                SceneView.RepaintAll();
            }
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            if (!isPlacingSpawnPoints)
                return;

            Event e = Event.current;

            // Show preview
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (previewObject == null)
                {
                    CreatePreviewObject();
                }

                // Update position of the preview object to the hit point
                previewObject.transform.position = hit.point;
                previewObject.SetActive(true);

                // Place spawn point on click
                if (e.type == EventType.MouseDown && e.button == 0 && !e.alt) // Left-click to place
                {
                    PlaceSpawnPoint(hit.point);
                    e.Use();
                }
            }

            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape) // Esc to exit
            {
                isPlacingSpawnPoints = false;
                SceneView.RepaintAll();
                DestroyImmediate(previewObject); // Remove preview when exiting
            }
        }

        private void PlaceSpawnPoint(Vector3 position)
        {
            PickupManager manager = (PickupManager)target;

            if (manager.spawnPoints.Count >= 12)
            {
                EditorUtility.DisplayDialog("Limit Reached", "You can only have up to 12 spawn points.", "OK");
                return;
            }

            Undo.RecordObject(manager, "Add Spawn Point");
            manager.spawnPoints.Add(position);
            Debug.Log($"Spawn Point placed at: {position}");
        }

        private void CreatePreviewObject()
        {
            // Create a simple preview object (sphere) to visualize the spawn point
            if (previewObject == null)
            {
                previewObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                previewObject.name = "SpawnPointPreview";
                previewObject.GetComponent<Renderer>().material = new Material(Shader.Find("Standard"))
                {
                    color = new Color(1f, 1f, 0f, 0.3f) // Semi-transparent yellow
                };
                previewObject.transform.localScale = Vector3.one * 0.5f; // Adjust size of the preview
                previewObject.SetActive(false); // Initially hidden until placement mode starts
            }
        }
    }
}