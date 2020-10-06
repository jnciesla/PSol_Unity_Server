public enum ServerPackets
{
    SIngame = 1,
    SPlayerData,
    SMessage,
    SSystemByte,
    SPulse,
    SGalaxy,
    SItems,
    SInventory,
    SGlobals,
    SRecipes
}

public enum ClientPackets
{
    CMovement,
    CMessage,
    CLogin,
    CRegister,
    CEquip,
    CAttack,
    CLoot,
    CShopBuy,
    CShopSell,
    CDischarge,
    CLog,
    CManufacture,
    CHangarBuy
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