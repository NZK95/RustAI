namespace RustAI
{
    internal enum Status
    {
        NONE,
        WAITING_FOR_SERVER_ID_CONNECT,
        WAITING_FOR_SERVER_ID_INFO,
        WAITING_FOR_PLAYER_ID,
        WAITING_FOR_PLAYER_ID_TRACK,
        WAITING_FOR_SERVER_ID_AUTOCONNECT,
        FAVORITE_PLAYER,
        FAVORITE_SERVER,
        CONNECTING
    }
}
