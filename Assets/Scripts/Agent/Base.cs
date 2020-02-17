using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : Agent
{
    private static float BaseHealth = 100f;

    protected override void OnEnable()
    {
        base.OnEnable();

        _maxHealth = BaseHealth;
        _health = _maxHealth;
    }

    protected override void DeathAction()
    {      
        GameController controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        controller.LoseGame(this.gameObject.tag);
        gameObject.SetActive(false);
    }
}
