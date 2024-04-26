using Godot;
using System;
namespace LambastNamespace
{
    [Tool]
    public partial class UI3D : Node3D
    {
        protected Sprite3D Sprite3DNode;
        protected Vector3 sprite3dposition;
        protected Control control;
        protected SubViewport subViewport;
        [Export]
        protected Vector3 Sprite3DPosition
        {
            get { return sprite3dposition; }
            set
            {
                sprite3dposition = value;
                Sprite3DNode.Position = value;
            }
        }
        public override void _EnterTree()
        {
            base._EnterTree();
            Utils.AddNodeAsChild<SubViewport>(ref subViewport, "SubViewport", this);
            subViewport.Disable3D = true;
            subViewport.TransparentBg = true;
            Utils.AddNodeAsChild<Control>(ref control, "Control", subViewport);
            control.SetAnchorsPreset(Control.LayoutPreset.Center, false);
            control.LayoutMode = 1;
            control.SetOffsetsPreset(Control.LayoutPreset.Center);
            Utils.AddNodeAsChild<Sprite3D>(ref Sprite3DNode, "Sprite3D", this);
            Sprite3DNode.Texture = subViewport.GetTexture();
        }
        public override void _ExitTree()
        {
            RemoveChild(subViewport);
            RemoveChild(control);
            RemoveChild(Sprite3DNode);
        }
    }
}
