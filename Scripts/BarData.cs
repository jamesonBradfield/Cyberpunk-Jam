using Godot;
using System;

public partial class BarData : Node
{
    [Export]
    public float Value { get; set; }
    [Export]
    public float MaxValue { get; set; }
    [Export]
    public float MinValue { get; set; }
    [ExportGroup("Graphics")]
    [Export]
    public float GraphicMinValue;
    [Export]
    public float GraphicMaxValue;
    [Export]
    public MeshInstance3D Graphic;
    public bool CanDecrementBarData(float DecValue)
    {
        if (Value - DecValue >= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void DecrementBarData(float DecValue)
    {
        Value -= DecValue;
    }

    public float GetValueAsClamp01()
    {
        return Mathf.Clamp(Value, MinValue, MaxValue) / MaxValue;
    }

}
