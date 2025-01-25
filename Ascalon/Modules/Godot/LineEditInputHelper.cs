#if GODOT
using Godot;
using System;

/// <summary>
/// Facilitates the de-selection of a LineEdit when the mouse clicks outside of it.
/// Also marks input to the LineEdit as handled.
/// </summary>
public partial class LineEditInputHelper : LineEdit
{
    public override void _UnhandledInput(InputEvent @event)
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

            //prevent undesired propagation of input while text entry is active
            if (HasFocus())
            {
                GetViewport().SetInputAsHandled();
            }
        }
    }
}
#endif
