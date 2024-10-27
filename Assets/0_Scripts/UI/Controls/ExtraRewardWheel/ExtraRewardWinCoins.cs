public class ExtraRewardWinCoins : ExtraRewardWheelPrizeBase {
    public ResultUI ResultUI;
    public float[] CoinMultipliers;
    public override void GiveReward(int sectorNo) {
        ResultUI.MultiplyRewardedCoins(CoinMultipliers[sectorNo]);
    }
}
