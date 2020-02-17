using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FormationPattern
{
    ///////////////////////////////////////////////////
    //////////////////// ATTRIBUTES ///////////////////
    ///////////////////////////////////////////////////
    
    /// <summary>
    /// Holds the number of slots currently in the pattern. This is updated
    /// in the GetDriftOffset method. It may be a fixed value.
    /// </summary>
    protected int _numberOfSlots;


    ///////////////////////////////////////////////////
    ///////////////////// METHODS /////////////////////
    ///////////////////////////////////////////////////

    /// <summary>
    /// Calculates the drift offset when characters are in given set of slots
    /// </summary>
    /// <param name="slotAssignments"></param>
    /// <returns></returns>
    public abstract Static GetDriftOffset(List<SlotAssignment> slotAssignments);

    /// <summary>
    /// Gets the location of the given slot index
    /// </summary>
    /// <param name="slotNumber"></param>
    /// <returns></returns>
    public abstract Static GetSlotLocation(int slotNumber);

    /// <summary>
    /// Returns true if the pattern can support the given number of slots
    /// </summary>
    /// <param name="slotCount"></param>
    /// <returns></returns>
    public abstract bool SupportSlots(int slotCount);

}
