using UnityEngine;
using TMPro;
using Core.MVC;
using UnityEngine.UI;

public class UserAssetsView : BaseView<UserAssetsViewMediator>
{
    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI adaBalanceText;
    [SerializeField] private TextMeshProUGUI snekBalanceText;
    [SerializeField] private TMP_InputField addressInputField;
    private string walletAddress = "";
    [SerializeField] private SnekkiesAssetSetting snekkiesAssetSetting;
    [SerializeField] private SnekUIObjectView snekUIObjectViewPrefab;
    [SerializeField] private Transform snekListContent; // ScrollView 的 Content
    [SerializeField] private int[] defaultNfts = new int[] { 11111, 22222, 33333 };

    private void Start()
    {
        addressInputField.onValueChanged.AddListener(UpdateFieldText);
    }
    public void UpdateFieldText(string inputText = "")
    {
        walletAddress = inputText;
    }
    public void OnRequestFetchWalletData()
    {
        walletAddress = "stake1uyfz4wt7npx7u9ukkewe6a9vvg6e53relzkfr87l55zqt9s70qhxc";
        mediator.OnRequestFetchWalletData(walletAddress);
    }

    public void UpdateAdaBalance(float balance)
    {
        Debug.Log($"Updating ADA Balance in UI: {balance}");
        if (adaBalanceText != null)
            adaBalanceText.text = $"{balance:N2} ADA";
    }

    public void UpdateSnekBalance(float balance)
    {
        Debug.Log($"Updating SNEK Balance in UI: {balance}");
        if (snekBalanceText != null)
            snekBalanceText.text = $"{balance:N0} SNEK";
    }

    public void UpdateNftCount(int[] nfts, int count)
    {
        Debug.Log($"Updating NFT Count in UI: {count}");

        // 清除舊的列表 (如果需要)

        for (int i = 0; i < defaultNfts.Length; i++)
        {
            SnekUIObjectView snekView = GetSnekUI(defaultNfts[i]);
            snekView.transform.SetParent(snekListContent, false);
        }
        // 根據數量生成新的 SnekView
        for (int i = 0; i < count; i++)
        {
            SnekUIObjectView snekView = GetSnekUI(nfts[i]);
            snekView.transform.SetParent(snekListContent, false);
        }
    }

    public SnekUIObjectView GetSnekUI(int id)
    {
        SnekUIObjectView snekView = Instantiate(snekUIObjectViewPrefab, snekListContent);

        SnekkiesAsset snekAsset = snekkiesAssetSetting.GetSnekkiesAsset(id);
        snekView.Setup(id, snekAsset.UISprite, (id) =>
        {
            SetSelectNFT(id);
        }
        );
        return snekView;
    }

    public void SetSelectNFT(int id)
    {
        mediator.OnSelectNFT(id);

    }


}
