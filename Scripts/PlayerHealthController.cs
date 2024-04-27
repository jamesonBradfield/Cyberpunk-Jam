using Godot;
using System;
using LambastNamespace;
public partial class PlayerHealthController : ProgressBar
{
    [Signal]
    public delegate void HealthIsDepletedEventHandler();
    [Export]
    private HurtArea3D HurtArea;

    public override void _Ready()
    {
        HurtArea.UpdateHealth += (HealthLost) =>
        {
            if (!Engine.IsEditorHint())
            {
                Value = Value - HealthLost;
                double HealthRatio = Value / MaxValue;
                Value = HealthRatio * MaxValue;
                if (Value <= 0)
                {
                    EmitSignal("HealthIsDepleted");
                }
            }
        };
    }
}
