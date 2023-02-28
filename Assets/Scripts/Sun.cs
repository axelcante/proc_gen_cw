using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour
{
    [SerializeField] private Transform m_Lava0;
    [SerializeField] private Transform m_Lava1;
    [SerializeField] private Transform m_Lava2;
    [SerializeField] private float m_rotSpeed;

    // Update is called once per frame
    void Update()
    {
        m_Lava0.Rotate(0f, 0f, m_rotSpeed * Time.deltaTime);
        m_Lava1.Rotate(0f, 0f, m_rotSpeed * -Time.deltaTime);
        m_Lava2.Rotate(0f, 0f, m_rotSpeed/2 * Time.deltaTime);
    }
}
