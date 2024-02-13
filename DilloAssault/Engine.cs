using DilloAssault.Assets;
using DilloAssault.Configuration;
using DilloAssault.Controls;
using DilloAssault.GameState;
using DilloAssault.GameState.Battle;
using DilloAssault.GameState.Editor;
using DilloAssault.GameState.Menu;
using DilloAssault.Graphics;
using DilloAssault.Web.Client;
using Microsoft.Xna.Framework;

namespace DilloAssault
{
    public class Engine : Game
    {
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
                    MenuManager.Update(Exit);
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
            GraphicsManager.Clear();

            switch (GameStateManager.State)
            {
                case State.Battle:
                    BattleManager.Draw();
                    break;
                case State.Editor:
                    EditorManager.Draw();
                    break;
            }

            base.Draw(gameTime);
        }
    }
}
