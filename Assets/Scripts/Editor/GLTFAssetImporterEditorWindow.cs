using Cysharp.Threading.Tasks;
using GLTFast;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.Threading;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using GLTFast.Schema;
using System.Linq;
using CharacterXYZ.GLTF;

namespace CharacterXYZ.Editor
{
    public class GLTFAssetImporterEditorWindow : OdinEditorWindow
    {
        public string savePath = "Assets/";

        private const string WindowName = "GLTF Asset Importer";

        [MenuItem("Character XYZ/" + WindowName)]
        public static void ShowWindow()
        {
            GetWindow<GLTFAssetImporterEditorWindow>(WindowName).Show();
        }

        [Button]
        public void LoadA(CharacterInfoData data)
        {
            UniTask.Create(() => LoadAndSaveCharacterParallel(data.mesh, data.animations));
        }

        private async UniTask LoadAndSaveCharacterParallel(string mesh, Dictionary<string, string> animations)
        {
            IDeferAgent defaultDeferAgent = new EditorDeferAgent();

            Debug.Log("Load Begin");

            // Animations
            IEnumerable<UniTask> tasks = animations.Select(async pair =>
            {
                // Download the animation data asynchronously
                byte[] response = await DownloadGLBAsync(pair.Value);

                Debug.Log($"New Animation GLB: {pair.Key}");

                // Load the animation data
                GltfImport gltfAnimation = new GltfImport(null, defaultDeferAgent);
                bool success = await gltfAnimation.LoadGltfBinary(response);

                if (!success)
                    return;

                // Save the animation data
                SaveAnimation(pair.Key, gltfAnimation);

                Debug.Log("Added Anim: " + pair.Key);
            });

            // Model
            tasks = tasks.Append(UniTask.Create(async () =>
            {
                // Download the model data asynchronously
                byte[] data = await DownloadGLBAsync(mesh);

                Debug.Log($"New Model GLB");

                // Load the model data
                GltfImport gltf = new GltfImport(null, defaultDeferAgent);
                bool success = await gltf.LoadGltfBinary(data);

                if (!success)
                    return;

                // Save the model data
                SaveModel(data, gltf);

                Debug.Log($"Added Model GLB: {gltf.GetSourceRoot().skins[0].name}");
            }));

            await UniTask.WhenAll(tasks);

            Debug.Log("Load Done");
        }

        private void SaveModel(byte[] data, GltfImport gltf)
        {
            string path = savePath + gltf.GetSourceRoot().skins[0].name + ".glb";
            File.WriteAllBytes(Application.dataPath + "/" + path, data);
            AssetDatabase.ImportAsset(path);
        }

        private void SaveAnimation(string animationName, GltfImport gltfAnimation)
        {
            AnimationClip[] clips = gltfAnimation.GetAnimationClips();

            if (clips == null)
                return;

            foreach (AnimationClip clip in clips)
            {
                clip.legacy = false;
                AssetDatabase.CreateAsset(clip, savePath + animationName + ".anim");
            }
        }

        private static async UniTask<byte[]> DownloadGLBAsync(string url)
        {
            UnityWebRequest unityWebRequest = UnityWebRequest.Get(url);

            // Time out cancellation
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfterSlim(TimeSpan.FromSeconds(10));

            // Sending the request
            await unityWebRequest.SendWebRequest().WithCancellation(cts.Token);

            // Converts to result
            byte[] data = unityWebRequest.downloadHandler.data;

            unityWebRequest.Dispose();

            return data;
        }
    }
}