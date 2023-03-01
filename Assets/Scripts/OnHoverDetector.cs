using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnHoverDetector : MonoBehaviour
{
    private Planet m_Planet;

    // Awake is called when this object is instantiated
    private void Awake ()
    {
        m_Planet = GetComponentInParent<Planet>();

        if (!m_Planet)
            Debug.LogWarning("Could not find Planet component on parent(s)");
    }

    // Mouse events
    private void OnMouseEnter ()
    {
        if (m_Planet)
            m_Planet.OnHoverEnter();
    }

    private void OnMouseExit ()
    {
        if (m_Planet)
            m_Planet.OnHoverExit();
    }

    private void OnMouseDown ()
    {
        if (m_Planet)
            m_Planet.OnClickDown();
    }

    private void OnMouseUp ()
    {
        if (m_Planet)
            m_Planet.OnClickUp();
    }
}
