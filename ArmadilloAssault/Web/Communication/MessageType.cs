namespace ArmadilloAssault.Web.Communication
{
    public enum ServerMessageType
    {
        Initiate,
        Kick,
        EndConnection,
        LobbyUpdate,
        BattleInitialization,
        BattleUpdate,
        BattleTermination,
        Pause,
        GameOver
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
        NextMode
    }
}
