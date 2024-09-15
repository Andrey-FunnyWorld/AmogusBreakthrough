using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class CoinsController : MonoBehaviour {
    public ScoreText casualCoinsText;
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

    public void AddCasualCoins(float coins) {
        int value = (int)Mathf.Ceil(coins); //UnityEngine.Random.Range(1, 4); //или брать мин/макс значения в зависимости от поверженного монстра, сами эти значения хранить, например, в скриптаблОбжектах
        if (extraCoinsPerk)
            value += UnityEngine.Random.Range(1, 2);
        casualCoins += value;
        UpdateUI();
    }

    public void AddPremiumCoins() {
        premiumCoins += 1; //или брать мин/макс значения в зависимости от поверженного монстра, сами эти значения хранить, например, в скриптаблОбжектах
        UpdateUI();
    }

    void UpdateUI() {
        if (casualCoinsText != null)
            casualCoinsText.Score = casualCoins;
        if (premiumCoinsText != null)
            premiumCoinsText.text = premiumCoins.ToString();
    }

    void HandleEnemyDied(object arg) {
        EnemyBase enemy = (EnemyBase)arg;
        AddCasualCoins(enemy.CoinReward);
    }

    void SubscribeEvents() =>
        EventManager.StartListening(EventNames.EnemyDied, HandleEnemyDied);

    void UnsubscribeEvents() =>
        EventManager.StopListening(EventNames.EnemyDied, HandleEnemyDied);
}
