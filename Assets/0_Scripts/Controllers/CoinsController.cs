using TMPro;
using UnityEngine;

public class CoinsController : MonoBehaviour {

    public TextMeshProUGUI casualCoinsText;
    public TextMeshProUGUI premiumCoinsText;

    int casualCoins;
    int premiumCoins;
    bool extraCoinsPerk;

    public int CasualCoins => casualCoins;
    public int PremiumlCoins => premiumCoins;

    void Awake() => SubscribeEvents();
    void OnDestroy() => UnsubscribeEvents();

    public void ApplyExtraCoinsPerk() =>
        extraCoinsPerk = true;

    public void AddCasualCoins() {
        int value = UnityEngine.Random.Range(1, 4); //или брать мин/макс значения в зависимости от поверженного монстра, сами эти значения хранить, например, в скриптаблОбжектах
        if (extraCoinsPerk)
            value += UnityEngine.Random.Range(1, 4);
        casualCoins += value;
        UpdateUI();
    }

    public void AddPremiumCoins() {
        premiumCoins += 1; //или брать мин/макс значения в зависимости от поверженного монстра, сами эти значения хранить, например, в скриптаблОбжектах
        UpdateUI();
    }

    void UpdateUI() {
        if (casualCoinsText != null)
            casualCoinsText.text = casualCoins.ToString();
        if (premiumCoinsText != null)
            premiumCoinsText.text = premiumCoins.ToString();
    }

    void HandleEnemyDied(object arg0) {
        AddCasualCoins();
    }

    void SubscribeEvents() =>
        EventManager.StartListening(EventNames.EnemyDied, HandleEnemyDied);

    void UnsubscribeEvents() =>
        EventManager.StopListening(EventNames.EnemyDied, HandleEnemyDied);
}
