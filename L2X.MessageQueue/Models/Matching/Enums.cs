namespace L2X.MessageQueue.Models.Matching;

public enum CancelReason : byte
{
    UserRequested = 1,
    NoLiquidity = 2, //MarketOrderNoLiquidity
    ImmediateOrCancel = 3,
    FillOrKill = 4,
    BookOrCancel = 5,
    ValidityExpired = 6,
    LessThanStepSize = 7, //MarketOrderCannotMatchLessThanStepSize
    InvalidOrder = 8,
    SelfMatch = 9,
}

public enum MatchState : byte
{
    //Success Result
    Order_Accepted = 1,
    Cancel_Acepted = 2,
    Order_Valid = 3,

    //Failure Result
    Order_Not_Exists = 11,
    Order_Invalid = 12, //Invalid_Price_Quantity_Stop_Price_Order_Amount_Or_Total_Quantity
    Duplicate_Order = 13,
    BOC_Cannot_MOS = 14, //Book_Or_Cancel_Cannot_Be_Market_Or_Stop_Order
    IOC_Cannot_MOS = 15, //Immediate_Or_Cancel_Cannot_Be_Market_Or_Stop_Order
    Iceberg_Cannot_MOSM = 16, //Iceberg_Order_Cannot_Be_Market_Or_Stop_Market_Order
    Iceberg_Cannot_FOKIOC = 17, //Iceberg_Order_Cannot_Be_FOK_or_IOC
    Invalid_Iceberg_Volume = 18, //Invalid_Iceberg_Order_Total_Quantity
    Fok_Cannot_Stop_Order = 19, //Fill_Or_Kill_Cannot_Be_Stop_Order
    Invalid_Cancel_On_For_GTD = 20,
    GTD_Cannot_Market_IOCFOK = 21, //GoodTillDate_Cannot_Be_Market_Or_IOC_or_FOK
    MOO_Not_Both_Amount_Or_Volume = 22, //Market_Order_Only_Supported_Order_Amount_Or_Quantity_No_Both
    Market_Buy_Amount_Only = 23, //Order_Amount_Only_Supported_For_Market_Buy_Order
    Not_Multiple_Of_Step_Size = 24 //Quantity_And_Total_Quantity_Should_Be_Multiple_Of_Step_Size
}

public enum SelfMatchAction : byte
{
    Match = 0,
    Cancel_Newest = 1,
    Cancel_Oldest = 2,
    Decrement = 3,
}