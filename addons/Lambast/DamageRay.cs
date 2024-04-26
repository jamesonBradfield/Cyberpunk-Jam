using Godot;
using System;
namespace LambastNamespace
{
	[Tool]
	public partial class DamageRay : DamageObject
	{
		private RayCast3D Ray;
		public override void _EnterTree()
		{
			base._EnterTree();
			Utils.AddNodeAsChild<RayCast3D>(ref Ray, "RayCast3D", this);
			Ray.TargetPosition = Vector3.Forward * 100;
		}
		public override void _ExitTree()
		{
			base._ExitTree();
			RemoveChild(Ray);
		}

		protected override void DealDamage()
		{
			if (!DisableDamageTimer && FlipFlop)
			{
				Node Collider;
				if (Ray.GetCollider() is HurtArea3D)
				{
					Collider = (Area3D)Ray.GetCollider();
					GD.Print("Damage ray has found collider : " + GD.VarToStr(Collider.Name));
					GD.Print("We Hit a Hurt Area " + GD.VarToStr(Collider.Name));
					Collider.Call("SubscribeToDamageObject", this);
				}
				EmitSignal("DamageInstanceDone", Damage[CurrentInstances].DamageNumber, CurrentInstances);
				base.DealDamage();
			}
		}

		protected override void InitDamageObject(DamageResource[] _damage)
		{
			base.InitDamageObject(_damage);
		}
	}
}
