using Godot;
using System;
namespace LambastNamespace
{
    [Tool]
    [GlobalClass]
    public partial class DamageResource : Resource
    {
        [Export]
        public string Type;
        [Export]
        public float DamageNumber;
        [Export]
        public double Cooldown;
    }
}
