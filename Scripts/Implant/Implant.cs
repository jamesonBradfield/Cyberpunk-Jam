using Godot;
using System;
public partial class Implant : Node
{
    [Signal]
    public delegate void ActivateImplantSignalEventHandler();
    [Signal]
    public delegate void DeActivateImplantSignalEventHandler();
    [Export]
    public float ActivateEnergyDrain;
    [Export]
    public float ProcessEnergyDrain;
    [Export]
    public Texture2D Sprite;
    public bool IsActive;

    public void ImplantReady()
    {
        GD.Print("Implant Ready Called on Implant.Name : " + GD.VarToStr(this.Name));
        ActivateImplantSignal += ActivateImplant;
        DeActivateImplantSignal += DeActivateImplant;
    }

    public void ImplantProcess(double delta)
    {

    }

    protected virtual void ActivateImplant()
    {
        GD.Print("Implant Activate Called on Implant.Name : " + GD.VarToStr(this.Name) + " should be draining " + GD.VarToStr(ProcessEnergyDrain) + " Energy per second");
        IsActive = true;
    }

    protected virtual void DeActivateImplant()
    {
        GD.Print("Implant DeActivate Called on Implant.Name : " + GD.VarToStr(this.Name));
        IsActive = false;
    }
}
