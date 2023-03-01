using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    public Sprite flag;

    public Faction (string n, Color c, Sprite f)
    {
        name = n;
        color = c;
        flag = f;
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
    [Range(1, 5)]
    [SerializeField] private int m_nbOfFactions;
    public List<string> m_FactionNames1 = new List<string>();
    public List<string> m_FactionNames2 = new List<string>();
    [Range(0f, 1f)]
    public float m_FactionColorS;
    [Range(0f, 1f)]
    public float m_FactionColorV;
    public List<Sprite> m_FactionFlags = new List<Sprite>();
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

    [Header("UI Elements")]
    [SerializeField] private GameObject m_UIDataHolder;
    [SerializeField] private TMP_Text m_PName;
    [SerializeField] private TMP_Text m_PType;
    [SerializeField] private TMP_Text m_PDist;
    [SerializeField] private TMP_Text m_PPopulation;
    [SerializeField] private TMP_Text m_PResources;
    [SerializeField] private TMP_Text m_PFaction;
    [SerializeField] private Image m_PFactionFlag;
    [SerializeField] private Image m_PFactionPattern;
    [SerializeField] private float m_PFlagAlpha;
    [SerializeField] private List<GameObject> m_FactionHolders = new List<GameObject>();
    [SerializeField] private List<TMP_Text> m_FactionNameDisplays = new List<TMP_Text>();
    [SerializeField] private List<Image> m_FactionFlagsBG = new List<Image>();
    [SerializeField] private List<Image> m_FactionFlagsPat = new List<Image>();
    private bool m_OrbitDisplay = false;

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
            


        if (Input.GetKeyDown(KeyCode.R)) {
            ResetAllContent();
        }
    }

    // Generate a random set of planet
    public void GeneratePlanets ()
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
    public void GenerateFactions ()
    {
        // Reset any previous factions created
        m_SystemFactions.Clear();

        for (int i = 0; i < m_nbOfFactions; i++) {
            string n = m_FactionNames1[Random.Range(0, m_FactionNames1.Count)]
                + " "
                + m_FactionNames2[Random.Range(0, m_FactionNames2.Count)];

            Color c = Color.HSVToRGB(
                Random.Range(0f, 1f),
                m_FactionColorS,
                m_FactionColorV
            );

            Sprite f = m_FactionFlags[Random.Range(0, m_FactionFlags.Count)];

            Faction faction = new Faction(n, c, f);
            m_SystemFactions.Add(faction);
        }

        // Display the generated factions in the UI
        UpdateFactionDisplay();
    }

    // Generate a backstory for each existing planet
    public void GenerateBackstories ()
    {
        if (m_Planets.Count > 0)
            foreach (Planet p in m_Planets)
                p.GenerateBackstory();
    }

    // Reset all procedurally generated content
    public void ResetAllContent ()
    {
        ClosePlanetData();
        foreach(GameObject h in m_FactionHolders)
            h.SetActive(false);

        foreach (Transform child in m_PlanetsHolder)
            Destroy(child.gameObject);

        m_Planets.Clear();

        // Set all faction UI elements to off and reset the array
        for (int i = 0; i < m_SystemFactions.Count; i++)
            m_FactionHolders[i].SetActive(false);

        m_SystemFactions.Clear();
    }

    // GETTERS AND SETTERS
    public List<Faction> GetSystemFactions () => m_SystemFactions;
    public float GetMinStep () => m_minStep;
    public GameObject GetPlanetTemplate () => m_PlanetTemplate;


    // ----------------------------------------------------------------


    // UI Methods - SHOULD REALLY BE IN THEIR OWN SCRIPT NOW SHOULDN'T THEY
    public void TogglePlanetData (Planet p)
    {
        m_UIDataHolder.SetActive(true);

        // Populate data UI elements
        m_PName.text = p.m_Identifier;
        m_PFaction.text = p.m_Faction.name;
        m_PDist.text = (Mathf.Round(p.GetDistanceFromGrav() * 100) / 10) + " m.km";

        switch (p.m_Type) {
            case PlanetType.Atmospheric:
                m_PType.text = "Atmospheric";
                break;
            case PlanetType.Barren:
                m_PType.text = "Barren";
                break;
            case PlanetType.GasGiant:
                m_PType.text = "Gas Giant";
                break;
            case PlanetType.Moon:
                m_PType.text = "Moon";
                break;
            default:
                m_PType.text = "Unknown";
                break;
        }

        m_PPopulation.text = p.m_Population;
        m_PResources.text = p.m_Resources;
        Color fCol = p.m_Faction.color;
        fCol.a = m_PFlagAlpha;
        m_PFactionFlag.color = fCol;
        m_PFactionPattern.sprite = p.m_Faction.flag;
        m_PFactionPattern.color = fCol;
    }

    public void ClosePlanetData ()
    {
        m_UIDataHolder.SetActive(false);
    }

    public void UpdateFactionDisplay ()
    {
        if (m_SystemFactions.Count == 0) return;

        for (int i = 0; i < m_SystemFactions.Count; i++) {
            Faction faction = m_SystemFactions[i];

            // Set the corresponding faction holder to active
            m_FactionHolders[i].SetActive(true);

            // Update the faction flag and symbol
            Color fCol = faction.color;
            fCol.a = m_PFlagAlpha;
            m_FactionFlagsBG[i].color = fCol;
            m_FactionFlagsPat[i].sprite = faction.flag;
            m_FactionFlagsPat[i].color = fCol;

            // Set the faction name
            m_FactionNameDisplays[i].text = faction.name;
            ;
        }
    }

    public void ToggleDisplayOrbits ()
    {
        if (m_Planets.Count > 0)
            foreach (Planet p in m_Planets)
                p.ToggleOrbitControl(!m_OrbitDisplay);

        m_OrbitDisplay = !m_OrbitDisplay;
    }
}
