using ArmadilloAssault.GameState.Battle.Camera;
using ArmadilloAssault.GameState.Editor;
using ArmadilloAssault.GameState.Menu;
using ArmadilloAssault.Graphics;
using ArmadilloAssault.Sound;

namespace ArmadilloAssault.GameState
{
    public static class GameStateManager
    {
        private static State _state;
        public static State State { get { return _state; } set { SetState(value); } }

        private static void SetState(State newState)
        {
            var oldState = _state;
            if (oldState != newState)
            {
                _state = newState;

                if (_state == State.Battle)
                {
                    SoundManager.PlayMusic(MusicSong.battle_music);
                }
                else if (_state == State.Menu)
                {
                    CameraManager.Disable();
                    MenuManager.Initialize();

                    if (oldState == State.Battle || oldState == State.None)
                    {
                        SoundManager.PlayMusic(MusicSong.menu_music);
                    }
                }
                else if (_state == State.Editor)
                {
                    CameraManager.Disable();
                    EditorManager.Initialize();
                }
            }
        }
    }
}
