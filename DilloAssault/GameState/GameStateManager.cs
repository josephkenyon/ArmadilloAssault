using DilloAssault.Configuration.Avatars;
using DilloAssault.GameState.Editor;
using DilloAssault.Graphics;
using DilloAssault.Sound;

namespace DilloAssault.GameState
{
    public static class GameStateManager
    {
        private static State _state;
        public static State State { get { return _state; } set { SetState(value); } }

        private static void SetState(State newState)
        {
            var oldState = _state;
            _state = newState;

            if (_state == State.Battle)
            {
                GraphicsManager.SetBattleCursor();

                SoundManager.PlayMusic(MusicSong.battle_music);
            }
            else if (_state == State.Menu)
            {
                GraphicsManager.SetMenuCursor();

                if (oldState == State.Battle || oldState == State.None)
                {
                    SoundManager.PlayMusic(MusicSong.menu_music);
                }
            }
            else if (_state == State.Editor)
            {
                GraphicsManager.SetMenuCursor();

                EditorManager.Initialize();
            }
        }
    }
}
