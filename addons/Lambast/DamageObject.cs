using Godot;
using System;

namespace LambastNamespace
{
	[Tool]
	public partial class DamageObject : Node3D
	{
		[Signal]
		public delegate void DamageInstanceDoneEventHandler(float Damage, int CurrentCounter);
		[Signal]
		public delegate void DamageDoneEventHandler();
		// a basic Timer to handle Damage cooldowns
		private Timer CooldownTimer;
		// animation player for any Damage_animations
		private AnimationPlayer DamageObjectAnimationPlayer;
		// sound player for Damage sounds
		private AudioStreamPlayer3D DamageObjectAudioStreamPlayer3D;
		[Export]
		// Damage is set by SignalObjects init_damage_object signal, this really shouldn't be exported
		protected DamageResource[] Damage;
		// control whether counter goes up or down.
		[Export]
		private bool Increment;
		// this node needs a init_Damage signal, and if wait_for_SignalObject is true also a can_damage signal
		[Export]
		private Node3D SignalObjectNode;
		private ISignalDamageObject SignalObject;
		// this bool is used to disable our Damage(by disabling the timer)
		protected bool DisableDamageTimer = false;
		// this is just a counter for how much instances of Damage we have dealt.
		protected int CurrentInstances = 0;
		// this bool is used to control when to "check for signal"
		protected bool FlipFlop = true;
		protected bool Debug = false;

		// TODO: add ExitTree function to remove children given ref.
		public override void _EnterTree()
		{
			Utils.AddNodeAsChild<Timer>(ref CooldownTimer, "Timer", this);
			CooldownTimer.OneShot = true;
			CooldownTimer.Timeout += () => FlipFlop = false;
			Utils.AddNodeAsChild<AnimationPlayer>(ref DamageObjectAnimationPlayer, "AnimationPlayer", this);
			Utils.AddNodeAsChild<AudioStreamPlayer3D>(ref DamageObjectAudioStreamPlayer3D, "AudioStreamPlayer3D", this);
		}
		public override void _ExitTree()
		{
			RemoveChild(CooldownTimer);
			RemoveChild(DamageObjectAnimationPlayer);
			RemoveChild(DamageObjectAudioStreamPlayer3D);
		}

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			if (!Engine.IsEditorHint())
			{
				if (SignalObjectNode is ISignalDamageObject)
				{
					SignalObject = SignalObjectNode as ISignalDamageObject;
					SignalObject.InitDamageObject += (DamageResource[] _Damage) => { InitDamageObject(_Damage); };
					SignalObject.DisableDamageTimer += () =>
					{
						DisableDamageTimer = true;
						if (Debug) { GD.Print("enable signal sent " + GD.VarToStr(DisableDamageTimer)); }
					};
					SignalObject.EnableDamageTimer += () =>
					{
						DisableDamageTimer = false;
						if (Debug) { GD.Print("disable signal sent " + GD.VarToStr(DisableDamageTimer)); }
					};
					SignalObject.DealDamageInstance += () => { DealDamage(); };
				}
				else
				{
					GD.PushError("SignalObjectNode doesn't have ISignalDamageObject as an interface, you should use ISignalDamageObject to get signals for your damageObjects and call said signals as needed.");
				}
			}
		}
		protected virtual void StartDamageObjectTimer()
		{
			if (!CooldownTimer.IsStopped()) { return; }
			if (Debug) { GD.Print("StartDamageObjectTimer called"); }
		}
		protected virtual void InitDamageObject(DamageResource[] _Damage)
		{
			Damage = _Damage;
			CooldownTimer.WaitTime = Damage[CurrentInstances].Cooldown;
			if (Debug)
			{
				GD.Print("wait time set to " + GD.VarToStr(Damage[CurrentInstances].Cooldown) + " and an index of " + GD.VarToStr(CurrentInstances) + " with a damage of " + GD.VarToStr(Damage[CurrentInstances].DamageNumber));
			}
		}
		protected virtual void DealDamage()
		{
			//NOTE: we have done a never-nester trick here if this signal seems to not work just know it could be this.
			if (DisableDamageTimer && !FlipFlop) { return; }
			if (Increment) { CurrentInstances++; } else { CurrentInstances--; }
			if (CurrentInstances != Damage.Length)
			{
				StartDamageObjectTimer();
				FlipFlop = false;
				return;
			}
			if (Increment) { CurrentInstances = 0; }
			else { CurrentInstances = Damage.Length; }
			// CooldownTimer.Stop();
			EmitSignal("DamageDone");

		}
	}
}
