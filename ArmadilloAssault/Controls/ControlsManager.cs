using ArmadilloAssault.GameState;
using ArmadilloAssault.GameState.Battle;
using ArmadilloAssault.GameState.Battle.Camera;
using ArmadilloAssault.Graphics;
using ArmadilloAssault.Web.Server;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ArmadilloAssault.Controls
{
    public static class ControlsManager
    {
        private static Dictionary<int, ControlsState> PlayerControlsStates { get; set; }

        private static bool JustToggledFullscreen { get; set; }

        public static void Initialize()
        {
            PlayerControlsStates = [];
        }

        public static bool IsControlDownStart(int playerIndex, Control control)
        {
            if (PlayerControlsStates.TryGetValue(playerIndex, out ControlsState value))
            {
                return value.IsControlDownStart(control);
            }

            return false;
        }

        public static bool IsControlDown(int playerIndex, Control control)
        {
            if (PlayerControlsStates.TryGetValue(playerIndex, out ControlsState value))
            {
                return value.IsControlDown(control);
            }

            return false;
        }

        public static bool IsControlPressed(int playerIndex, Control control)
        {
            if (PlayerControlsStates.TryGetValue(playerIndex, out ControlsState value))
            {
                return value.IsControlPressed(control);
            }

            return false;
        }

        public static Vector2? GetNullableAimPosition(int playerIndex)
        {
            if (PlayerControlsStates.TryGetValue(playerIndex, out ControlsState value))
            {
                return value.AimPosition;
            }

            return null;
        }

        public static Vector2 GetAimPosition(int playerIndex)
        {
            if (PlayerControlsStates.TryGetValue(playerIndex, out ControlsState value))
            {
                return value.AimPosition == null ? Vector2.Zero : (Vector2)value.AimPosition;
            }

            return Vector2.Zero;
        }

        public static void Update()
        {
            UpdateControlState();

            if (ServerManager.IsServing)
            {
                foreach (var index in ServerManager.PlayerIndices)
                {
                    if (index > 0)
                    {
                        UpdateControlState(index, ServerManager.GetPlayerControlsDown(index), ServerManager.GetPlayerAimPosition(index));
                    }
                }
            }

            var keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.LeftAlt) && keyboardState.IsKeyDown(Keys.Enter) && !JustToggledFullscreen)
            {
                GraphicsManager.ToggleFullscreen();
                JustToggledFullscreen = true;
            }
            else if (keyboardState.IsKeyUp(Keys.LeftAlt) && keyboardState.IsKeyUp(Keys.Enter) && JustToggledFullscreen) {
                JustToggledFullscreen = false;
            }
        }

        private static void UpdateControlState()
        {
            ControlsState controlsState;

            if (!PlayerControlsStates.TryGetValue(0, out ControlsState value))
            {
                controlsState = new ControlsState();
                PlayerControlsStates.Add(0, controlsState);
            }
            else
            {
                controlsState = value;
            }

            UpdateMouseKeyboardControlState(controlsState);
        }

        private static void UpdateControlState(int playerIndex, List<Control> areControlsDown, Vector2 aimPosition)
        {
            ControlsState controlsState;

            if (!PlayerControlsStates.TryGetValue(playerIndex, out ControlsState value))
            {
                controlsState = new ControlsState();
                PlayerControlsStates.Add(playerIndex, controlsState);
            }
            else
            {
                controlsState = value;
            }

            foreach (var control in Enum.GetValues<Control>())
            {
                if (areControlsDown.Contains(control))
                {
                    controlsState.OnControlDown(control);
                }
                else
                {
                    controlsState.OnControlUp(control);
                }
            }

            controlsState.AimPosition = aimPosition;
        }

        private static void UpdateMouseKeyboardControlState(ControlsState controlsState)
        {
            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();

            foreach (var controlMapping in ControlsHelper.KeyControlMappings)
            {
                if (keyboardState.IsKeyUp(controlMapping.Key))
                {
                    controlsState.OnControlUp(controlMapping.Value);
                }
                else if (keyboardState.IsKeyDown(controlMapping.Key))
                {
                    controlsState.OnControlDown(controlMapping.Value);
                }
            }

            void updateMouseControl(ButtonState buttonState, Control control)
            {
                if (buttonState == ButtonState.Pressed)
                {
                    controlsState.OnControlDown(control);
                }
                else if (buttonState == ButtonState.Released)
                {
                    controlsState.OnControlUp(control);
                }
            }

            updateMouseControl(mouseState.LeftButton, Control.Fire_Primary);
            updateMouseControl(mouseState.RightButton, Control.Fire_Secondary);
            updateMouseControl(mouseState.MiddleButton, Control.Toggle_Scope);
            updateMouseControl(mouseState.LeftButton, Control.Confirm);

            if (GameStateManager.State == State.Battle && !BattleManager.Paused)
            {
                CameraManager.UpdateFocusOffset(mouseState.Position.ToVector2());

                controlsState.AimPosition = CameraManager.CursorPosition;
            }
            else
            {
                controlsState.AimPosition = new Vector2(mouseState.Position.X, mouseState.Position.Y);
            }
        }

        private static void UpdateGamePadControlState(ControlsState controlsState, PlayerIndex playerIndex)
        {
            var state = GamePad.GetState(playerIndex);

            foreach (var controlMapping in ControlsHelper.ButtonControlMappings)
            {
                if (state.IsButtonDown(controlMapping.Key))
                {
                    controlsState.OnControlDown(controlMapping.Value);
                }
                else if (state.IsButtonUp(controlMapping.Key))
                {
                    controlsState.OnControlUp(controlMapping.Value);
                }
            }

            void updateThumbstickControl(bool evaluation, Control control)
            {
                if (evaluation)
                {
                    controlsState.OnControlDown(control);
                }
                else
                {
                    controlsState.OnControlUp(control);
                }
            }

            updateThumbstickControl(state.ThumbSticks.Left.X < -ControlsHelper.ThumbStickConstant, Control.Left);
            updateThumbstickControl(state.ThumbSticks.Left.X > ControlsHelper.ThumbStickConstant, Control.Right);
            updateThumbstickControl(state.ThumbSticks.Left.Y > ControlsHelper.ThumbStickConstant, Control.Up);
            updateThumbstickControl(state.ThumbSticks.Left.Y < -ControlsHelper.ThumbStickConstant, Control.Down);

            controlsState.AimPosition = new Vector2(state.ThumbSticks.Right.X, -state.ThumbSticks.Right.Y);
        }

        private static PlayerIndex GetIndex(int index)
        {
            return index switch
            {
                0 => PlayerIndex.One,
                1 => PlayerIndex.Two,
                2 => PlayerIndex.Three,
                _ => PlayerIndex.Four,
            };
        }

        public static List<Control> AreControlsDown(int index)
        {
            var list = new List<Control>();

            foreach (var control in Enum.GetValues<Control>())
            {
                var isControlDown = IsControlDown(index, control);
                if (isControlDown)
                {
                    list.Add(control);
                }
            }

            return list;
        }
    }
}
