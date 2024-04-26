using Godot;
using System;
namespace LambastNamespace
{
    [Tool]
    [GlobalClass]
    public partial class Feedback : Resource
    {
        [Export]
        private AudioStreamOggVorbis Sound;
        [Export]
        public StringName AnimName;

        public void PlayAnimation(AnimationPlayer AnimPlayer)
        {
            if (!AnimPlayer.IsPlaying()) { AnimPlayer.Play(AnimName); };
        }

        public void PlaySound(AudioStreamPlayer3D SoundPlayer)
        {
            SoundPlayer.Stream = Sound;
            SoundPlayer.Play();
        }

        public void PlayBoth(AnimationPlayer AnimPlayer, AudioStreamPlayer3D SoundPlayer)
        {
            if (Sound != null)
            {
                SoundPlayer.Stream = Sound;
                SoundPlayer.Play();
            }
            if (AnimPlayer.HasAnimation(AnimName) && !AnimPlayer.IsPlaying())
            {
                AnimPlayer.Play(AnimName);
            }
        }

        public void PlayBothForce(AnimationPlayer AnimPlayer, AudioStreamPlayer3D SoundPlayer)
        {
            if (Sound != null)
            {
                SoundPlayer.Stream = Sound;
                SoundPlayer.Play();
            }
            if (AnimPlayer.HasAnimation(AnimName))
            {
                AnimPlayer.Stop();
                AnimPlayer.Play(AnimName);
            }
            else
            {
                GD.PushWarning("Animation " + GD.VarToStr(AnimName) + " not found on " + GD.VarToStr(AnimPlayer));
            }

        }
    }
}
