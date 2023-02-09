using System.Threading;
using System;
using UnityEngine;
using UnityEngine.Networking;
using CharacterXYZ.API;
using Cysharp.Threading.Tasks;
using GLTFast;
using Sirenix.OdinInspector;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.TextCore.Text;

namespace CharacterXYZ.GLTF
{
    public class GLTFBinaryImporter : MonoBehaviour
    {
        [Title("GLPImporter")]
        [SerializeField] private string baseUrl = "https://us-central1-charaktor-933a4.cloudfunctions.net/server/api/integration-test";
        [SerializeField] private string accessKey = "cxyz-tamashi-hoodoff";

        [Title("Components")]
        [SerializeField] private Animator animator;
        [SerializeField] private RuntimeAnimatorController animatorBase;

        [Title("Skins")]
        [SerializeField] private CharacterInfoData hoodOn;
        [SerializeField] private CharacterInfoData hoodOff;

        private bool isDownloading;

        #region Buttons
        /// <summary> Update the character by Query Params Values </summary>
        [Button]
        public void Fetch(string accessValue)
        {
            UniTask.Create(() => Request(accessValue));
        }

        /// <summary> Update the character by CharacterInfoData </summary>
        [Button]
        public void LoadCharacterData(CharacterInfoData skin)
        {
            CharacterInfo character = new CharacterInfo()
            {
                mesh = new MeshData() { glb = skin.mesh },
                animations = skin.animations
            };

            UniTask.Create(() => LoadCharacterSkin(character));
        }

        private void OnGUI()
        {
            float buttonWidth = 100;
            float buttonHeight = 50;
            float textFieldWidth = 200;
            float textFieldHeight = 20;

            float rightEdge = Screen.width - 10;
            float topEdge = 10;

            Rect rectSkinA = new Rect(10, topEdge, buttonWidth, buttonHeight);
            Rect rectSkinB = new Rect(10, topEdge + buttonHeight + 10, buttonWidth, buttonHeight);
            Rect rectTextField = new Rect(rightEdge - textFieldWidth, topEdge, textFieldWidth, textFieldHeight);
            Rect rectFetch = new Rect(rightEdge - textFieldWidth, topEdge + textFieldHeight + 10, textFieldWidth, buttonHeight);

            if (GUI.Button(rectSkinA, "Hood On"))
            {
                LoadCharacterData(hoodOn);
            }

            if (GUI.Button(rectSkinB, "Hood Off"))
            {
                LoadCharacterData(hoodOff);
            }

            accessKey = GUI.TextField(rectTextField, accessKey, 25);

            if (GUI.Button(rectFetch, "Fetch"))
            {
                Fetch(accessKey);
            }
        }
        #endregion

        private async UniTask Request(string accessValue)
        {
            if (isDownloading)
                return;

            isDownloading = true;

            // Get character Data
            UnityWebRequest request = new RequestBuilder(HTTPVerb.Get, baseUrl).WithParams(accessKey, accessValue);
            ApiResult<CharacterData> userDataResult = await RequestT<CharacterData>(request);

            // Update skin
            if (userDataResult.Successful)
            {
                await LoadCharacterSkin(userDataResult.Value.response);
            }

            isDownloading = false;
        }

        private async UniTask LoadCharacterSkin(CharacterInfo characterInfo)
        {
            if (isDownloading)
                return;

            isDownloading = true;

            // Create a new instance of Override Controller
            AnimatorOverrideController animatorOverrideController = new AnimatorOverrideController(animatorBase);

            // Animations
            IEnumerable<UniTask> tasks = characterInfo.animations.Select(async pair =>
            {
                // Download animation data
                byte[] response = await RequestUrlData(pair.Value);

                // Load animation
                GltfImport gltfAnimation = new();
                bool success = await gltfAnimation.LoadGltfBinary(response);

                if (!success)
                    return;

                // Create a new AnimationClip and add it to the AnimatorOverrideController
                AnimationClip[] clips = gltfAnimation.GetAnimationClips();

                if (clips == null)
                    return;

                // Set new animation
                clips[0].legacy = false;
                animatorOverrideController[pair.Key] = clips[0];
            });

            UniTask replaceSkin = UniTask.CompletedTask;

            // Model
            tasks = tasks.Append(UniTask.Create(async () =>
            {
                // Download model data
                byte[] data = await RequestUrlData(characterInfo.mesh.glb);

                // Load model
                GltfImport gltf = new();
                bool success = await gltf.LoadGltfBinary(data);

                if (!success)
                    return;

                replaceSkin = ReplaceSkin(gltf, animatorOverrideController);
            }));

            // Wait for all tasks to complete
            await UniTask.WhenAll(tasks);
            await replaceSkin;

            isDownloading = false;
        }

        private async UniTask ReplaceSkin(GltfImport gltf, AnimatorOverrideController animatorOverrideController)
        {
            // Replace the current skin
            foreach (Transform child in animator.transform)
            {
                Destroy(child.gameObject);
            }

            // Instantiate model
            await gltf.InstantiateMainSceneAsync(animator.transform);

            // Give a name to the override controller
            string skinName = gltf.GetSourceRoot().skins[0].name;
            animatorOverrideController.name = $"[Generated] {skinName}";

            // Assign the AnimatorOverrideController to the Animator component and force a reload
            animator.runtimeAnimatorController = animatorOverrideController;
            animator.Rebind();
            animator.Update(0f);
        }

        #region Web Requests
        private static async UniTask<byte[]> RequestUrlData(string url)
        {
            ApiResult requestResult = await RequestUrl(url);
            return requestResult.Data;
        }

        private static async UniTask<ApiResult> RequestUrl(string url)
        {
            UnityWebRequest unityWebRequest = UnityWebRequest.Get(url);
            return await DownloadAsync(unityWebRequest);
        }

        private static async UniTask<ApiResult<T>> RequestT<T>(UnityWebRequest unityWebRequest)
        {
            ApiResult requestResult = await DownloadAsync(unityWebRequest);
            return ApiResult<T>.TConvert(requestResult);
        }

        private static async UniTask<ApiResult> DownloadAsync(UnityWebRequest unityWebRequest)
        {
            // Time out cancellation
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfterSlim(TimeSpan.FromSeconds(10));

            // Sending the request
            await unityWebRequest.SendWebRequest().WithCancellation(cts.Token);

            // Converts to result
            ApiResult requestResult = new ApiResult(unityWebRequest);

            unityWebRequest.Dispose();

            return requestResult;
        }
        #endregion
    }
}