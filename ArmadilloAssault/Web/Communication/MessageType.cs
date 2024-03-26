namespace ArmadilloAssault.Web.Communication
{
    public enum ServerMessageType
    {
        Initiate,
        Kick,
        EndConnection,
        LobbyUpdate,
        BattleInitialization,
        BattleFrame,
        BattleTermination,
        Pause,
        GameOver,
        BattleUpdate
    }

    public enum ClientMessageType
    {
        JoinGame,
        InputUpdate,
        AvatarSelection,
        NextLevel,
        PreviousLevel,
        Pause,
        PreviousMode,
        NextMode,
        TeamIndexIncrement,
        CrownPlayer
    }
}
