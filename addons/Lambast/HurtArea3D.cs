using Godot;
namespace LambastNamespace
{
    [Tool]
    public partial class HurtArea3D : Area3D
    {
        [Signal]
        public delegate void UpdateHealthEventHandler(float HealthLost);
        private CollisionShape3D HurtCollider;
        public override void _EnterTree()
        {
            Utils.AddNodeAsChild<CollisionShape3D>(ref HurtCollider, "HurtAreaCollider", this);
        }
        public override void _ExitTree()
        {
            RemoveChild(HurtCollider);
        }

        public void SubscribeToDamageObject(DamageObject _DamageObject)
        {
            _DamageObject.DamageInstanceDone += SendDamageToHealthBar;
        }

        public void UnsubscribeToDamageObject(DamageObject _DamageObject)
        {
            _DamageObject.DamageInstanceDone -= SendDamageToHealthBar;
        }

        public void SendDamageToHealthBar(float Damage, int _CurrentAmmo)
        {
            GD.Print(GD.VarToStr(this.Name) + " has taken " + GD.VarToStr(Damage) + " damage!");
            EmitSignal("UpdateHealth", Damage);
        }
    }
}
