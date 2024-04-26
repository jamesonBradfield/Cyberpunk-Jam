using Godot;
using System;
namespace LambastNamespace
{
    [Tool]
    [GlobalClass]
    public partial class WeaponResource : Resource
    {
        [Export]
        public StringName WeaponName { get; set; }
        public StringName PublicWeaponName;
        [Export]
        public Mesh WeaponMesh;
        [ExportCategory("Weapon Orientation")]
        [Export]
        public Vector3 Position;
        [Export]
        public Vector3 Rotation;
        [Export]
        public Vector3 GunMuzzle;
        [ExportCategory("Weapon Statistics")]
        [Export]
        public DamageResource[] Damage;
        [Export]
        public bool IsSemiAuto;
        [Export]
        public Vector2 SprayRandomness;
        [ExportCategory("Feedbacks")]
        [Export]
        public Feedback ReloadFeedback;
        [Export]
        public Feedback ShootingFeedback;
        [Export]
        public Feedback ActivateFeedback;
    }
}
