using Godot;
using System;
namespace LambastNamespace
{
    [Tool]
    public partial class HealthBar3D : UI3D
    {
        [Signal]
        public delegate void HealthIsDepletedEventHandler();
        [Export]
        private HurtArea3D HurtArea;
        private ProgressBar HealthBarNode;
        public override void _EnterTree()
        {
            base._EnterTree();
            HealthBarNode = this.GetNode<ProgressBar>("HealthBar");
            if (HealthBarNode == null)
            {
                HealthBarNode = new();
                this.AddChild(HealthBarNode);
                HealthBarNode.Name = "HealthBar";
                HealthBarNode.Owner = HealthBarNode.GetTree().EditedSceneRoot;
            }
            HealthBarNode.SetAnchorsPreset(Control.LayoutPreset.Center, false);
            HealthBarNode.LayoutMode = 1;
            HealthBarNode.ShowPercentage = false;
        }
        public override void _ExitTree()
        {
            base._ExitTree();
            RemoveChild(HealthBarNode);
        }



        public override void _Ready()
        {
            Sprite3DNode.Position = sprite3dposition;
            HurtArea.UpdateHealth += (HealthLost) =>
            {
                if (!Engine.IsEditorHint())
                {
                    GD.Print("CurrentHealth is " + GD.VarToStr(HealthBarNode.Value));
                    HealthBarNode.Value = HealthBarNode.Value - HealthLost;
                    GD.Print("CurrentHealth is now " + GD.VarToStr(HealthBarNode.Value));
                    double HealthRatio = HealthBarNode.Value / HealthBarNode.MaxValue;
                    GD.Print("HealthRatio is " + GD.VarToStr(HealthRatio));
                    HealthBarNode.Value = HealthRatio * HealthBarNode.MaxValue;
                    if (HealthBarNode.Value <= 0)
                    {
                        EmitSignal("HealthIsDepleted");
                    }
                }
            };
        }
    }
}
