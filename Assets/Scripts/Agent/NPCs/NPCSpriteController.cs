using System;
using UnityEngine;

[RequireComponent(typeof(AgentNpc))]
public class NPCSpriteController : MonoBehaviour
{
    ///////////////////////////////////////////////////
    //////////////////// ATTRIBUTES ///////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// The base sprite
    /// </summary>
    [SerializeField] private GameObject _baseSprite;
    
    /// <summary>
    /// The faction sprite
    /// </summary>
    [SerializeField] private GameObject _factionSprite;

    private Quaternion _rotation;

    ///////////////////////////////////////////////////
    ///////////////////// METHODS /////////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// Use this for initialization.
    /// </summary>
    /// <exception cref="Exception">NPCSprite requires a base sprite and a faction sprite reference</exception>
    private void OnEnable()
    {
        if ((_baseSprite == null) || (_factionSprite == null))
        {
            throw new Exception("NPCSprite requires a base sprite and a faction sprite reference");
        }

        AgentNpc aNpc = gameObject.GetComponent<AgentNpc>();
        NPCProperties npcProperties = aNpc.NpcProperties;
        _baseSprite.GetComponent<SpriteRenderer>().sprite = npcProperties.NpcSprite;
        _factionSprite.GetComponent<SpriteRenderer>().color = (aNpc.Faction == NPCProperties.Faction.ALLY) ? Color.cyan : Color.red;
    }

    private void Awake() {
        // Check this and LateUpdate()
        // This is needed so the children base sprite won't rotate along the parent
        _rotation = _baseSprite.transform.rotation;
    }

    private void LateUpdate() {
        _baseSprite.transform.rotation = _rotation;
    }
}
