public class TransitionEvent
{
    public const string REQUEST_TRANSITION = "REQUEST_TRANSITION";
    public const string TRANSITION_COMPLETE = "TRANSITION_COMPLETE";
}

public class ConnectWalletEvent
{
    public const string REQUEST_FETCH_WALLET_DATA = "REQUEST_FETCH_WALLET_DATA";
    public const string ON_ADA_BALANCE_UPDATED = "ON_ADA_BALANCE_UPDATED";
    public const string ON_SNEK_UPDATED = "ON_SNEK_UPDATED";
    public const string ON_NFTS_UPDATED = "ON_NFTS_UPDATED";

}

public class GameEvent
{
    public const string ON_STAGE_CHANGED = "ON_STAGE_CHANGED";
}

public class SkinEvent
{

    public const string ON_SETUP_SKIN = "ON_SETUP_SKIN";
    public const string RUNNER_SKIN_SETUP_COMPLETE = "RUNNER_SKIN_SETUP_COMPLETE";
}
