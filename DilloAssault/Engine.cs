using DilloAssault.Assets;
using DilloAssault.Configuration;
using DilloAssault.Controls;
using DilloAssault.GameState;
using DilloAssault.GameState.Battle;
using DilloAssault.GameState.Editor;
using DilloAssault.GameState.Menu;
using DilloAssault.Graphics;
using DilloAssault.Web.Client;
using DilloAssault.Web.Server;
using Microsoft.Xna.Framework;
using System;

namespace DilloAssault
{
    public class Engine : Game
    {
        private static Action ExitAction { get; set; }

        public Engine()
        {
            GraphicsManager.Initialize(this);

            Content.RootDirectory = "Content"; 
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            ControlsManager.Initialize();

            GameStateManager.State = State.Menu;

            ExitAction = Exit;

            base.Initialize();
        }

        protected override void LoadContent()
        {

            GraphicsManager.LoadContent(GraphicsDevice, Content);
            ConfigurationManager.LoadContent(Content);

            var Scene = new Scene(ConfigurationManager.GetSceneConfiguration());

            EditorManager.Initialize(Scene);

            BattleManager.Scene = Scene;

            //BattleManager.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            ControlsManager.Update();

            if (ClientManager.IsActive)
            {
                ClientManager.BroadcastUpdate();
            }

            switch (GameStateManager.State)
            {
                case State.Menu:
                    MenuManager.Update();
                    break;
                case State.Battle:
                    if (ServerManager.IsServing)
                    {
                        BattleManager.UpdateServer();
                    }
                    else
                    {
                        BattleManager.UpdateClient();
                    }
                    break;
                case State.Editor:
                    EditorManager.Update();
                    break;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsManager.Clear();

            switch (GameStateManager.State)
            {
                case State.Menu:
                    MenuManager.Draw();
                    break;
                case State.Battle:
                    BattleManager.Draw();
                    break;
                case State.Editor:
                    EditorManager.Draw();
                    break;
            }

            base.Draw(gameTime);
        }

        public static void Quit()
        {
            ExitAction.Invoke();
        }
    }
}
