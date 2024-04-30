using Godot;
using System;
using LambastNamespace;
public partial class DamageTimerExample : Node3D, ISignalDamageObject
{
    public event Action DealDamageInstance;
    public event Action<DamageResource[]> InitDamageObject;
    public event Action DisableDamageTimer;
    public event Action EnableDamageTimer;
    [Export]
    private DamageResource[] damage;
    [Export]
    private DamageObject _DamageObject;
    [Export]
    private Timer timer;

    public override void _Ready()
    {
        InitDamageObject(damage);
        timer.Timeout += () =>
        {
            DealDamageInstance();
        };
    }
}
