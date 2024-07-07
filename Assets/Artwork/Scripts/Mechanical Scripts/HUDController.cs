using UnityEngine;
using TMPro;

public class HUDController : MonoBehaviour
{
    [SerializeField] private TMP_Text hudText;
    [SerializeField] private TMP_Text keysCollectedText;
    [SerializeField] private TMP_Text torchlightText;
    [SerializeField] private TMP_Text staminaText;
    private int score = 10;
    private int keysCollected = 0;
    private int torchlight = 70;
    private int stamina = 5;
    private float staminaTimer = 0f;


    void Start()
    {
        UpdateHUD();
    }
    
    private void Update() {
        if (stamina < 5) {
            staminaTimer += Time.deltaTime;
            if (staminaTimer >= 3f) {
                stamina++;
                staminaTimer = 0f;
                UpdateHUD();
            }
        }
    }

    public void DecrementScore()
    {
        //score can be -ve too
        score--;
        UpdateHUD();
    }

    public void YakshiAttack()
    {
        score = 0;
        UpdateHUD();
    }

    public void StumbleOnDead()
    {
        score = 0;
        UpdateHUD();
    }

    public void CollectKey()
    {
        keysCollected++;
        UpdateHUD();
    }

    public void DecrementTorchlight()
    {
        if (torchlight >= 0)
        {
            torchlight--;
            UpdateHUD();
        }
    }

    public void IncrementTorchlight()
    {
        if (torchlight <= 240) {
            torchlight += 10;
            UpdateHUD();
        }
    }

    public int GetTorchlight()
    {
        return torchlight;
    }

    public bool incrementLives() {
        if (score < 10) {
            score++;
            UpdateHUD();
            return true;
        } else {
            return false;
        }
    }

    public void DecrementStamina()
    {
        if (stamina >= 0)
        {
            stamina--;
            UpdateHUD();
        }
    }

    public bool IsStaminaFine()
    {
        return stamina > 0;
    }

    public int GetKeysCollected()
    {
        return keysCollected;
    }

    private void UpdateHUD()
    {
        hudText.text = "Score: " + score;
        keysCollectedText.text = "Keys Collected: " + keysCollected;
        torchlightText.text = "Torchlight: " + torchlight + " s";
        staminaText.text = "Stamina: " + stamina;
    }



}
