using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugController : MonoBehaviour
{

    private bool _showDebug;
    private bool _showInfluenceMap;

    [SerializeField] private InfluenceMap _influenceMap;

    public bool ShowDebug
    {
        get { return _showDebug; }
    }

    // Use this for initialization
    void Start ()
    {
        _showDebug = false;
        _showInfluenceMap = false;
        _influenceMap.SetAlpha(0f);
    }
	
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            _showDebug = !_showDebug;
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            if (_showInfluenceMap)
            {
                _showInfluenceMap = false;
                _influenceMap.SetAlpha(0f);
            }
            else
            {
                _showInfluenceMap = true;
                _influenceMap.SetAlpha(0.5f);
            }
        }
    }
}
