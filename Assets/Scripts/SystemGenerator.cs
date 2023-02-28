using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlanetType
{
    Atmospheric,
    GasGiant,
    Barren,
    Moon
}

public struct Faction
{
    public string name;
    public Color color;

    public Faction (string n, Color c)
    {
        name = n;
        color = c;
    }
}

public class SystemGenerator : MonoBehaviour
{
    private static SystemGenerator m_instance;
    public static SystemGenerator GetInstance () { return m_instance; }

    [Header("System&Prefabs")]
    [SerializeField] private int m_maxRadiusAttempts;
    [SerializeField] private Transform m_PlanetsHolder;
    [SerializeField] private GameObject m_PlanetTemplate;
    [Range(1, 4)]
    [SerializeField] private int m_nbOfFactions;
    public List<string> m_FactionNames1 = new List<string>();
    public List<string> m_FactionNames2 = new List<string>();
    [Range(0f, 1f)]
    public float m_FactionColorS;
    [Range(0f, 1f)]
    public float m_FactionColorV;
    private List<Faction> m_SystemFactions = new List<Faction>();

    [Header("Planet Parameters")]
    [SerializeField] private int m_nbOfPlanets;
    [SerializeField] private float m_minDistance;
    [SerializeField] private float m_maxDistance;
    [SerializeField] private float m_minStep;
    [SerializeField] private float m_minSize;
    [SerializeField] private float m_maxSize;
    [SerializeField] private float m_dThresholdGas;

    [Header("PlanetPerc")]
    [SerializeField] private float m_atmChanceLowRad;
    [SerializeField] private float m_atmChanceHighRad;
    [SerializeField] private float m_gasChance;

    //private int m_iterator = 0;
    private bool m_hasAtmospheric = false;
    private List<Planet> m_Planets = new List<Planet>();


    // Awake is called when this object is instantiated
    private void Awake ()
    {
        // Create singleton instance reference for other scripts to call
        if (m_instance != null)
            // This happens when you load back into the main menu, as we already have a music player
            Destroy(gameObject);
        else
            m_instance = this;
    }


    // Update is called once per frame
    private void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            GeneratePlanets();
        }

        if (Input.GetKeyDown(KeyCode.L)) {
            GenerateFactions();

            foreach (Planet p in m_Planets) {
                p.GenerateBackstory();
            }
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            Reset();
        }
    }

    // Generate a random set of planet
    private void GeneratePlanets ()
    {
        for (int i = 0; i < m_nbOfPlanets; i++) {
            // Generate a random radius (from the center, Sun), angle and scale
            float radius = GenerateDistanceFromSun();
            float angle = Random.Range(0f, Mathf.PI * 2f);

            // Generate a random type depending on radius
            PlanetType t = GeneratePlanetType(radius);

            // Generate a planet scale (gas type can't be small)
            float scale = Random.Range(t == PlanetType.GasGiant ? m_minSize * 2 : m_minSize, m_maxSize);

            Planet p = Instantiate(m_PlanetTemplate, m_PlanetsHolder).GetComponent<Planet>();
            p.GeneratePlanet(radius, angle, scale, t);
            m_Planets.Add(p);
        }
    }

    // Generate a random radius; if radius too close to existing orbit, try again
    // When setting a high planet count, this may fail leading to potential overlaps between planets
    private float GenerateDistanceFromSun (int attempts = 0)
    {
        float d = Random.Range(m_minDistance, m_maxDistance);

        // As long as we're clashing with an existing orbit, and that we can still afford to keep going, try again!
        if (attempts < m_maxRadiusAttempts) {
            if (m_Planets.Count > 0) {
                bool isClashing = false;
                foreach (Planet p in m_Planets) {
                    if (p.GetDistanceFromGrav() - m_minStep < d && d <= p.GetDistanceFromGrav() + m_minStep) {
                        isClashing = true;
                    }
                }

                if (isClashing)
                    d = GenerateDistanceFromSun(attempts + 1);
            }
        } else
            Debug.LogWarning("Couldn't generate all radiuses");

        return d;
    }

    // Generate a random planet type depending on radius
    // All probabilities are completely arbitrary
    private PlanetType GeneratePlanetType (float radius)
    {
        float c = Random.Range(0f, 1f);

        // 1st case: below the gaseous giant threshold (only Atmospheric or Barren types)
        if (radius < m_dThresholdGas) {
            // At least one Atmospheric type planet per system
            if (m_hasAtmospheric == false) {
                m_hasAtmospheric = true;
                return PlanetType.Atmospheric;
            } else
                return c <= m_atmChanceLowRad ? PlanetType.Atmospheric : PlanetType.Barren;
        }

        // 2nd case: above threshold (can be all types but low Atmospheric chance)
        else
            if (c <= m_atmChanceHighRad)
                return PlanetType.Atmospheric;
            else
                return c <= m_gasChance ? PlanetType.GasGiant : PlanetType.Barren;
    }

    // Generate a number of randomized factions (name and color)
    private void GenerateFactions ()
    {
        for (int i = 0; i < m_nbOfFactions; i++) {
            string n = m_FactionNames1[Random.Range(0, m_FactionNames1.Count)]
                + " "
                + m_FactionNames2[Random.Range(0, m_FactionNames2.Count)];

            Color c = Color.HSVToRGB(
                Random.Range(0f, 1f),
                m_FactionColorS,
                m_FactionColorV
            );

            m_SystemFactions.Add(new Faction(n, c));
        }
    }

    // Reset all procedurally generated content
    private void Reset ()
    {
        foreach (Transform child in m_PlanetsHolder)
            Destroy(child.gameObject);

        m_Planets.Clear();
    }

    // GETTERS AND SETTERS
    public List<Faction> GetSystemFactions () => m_SystemFactions;
}
