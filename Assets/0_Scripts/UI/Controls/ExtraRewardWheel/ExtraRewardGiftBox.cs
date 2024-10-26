using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraRewardGiftBox : ExtraRewardWheelPrizeBase {
    public GiftController GiftController;
    public GiftType[] GiftBySector;
    public override void GiveReward(int sectorNo) {
        HtmlBridge.Instance.ReportMetric(MetricNames.GiftAd);
        GiftController.GiftBox.Open();
        GiftController.GetGift(GiftBySector[sectorNo], () => {
            Destroy(GiftController.GiftBox.gameObject);
        });
    }
}
