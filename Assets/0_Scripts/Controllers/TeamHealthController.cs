using System;
using System.Collections;
using UnityEngine;

public class TeamHealthController : MonoBehaviour {
    public ProgressBarUI HpBar;
    public float maxHealth = 100;

    public RunningTexture BubbleShield;
    public float bubbleTime = 2f;
    public MultiStepSound MultiStepSound;
    public AudioSource DefeatHitSource;
    public float HealthRecoveryDuration = 0.5f; //every 0.5s increase health by minRecoverHealthValue
    public float MinRecoverHealthValue = 0.5f;

    bool isBubbleActive;
    bool recoverHealth;
    float currentHealth;
    float additionalHealthFactor = 1.5f;
    float additionalUltraHealthFactor = 3f;
    Coroutine healthRegenCoroutine;
    void Awake() {
        currentHealth = maxHealth;
        HpBar.Value = currentHealth;
        InitBubbleShield();
    }
    void Start() => SubscribeEvents();
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
    IEnumerator HealthRegen(float tickDuration) {
        float timer = 0;
        while (true) {
            if (currentHealth == maxHealth) {
                yield return null;    
            }
            timer += Time.deltaTime;
            if (timer >= tickDuration) {
                timer = 0;
                float healthToAdd = Mathf.Min(MinRecoverHealthValue, maxHealth - currentHealth);
                currentHealth += healthToAdd;
                UpdateHealthUI();
            }
            yield return null;            
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

    public void TakeDamage(float damage) {
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
    void StartMovement(object arg) {
        if (recoverHealth)
            healthRegenCoroutine = StartCoroutine(HealthRegen(HealthRecoveryDuration));
    }
    void RoadFinished(object arg) {
        if (healthRegenCoroutine != null) StopCoroutine(healthRegenCoroutine);
    }
    void OnTeamDie() {
        DefeatHitSource.Play();
        EventManager.TriggerEvent(EventNames.TeamDead);
    }

    #region Events
    void SubscribeEvents() {
        EventManager.StartListening(EventNames.AbilityBubble, HandleBubbleAbility);
        EventManager.StartListening(EventNames.StartMovement, StartMovement);
        EventManager.StartListening(EventNames.RoadFinished, RoadFinished);
    }

    void UnsubscribeEvents() {
        EventManager.StopListening(EventNames.AbilityBubble, HandleBubbleAbility);
        EventManager.StopListening(EventNames.StartMovement, StartMovement);
        EventManager.StopListening(EventNames.RoadFinished, RoadFinished);
    }
    #endregion
}
