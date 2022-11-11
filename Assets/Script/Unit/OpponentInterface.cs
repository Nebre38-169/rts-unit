using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <c>Interface Opponent Interface</c>
/// Used by opponent of a target, to unset the target when it desapear
/// Made by : Nebre 38-169
/// Last Update : 09/11/2022 by Nebre 38-169
/// </summary>
public interface OpponentInterface
{
    /// <summary>
    /// <c>Function unset Target</c>
    /// Warn the opponent that the target is not available anymore
    /// </summary>
    /// <param name="target"></param>
    public void unSetTarget(Target target);
}
