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
	private TextureRect IconRect;
	[Export]
	private BarData EnergyBar;
	[Export]
	private Color DisabledAlbedoColor;
	[Export]
	private Color DisabledEmissionColor;
	private MeshInstance3D ScreenMesh;

	public override void _Ready()
	{
		ScreenMesh = GetNode<MeshInstance3D>("../Head/Camera3D/Arms/left_arm/Skeleton3D/BoneAttachment3D/Cube_002");
		GD.Print(IconRect.Name);
		foreach (Implant child in AvailableImplants)
		{
			child.ImplantReady();
		}
		SelectedImplant = AvailableImplants[0];
		ScreenMesh.Call("set_icon_used", DisabledAlbedoColor, DisabledEmissionColor);
		IconRect.Texture = SelectedImplant.Sprite;
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
			if (CurrentImplant[i] != null && EnergyBar.CanDecrementBarData(CurrentImplant[i].ProcessEnergyDrain))
			{
				CurrentImplant[i].ImplantProcess(delta);
				EnergyBar.DecrementBarData(CurrentImplant[i].ProcessEnergyDrain);
			}
			else if (CurrentImplant[i] != null && !EnergyBar.CanDecrementBarData(CurrentImplant[i].ProcessEnergyDrain))
			{
				CurrentImplant[i].EmitSignal("DeActivateImplantSignal");
				CurrentImplant[i] = null;
				ScreenMesh.Call("set_icon_used", DisabledAlbedoColor, DisabledEmissionColor);
			}
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("implant_use") && EnergyBar.CanDecrementBarData(SelectedImplant.ActivateEnergyDrain + SelectedImplant.ProcessEnergyDrain))
		{
			if (ImplantExistsInCurrentImplants(SelectedImplant))
			{
				CurrentImplant[GetIndexGivenImplant(SelectedImplant)] = null;
				SelectedImplant.EmitSignal("DeActivateImplantSignal");
				ScreenMesh.Call("set_icon_used", DisabledAlbedoColor, DisabledEmissionColor);
			}
			else
			{
				EnergyBar.DecrementBarData(SelectedImplant.ActivateEnergyDrain);
				if (CurrentImplant[0] != null)
				{
					CurrentImplant.SetValue(CurrentImplant.GetValue(0), 1);
				}
				CurrentImplant.SetValue(SelectedImplant, 0);
				SelectedImplant.EmitSignal("ActivateImplantSignal");
				ScreenMesh.Call("set_icon_used", Colors.White, Colors.White);
			}
		}
		if (@event.IsActionPressed("implant_next"))
		{
			SelectedImplantIndexInAvailableImplants += 1;
			SetIconAndColor();
		}
		if (@event.IsActionPressed("implant_prev"))
		{
			SelectedImplantIndexInAvailableImplants -= 1;
			SetIconAndColor();
		}
	}

	private void SetIconAndColor()
	{
		SelectedImplantOverflow();
		SelectedImplant = (AvailableImplants[SelectedImplantIndexInAvailableImplants] as Implant);
		IconRect.Texture = SelectedImplant.Sprite;
		if (SelectedImplant.IsActive) { ScreenMesh.Call("set_icon_used", Colors.White, Colors.White); }
		else { ScreenMesh.Call("set_icon_used", DisabledAlbedoColor, DisabledEmissionColor); }
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

	private bool ImplantExistsInCurrentImplants(Implant checkImplant)
	{
		foreach (Implant CI in CurrentImplant)
		{
			if (CI == checkImplant) { return true; }
			else if (CI != checkImplant) { return false; }
		}
		return false;
	}

	private int GetIndexGivenImplant(Implant search)
	{
		for (int i = 0; i < CurrentImplant.Length; i++)
		{
			if (search == CurrentImplant[i])
			{
				return i;
			}
			GD.Print("Something is very wrong. your CurrentIndex is somehow acting cray cray!");
			return 3;
		}
		return 4;
	}
}

