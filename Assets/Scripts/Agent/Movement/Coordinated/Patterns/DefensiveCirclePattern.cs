using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefensiveCirclePattern : FormationPattern
{
    ///////////////////////////////////////////////////
    //////////////////// ATTRIBUTES ///////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// The radius of ine character, this is needed to determine how close
    /// we can pack a given number of characters around a circle
    /// </summary>
    private float _characterRadius;

    /// <summary>
    /// Maximum number of slots in the pattern
    /// </summary>
    private int _maxSlotNumber;


    ///////////////////////////////////////////////////
    ///////////////////// METHODS /////////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="characterRadius"></param>
    public DefensiveCirclePattern(float characterRadius)
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
            //center.Orientation += location.Orientation;
        }

        // Divide through to get the drift offset
        int numberOfAssignments = slotAssignments.Count;
        center.Position /= numberOfAssignments;
        //center.Orientation /= numberOfAssignments;

        return center;
    }

    /// <summary>
    /// Calculates the position of a slot
    /// </summary>
    /// <param name="slotNumber"></param>
    /// <returns></returns>
    public override Static GetSlotLocation(int slotNumber)
    {
        // We place the slots around a circle based on their slot number
        float angleAroundCircle = (float) slotNumber / (float) _numberOfSlots * Mathf.PI * 2;

        // The radius depends on the radius of the character and the number
        // of characters in the circle: we want there to be no gap between
        // character's shoulders
        float radius = _characterRadius / Mathf.Sin(Mathf.PI / (float) _numberOfSlots);

        // Create a location and fill its components based
        // on the angle around circle
        Static location = new Static();
        float x = radius * Mathf.Cos(angleAroundCircle);
        float y = radius * Mathf.Sin(angleAroundCircle);
        location.Position = new Vector3(x, y, 0f);

        // The characters should be facing out
        location.Orientation = angleAroundCircle * Mathf.Rad2Deg;

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
