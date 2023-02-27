using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    private float m_distanceFromSun;
    private int m_minOrbitPoints = 50;
    private LineRenderer m_OrbitRenderer;

    [Header("Planet Visuals")]
    [SerializeField] private GameObject m_CentreMass;
    [SerializeField] private float m_zOffset;   // Control which sprite renders on top
    [SerializeField] private SpriteRenderer m_Body;
    [SerializeField] private SpriteRenderer m_Landmass;
    [SerializeField] private SpriteRenderer m_Atmosphere;

    // Awake is called on this instance's loading
    private void Awake ()
    {
        m_OrbitRenderer = GetComponent<LineRenderer>();
    }

    // Generate a random planet and its elements
    public void GeneratePlanet (float radius, float angle, Sprite body, Sprite land, Sprite atm)
    {
        // Set position with radius and angle
        m_distanceFromSun = radius;
        float posX = radius * Mathf.Cos(angle);
        float posY = radius * Mathf.Sin(angle);
        m_CentreMass.transform.position = new Vector3(posX, posY, m_zOffset);

        // Set planet visuals
        m_Body.sprite = body;
        m_Atmosphere.sprite = atm;
        m_Landmass.sprite = land;

        DrawOrbit(radius);
    }

    // Draw an orbit around the gravitational point (the sun)
    public void DrawOrbit (float radius)
    {
        // Scale the amount of points in the orbit circle based on radius
        // This ensures larger orbits are still circular
        // I add +1 to close off the circle (for some reason the circles weren't closed otherwise)
        int points = Mathf.Max((int)radius * 2, m_minOrbitPoints);
        m_OrbitRenderer.positionCount = points + 1;

        for (int i = 0; i < points; i++) {
            float angle = i * Mathf.PI * 2f / points;
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            m_OrbitRenderer.SetPosition(i, new Vector3(x, y, 0f));
        }

        // Manually set the position of the final point to be the same as the first, in order to fully close the circle
        m_OrbitRenderer.SetPosition(points, m_OrbitRenderer.GetPosition(0));
    }
}
