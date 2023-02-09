using CharacterXYZ.Shared;
using UnityEngine;

namespace CharacterXYZ.Gameplay
{

    public class MainCamera : Singleton<MainCamera>
    {
        public static Transform Transform => Instance.transform;
        public static Transform PlayerTarget { get; private set; }

        public static void SetPlayerTarget(Transform cameraTarget)
        {
            PlayerTarget = cameraTarget;
        }
    }
}