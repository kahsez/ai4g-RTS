using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private bool _totalWarMode;

    [SerializeField] private StrategicDecisionSystem _allyDecisionSystem;
    [SerializeField] private StrategicDecisionSystem _enemyDecisionSystem;
    [SerializeField] private SelectionController _selectionController;

    [SerializeField] private GameObject _winMessagePanel;

    void OnEnable()
    {
        //Random.InitState(12345);
        DeactivateTotalWarMode();
        _winMessagePanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (!_totalWarMode)
            {
                ActivateTotalWarMode();
            }
        }
    }

    private void ActivateTotalWarMode()
    {
        _totalWarMode = true;

        if (_allyDecisionSystem != null)
            _allyDecisionSystem.gameObject.SetActive(true);

        _selectionController.gameObject.SetActive(false);
        /*
        GameObject[] agents = GameObject.FindGameObjectsWithTag("AllyAgent");
        
        foreach (GameObject agent in agents)
        {
            agent.GetComponent<TacticDecisionSystem>().enabled = true;
        }      
        */
    }

    private void DeactivateTotalWarMode()
    {
        _totalWarMode = false;

        if (_allyDecisionSystem != null)
            _allyDecisionSystem.gameObject.SetActive(false);

        _selectionController.gameObject.SetActive(true);
        /*
        GameObject[] agents = GameObject.FindGameObjectsWithTag("AllyAgent");
        
        foreach (GameObject agent in agents)
        {
            agent.GetComponent<TacticDecisionSystem>().enabled = false;
        }
        */
    }

    public void Respawn(AgentNPC agent, float delay)
    {
        StartCoroutine(RespawnAgent(agent, delay));
    }

    private IEnumerator RespawnAgent(AgentNPC agent, float delay)
    {
        yield return new WaitForSeconds(delay);
        agent.gameObject.SetActive(true);
        agent.Respawn();
        if (_totalWarMode)
        {
            TacticDecisionSystem tactic = agent.GetComponent<TacticDecisionSystem>();
            if (!tactic.enabled)
                tactic.enabled = true;
        }
    }

    public void LoseGame(string tag)
    {
        _winMessagePanel.SetActive(true);
        TextMeshProUGUI text = _winMessagePanel.GetComponentInChildren<TextMeshProUGUI>();
        if (tag == "AllyBase")
        {
            text.color = Color.red;
            text.text = "Enemy Wins";
        }
        else
        {           
            text.color = Color.cyan;
            text.text = "Ally Wins";
        }

        if (_allyDecisionSystem != null)
            _allyDecisionSystem.gameObject.SetActive(false);

        if (_allyDecisionSystem != null)
            _enemyDecisionSystem.gameObject.SetActive(false);

        
        _selectionController.gameObject.SetActive(false);
        
        GameObject[] agents = GameObject.FindGameObjectsWithTag("AllyAgent");
        
        foreach (GameObject agent in agents)
        {
            agent.GetComponent<TacticDecisionSystem>().enabled = false;
        }

        agents = GameObject.FindGameObjectsWithTag("EnemyAgent");

        foreach (GameObject agent in agents)
        {
            agent.GetComponent<TacticDecisionSystem>().enabled = false;
        }
    }
}
