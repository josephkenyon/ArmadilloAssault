using ArmadilloAssault.Configuration;
using ArmadilloAssault.Configuration.Textures;
using ArmadilloAssault.Controls;
using ArmadilloAssault.GameState;
using ArmadilloAssault.GameState.Battle;
using ArmadilloAssault.GameState.Editor;
using ArmadilloAssault.GameState.Menus;
using ArmadilloAssault.Graphics;
using ArmadilloAssault.Graphics.Drawing;
using ArmadilloAssault.Graphics.Drawing.Textures;
using ArmadilloAssault.Sound;
using ArmadilloAssault.Web.Client;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace ArmadilloAssault
{
    public class Engine : Game
    {
        private static Action ExitAction { get; set; }

        public static bool Active { get; private set; }

        public Engine()
        {
            GraphicsManager.Initialize(this);

            Content.RootDirectory = "Content"; 
            IsMouseVisible = true;

            Window.IsBorderless = true;
        }

        protected override void Initialize()
        {
            ControlsManager.Initialize();

            ExitAction = Exit;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            ConfigurationManager.LoadContent(Content);
            GraphicsManager.LoadContent(GraphicsDevice, Content);
            SoundManager.LoadContent(Content);

            GameStateManager.PushNewState(State.Menu);

            Mouse.SetCursor(MouseCursor.FromTexture2D(TextureManager.GetTexture(TextureName.cursor), 0, 0));
        }

        protected override void Update(GameTime gameTime)
        {
            Active = IsActive;
            if (Active && GraphicsManager.IsFullscreen)
            {
                var mouseState = Mouse.GetState();
                Mouse.SetPosition(Math.Clamp(mouseState.Position.X, 0, 1920), Math.Clamp(mouseState.Position.Y, 0, 1080));
            }

            GameStateManager.Update();

            ControlsManager.Update(IsActive);

            if (ClientManager.IsActive)
            {
                _ = ClientManager.BroadcastUpdate();
            }

            IsMouseVisible = GameStateManager.State != State.Battle || BattleManager.ShowCursor;

            switch (GameStateManager.State)
            {
                case State.Menu:
                    MenuManager.Update();
                    break;
                case State.Battle:
                    BattleManager.Update();
                    break;
                case State.Editor:
                    EditorManager.Update();
                    break;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
                        DrawingManager.Begin();

            switch (GameStateManager.State)
            {
                case State.Menu:
                    MenuManager.Draw();
                    break;
                case State.Battle:
                    BattleManager.Draw();
                    break;
                case State.Editor:
                    GraphicsManager.Clear();
                    EditorManager.Draw();
                    break;
            }

            DrawingManager.End();

            base.Draw(gameTime);
        }

        public static void Quit()
        {
            ExitAction.Invoke();
        }
    }
}
