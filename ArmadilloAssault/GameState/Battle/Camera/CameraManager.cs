﻿using ArmadilloAssault.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace ArmadilloAssault.GameState.Battle.Camera
{
    public static class CameraManager
    {
        public static Vector2 FocusPoint { get; private set; }
        private static Vector2 FocusOffset { get; set; }

        public static Point Offset => GameStateManager.State == State.Battle ? CameraOffset : Point.Zero;
        private static Point CameraOffset { get; set; }

        private static Point SceneSize { get; set; }

        public static Vector2 CursorPosition { get; private set; }
        public static bool Scoped { get; private set; }

        public static void Initialize(Point sceneSize)
        {
            Scoped = false;
            SceneSize = sceneSize;

            FocusPoint = sceneSize.ToVector2() / 2;

            var screenCenter = GraphicsManager.ScreenCenter;
            Mouse.SetPosition(screenCenter.X, screenCenter.Y);
            CursorPosition = screenCenter.ToVector2();
        }

        public static void ToggleScoped()
        {
            Scoped = !Scoped;

            if (Scoped)
            {
                var screenCenter = GraphicsManager.ScreenCenter;

                var aimOffset = CursorPosition - screenCenter.ToVector2();

                FocusOffset = new Vector2(
                    Math.Clamp(aimOffset.X / 2, -800, 800),
                    Math.Clamp(aimOffset.Y / 2, -400, 400)
                );

                var oldOffset = CameraOffset;
                UpdateCameraOffset();
                var newOffset = CameraOffset;

                CursorPosition -= (newOffset - oldOffset).ToVector2() / 2;

                Mouse.SetPosition(screenCenter.X, screenCenter.Y);
            }
        }

        public static void UpdateFocusPoint(Vector2 avatarPosition)
        {
            FocusPoint = avatarPosition + new Vector2(64, 64);

            UpdateCameraOffset();
        }

        public static void UpdateFocusOffset(Vector2 aimPosition)
        {
            if (Scoped)
            {
                var screenCenter = GraphicsManager.ScreenCenter;

                var aimOffset = aimPosition - screenCenter.ToVector2();

                CursorPosition += aimOffset / 2;
                FocusOffset = new Vector2(FocusOffset.X + aimOffset.X / 2, FocusOffset.Y + aimOffset.Y / 2);
            
                Mouse.SetPosition(screenCenter.X, screenCenter.Y);
            }
            else
            {
                CursorPosition = aimPosition;
                FocusOffset = Vector2.Zero;
            }

            UpdateCameraOffset();
        }

        private static void UpdateCameraOffset() {
            CameraOffset = new Point(
                (int)Math.Clamp(FocusPoint.X + Math.Clamp(FocusOffset.X, -900, 900), 960, SceneSize.X - 960) - 960,
                (int)Math.Clamp(FocusPoint.Y + Math.Clamp(FocusOffset.Y, -450, 450), 540, SceneSize.Y - 540) - 540
            );
        }

        public static void Disable()
        {
            CameraOffset = Point.Zero;
        }

        public static Rectangle GetBackgroundSourceRectangle(bool backBack = false)
        {
            int x, y, width, height;

            x = 0;
            y = 0;
            width = 960;
            height = 540;

            if (SceneSize.X == 7680)
            {
                width -= (int)(width / (backBack ? 5f : 4));
                x += ScaleValue(Offset.X + 960, 960, SceneSize.X - 960, 0, 1920 - width);
            }
            else if (SceneSize.X != 1920)
            {
                width -= (int)(width / (backBack ? 5f : 4));
                x += ScaleValue(Offset.X + 960, 960, SceneSize.X - 960, 0, 960 - width);
            }

            if (SceneSize.Y != 1080)
            {
                height -= (int)(height / (backBack ? 5f : 4));
                y += ScaleValue(Offset.Y + 540, 540, SceneSize.Y - 540, 0, 540 - height);
            }

            return new Rectangle(x, y, width, height);
        }

        public static Rectangle GetEnvironmentalEffectDestinationRectangle(Rectangle effectRectangle)
        {
            float x, y, width, height;

            x = effectRectangle.X;
            y = effectRectangle.Y;
            width = effectRectangle.Width;
            height = effectRectangle.Height;

            if (SceneSize.X != 1920)
            {
                width += (width / 3f);
                x *= 1 + (1 / 3f);
                x -= ScaleValue(Offset.X + 960, 960, SceneSize.X - 960, 0, 960 - width);
            }

            if (SceneSize.Y != 1080)
            {
                height += (height / 3f);
                y *= 1 + (1 / 3f);
                y -= ScaleValue(Offset.Y + 540, 540, SceneSize.Y - 540, 0, 540 - height) * 0.92f;
            }

            return new Rectangle((int)Math.Round(x), (int)Math.Round(y), (int)Math.Round(width), (int)Math.Round(height));
        }

        public static Rectangle GetFlowDestinationRectangle(Rectangle flowRectangle)
        {
            float x, y, width, height;

            x = flowRectangle.X;
            y = flowRectangle.Y;
            width = flowRectangle.Width;
            height = flowRectangle.Height;

            if (SceneSize.X != 1920)
            {
                width += (width / 3f);
                x *= 1 + (1 / 3f);
            }

            if (SceneSize.Y != 1080)
            {
                height += (height / 3f);
                y *= 1 + (1 / 3f);
                y -= ScaleValue(Offset.Y + 540, 540, SceneSize.Y - 540, 0, 540 - height) * 0.92f;
            }

            return new Rectangle((int)Math.Round(x), (int)Math.Round(y), (int)Math.Round(width), (int)Math.Round(height));
        }

        public static int ScaleValue(double value, double oldMin, double oldMax, double newMin, double newMax)
        {
            double clampedValue = Math.Max(Math.Min(value, oldMax), oldMin);
            double ratio = (clampedValue - oldMin) / (oldMax - oldMin);
            double scaledValue = ratio * (newMax - newMin) + newMin;

            return (int)Math.Round(scaledValue);
        }

        public static Vector2 GetAimAngle()
        {
            return CursorPosition - (FocusPoint - CameraOffset.ToVector2());
        }
    }
}
