using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("System&Prefabs")]
    public Transform m_SolarCenter;
    public GameObject m_PlanetTemplate;

    [Header("Planet Parts")]
    public List<Sprite> m_Spheres = new List<Sprite>();
    public List<Sprite> m_Lights = new List<Sprite>();
    public List<Sprite> m_Atmospheres = new List<Sprite>();
    public List<Sprite> m_Landmasses = new List<Sprite>();

    [Header("Planet Parameters")]
    public int m_nbOfPlanets;
    public float m_minDistance;
    public float m_maxDistance;
    public float m_minStep;
    public float m_maxSize;
    public float m_minSize;

    private int m_iterator = 0;

    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            GeneratePlanets();
        }
    }

    // Generate a random planet
    private void GeneratePlanets ()
    {
        // Generate a random radius (from the center, Sun) and angle
        float radius = Random.Range(m_minDistance, m_maxDistance);
        float angle = Random.Range(0f, Mathf.PI * 2f);

        // Generate the planet's visuals, via sprites
        Sprite body = m_Spheres[Random.Range(0, m_Spheres.Count)];
        Sprite atm = m_Atmospheres[Random.Range(0, m_Atmospheres.Count)];
        Sprite land = m_Landmasses[Random.Range(0, m_Landmasses.Count)];

        Planet p = Instantiate(m_PlanetTemplate, m_SolarCenter).GetComponent<Planet>();
        p.GeneratePlanet(radius, angle, body, land, atm);
    }
}
