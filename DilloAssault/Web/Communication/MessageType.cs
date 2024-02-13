namespace DilloAssault.Web.Communication
{
    public enum ServerMessageType
    {
        Initiate,
        Termination,
        LobbyUpdate,
        BattleInitialization,
        BattleUpdate,
        BattleTermination,
    }

    public enum ClientMessageType
    {
        JoinGame,
        LeaveGame,
        InputUpdate
    }
}
