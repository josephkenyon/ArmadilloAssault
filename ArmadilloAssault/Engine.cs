using ArmadilloAssault.Assets;
using ArmadilloAssault.Configuration;
using ArmadilloAssault.Configuration.Avatars;
using ArmadilloAssault.Controls;
using ArmadilloAssault.GameState;
using ArmadilloAssault.GameState.Battle;
using ArmadilloAssault.GameState.Battle.Web;
using ArmadilloAssault.GameState.Editor;
using ArmadilloAssault.GameState.Menus;
using ArmadilloAssault.Graphics;
using ArmadilloAssault.Sound;
using ArmadilloAssault.Web.Client;
using ArmadilloAssault.Web.Server;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ArmadilloAssault
{
    public class Engine : Game
    {
        private static Action ExitAction { get; set; }

        private static Queue<Action> Actions { get; set; } = [];

        public Engine()
        {
            GraphicsManager.Initialize(this);

            Content.RootDirectory = "Content"; 
            IsMouseVisible = true;
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

            //BattleManager.Initialize(new Dictionary<PlayerIndex, AvatarType>
            //{
            //    { PlayerIndex.One, AvatarType.Arthur }
            //}, "editor");

            GameStateManager.State = State.Menu;
        }

        protected override void Update(GameTime gameTime)
        {
            while (Actions.Count > 0)
            {
                var action = Actions.Dequeue();
                try
                {
                    action.Invoke();
                }
                catch (Exception ex)
                {
                    Trace.Write(ex);
                }
            }

            ControlsManager.Update();

            if (ClientManager.IsActive)
            {
                _ = ClientManager.BroadcastUpdate();
            }

            IsMouseVisible = GameStateManager.State != State.Battle || BattleManager.Paused;

            switch (GameStateManager.State)
            {
                case State.Menu:
                    MenuManager.Update();
                    break;
                case State.Battle:
                    if (ServerManager.IsServing || true)
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

            base.Draw(gameTime);
        }

        public static void QueueAction(Action action)
        {
            Actions.Enqueue(action);
        }

        public static void Quit()
        {
            ExitAction.Invoke();
        }
    }
}
