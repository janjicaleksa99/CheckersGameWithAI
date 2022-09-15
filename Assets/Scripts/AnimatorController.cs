using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    public static RuntimeAnimatorController currentAnimatiorController;

    public static RuntimeAnimatorController UpperLeftMove1 = 
        Resources.Load<RuntimeAnimatorController>("Animations/UpperLeftMove1") as RuntimeAnimatorController;
    public static RuntimeAnimatorController UpperLeftMove2 =
        Resources.Load<RuntimeAnimatorController>("Animations/UpperLeftMove2") as RuntimeAnimatorController;
    public static RuntimeAnimatorController UpperRightMove1 =
        Resources.Load<RuntimeAnimatorController>("Animations/UpperRightMove1") as RuntimeAnimatorController;
    public static RuntimeAnimatorController UpperRightMove2 =
        Resources.Load<RuntimeAnimatorController>("Animations/UpperRightMove2") as RuntimeAnimatorController;
    public static RuntimeAnimatorController BottomLeftMove1 =
        Resources.Load<RuntimeAnimatorController>("Animations/BottomLeftMove1") as RuntimeAnimatorController;
    public static RuntimeAnimatorController BottomLeftMove2 =
        Resources.Load<RuntimeAnimatorController>("Animations/BottomLeftMove2") as RuntimeAnimatorController;
    public static RuntimeAnimatorController BottomRightMove1 =
        Resources.Load<RuntimeAnimatorController>("Animations/BottomRightMove1") as RuntimeAnimatorController;
    public static RuntimeAnimatorController BottomRightMove2 =
        Resources.Load<RuntimeAnimatorController>("Animations/BottomRightMove2") as RuntimeAnimatorController;
}
