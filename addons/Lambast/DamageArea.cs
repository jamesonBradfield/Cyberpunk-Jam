using Godot;
using System;
namespace LambastNamespace
{
	[Tool]
	public partial class DamageArea : DamageObject
	{
		private Area3D DamagingArea;
		private CollisionShape3D CollisionShapeNode;
		private Godot.Collections.Array<HurtArea3D> HitNodes = new();

		public override void _EnterTree()
		{
			base._EnterTree();
			if (DamagingArea == null)
			{
				GD.Print("Damaging area " + GD.VarToStr(DamagingArea));
				Utils.AddNodeAsChild<Area3D>(ref DamagingArea, "DamageArea", this);
				Utils.AddNodeAsChild<CollisionShape3D>(ref CollisionShapeNode, "DamageShape", DamagingArea);
				DamagingArea.AreaEntered += AddAreaToHitNodes;
				DamagingArea.AreaExited += RemoveAreaFromHitNodes;
			}
			else
			{
				DamagingArea.AreaEntered += AddAreaToHitNodes;
				DamagingArea.AreaExited += RemoveAreaFromHitNodes;
			}
		}


		protected void AddAreaToHitNodes(Area3D area)
		{
			GD.Print(area);
			if (area is HurtArea3D)
			{
				GD.Print("Area has been added to Hit nodes");
				HitNodes.Add(area as HurtArea3D);
				(area as HurtArea3D).SubscribeToDamageObject(this);
			}
		}

		protected void RemoveAreaFromHitNodes(Area3D area)
		{
			if (area is HurtArea3D)
			{
				GD.Print("Area has been removed from Hit nodes");
				HitNodes.Remove(area as HurtArea3D);
				(area as HurtArea3D).UnsubscribeToDamageObject(this);
			}
		}

		protected override void DealDamage()
		{
			for (int i = 0; i <= HitNodes.Count; i++)
			{
				GD.Print("about to damage " + HitNodes[i]);
				//NOTE: we really have to find a better way of doing this
				EmitSignal("DamageInstanceDone", Damage[CurrentInstances].DamageNumber, CurrentInstances);
			}
			if (Debug && !Engine.IsEditorHint())
			{
				GD.Print("deal_damage has been called with index of " + GD.VarToStr(CurrentInstances) + " and a damage of " + GD.VarToStr(Damage[CurrentInstances].DamageNumber));
				GD.Print("current_instances : " + GD.VarToStr(CurrentInstances));
			}
			base.DealDamage();
		}
		protected override void InitDamageObject(DamageResource[] _damage)
		{
			base.InitDamageObject(_damage);
		}
	}
}
