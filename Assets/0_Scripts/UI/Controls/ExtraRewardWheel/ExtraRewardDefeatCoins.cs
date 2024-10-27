public class ExtraRewardDefeatCoins : ExtraRewardWheelPrizeBase {
    public DefeatUI DefeatUI;
    public int[] CoinAdditions;
    public override void GiveReward(int sectorNo) {
        DefeatUI.AddRewardedCoins(CoinAdditions[sectorNo]);
    }
}
