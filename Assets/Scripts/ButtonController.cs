using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    public Image m_Border;
    public Color m_NeutralColor;
    public Color m_HoveredColor;
    public Color m_PressedColor;

    public void OnHoverEnter ()
    {
        m_Border.color = m_HoveredColor;
    }

    public void OnHoverExit ()
    {
        m_Border.color = m_NeutralColor;
    }

    public void OnClickDown ()
    {
        m_Border.color = m_PressedColor;
    }

    public void OnClickUp ()
    {
        m_Border.color = m_HoveredColor;
    }
}
