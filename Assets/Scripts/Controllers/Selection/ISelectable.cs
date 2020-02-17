using UnityEngine;

public interface ISelectable
{
    /// <summary>
    /// Gets or sets a value indicating whether this instance is selected.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is selected; otherwise, <c>false</c>.
    /// </value>
    bool IsSelected { get; set; }

    /// <summary>
    /// Gets the position.
    /// </summary>
    /// <value>
    /// The position.
    /// </value>
    Vector3 Position { get; }
}
