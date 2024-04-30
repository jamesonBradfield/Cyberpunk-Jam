using Godot;
using System;
using LambastNamespace;
[Tool]
public partial class Weapon : Node3D, ISignalDamageObject
{
	public event Action DealDamageInstance;
	public event Action<DamageResource[]> InitDamageObject;
	public event Action DisableDamageTimer;
	public event Action EnableDamageTimer;

	[Export]
	public WeaponResource WT
	{
		get
		{
			{ return wt; }
		}
		set
		{
			this.wt = value;
			if (Engine.IsEditorHint())
			{
				LoadWeapon();
			}
		}
	}
	private WeaponResource wt;
	private MeshInstance3D WeaponMesh;
	private AnimationPlayer AnimPlayer;
	private AudioStreamPlayer3D SoundPlayer;
	private RichTextLabel WeaponAmmoText;
	private Node3D GunMuzzle;
	private DamageRay Ray;
	private RayCast3D HeadRaycast;
	// [Export]
	// private Node3D RightHandTarget;

	public override void _Ready()
	{
		WeaponMesh = GetNode<MeshInstance3D>("WeaponMesh");
		AnimPlayer = GetNode<AnimationPlayer>("AnimPlayer");
		SoundPlayer = GetNode<AudioStreamPlayer3D>("SoundPlayer");
		WeaponAmmoText = GetNode<RichTextLabel>("../../Control/WeaponAmmoText");
		GunMuzzle = GetNode<Node3D>("GunMuzzle");
		Ray = GetNode<DamageRay>("DamageRay");
		HeadRaycast = GetNode<RayCast3D>("../RayCast3D");
		ReadyWeapon();
		InitDamageObject(wt.Damage);
		Ray.DamageDone += () =>
		{
			WeaponAmmoText.Text = GD.VarToStr(wt.Damage.Length) + "/" + GD.VarToStr(wt.Damage.Length);
			if (wt != null)
			{
				wt.ReloadFeedback.PlayBothForce(AnimPlayer, SoundPlayer);
			}
			DisableDamageTimer();
		};
		Ray.DamageInstanceDone += (float Damage, int CurrentCounter) =>
		{
			WeaponAmmoText.Text = GD.VarToStr(wt.Damage.Length - CurrentCounter - 1) + "/" + GD.VarToStr(wt.Damage.Length);
			if (wt != null)
			{
				wt.ShootingFeedback.PlayBoth(AnimPlayer, SoundPlayer);
			}
		};
		AnimPlayer.AnimationFinished += (StringName animation) =>
		{
			GD.Print("AnimationFinished called");
			if (animation == wt.ReloadFeedback.AnimName) { EnableDamageTimer(); }
		};
	}
	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		if (@event.IsActionPressed("shoot"))
		{
			Shoot();
		}
	}
	protected virtual void ReadyWeapon()
	{
		if (wt != null)
		{
			LoadWeapon();
			WeaponAmmoText.Text = GD.VarToStr(wt.Damage.Length) + "/" + GD.VarToStr(wt.Damage.Length);
			if (wt.ActivateFeedback != null) { wt.ActivateFeedback.PlayBoth(AnimPlayer, SoundPlayer); }
		}
	}
	protected virtual void LoadWeapon()
	{
		if (wt != null)
		{
			if (wt.WeaponMesh != null)
			{
				WeaponMesh.Mesh = wt.WeaponMesh;
				Position = wt.WeaponBasePosition;
				WeaponMesh.RotationDegrees = wt.WeaponMeshRotation;
				WeaponMesh.Position = wt.WeaponMeshPosition;
				// RightHandTarget.Position = wt.RightHandPostition;
				// RightHandTarget.RotationDegrees = wt.RightHandRotation;
			}
		}
	}

	protected virtual void Shoot()
	{
		if (wt != null)
		{
			DealDamageInstance();
		}
	}

	private void printValue(String name, Variant var)
	{
		GD.Print(name + " has been set to " + var.AsString());
	}
}
