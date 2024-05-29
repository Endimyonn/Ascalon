#if GODOT
using Godot;
using System;

/// <summary>
/// Facilitates the de-selection of a LineEdit when the mouse clicks outside of it.
/// While this may be handled by Godot if the click is on another UI element, it
/// doesn't do so without a UI element.
/// </summary>
public partial class LineEditClickOff : LineEdit
{
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
            if (mouseEvent.ButtonIndex == MouseButton.Left)
            {
                Vector2 localMousePosition = GetLocalMousePosition();
                if (localMousePosition.X < 0 || localMousePosition.X > this.Size.X || localMousePosition.Y < 0 || localMousePosition.Y > this.Size.Y)
                {
                    this.ReleaseFocus();
                }
            }
        }
    }
}
#endif
