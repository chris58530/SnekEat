using System;

[Serializable]
public class CardanoAccountInfo
{
    public string stake_address;
    public bool active;
    public string controlled_amount; // 注意：Blockfrost回傳的是字串，數值可能很大
    public string rewards_sum;
    public string withdrawals_sum;
    public string reserves_sum;
    public string treasury_sum;
    public string withdrawable_amount;
    public string pool_id;
}
[Serializable]
public class CardanoAsset
{
    public string unit;      // 代幣的 ID (PolicyID + HexName)
    public string quantity;  // 數量 (注意：也是字串，需要轉換)
}