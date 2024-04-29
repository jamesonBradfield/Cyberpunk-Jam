using Godot;
using System;
public partial class Implant : Node
{
    [Export]
    public float ActivateEnergyDrain;
    [Export]
    public float ProcessEnergyDrain;
    [Export]
    public Texture2D Sprite;

    public void ImplantReady()
    {

        GD.Print("Implant Ready Called on Implant.Name : " + GD.VarToStr(this.Name));
    }

    public void ImplantProcess(double delta)
    {

    }

    public virtual void ActivateImplant()
    {
        GD.Print("Implant Activate Called on Implant.Name : " + GD.VarToStr(this.Name) + " should be draining " + GD.VarToStr(ProcessEnergyDrain) + " Energy per second");
    }

    public virtual void DeActivateImplant()
    {
        GD.Print("Implant DeActivate Called on Implant.Name : " + GD.VarToStr(this.Name));
    }
}
