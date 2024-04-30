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
        public Vector3 WeaponBasePosition;
        [Export]
        public Vector3 WeaponMeshPosition;
        [Export]
        public Vector3 WeaponMeshRotation;
        [Export]
        public Vector3 GunMuzzle;
        [Export]
        public Vector3 RightHandPostition;
        [Export]
        public Vector3 RightHandRotation;
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
