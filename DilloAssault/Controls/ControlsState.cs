using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace DilloAssault.Controls
{
    public class ControlsState
    {
        public Vector2? AimPosition { get; set; }
        private Dictionary<Control, int> ControlDownFrames { get; set; }
        private Dictionary<Control, bool> ControlDown { get; set; }
        private Dictionary<Control, bool> ControlPress { get; set; }

        public ControlsState() {
            ControlDownFrames = [];
            ControlDown = [];
            ControlPress = [];
        }

        public bool IsControlDown(Control control)
        {
            ControlDown.TryGetValue(control, out var down);

            return down;
        }

        public bool IsControlDownStart(Control control)
        {
            ControlDown.TryGetValue(control, out var down);

            ControlDownFrames.TryGetValue(control, out var frames);

            return down && frames == 1;
        }

        public bool IsControlPressed(Control control)
        {
            ControlPress.TryGetValue(control, out var pressed);

            return pressed;
        }

        public void OnControlDown(Control control)
        {
            if (!ControlDown.TryAdd(control, true))
            {
                ControlDown[control] = true;
            }

            ControlDownFrames.TryAdd(control, 0);
            ControlDownFrames[control] = ControlDownFrames[control] + 1;
        }

        public void OnControlUp(Control control)
        {
            ControlDown.TryGetValue(control, out bool isDown);

            if (isDown) {
                if (!ControlDown.TryAdd(control, false))
                {
                    ControlDown[control] = false;
                }

                if (!ControlDownFrames.TryAdd(control, 0))
                {
                    ControlDownFrames[control] = 0;
                }

                if (!ControlPress.TryAdd(control, true))
                {
                    ControlPress[control] = true;
                }
            }
            else
            {
                if (!ControlPress.TryAdd(control, false))
                {
                    ControlPress[control] = false;
                }
            }
        }
    }
}
