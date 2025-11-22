using UnityEngine;
using Core.MVC;
using System.Collections;
using UnityEngine.Networking;
using System.Text;
using System;
using System.Collections.Generic;
using Zenject;

public class ConnectWalletCmd : ICommand
{
    [Inject] private WalletProxy walletProxy;

    private string apiKey = "mainnetHmNiDJMzMF9M9Nb4PCEDkFJhgaBUeZL7";
    private string baseUrl = "https://cardano-mainnet.blockfrost.io/api/v0";
    // private string myTestAddress = "stake1uyfz4wt7npx7u9ukkewe6a9vvg6e53relzkfr87l55zqt9s70qhxc";
    private string walletAddress;
    private const string SNEK_POLICY_ID = "279c909f348e533da5808898f87f9a14bb2c3dfbbacccd631d927a3f534e454b";
    private const string SNEKKIES_NFT_POLICY_ID = "b558ea5ecfa2a6e9701dab150248e94104402f789c090426eb60eb60";

    private bool isFetching = false;
    private bool isRunning = false;
    private bool lastRequestSuccess = false;
    private int fetchLimitTime = 10;

    public override void Execute(MonoBehaviour mono)
    {
        isLazy = true;
        if (walletAddress == null)
        {
            return;
        }
        mono.StartCoroutine(FetchAllWalletData(walletAddress));
    }

    [Listener(ConnectWalletEvent.REQUEST_FETCH_WALLET_DATA)]
    public void FetchWalletData(string walletAddress)
    {
        this.walletAddress = walletAddress;
        Debug.Log($"FetchWalletData called with address: {walletAddress}");
        if (string.IsNullOrEmpty(walletAddress))
        {
            Debug.LogError("Wallet address is null or empty.");
            return;
        }
        if (!isFetching && !isRunning)
        {
            Debug.Log("Request received. Starting fetch process.");
            isFetching = true;
        }
        else
        {
            Debug.LogWarning("Fetch process is already running or pending.");
        }
    }

    IEnumerator FetchAllWalletData(string stakeAddress)
    {
        while (true)
        {
            yield return new WaitUntil(() => isFetching);

            isFetching = false;
            isRunning = true;
            lastRequestSuccess = false;

            if (string.IsNullOrEmpty(stakeAddress) || !stakeAddress.StartsWith("stake1"))
            {
                Debug.LogError("Invalid Stake Address format.");
                isRunning = false;
                continue;
            }

            yield return GetAccountInfo(stakeAddress);

            if (lastRequestSuccess)
            {
                yield return GetAssets(stakeAddress);
            }
            else
            {
                Debug.LogError("Account info fetch failed or timed out. Skipping asset fetch.");
            }

            isRunning = false;
            Debug.Log("Fetch cycle complete. Waiting for next request.");
        }
    }

    IEnumerator GetAccountInfo(string stakeAddress)
    {
        string url = $"{baseUrl}/accounts/{stakeAddress}";
        lastRequestSuccess = false;

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("project_id", apiKey);
            request.timeout = fetchLimitTime;

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResult = request.downloadHandler.text;
                CardanoAccountInfo info = JsonUtility.FromJson<CardanoAccountInfo>(jsonResult);

                if (long.TryParse(info.controlled_amount, out long lovelace))
                {
                    float ada = lovelace / 1000000.0f;
                    Debug.Log($"ADA Balance: {ada}");
                    walletProxy.SetADABalances(ada);
                    lastRequestSuccess = true;
                }
            }
            else
            {
                Debug.LogError($"ADA Fetch Error: {request.error}");
            }
        }
    }

    IEnumerator GetAssets(string stakeAddress)
    {
        string url = $"{baseUrl}/accounts/{stakeAddress}/addresses/assets";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("project_id", apiKey);
            request.timeout = fetchLimitTime;

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResult = request.downloadHandler.text;

                try
                {
                    CardanoAsset[] assets = JsonHelper.FromJson<CardanoAsset>(jsonResult);
                    List<int> foundNftIds = new List<int>();
                    long totalSnekToken = 0;

                    if (assets != null)
                    {
                        foreach (var asset in assets)
                        {
                            if (asset.unit.StartsWith(SNEK_POLICY_ID) && asset.unit.Length > 56)
                            {
                                if (long.TryParse(asset.quantity, out long amount))
                                {
                                    totalSnekToken += amount;
                                }
                                continue;
                            }

                            string policyId = asset.unit.Substring(0, 56);
                            if (policyId == SNEKKIES_NFT_POLICY_ID)
                            {
                                string assetNameHex = asset.unit.Substring(56);
                                string realName = HexStringToString(assetNameHex);
                                string numberPart = realName.Replace("Snekkie", "");
                                if (int.TryParse(numberPart, out int snekId))
                                {
                                    foundNftIds.Add(snekId);
                                }
                            }
                        }
                        walletProxy.SetSnekBalances(totalSnekToken);
                        walletProxy.SetSnekNFT(foundNftIds.ToArray());

                        Debug.Log($"Assets updated. SNEK: {totalSnekToken}, NFTs: {foundNftIds.Count}");
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Asset parsing error: {e.Message}");
                }
            }
            else
            {
                Debug.LogError($"Asset Fetch Error: {request.error}");
            }
        }
    }

    private string HexStringToString(string hexString)
    {
        if (hexString == null || (hexString.Length % 2) != 0) return hexString;
        var sb = new StringBuilder();
        try
        {
            for (var i = 0; i < hexString.Length; i += 2)
            {
                var hexChar = hexString.Substring(i, 2);
                sb.Append((char)Convert.ToByte(hexChar, 16));
            }
            return sb.ToString();
        }
        catch { return hexString; }
    }
}