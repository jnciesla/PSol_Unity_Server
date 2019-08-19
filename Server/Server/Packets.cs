public enum ServerPackets
{
    SIngame = 1,
    SPlayerData,
    SMessage,
    SSystemByte,
    SPulse,
    SGalaxy,
    SItems,
}

public enum ClientPackets
{
    CMovement,
    CMessage,
    CLogin,
}

public enum SystemBytes
{
    SysInvPass = 1,
}

public enum ChatPackets
{
    Chat = 1,
    Notification,
    Error
}