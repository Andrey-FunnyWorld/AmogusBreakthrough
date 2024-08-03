using UnityEngine;

public class TeamHealthController : MonoBehaviour {

    public ProgressBarUI HpBar;
    public float maxHealth = 100;

    bool recoverHealth;
    float currentHealth;
    float additionalHealthFactor = 1.5f;
    float additionalUltraHealthFactor = 3f;
    float healthRecoveryTimer;
    float healthRecoveryDuration = 0.5f; //every 0.5s increase health by minRecoverHealthValue
    float minRecoverHealthValue = 0.1f;

    void Awake() => currentHealth = maxHealth;
    void Start() => SubscribeEvents();
    void Update() => RecoverHealth();
    void OnDestroy() => UnsubscribeEvents();

    [ContextMenu("DebugAddExtraHpPerk")]
    public void DebugAddExtraHpPerk() {
        ChangeHealthBy(additionalUltraHealthFactor);
    }

    public void HandlePerk(PerkType perk) {
        if (perk == PerkType.ExtraHealth) {
            ChangeHealthBy(additionalHealthFactor);
        } else if (perk == PerkType.ExtraHealthUltra) {
            ChangeHealthBy(additionalUltraHealthFactor);
        } else if (perk == PerkType.RegenHP) {
            recoverHealth = true;
        }
    }

    [ContextMenu("DEBUG_TakeRandomDamage")]
    public void ForDebugTakeDamage() { //TODO remove later
        TakeDamage(UnityEngine.Random.Range(1f, 50f));
    }

    void TakeDamage(float damage) {
        float realDamage = Mathf.Min(currentHealth, damage);
        currentHealth -= realDamage;

        UpdateHealthUI();

        if (currentHealth <= 0)
            OnTeamDie();
    }

    void ChangeHealthBy(float factor) {
        maxHealth = (int)(maxHealth * factor);
        currentHealth = (int)(currentHealth * factor);
        UpdateHealthUI(true);
    }

    void UpdateHealthUI(bool isPerk = false) {
        // HpBar.Value = currentHealth / maxHealth;
        if (isPerk)
            HpBar.MaxValue = maxHealth;
        HpBar.Value = currentHealth;
    }

    void RecoverHealth() {
        if (!recoverHealth)
            return;

        if (currentHealth == maxHealth)
            return;

        healthRecoveryTimer += Time.deltaTime;

        if (healthRecoveryTimer >= healthRecoveryDuration) {
            healthRecoveryTimer = 0;

            float healthToAdd = Mathf.Min(minRecoverHealthValue, maxHealth - currentHealth);
            currentHealth += healthToAdd;
            Debug.Log($"Health recovery, curent: {currentHealth}");

            UpdateHealthUI();
        }
    }
    
    void OnTeamDie() {
        EventManager.TriggerEvent(EventNames.TeamDead);
    }

    #region Events
    void SubscribeEvents() {
        //
    }
    void UnsubscribeEvents() {
        //
    }
    #endregion
}
