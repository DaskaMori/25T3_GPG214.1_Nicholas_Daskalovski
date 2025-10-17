using System;
using UnityEngine;

public class BinTrigger : MonoBehaviour
{
    public BoxType acceptsType = BoxType.Red;
    public int correctCount = 0;
    public int incorrectCount = 0;

    private void OnTriggerEnter(Collider other)
    {
        BoxData data = other.GetComponent<BoxData>();
        if (data == null)
        {
            return;
        }

        if (data.boxType == acceptsType)
        {
            correctCount++;
            Destroy(other.gameObject);
        }
        else
        {
            incorrectCount++;
            Destroy(other.gameObject);
        }
    }
}
