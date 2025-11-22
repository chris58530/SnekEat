public class TransitionEvent
{
    public const string REQUEST_TRANSITION = "TransitionEvent.REQUEST_TRANSITION";
    public const string TRANSITION_COMPLETE = "TransitionEvent.TRANSITION_COMPLETE";
}

public class ConnectWalletEvent
{
    public const string REQUEST_FETCH_WALLET_DATA = "ConnectWalletEvent.REQUEST_FETCH_WALLET_DATA";
    public const string ON_ADA_BALANCE_UPDATED = "ConnectWalletEvent.ON_ADA_BALANCE_UPDATED";
    public const string ON_SNEK_UPDATED = "ConnectWalletEvent.ON_SNEK_UPDATED";
    public const string ON_NFTS_UPDATED = "ConnectWalletEvent.ON_NFTS_UPDATED";

}
public class NetworkEvent
{
    public const string ON_NETWORK_CONNECTING = "NetworkEvent.ON_NETWORK_CONNECTING";
    public const string ON_NETWORK_CONNECTED = "NetworkEvent.ON_NETWORK_CONNECTED";
}
public class GameEvent
{
    public const string ON_STAGE_CHANGED = "GameEvent.ON_STAGE_CHANGED";
    public const string ON_GAME_INIT = "GameEvent.ON_GAME_INIT";
}

public class SkinEvent
{

    public const string ON_SETUP_SKIN = "SkinEvent.ON_SETUP_SKIN";
    public const string RUNNER_SKIN_SETUP_COMPLETE = "SkinEvent.RUNNER_SKIN_SETUP_COMPLETE";
}
