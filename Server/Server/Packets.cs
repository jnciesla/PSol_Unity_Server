public enum ServerPackets
{
    SIngame =1,
    SPlayerData,
    SPlayerMove,
    SMessage,
    SSystemByte,
    SPulse
}

public enum ClientPackets
{
    CThankYouMessage = 1,
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