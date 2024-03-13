using ArmadilloAssault.GameState.Battle.Camera;
using ArmadilloAssault.GameState.Editor;
using ArmadilloAssault.GameState.Menus;
using ArmadilloAssault.Sound;

namespace ArmadilloAssault.GameState
{
    public static class GameStateManager
    {
        public static State State { get; private set; }
        private static State? NewState { get; set; }

        public static void PushNewState(State newState)
        {
            NewState = newState;
        }

        public static void Update()
        {
            if (NewState != null)
            {
                SetState((State)NewState);
                NewState = null;
            }
        }

        private static void SetState(State newState)
        {
            var oldState = State;
            if (oldState != newState)
            {
                State = newState;

                if (State == State.Battle)
                {
                    SoundManager.PlayMusic(MusicSong.battle_music);
                }
                else if (State == State.Menu)
                {
                    CameraManager.Disable();
                    MenuManager.Initialize();

                    if (oldState == State.Battle || oldState == State.None)
                    {
                        SoundManager.PlayMusic(MusicSong.menu_music);
                    }
                }
                else if (State == State.Editor)
                {
                    CameraManager.Disable();
                    EditorManager.Initialize();
                }
            }
        }
    }
}
