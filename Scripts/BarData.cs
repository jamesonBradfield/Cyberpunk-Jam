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

}
