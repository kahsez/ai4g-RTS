using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AgentNPC))]
public class FormationManager : MonoBehaviour
{
    ///////////////////////////////////////////////////
    //////////////////// ATTRIBUTES ///////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// Holds a list of slots assignments
    /// </summary>
    private List<SlotAssignment> _slotAssignments = new List<SlotAssignment>();


    /// <summary>
    /// List to keep track of all the agents in the formation.
    /// </summary>
    private List<AgentNPC> _agentsInSlots = new List<AgentNPC>();

    /// <summary>
    /// Holds a Static structure representing the drift offset for
    /// the currently filled slots
    /// </summary>
    private Static _driftOffset;

    /// <summary>
    /// Holds the formation pattern
    /// </summary>
    private FormationPattern _pattern;

    /// <summary>
    /// Invisible leader that acts as anchor point
    /// </summary>
    private AgentNPC _invisibleLeader;


    ///////////////////////////////////////////////////
    ///////////////////// ACCESS //////////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// Gets the invisible leader.
    /// </summary>
    /// <value>
    /// The invisible leader.
    /// </value>
    public AgentNPC InvisibleLeader 
    {
        get { return _invisibleLeader; }
    }

    /// <summary>
    /// Gets the agents in formation.
    /// </summary>
    /// <value>
    /// The agents in formation.
    /// </value>
    public List<AgentNPC> AgentsInFormation 
    {
        get { return _agentsInSlots; }
    }

    /// <summary>
    /// Gets or sets the pattern.
    /// </summary>
    /// <value>
    /// The pattern.
    /// </value>
    public FormationPattern Pattern 
    {
        get { return _pattern; }
        set { _pattern = value; }
    }

    ///////////////////////////////////////////////////
    ///////////////////// METHODS /////////////////////
    ///////////////////////////////////////////////////

    private void OnEnable()
    {
        _invisibleLeader = this.gameObject.GetComponent<AgentNPC>();
    }

    private void Update()
    {
        if ((_pattern != null) && _slotAssignments.Count > 0)
        {
            UpdateSlots();
        }
    }


    /// <summary>
    /// Updates the assignment of characters to slots
    /// </summary>
    private void UpdateSlotAssignments()
    {
        // A very simple assignment algorithm: we simply go through
        // each assignment in the list and assign sequential slot numbers
        for (int i = 0; i < _slotAssignments.Count; i++)
        {
            _slotAssignments[i].SlotNumber = i;
        }

        // Update the drift offset
        _driftOffset = _pattern.GetDriftOffset(_slotAssignments);
    }

    /// <summary>
    /// Add a new character to the firs available slot.
    /// </summary>
    /// <param name="agent">Character to be added</param>
    /// <returns>Returns false if no more slots are available</returns>
    public bool AddAgent(AgentNPC agent)
    {
        // Find out how many slots we have occupied
        int occupiedSlots = _slotAssignments.Count;

        // Check if the pattern supports more slots
        if (_pattern.SupportSlots(occupiedSlots + 1))
        {
            // Add a new slot assignment
            SlotAssignment slotAssignment = new SlotAssignment(agent);
            _slotAssignments.Add(slotAssignment);

            // Update the slot assignments and return success
            UpdateSlotAssignments();
            _agentsInSlots.Add(agent);
            return true;
        }

        // Otherwise we've failed to add the character
        return false;
    }

    /// <summary>
    /// Removes a character from its slot
    /// </summary>
    /// <param name="agent"></param>
    public void RemoveAgent(AgentNPC agent)
    {
        // Find the character's slot
        int slot = _slotAssignments.Find(
                assignment => assignment.Agent.Equals(agent))
            .SlotNumber;
        // Remove the slot

        _slotAssignments[slot].Destroy();
        _slotAssignments.RemoveAt(slot);
        _agentsInSlots.Remove(agent);

        // Update the assignments
        UpdateSlotAssignments();

        // Automatically remove this formation when there's one to no agents in it
        if(_slotAssignments.Count <= 1)
        {
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// Write new slot locations to each character
    /// </summary>
    private void UpdateSlots()
    {
        // Find the anchor point
        Static anchor = GetAnchorPoint();

        // Get the orientation of the anchor point as a matrix
        Vector3 orientation = Vector3.zero;
        orientation.z = anchor.Orientation;
        Quaternion orientationMatrix = Quaternion.Euler(orientation);

        // Go through each character in turn
        for (int i = 0; i < _slotAssignments.Count; i++)
        {
            // Ask for the location of the slot relative to the anchor point.
            Static relativeLoc = _pattern.GetSlotLocation(_slotAssignments[i].SlotNumber);
            // And add the drift component
            relativeLoc.Position -= _driftOffset.Position;
            relativeLoc.Orientation -= _driftOffset.Orientation;

            // Transform it by the anchor point's position and orientation
            Agent location = _slotAssignments[i].Target;
            location.Position = anchor.Position + orientationMatrix * relativeLoc.Position;
            

            location.Orientation = anchor.Orientation + relativeLoc.Orientation;

            // And add the drift component. This is from the book and it's wrong, apparently
            //location.Position = location.Position + _driftOffset.Position;
            //location.Orientation = location.Orientation + _driftOffset.Orientation;

            //Debug.Log(_driftOffset.Position);

            // Write the static to the character
            _slotAssignments[i].Agent.SetTarget<Arrive>(location);
            _slotAssignments[i].Agent.SetTarget<Align>(location);
        }
    }

    private void OnDestroy()
    {
        Destroy(_invisibleLeader.gameObject);
    }

    /// <summary>
    /// Calculates an anchor point using the position and velocity
    /// of the center of mass of the characters in the formation
    /// </summary>
    /// <returns>Anchor point</returns>
    private Static GetAnchorPoint()
    {
        // ONLY IF THE FORMATION DOESN'T HAVE A STEERED LEADER
        /*
        Vector3 centerOfMassPosition = Vector3.zero;
        Vector3 centerOfMassVelocity = Vector3.zero;
        float centerOfMassOrientation = 0f;

        foreach (SlotAssignment slotAssignment in _slotAssignments)
        {
            centerOfMassPosition += slotAssignment.Agent.Position;
            centerOfMassVelocity += slotAssignment.Agent.Velocity;
            centerOfMassOrientation += slotAssignment.Agent.Orientation;
        }

        int numberOfAssignments = _slotAssignments.Count;
        centerOfMassPosition /= numberOfAssignments;
        centerOfMassVelocity /= numberOfAssignments;
        centerOfMassOrientation /= numberOfAssignments;

        Static anchor = new Static();
        anchor.Position = centerOfMassPosition + centerOfMassVelocity;
        anchor.Orientation = centerOfMassOrientation;

        return anchor;
        */
        Static anchor = new Static();
        anchor.Position = _invisibleLeader.Position;
        anchor.Orientation = _invisibleLeader.Orientation;
        return anchor;
    }

}

/// <summary>
/// Holds the assignment of a single character to a slot
/// </summary>
[System.Serializable]
public class SlotAssignment
{
    ///////////////////////////////////////////////////
    //////////////////// ATTRIBUTES ///////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// The agent
    /// </summary>
    private AgentNPC _agent;

    /// <summary>
    /// The slot number
    /// </summary>
    private int _slotNumber;

    /// <summary>
    /// The target
    /// </summary>
    private Agent _target;

    /// <summary>
    /// Initializes a new instance of the <see cref="SlotAssignment"/> class.
    /// </summary>
    /// <param name="agent">The agent.</param>
    public SlotAssignment(AgentNPC agent)
    {
        _agent = agent;
        _target = new GameObject(agent.name + " TARGET").AddComponent<Agent>();
        _target.InteriorRadius = 0.25f;
        _target.ExteriorRadius = 0.75f;
        _target.InteriorAngle = 5f;
        _target.ExteriorAngle = 20f;
    }


    ///////////////////////////////////////////////////
    ///////////////////// ACCESS //////////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// Gets the agent.
    /// </summary>
    /// <value>
    /// The agent.
    /// </value>
    public AgentNPC Agent
    {
        get { return _agent; }
    }

    /// <summary>
    /// Gets or sets the slot number.
    /// </summary>
    /// <value>
    /// The slot number.
    /// </value>
    public int SlotNumber
    {
        get { return _slotNumber; }
        set { _slotNumber = value; }
    }

    /// <summary>
    /// Gets the target.
    /// </summary>
    /// <value>
    /// The target.
    /// </value>
    public Agent Target
    {
        get { return _target; }
    }

    ///////////////////////////////////////////////////
    ///////////////////// METHODS /////////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// Call it before removing the SlotAssignment from the list
    /// Destroy any gameobjects we created before 
    /// </summary>
    public void Destroy()
    {
        GameObject.Destroy(_target.gameObject);
    }
}
