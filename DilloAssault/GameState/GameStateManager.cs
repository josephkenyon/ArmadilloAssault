using DilloAssault.GameState.Editor;
using DilloAssault.Graphics;

namespace DilloAssault.GameState
{
    public static class GameStateManager
    {
        private static State _state;
        public static State State { get { return _state; } set { SetState(value); } }

        private static void SetState(State newState)
        {
            _state = newState;

            if (_state == State.Battle)
            {
                GraphicsManager.SetBattleCursor();
            }
            else
            {
                if (_state == State.Editor)
                {
                    EditorManager.Initialize();
                }

                GraphicsManager.SetMenuCursor();
            }
        }
    }
}
