using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnHoverDetector : MonoBehaviour
{
    private void OnMouseEnter ()
    {
        Debug.Log("entered");
    }

    private void OnMouseExit ()
    {
        Debug.Log("exit");
    }
}
