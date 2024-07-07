using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{

    /*External References*/
    [SerializeField] private float health = 5;
    [SerializeField] private int charge = 60;
    [SerializeField] private int keys = 0;
    [SerializeField] private int stamina = 5;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI chargeText;
    [SerializeField] private TextMeshProUGUI keysText;
    [SerializeField] private TextMeshProUGUI staminaText;
    [SerializeField] private TextMeshProUGUI diedText;


    void Start()
    {
        if (healthText != null) healthText.text = health.ToString();
        if (chargeText != null) chargeText.text = health.ToString() + "s";
        if (keysText != null) keysText.text = keys.ToString();
        if (staminaText != null) staminaText.text = stamina.ToString();
        if (diedText != null) diedText.text = "";
        StartCoroutine(UseCharge());
    }

    // Update is called once per frame
    void Update()
    {
    }

    /*Public Methods*/
    public void AttackFromYakshi()
    {
        Debug.Log("Attack from Yakshi Invoked.");
        healthText.text = "0";
        diedText.text = "You've died.";
    }

    public void AttackFromSpirit()
    {
        if (health > 0)
        {
            Debug.Log("Attack from Yakshi Invoked.");
            health -= 1;
            if (health == 0) diedText.text = "You've died.";
            healthText.text = health.ToString();
        }
    }

    public void SmellFromDeadBody()
    {
        if (health > 0)
        {
            Debug.Log("Attack from Yakshi Invoked.");
            health -= 0.5f;
            if (health == 0) diedText.text = "You've died.";
            healthText.text = health.ToString();
        }
    }

    public void AttackFromBat()
    {
        if (health > 0)
        {
            Debug.Log("Attack from Bat Invoked.");
            health -= 0.5f;
            if (health == 0) diedText.text = "You've died.";
            healthText.text = health.ToString();
        }
    }

    public void CollectedLife()
    {
        if (health < 5)
        {
            Debug.Log("Life collected and added.");
            health += 1f;
            healthText.text = health.ToString();
        }
    }

    public void CollectedBattery()
    {
        if (charge < 60 && charge != 0)
        {
            Debug.Log("Battery collected and added.");
            charge += 10;
            chargeText.text = charge.ToString() + "s";
        }
    }

    private IEnumerator UseCharge()
    {
        while (charge != 0)
        {
            charge -= 1;
            chargeText.text = charge.ToString() + "s";
            yield return new WaitForSeconds(1);
        }
        chargeText.text = "-";
        //TurnOffLight();
    }

    public void CollectedKey()
    {
        keys += 1;
        keysText.text = keys.ToString();
    }

    public void ReduceStamina()
    {
        if (stamina > 0)
        {
            stamina += 1;
            staminaText.text = stamina.ToString();
        }
    }

    public int getStamina()
    {
        return stamina;
    }

    private IEnumerator BoostStamina()
    {
        while (true)
        {
            if (stamina < 5)
            {
                stamina += 1;
                staminaText.text += stamina.ToString();
            }
            yield return new WaitForSeconds(3);
        }
    }

}
