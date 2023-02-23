using System.Threading;
using System;
using UnityEngine;
using UnityEngine.Networking;
using GLTFImporter.API;
using Cysharp.Threading.Tasks;
using GLTFast;
using Sirenix.OdinInspector;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Events;

namespace GLTFImporter.Importer
{
    public class FoxImporter : MonoBehaviour
    {
        [Title("Importer")]
        [SerializeField] private string url = "https://github.com/KhronosGroup/glTF-Sample-Models/raw/master/2.0/Fox/glTF-Binary/Fox.glb";

        [Title("Components")]
        [SerializeField] private RuntimeAnimatorController animatorBase;

        [Title("Events")]
        [SerializeField] private UnityEvent<Animation> onChangeAnimation;

        private ImportSettings ImportSettings => new()
        {
            AnimationMethod = AnimationMethod.Mecanim,
        };

        private bool legacyAnimation = true;
        private bool isLoading;

        private void OnGUI()
        {
            // Right
            float rectHeight = 20;
            float rectWidth = 200;

            float rightEdge = Screen.width - 10;
            float margin = 10;

            Rect rectTextField = new(rightEdge - rectWidth, margin, rectWidth, rectHeight);
            Rect rectToggle = new(rightEdge - rectWidth, rectHeight + margin * 2, rectWidth, rectHeight);
            Rect rectFetch = new(rightEdge - rectWidth, rectHeight * 2 + margin * 3, rectWidth, rectHeight);

            // Right
            url = GUI.TextField(rectTextField, url);

            legacyAnimation = GUI.Toggle(rectToggle, legacyAnimation, "Legacy Animation");

            if (GUI.Button(rectFetch, "Fetch"))
            {
                UniTask.Create(() => LoadCharacterSkin());
            }
        }
        private async UniTask LoadCharacterSkin()
        {
            if (isLoading)
                return;

            isLoading = true;

            // Download Content
            UnityWebRequest request = new RequestBuilder(HTTPVerb.Get, url);
            byte[] data = await DownloadAsync(request);

            // Load GLB
            GltfImport gltf = new();

            ImportSettings importSettings = legacyAnimation ? null : ImportSettings;
            await gltf.LoadGltfBinary(data, importSettings: importSettings);

            // Replace the current skin
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            // Instantiate model
            await gltf.InstantiateMainSceneAsync(transform);


            if (legacyAnimation)
            {
                LoadLegacyAnimation(gltf);
            }
            else
            {
                LoadMecanimAnimation(gltf);
            }

            // Finish
            isLoading = false;
        }

        private void LoadLegacyAnimation(GltfImport gltf)
        {
            Animation animation = transform.GetComponentInChildren<Animation>();
            onChangeAnimation.Invoke(animation);
        }

        private void LoadMecanimAnimation(GltfImport gltf)
        {
            // Create a new instance of Override Controller
            AnimatorOverrideController animatorOverrideController = new AnimatorOverrideController(animatorBase);

            AnimationClip[] clips = gltf.GetAnimationClips();
            foreach (AnimationClip clip in clips)
            {
                animatorOverrideController[clip.name] = clip;
            }

            // Give a name to the override controller
            string skinName = gltf.GetSourceRoot().skins[0].name;
            animatorOverrideController.name = $"[Generated] {skinName}";

            // Assign the AnimatorOverrideController to the Animator component and force a reload
            Animator animator = transform.GetComponentInChildren<Animator>();
            animator.runtimeAnimatorController = animatorOverrideController;
            animator.Rebind();
            //animator.Update(0f);
        }

        #region Web Requests

        private static async UniTask<byte[]> DownloadAsync(UnityWebRequest unityWebRequest)
        {
            // Time out cancellation
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfterSlim(TimeSpan.FromSeconds(10));

            // Sending the request
            await unityWebRequest.SendWebRequest().WithCancellation(cts.Token);

            // Converts to result
            ApiResult requestResult = new ApiResult(unityWebRequest);
            byte[] bytes = requestResult.Data;

            unityWebRequest.Dispose();

            return bytes;
        }
        #endregion
    }
}