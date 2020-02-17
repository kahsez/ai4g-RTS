using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFormable
{
    /// <summary>
    /// Gets the formation.
    /// </summary>
    /// <value>
    /// The formation.
    /// </value>
    FormationManager Formation { get; }

    /// <summary>
    /// Joins the formation.
    /// </summary>
    /// <param name="formation">The formation.</param>
    void JoinFormation(FormationManager formation);

    /// <summary>
    /// Abandons the formation.
    /// </summary>
    void AbandonFormation();


}