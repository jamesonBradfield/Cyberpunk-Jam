using Godot;
using System.Collections;

public partial class ImplantManager : Node
{
	[Export]
	private Implant[] AvailableImplants;
	[Export]
	private int SelectedImplantIndexInAvailableImplants;
	[Export]
	private Implant SelectedImplant;
	[Export]
	private Implant[] CurrentImplant = new Implant[2];
	[Export]
	private ProgressBar EnergyBar;

	public override void _Ready()
	{
		foreach (Implant child in AvailableImplants)
		{
			child.ImplantReady();
		}
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		ImplantHandler(delta);
	}

	private void ImplantHandler(double delta)
	{
		for (int i = 0; i < CurrentImplant.Length; i++)
		{
			if (CurrentImplant[i] != null && CanDecrementEnergyBar(CurrentImplant[i].ProcessEnergyDrain))
			{
				CurrentImplant[i].ImplantProcess(delta);
				DecrementEnergyBar(CurrentImplant[i].ProcessEnergyDrain);
			}
			else if (CurrentImplant[i] != null && !CanDecrementEnergyBar(CurrentImplant[i].ProcessEnergyDrain))
			{
				CurrentImplant[i].DeActivateImplant();
				CurrentImplant[i] = null;
			}
		}
	}
	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("implant_use") && CanDecrementEnergyBar(SelectedImplant.ActivateEnergyDrain + SelectedImplant.ProcessEnergyDrain))
		{
			if (ImplantExistsInCurrentImplants(SelectedImplant))
			{
				GD.Print("Value " + GD.VarToStr(CurrentImplant[SelectedImplantIndexInAvailableImplants]) + " Index " + GD.VarToStr(SelectedImplantIndexInAvailableImplants) + " set to null");
				CurrentImplant.SetValue(null, SelectedImplantIndexInAvailableImplants);
				SelectedImplant.DeActivateImplant();
			}
			else
			{
				DecrementEnergyBar(SelectedImplant.ActivateEnergyDrain);
				CurrentImplant.SetValue(CurrentImplant.GetValue(0), 1);
				CurrentImplant.SetValue(SelectedImplant, 0);
				SelectedImplant.ActivateImplant();
			}
		}
		if (@event.IsActionPressed("implant_next"))
		{
			SelectedImplantIndexInAvailableImplants += 1;
			SelectedImplantOverflow();
			SelectedImplant = (AvailableImplants[SelectedImplantIndexInAvailableImplants] as Implant);
			GD.Print("Implant Selected : " + GD.VarToStr(SelectedImplant.Name));
		}
		if (@event.IsActionPressed("implant_prev"))
		{
			SelectedImplantIndexInAvailableImplants -= 1;
			SelectedImplantOverflow();
			SelectedImplant = (AvailableImplants[SelectedImplantIndexInAvailableImplants] as Implant);
			GD.Print("Implant Selected : " + GD.VarToStr(SelectedImplant.Name));
		}
	}

	public void SelectedImplantOverflow()
	{
		if (SelectedImplantIndexInAvailableImplants < 0)
		{
			SelectedImplantIndexInAvailableImplants = AvailableImplants.Length - 1;
		}
		if (SelectedImplantIndexInAvailableImplants > AvailableImplants.Length - 1)
		{
			SelectedImplantIndexInAvailableImplants = 0;
		}
	}

	public bool CanDecrementEnergyBar(float DecValue)
	{
		if (EnergyBar.Value - DecValue >= 0)
		{
			return true;
		}
		else
		{
			GD.Print("Can't Decrement Energy bar");
			return false;
		}
	}
	private void DecrementEnergyBar(float DecValue)
	{
		EnergyBar.Value -= DecValue;
	}

	private bool ImplantExistsInCurrentImplants(Implant checkImplant)
	{
		foreach (Implant CI in CurrentImplant)
		{
			if (CI == checkImplant) { return true; }
			else if (CI != checkImplant) { return false; }
		}
		return false;
	}
}

