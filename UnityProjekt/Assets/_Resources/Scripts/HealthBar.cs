﻿using UnityEngine;
using System.Collections;

[System.Serializable]
public class HealthBar : MonoBehaviour 
{

    public Transform healthBar;
    
    [SerializeField]
    private float health = 0, maxHealth = 1;
    [SerializeField]
    private float currentHealth = 0;

    public float change = 5.0f;

    private bool updatebar = true;

    public void Reset()
    {
        currentHealth = 0;
        maxHealth = 1;
        health = 0;
        UpdateBar(true);
    }

    public void UpdateBar(float phealth, float pmaxHealth, bool instant = false)
    {
        maxHealth = pmaxHealth;
        health = Mathf.Clamp(phealth, 0, maxHealth);
        
        if(instant)
            currentHealth = health;

        updatebar = (maxHealth > 0);
    }

    void Update()
    {
        if (updatebar)
        {
            UpdateBar();
        }
    }

    public void UpdateBar(bool instant = false)
    {
        if (!instant)
        {
            currentHealth = Mathf.Lerp(currentHealth, health, Time.deltaTime * change);
        }
        else
        {
            currentHealth = health;
        }
        healthBar.localScale = new Vector3(currentHealth / maxHealth, healthBar.localScale.y, healthBar.localScale.z);
    }
}
