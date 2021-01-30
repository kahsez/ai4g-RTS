using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class SelectionController : MonoBehaviour
{
    ///////////////////////////////////////////////////
    //////////////////// ATTRIBUTES ///////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// The camera
    /// </summary>
    [SerializeField] private Camera _cam;

    /// <summary>
    /// The map
    /// </summary>
    [SerializeField] private Map _map;

    /// <summary>
    /// The threshold distance
    /// </summary>
    private const float ThresholdDistance = 0.2f;

    /// <summary>
    /// The double click distance
    /// </summary>
    private const float DoubleClickDistance = 0.1f;

    /// <summary>
    /// The character radius
    /// </summary>
    private const float CharacterRadius = 2f;

    /// <summary>
    /// The agent layer
    /// </summary>
    [SerializeField] private LayerMask _agentLayer;

    /// <summary>
    /// The base manager
    /// </summary>
    [SerializeField] private FormationManager _baseManager;

    /// <summary>
    /// The selection
    /// </summary>
    private List<ISelectable> _selection;

    /// <summary>
    /// The line render
    /// </summary>
    private LineRenderer _lineRender;

    /// <summary>
    /// The is dragging
    /// </summary>
    private bool _isDragging = false;

    /// <summary>
    /// The point1
    /// </summary>
    private Vector2 _point1, _point2;

    /// <summary>
    /// The double click timestamp
    /// </summary>
    private float _doubleClickTimestamp;

    private NpcMode _mode;

    [SerializeField] private TextMeshProUGUI _modeText;


    ///////////////////////////////////////////////////
    ///////////////////// ACCESS //////////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// Gets the current map.
    /// </summary>
    /// <value>
    /// The current map.
    /// </value>
    public Map CurrentMap 
    {
        get { return _map; }
    }


    ///////////////////////////////////////////////////
    ///////////////////// METHODS /////////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// Use this for initialization.
    /// </summary>
    void Start()
    {
        _selection = new List<ISelectable>();
        _lineRender = gameObject.GetComponent<LineRenderer>();
        _lineRender.positionCount = 4;

        _mode = NpcMode.OFFENSE;
        _modeText.text = "ATK";
        _modeText.color = Color.red;


        HideSelection();
    }

    void OnDisable()
    {
        HideSelection();
        ClearSelection();
        _modeText.gameObject.SetActive(false);
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        SelectionInput();
        FormationInput();
        ActionInput();
    }

    /// <summary>
    /// Manage the selection input.
    /// </summary>
    private void SelectionInput()
    {
        Vector2 currentMousePos = _cam.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            if (!_isDragging)
            {
                _isDragging = true;
                _point1 = currentMousePos;
                _point2 = currentMousePos;
                ShowSelection();
            }
        }

        if (Input.GetMouseButton(0) && _isDragging)
        {
            _point2 = currentMousePos;
            DrawSelection(_point1, _point2);
        }

        if (Input.GetMouseButtonUp(0) && _isDragging)
        {
            _isDragging = false;
            // Determine whether we are selecting a single agent or multiple agents
            // we assume that the player can move their cursor a little bit, so
            // we're being a lil flexible
            if (Vector3.Distance(_point1, _point2) <= ThresholdDistance)
            {
                // Simple selection
                ClearSelection();
                Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if ((hit.collider != null) && (hit.collider.CompareTag("AllyAgent")))
                    {
                        GameObject selectedObject = hit.collider.gameObject;
                        ISelectable selected = selectedObject.GetComponent<ISelectable>();
                        IFormable formableObject = selectedObject.GetComponent<IFormable>();
                        // Check if the single selected unit is in formation
                        if ((formableObject != null) && (formableObject.Formation != null))
                        {
                            SelectFormation(formableObject);
                        }
                        else
                        {
                            Select(selected);
                        }                          
                    }
                }
            }
            else
            {
                // Multiple selection
                SelectArea(_point1, _point2);
            }

            HideSelection();
        }
    }

    /// <summary>
    /// Manage the action input.
    /// </summary>
    private void ActionInput()
    {
        Vector3 mousePos = _cam.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetKeyDown(KeyCode.M))
        {
            if (_mode == NpcMode.DEFENSE)
            {
                _mode = NpcMode.OFFENSE;
                if (_modeText != null)
                {
                    _modeText.text = "ATK";
                    _modeText.color = Color.red;
                }                
            }
            else
            {
                _mode = NpcMode.DEFENSE;
                if (_modeText != null)
                {
                    _modeText.text = "DEF";
                    _modeText.color = Color.green;
                }                   
            }
        }

        if (Input.GetMouseButtonUp(1) && (_selection.Count > 0))
        {
            foreach (AgentNpc agentNpc in _selection)
            {
                AgentNpc agent = agentNpc;

                TacticDecisionSystem tds = agent.GetComponent<TacticDecisionSystem>();
                tds.ChangeMode(_mode, _map.WorldToGrid(mousePos));

                if (agentNpc.Formation != null)
                {
                    agent = agentNpc.Formation.InvisibleLeader;

                    Action action = (agent.PathfindingAlgorithm == PathfindingAlgorithm.A_STAR)
                        ? (PathfindingAction) new AStarAction(10f, 1, agent, _map.WorldToGrid(mousePos))
                        : (PathfindingAction) new LrtaStarAction(10f, 1, agent, _map.WorldToGrid(mousePos));
                    agent.ScheduleAction(action);
                }        
            }           
        }
    }

    /// <summary>
    /// Manage the formation input.
    /// </summary>
    private void FormationInput()
    {
        // We won't do formation stuff if there's no selectable units
        if (_selection.Count <= 0) return;

        FormationManager formation = null;
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            formation = CreateCircleFormation();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            formation = CreateTurtleFormation();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            formation = CreateArrowFormation();
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            foreach (AgentNpc agent in _selection)
            {
                Action action = new AbandonFormationAction(3000f, 0, agent);
                agent.ScheduleAction(action);
            }
        }

        // Check if we have a formation
        if (formation == null) return;

        // Calculate center of mass
        Vector3 formationCenter = Vector3.zero;
        foreach (AgentNpc agent in _selection)
        {
            formationCenter += agent.Position;
        }

        formationCenter /= _selection.Count;
        formation.transform.position = formationCenter;

        formation.gameObject.SetActive(true);

        foreach (AgentNpc agent in _selection)
        {
            Action action = new JoinFormationAction(3000f, 0, agent, formation);
            agent.ScheduleAction(action);
        }
    }

    /// <summary>
    /// Shows the selection.
    /// </summary>
    private void ShowSelection()
    {
        _lineRender.enabled = true;
    }

    /// <summary>
    /// Hides the selection.
    /// </summary>
    private void HideSelection()
    {
        _lineRender.enabled = false;
    }

    /// <summary>
    /// Draws the selection.
    /// </summary>
    /// <param name="pos1">The position 1.</param>
    /// <param name="pos2">The position 2.</param>
    private void DrawSelection(Vector2 pos1, Vector2 pos2)
    {
        float z = gameObject.transform.position.z;
        Vector3[] positions = new Vector3[4]
        {
            new Vector3(Mathf.Max(pos1.x, pos2.x), Mathf.Max(pos1.y, pos2.y), z),
            new Vector3(Mathf.Max(pos1.x, pos2.x), Mathf.Min(pos1.y, pos2.y), z),
            new Vector3(Mathf.Min(pos1.x, pos2.x), Mathf.Min(pos1.y, pos2.y), z),
            new Vector3(Mathf.Min(pos1.x, pos2.x), Mathf.Max(pos1.y, pos2.y), z)
        };
        
        _lineRender.SetPositions(positions);
    }


    /// <summary>
    /// Determines whether the specified selectable is inside selection.
    /// </summary>
    /// <param name="selectable">The selectable.</param>
    /// <param name="pos1">The pos1.</param>
    /// <param name="pos2">The pos2.</param>
    /// <returns>
    ///   <c>true</c> if the specified selectable is inside selection; otherwise, <c>false</c>.
    /// </returns>
    private bool IsInsideSelection(ISelectable selectable, Vector2 pos1, Vector2 pos2)
    {
        float minX = Mathf.Min(pos1.x, pos2.x);
        float maxX = Mathf.Max(pos1.x, pos2.x);
        float minY = Mathf.Min(pos1.y, pos2.y);
        float maxY = Mathf.Max(pos1.y, pos2.y);

        Vector3 pos = selectable.Position;
        return (pos.x >= minX) && (pos.x <= maxX) && (pos.y >= minY) && (pos.y <= maxY);
    }

    /// <summary>
    /// Selects the area.
    /// </summary>
    /// <param name="pos1">The pos1.</param>
    /// <param name="pos2">The pos2.</param>
    private void SelectArea(Vector3 pos1, Vector3 pos2)
    {
        ClearSelection();
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("AllyAgent"))
        {
            ISelectable a = go.GetComponent<ISelectable>();
            if ((a != null) && IsInsideSelection(a, pos1, pos2))
            {
                Select(a);
            }
        }
    }

    /// <summary>
    /// Selects the specified selectable.
    /// </summary>
    /// <param name="selectable">The selectable.</param>
    private void Select(ISelectable selectable)
    {
        // Deselect if we're pressing shift and the unit it's already selected
        if (selectable.IsSelected && Input.GetKey(KeyCode.LeftShift))
        {
            Deselect(selectable);
            return;
        }

        _selection.Add(selectable);
        selectable.IsSelected = true;
    }

    /// <summary>
    /// Selects the formation.
    /// </summary>
    /// <param name="formationTarget">The formation target.</param>
    private void SelectFormation(IFormable formationTarget)
    {
        // Select all agents in this formable agent formation
        foreach (AgentNpc agent in formationTarget.Formation.AgentsInFormation)
        {
            Select(agent);
        }
    }

    /// <summary>
    /// Deselects the specified selected.
    /// </summary>
    /// <param name="selected">The selected.</param>
    private void Deselect(ISelectable selected)
    {
        _selection.Remove(selected);
        selected.IsSelected = false;
    }

    /// <summary>
    /// Clears the selection.
    /// </summary>
    private void ClearSelection()
    {
        // Clear the selection UNLESS we are pressing SHIFT
        if (Input.GetKey(KeyCode.LeftShift)) return;

        foreach (ISelectable s in _selection)
        {
            s.IsSelected = false;
        }

        _selection.Clear();
    }

    /// <summary>
    /// Creates the circle formation.
    /// </summary>
    /// <returns></returns>
    private FormationManager CreateCircleFormation()
    {
        FormationManager manager = Instantiate(_baseManager);
        manager.Pattern = new DefensiveCirclePattern(CharacterRadius);
        manager.name = "Circle Formation";       
        return manager;
    }

    /// <summary>
    /// Creates the turtle formation.
    /// </summary>
    /// <returns></returns>
    private FormationManager CreateTurtleFormation()
    {
        FormationManager manager = Instantiate(_baseManager);
        manager.Pattern = new TurtlePattern(CharacterRadius);
        manager.name = "Turtle Formation";
        return manager;
    }
    
    /// <summary>
    /// Creates the turtle formation.
    /// </summary>
    /// <returns></returns>
    private FormationManager CreateArrowFormation()
    {
        FormationManager manager = Instantiate(_baseManager);
        manager.Pattern = new ArrowPattern(CharacterRadius);
        manager.name = "Arrow Formation";
        return manager;
    }
}
