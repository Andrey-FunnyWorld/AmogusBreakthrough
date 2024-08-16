using System.Collections;
using UnityEngine;

public class TeamHealthController : MonoBehaviour {

    public ProgressBarUI HpBar;
    public float maxHealth = 100;

    public RunningTexture BubbleShield;
    public float bubbleTime = 2f;
    public MultiStepSound MultiStepSound;
    bool isBubbleActive;

    bool recoverHealth;
    float currentHealth;
    float additionalHealthFactor = 1.5f;
    float additionalUltraHealthFactor = 3f;
    float healthRecoveryTimer;
    float healthRecoveryDuration = 0.5f; //every 0.5s increase health by minRecoverHealthValue
    float minRecoverHealthValue = 0.1f;

    [ContextMenu("DEBUG_TakeRandomDamage")]
    public void ForDebugTakeDamage() { //TODO remove later
        TakeDamage(UnityEngine.Random.Range(1f, 50f));
    }

    [ContextMenu("DEBUG_AddExtraHpPerk")]
    public void DebugAddExtraHpPerk() { //todo remove later
        ChangeHealthBy(additionalUltraHealthFactor);
    }

    void Awake() {
        currentHealth = maxHealth;
        InitBubbleShield();
    }
    void Start() => SubscribeEvents();
    void Update() => RecoverHealth();
    void OnDestroy() => UnsubscribeEvents();


    public void HandlePerk(PerkType perk) {
        if (perk == PerkType.ExtraHealth) {
            ChangeHealthBy(additionalHealthFactor);
        } else if (perk == PerkType.ExtraHealthUltra) {
            ChangeHealthBy(additionalUltraHealthFactor);
        } else if (perk == PerkType.RegenHP) {
            recoverHealth = true;
        }
    }

    void InitBubbleShield() {
        BubbleShield.SetSpeed(7f);
    }

    void RunBubble(bool run) {
        BubbleShield.gameObject.SetActive(run);
        BubbleShield.IsRunning = run;
        if (run) {
            MultiStepSound.Play();
            StartCoroutine(nameof(BubbleVFXRoutine));
        } else {
            MultiStepSound.Stop();
        }
    }

    IEnumerator BubbleVFXRoutine() {
        float timer = 0f;
        float counter = 0f;
        while(timer < bubbleTime) {
            timer += Time.deltaTime;
            counter = timer * 3;
            var value = Mathf.Sin(counter) * 0.5f + 7.5f;
            BubbleShield.transform.localScale = Vector3.one * value;
            yield return null;
        }
    }

    void TakeDamage(float damage) {
        if (isBubbleActive)
            return;

        float realDamage = Mathf.Min(currentHealth, damage);
        currentHealth -= realDamage;

        UpdateHealthUI();

        if (currentHealth <= 0)
            OnTeamDie();
    }

    void HandleBubbleAbility(object arg) {
        isBubbleActive = true;
        RunBubble(isBubbleActive);
        StartCoroutine(Utils.WaitAndDo(bubbleTime, () => {
            isBubbleActive = false;
            RunBubble(isBubbleActive);
        }));
    }

    void ChangeHealthBy(float factor) {
        maxHealth = (int)(maxHealth * factor);
        currentHealth = (int)(currentHealth * factor);
        UpdateHealthUI(true);
    }

    void UpdateHealthUI(bool isPerk = false) {
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

            UpdateHealthUI();
        }
    }
    
    void OnTeamDie() {
        EventManager.TriggerEvent(EventNames.TeamDead);
    }

    #region Events
    void SubscribeEvents() {
        EventManager.StartListening(EventNames.AbilityBubble, HandleBubbleAbility);
    }

    void UnsubscribeEvents() {
        EventManager.StopListening(EventNames.AbilityBubble, HandleBubbleAbility);
    }
    #endregion
}
