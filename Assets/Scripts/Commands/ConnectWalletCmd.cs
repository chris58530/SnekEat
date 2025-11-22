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
    [Inject] private new Listener listener;
    [Inject] private WalletProxy walletProxy;
    private string apiKey = "mainnetHmNiDJMzMF9M9Nb4PCEDkFJhgaBUeZL7";
    private string baseUrl = "https://cardano-mainnet.blockfrost.io/api/v0";
    private string walletAddress;
    private const string SNEK_POLICY_ID = "279c909f348e533da5808898f87f9a14bb2c3dfbbacccd631d927a3f534e454b";
    private const string SNEKKIES_NFT_POLICY_ID = "b558ea5ecfa2a6e9701dab150248e94104402f789c090426eb60eb60";

    private bool isFetching = false;
    private bool isRunning = false;
    private bool lastRequestSuccess = false;
    private int fetchLimitTime = 10;

    public override void Execute(MonoBehaviour mono)
    {
        // 註冊事件監聽
        listener.RegisterListeners(this);

        isLazy = true;
        if (walletAddress == null)
        {
            // 即使 walletAddress 為 null，也要啟動 Coroutine 等待事件觸發
            mono.StartCoroutine(FetchAllWalletData(null));
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

            if (string.IsNullOrEmpty(this.walletAddress) || !this.walletAddress.StartsWith("stake1"))
            {
                Debug.LogError("Invalid Stake Address format.");
                isRunning = false;
                continue;
            }

            yield return GetAccountInfo(this.walletAddress);

            if (lastRequestSuccess)
            {
                yield return GetAssets(this.walletAddress);
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
                    // 【修正】Blockfrost 返回的是 JSON 陣列 [...]，JsonUtility 無法直接解析
                    // 需要手動包裝成 { "Items": [...] }
                    string wrappedJson = "{\"Items\":" + jsonResult + "}";
                    CardanoAsset[] assets = JsonHelper.FromJson<CardanoAsset>(wrappedJson);

                    List<int> foundNftIds = new List<int>();
                    long totalSnekToken = 0;

                    if (assets != null)
                    {
                        foreach (var asset in assets)
                        {
                            // 【修正】加入空值檢查
                            if (asset == null || string.IsNullOrEmpty(asset.unit)) continue;

                            if (asset.unit.StartsWith(SNEK_POLICY_ID) && asset.unit.Length > 56)
                            {
                                if (long.TryParse(asset.quantity, out long amount))
                                {
                                    totalSnekToken += amount;
                                }
                                continue;
                            }

                            // 確保長度足夠再取 Substring
                            if (asset.unit.Length > 56)
                            {
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
                        }
                        walletProxy.SetSnekBalances(totalSnekToken);
                        walletProxy.SetSnekNFT(foundNftIds.ToArray());

                        Debug.Log($"Assets updated. SNEK: {totalSnekToken}, NFTs: {foundNftIds.Count}");
                    }
                    else
                    {
                        Debug.LogWarning("Parsed assets array is null.");
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Asset parsing error: {e.Message}\nStack Trace: {e.StackTrace}");
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

    // 【新增】內部 JsonHelper 類別，用於解析陣列
    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.Items;
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }
    }
}
