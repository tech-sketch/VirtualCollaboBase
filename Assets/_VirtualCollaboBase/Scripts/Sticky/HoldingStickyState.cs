using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class HoldingStickyState 
{
    public static ReactiveProperty<bool> isHolding = new ReactiveProperty<bool>(false);
    public static IReadOnlyReactiveProperty<bool> OnChangeHoldingStickyState  { get { return　isHolding; } }
    public static GameObject targetSticky = null;
}
