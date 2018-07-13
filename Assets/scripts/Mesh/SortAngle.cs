using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortAngle : MonoBehaviour {

    public int Index;

    public float Angle;

    public int CompareTo(SortAngle item)

    {

        return item.Angle.CompareTo(Angle);

    }


}
