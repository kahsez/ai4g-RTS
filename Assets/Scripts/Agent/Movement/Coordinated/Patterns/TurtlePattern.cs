using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurtlePattern : FormationPattern
{
    ///////////////////////////////////////////////////
    //////////////////// ATTRIBUTES ///////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// The radius of one character, this is needed to determine how close
    /// we can pack a given number of characters around a circle
    /// </summary>
    private float _characterRadius;

    /// <summary>
    /// Maximum number of slots in the pattern
    /// </summary>
    private int _maxSlotNumber;

    /// <summary>
    /// Automatically calculated when adding or removing slots
    /// </summary>
    private int _squareSize;


    ///////////////////////////////////////////////////
    ///////////////////// METHODS /////////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="characterRadius"></param>
    public TurtlePattern(float characterRadius)
    {
        _characterRadius = characterRadius;
    }

    /// <summary>
    /// Calculates the number of slots in the pattern from the assignment data.
    /// </summary>
    /// <param name="slotAssignments"></param>
    /// <returns>Number of slots</returns>
    private int CalculateNumberOfSlots(List<SlotAssignment> slotAssignments)
    {
        // General case
        /*
        // Find the number of filled slots: it will be the highest slot
        // number in the slotAssignments
        int filledSlots = 0;
        foreach (SlotAssignment assignment in slotAssignments)
        {
            if (assignment.SlotNumber >= _maxSlotNumber)
            {
                filledSlots = assignment.SlotNumber;
            }
        }

        return _numberOfSlots;
        */

        // Only works with the very simple assignment algorithm

        return slotAssignments.Count;
    }

    /// <summary>
    /// Calculates the drift offset of the pattern
    /// </summary>
    /// <param name="slotAssignments"></param>
    /// <returns></returns>
    public override Static GetDriftOffset(List<SlotAssignment> slotAssignments)
    {
        _numberOfSlots = CalculateNumberOfSlots(slotAssignments);
        _squareSize = Mathf.CeilToInt(Mathf.Sqrt(_numberOfSlots));

        // Store the center of mass
        Static center = new Static();

        // Now go through each assignment and add its contribution to the center
        foreach (SlotAssignment assignment in slotAssignments)
        {
            Static location = GetSlotLocation(assignment.SlotNumber);
            center.Position += location.Position;
            // center.Orientation += location.Orientation;
        }

        // Divide through to get the drift offset
        int numberOfAssignments = slotAssignments.Count;
        center.Position /= numberOfAssignments;
        // center.Orientation /= numberOfAssignments;

        return center;
    }

    /// <summary>
    /// Calculates the position of a slot
    /// </summary>
    /// <param name="slotNumber"></param>
    /// <returns></returns>
    public override Static GetSlotLocation(int slotNumber)
    {
        Static location = new Static();
        if (_squareSize < 2)
        {
            return location;
        }

        float subdiv = 2f / (_squareSize - 1);
        float sqLength = _characterRadius * _squareSize;

        int xPos = slotNumber % _squareSize;
        int yPos = (int) (slotNumber / _squareSize);

        float slotX = xPos * subdiv - 1;
        float slotY = yPos * subdiv - 1;

        location.Position = new Vector3(slotX, slotY) * sqLength;

        // Return the slot location
        return location;
    }

    /// <summary>
    /// We support any number of slots.
    /// </summary>
    /// <param name="slotCount"></param>
    /// <returns></returns>
    public override bool SupportSlots(int slotCount)
    {
        return true;
    }
}
