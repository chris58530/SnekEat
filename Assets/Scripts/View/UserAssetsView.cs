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

    public void UpdateNftCount(int count)
    {
        Debug.Log($"Updating NFT Count in UI: {count}");

    }
}
