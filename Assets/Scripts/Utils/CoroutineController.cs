using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to start coroutines from non mobobehaviour classes
/// </summary>
public class CoroutineController : MonoBehaviour
{
    ///////////////////////////////////////////////////
    ///////////////////// METHODS /////////////////////
    ///////////////////////////////////////////////////
    
    /// <summary>
    /// Starts the child coroutine.
    /// </summary>
    /// <param name="coroutine">The coroutine.</param>
    public void StartChildCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }
	
}
