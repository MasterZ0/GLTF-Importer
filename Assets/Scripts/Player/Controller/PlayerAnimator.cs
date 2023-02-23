using System;
using UnityEngine;

namespace GLTFImporter.Player
{
    [Serializable]
    public sealed class PlayerAnimator : PlayerControllerComponent
    {
        [Header("Player Animator")]
        [SerializeField] private Animation animation;

        [Header("States")]
        [SerializeField] private string idleState = "Idle";
        [SerializeField] private string walkState = "Walk";
        [SerializeField] private string runState = "Run";
        [SerializeField] private string jumpState = "Jump";
        [SerializeField] private string fallingState = "Falling";
        [SerializeField] private string landingState = "Landing";

        private string idleOverride;

        internal void ChangeAnimation(Animation newAnimation)
        {
            animation = newAnimation;
            Play(idleState);
        }

        internal void Idle()
        {
            if (string.IsNullOrEmpty(idleOverride))
            {
                idleOverride = idleState;
            }

            Play(idleOverride);
            idleOverride = string.Empty;
        }

        internal void OverrideIdleAsLanding() => idleOverride = landingState;
        internal void Falling() => Play(fallingState);
        internal void Jump() => Play(jumpState);
        internal void Walk() => Play(walkState);
        internal void Run() => Play(runState);

        private void Play(string stateName, float transition = 0.25f)
        {
            animation.CrossFade(stateName, transition);
        }
    }
}