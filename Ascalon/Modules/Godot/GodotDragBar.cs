#if GODOT
using Godot;
using System;

public partial class GodotDragBar : Control
{
    private Vector2 dragPoint = Vector2.Zero;
    private bool hovered = false;


    [Export] private Control dragRecipient;
    

    
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent)
        {
            if (mouseEvent.ButtonIndex == MouseButton.Left)
            {
                if (mouseEvent.Pressed == true && hovered == true)
                {
                    dragPoint = GetGlobalMousePosition() - dragRecipient.Position;
                }
                else
                {
                    dragPoint = Vector2.Zero;
                }
            }
        }

        if (@event is InputEventMouseMotion && dragPoint != Vector2.Zero)
        {
            dragRecipient.Position = GetGlobalMousePosition() - dragPoint;
        }
    }

    public void SetHovered(bool argNewState)
    {
        hovered = argNewState;
    }
}
#endif
