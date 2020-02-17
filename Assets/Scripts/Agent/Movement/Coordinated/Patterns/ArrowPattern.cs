using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowPattern : FormationPattern
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
    public ArrowPattern(float characterRadius)
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

        float x, y;

        switch (slotNumber)
        {
            case 0:
                x = _characterRadius;
                y = 0;
                break;
            case 1:
                x = 0;
                y = _characterRadius;
                break;
            case 2:
                x = 0;
                y = -_characterRadius;
                break;
            case 3:
                x = -_characterRadius;
                y = 2 * _characterRadius;
                break;
            case 4:
                x = -_characterRadius;
                y = -2 * _characterRadius;
                break;
            default:
                x = 0;
                y = 0;
                break;
        }

        location.Position = new Vector3(x, y, 0f);

        // Return the slot location
        return location;
    }

    /// <summary>
    /// We support 5 slots.
    /// </summary>
    /// <param name="slotCount"></param>
    /// <returns></returns>
    public override bool SupportSlots(int slotCount)
    {
        return slotCount <= 5;
    }
}
