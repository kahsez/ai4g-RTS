using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(LineRenderer))]
public class FixedPath : Path
{
    ///////////////////////////////////////////////////
    //////////////////// ATTRIBUTES ///////////////////
    ///////////////////////////////////////////////////

    [SerializeField] private GameObject[] _nodesArray;

    private LineRenderer _lineRenderer;

    protected DebugController _debugController;

    ///////////////////////////////////////////////////
    ///////////////////// METHODS /////////////////////
    ///////////////////////////////////////////////////

    void Start()
    {
        _debugController = GameObject.FindGameObjectWithTag("DebugController").GetComponent<DebugController>();

        foreach (GameObject node in _nodesArray)
        {
            AddNode(node);
        }

        InitLineRenderer();
    }

    void Update()
    {
        _lineRenderer.enabled = _debugController.ShowDebug;
    }

    private void InitLineRenderer()
    {
        _lineRenderer = gameObject.GetComponent<LineRenderer>();
        _lineRenderer.useWorldSpace = true;
        _lineRenderer.shadowCastingMode = ShadowCastingMode.Off;

        Color color = Color.yellow;
        color.a = 0.5f;
        _lineRenderer.startColor = _lineRenderer.endColor = color;

        _lineRenderer.startWidth = _lineRenderer.endWidth = 0.33f;

        _lineRenderer.positionCount = _nodesArray.Length + 1;
        for (int i = 0; i < _lineRenderer.positionCount - 1; i++)
        {
            _lineRenderer.SetPosition(i, _nodesArray[i].transform.position);
        }

        _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, _nodesArray[0].transform.position);

        _lineRenderer.enabled = false;
    }

    public override void Destroy()
    {
        // Empty on purpose
    }
}
