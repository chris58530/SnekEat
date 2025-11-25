using UnityEngine;

public class TransitionEvent
{
    public const string REQUEST_TRANSITION = "TransitionEvent.REQUEST_TRANSITION";
    public const string TRANSITION_COMPLETE = "TransitionEvent.TRANSITION_COMPLETE";

    public const string FADE_OUT_COMPLETE = "TransitionEvent.FADE_OUT_COMPLETE";
    public const string FADE_IN_COMPLETE = "TransitionEvent.FADE_IN_COMPLETE";
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
    public const string ON_MENU_INIT = "GameEvent.ON_MENU_INIT";
    public const string ON_GAME_INIT = "GameEvent.ON_GAME_INIT";
}

public class SkinEvent
{
    public const string ON_SETUP_SKIN = "SkinEvent.ON_SETUP_SKIN";
    public const string RUNNER_SKIN_SETUP_COMPLETE = "SkinEvent.RUNNER_SKIN_SETUP_COMPLETE";
}

public class PortalEvent
{
    public const string ON_PORTAL_SPAWN = "PortalEvent.ON_PORTAL_SPAWN";
    public const string ON_PORTAL_ENTERED = "PortalEvent.ON_PORTAL_ENTERED";
    public const string ON_PORTAL_HIT = "PortalEvent.ON_PORTAL_HIT";
    public const string ON_PORTAL_DESPAWN = "PortalEvent.ON_PORTAL_DESPAWN";
}

public class BackgroundEvent
{
    public const string ON_SETUP_BACKGROUND = "BackgroundEvent.ON_SETUP_BACKGROUND";
}

public class BossEvent
{
    //組裝事件============
    public const string REQUEST_START_FEATURE = "BossEvent.REQUEST_START_FEATURE";
    //====================

    public const string REQUEST_START_FEATURE_GREEN = "BossEvent.REQUEST_START_FEATURE_GREEN";
}

public class PlayerActionEvent
{
    public const string ON_PLAYER_SHOOT = "PlayerActionEvent.ON_PLAYER_SHOOT";
    public const string ON_PLAYER_DASH = "PlayerActionEvent.ON_PLAYER_DASH";
}

public class CameraEvent
{
    public const string ON_SET_CAMERA_TARGET = "CameraEvent.ON_SET_CAMERA_TARGET";
    public const string ON_FOCUS_TEMPORARY = "CameraEvent.ON_FOCUS_TEMPORARY";
}

public class CameraFocusSetting
{
    public Transform Target;
    public float FocusSize;
    public float? FocusZ; // Optional Z-axis focus
    public float InDuration;
    public float StayDuration;
    public float OutDuration;
}
