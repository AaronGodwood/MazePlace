using System.Linq;
using MazePlace.Game;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace MazePlace.Editor
{
    /// <summary>
    /// Builds the project's materials and scene from code, so they can be recreated rather than
    /// hand-edited. Run from the menu or headless: -executeMethod MazePlace.Editor.ProjectSetup.Run
    /// </summary>
    public static class ProjectSetup
    {
        private const string MaterialsFolder = "Assets/Materials";
        private const string ScenePath = "Assets/Scenes/Maze.unity";

        [MenuItem("MazePlace/Rebuild Scene")]
        public static void Run()
        {
            EnsureFolder(MaterialsFolder);
            EnsureFolder("Assets/Scenes");

            var wall = CreateMaterial("Wall", new Color(0.86f, 0.84f, 0.80f));
            var floor = CreateMaterial("Floor", new Color(0.33f, 0.35f, 0.38f));
            var start = CreateMaterial("Start", new Color(0.30f, 0.75f, 0.40f));
            var goal = CreateMaterial("Goal", new Color(0.85f, 0.30f, 0.30f));
            var explored = CreateMaterial("Explored", new Color(0.35f, 0.55f, 0.90f));
            var path = CreateMaterial("Path", new Color(1.00f, 0.80f, 0.25f));

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var light = new GameObject("Sun", typeof(Light)).GetComponent<Light>();
            light.type = LightType.Directional;
            light.shadows = LightShadows.Soft;
            light.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.45f, 0.45f, 0.5f);

            var overview = CreateCamera("Overview Camera", null);
            overview.gameObject.AddComponent<AudioListener>();
            overview.gameObject.AddComponent<OverviewCamera>();

            var player = new GameObject("Player", typeof(CharacterController), typeof(PlayerController));
            var body = player.GetComponent<CharacterController>();
            body.height = 1.6f;
            body.radius = 0.35f;
            body.center = new Vector3(0f, 0.8f, 0f);
            var head = CreateCamera("Head", player.transform);
            head.transform.localPosition = new Vector3(0f, 1.5f, 0f);
            head.nearClipPlane = 0.05f;
            head.gameObject.AddComponent<AudioListener>();

            // Lower levels are roofed over by the floor above, so the sun never reaches them.
            var lantern = new GameObject("Lantern", typeof(Light)).GetComponent<Light>();
            lantern.transform.SetParent(head.transform, worldPositionStays: false);
            lantern.type = LightType.Point;
            lantern.range = 14f;
            lantern.intensity = 1.5f;
            lantern.color = new Color(1f, 0.93f, 0.82f);
            Assign(player.GetComponent<PlayerController>(), ("head", head.transform));

            var maze = new GameObject("Maze", typeof(MazeRenderer), typeof(SolveAnimator), typeof(MazeSession));
            Assign(maze.GetComponent<MazeRenderer>(), ("wallMaterial", wall), ("floorMaterial", floor), ("startMaterial", start), ("goalMaterial", goal));
            Assign(maze.GetComponent<SolveAnimator>(), ("exploredMaterial", explored), ("pathMaterial", path));
            Assign(maze.GetComponent<MazeSession>(),
                ("mazeRenderer", maze.GetComponent<MazeRenderer>()),
                ("solveAnimator", maze.GetComponent<SolveAnimator>()),
                ("overview", overview.GetComponent<OverviewCamera>()),
                ("player", player.GetComponent<PlayerController>()));
            player.SetActive(false);

            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
            AssetDatabase.SaveAssets();
        }

        private static Camera CreateCamera(string name, Transform parent)
        {
            var camera = new GameObject(name, typeof(Camera)) { tag = "MainCamera" }.GetComponent<Camera>();
            camera.transform.SetParent(parent, worldPositionStays: false);
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.12f, 0.13f, 0.16f);
            return camera;
        }

        private static Material CreateMaterial(string name, Color colour)
        {
            var path = $"{MaterialsFolder}/{name}.mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                material = new Material(Shader.Find("Standard"));
                AssetDatabase.CreateAsset(material, path);
            }
            material.color = colour;
            material.SetFloat("_Glossiness", 0.1f);
            EditorUtility.SetDirty(material);
            return material;
        }

        private static void Assign(Object target, params (string Field, Object Value)[] fields)
        {
            var serialized = new SerializedObject(target);
            foreach (var (field, value) in fields)
                serialized.FindProperty(field).objectReferenceValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            var parts = path.Split('/');
            AssetDatabase.CreateFolder(string.Join("/", parts.Take(parts.Length - 1)), parts.Last());
        }
    }
}
