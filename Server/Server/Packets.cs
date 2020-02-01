public enum ServerPackets
{
    SIngame = 1,
    SPlayerData,
    SMessage,
    SSystemByte,
    SPulse,
    SGalaxy,
    SItems,
    SInventory
}

public enum ClientPackets
{
    CMovement,
    CMessage,
    CLogin,
    CRegister,
    CEquip
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