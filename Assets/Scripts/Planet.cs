using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    private float m_distanceFromGrav;
    private int m_minOrbitPoints = 50;
    private LineRenderer m_OrbitRenderer;
    private float m_orbitSpeed;
    private List<Planet> m_Moons = new List<Planet>();

    [Header("Planet Data")]
    public PlanetType m_Type;
    public string m_Identifier;
    public string m_Population;
    public string m_Resources;
    public Faction m_Faction;

    [Header("Planet Story Generator")]
    public List<string> m_PlanetNames1 = new List<string>();
    public List<string> m_PlanetNames2 = new List<string>();
    public List<string> m_PlanetNames3 = new List<string>();
    public List<string> m_ResourceAmt = new List<string>();
    [SerializeField] private float m_barrenPopChance;

    [Header("Planet Parts")]
    public List<Sprite> m_Atmospheres = new List<Sprite>();
    public List<Sprite> m_Landmasses = new List<Sprite>();
    public List<Sprite> m_Craters = new List<Sprite>();
    public List<Sprite> m_Lights = new List<Sprite>();
    [SerializeField] private Collider2D m_Collider;

    [Header("Planet Visuals")]
    [SerializeField] private GameObject m_CentreMass;
    [SerializeField] private float m_zOffset;   // Control which sprite renders on top
    [SerializeField] private SpriteRenderer m_Halo;
    [SerializeField] private SpriteRenderer m_Ring;
    [SerializeField] private SpriteRenderer m_Body;
    [SerializeField] private SpriteRenderer m_Landmass;
    [SerializeField] private SpriteRenderer m_Atmosphere;
    [SerializeField] private SpriteRenderer m_Light;
    
    [Space(10)]
    [Range(0f, 360f)]
    [SerializeField] private float m_minHAtm;
    [Range(0f, 360f)]
    [SerializeField] private float m_maxHAtm;
    [Range(0f, 360f)]
    [SerializeField] private float m_minH;
    [Range(0f, 360f)]
    [SerializeField] private float m_maxH;
    [Range(0f, 1f)]
    [SerializeField] private float m_minS;
    [Range(0f, 1f)]
    [SerializeField] private float m_maxS;
    [Range(0f, 1f)]
    [SerializeField] private float m_minV;
    [Range(0f, 1f)]
    [SerializeField] private float m_maxV;
    [Range(0f, 1f)]
    [SerializeField] private float m_minSLand;
    [Range(0f, 1f)]
    [SerializeField] private float m_maxSLand;
    [Range(0f, 1f)]
    [SerializeField] private float m_minVLand;
    [Range(0f, 1f)]
    [SerializeField] private float m_maxVLand;

    [Header("Animations")]
    [SerializeField] private float m_rotSpeed;
    [SerializeField] private float m_minOrbitSpeed;
    [SerializeField] private float m_maxOrbitSpeed;
    [SerializeField] private float m_aOffsetLight;
    [SerializeField] private float m_aOffsetBody;

    // Awake is called on this instance's loading
    private void Awake ()
    {
        m_OrbitRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    private void Update ()
    {
        // Here we can animate the planet to rotate around itself/orbit and move the planet bits!
        transform.Rotate(0f, 0f, m_orbitSpeed * Time.deltaTime);
        if (m_Landmass)
            m_Landmass.transform.Rotate(0f, 0f, m_rotSpeed/3 * Time.deltaTime);
        
        if (m_Atmosphere)
            m_Atmosphere.transform.Rotate(0f, 0f, m_rotSpeed * Time.deltaTime);
    }

    // Generate a random planet and its elements depending on its type
    public void GeneratePlanet (float radius, float angle, float scale, PlanetType type)
    {
        // Set position with radius and angle, as well as scale
        m_distanceFromGrav = radius;
        m_Type = type;
        float posX = radius * Mathf.Cos(angle);
        float posY = radius * Mathf.Sin(angle);
        m_CentreMass.transform.position = new Vector3(posX, posY, m_zOffset);
        m_CentreMass.transform.localScale = new Vector3(scale, scale, scale);

        // Generate the planet's sprites
        Sprite atm = null;
        if (m_Type == PlanetType.Atmospheric || m_Type == PlanetType.GasGiant)
            atm = m_Atmospheres[Random.Range(0, m_Atmospheres.Count)];

        Sprite land = null;
        if (m_Type == PlanetType.Barren || m_Type == PlanetType.Moon)
            land = m_Craters[Random.Range(0, m_Craters.Count)];
        else if (m_Type == PlanetType.Atmospheric)
            land = m_Landmasses[Random.Range(0, m_Landmasses.Count)];

        // Set planet visuals
        // Body/Halo
        Color bCol = GenerateColor(m_Type);
        m_Body.color = bCol;
        m_Halo.color = bCol;

        // Ring
        if (m_Type == PlanetType.GasGiant)
            m_Ring.color = bCol;
        else
            m_Ring.gameObject.SetActive(false);

        // Atmosphere
        if (atm) {
            m_Atmosphere.sprite = atm;
            Color aCol = GenerateColor();
            //m_Halo.color = aCol;
            aCol.a = 0.5f;
            m_Atmosphere.color = aCol;
        } else
            m_Atmosphere.gameObject.SetActive(false);

        // Landmass
        if (land) {
            m_Landmass.sprite = land;
            if (m_Type == PlanetType.Barren || m_Type == PlanetType.Moon)
                m_Landmass.color = bCol;
            else
                m_Landmass.color = GenerateColor();
        } else
            m_Landmass.gameObject.SetActive(false);

        // Draw an orbit that the planet will sit on
        DrawOrbit(radius);

        // Decide an orbit speed and random direction
        int randomSign = Random.Range(0, 2) == 0 ? -1 : 1;
        m_orbitSpeed = Random.Range(m_minOrbitSpeed, m_maxOrbitSpeed) * randomSign;

        // Orient the light sprite to face the sun
        OrientLight();
    }

    // Generate a random color with fixed saturation and value
    // This is horrifyingly coded, and I am sorry for whoever is reading this
    private Color GenerateColor (PlanetType t = PlanetType.Barren, bool isLand = false)
    {
        return Color.HSVToRGB(
            t == PlanetType.Atmospheric ? Random.Range(m_minHAtm / 360, m_maxH / 360) : Random.Range(m_minH / 360, m_maxH / 360),
            isLand ? Random.Range(m_minS, m_maxS) : Random.Range(m_minS, m_maxS),
            Random.Range(m_minV, m_maxV)
        );
    }

    // Draw an orbit around the gravitational point (the sun or another planet)
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

    // Orient light sprite/body to face the sun (only needs to be done once)
    private void OrientLight ()
    {
        m_Light.sprite = m_Lights[Random.Range(0, m_Lights.Count)];

        Vector2 direction = transform.position - m_Light.transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        m_Light.transform.rotation = Quaternion.AngleAxis(angle - m_aOffsetLight, Vector3.forward);
        direction = transform.position - m_Body.transform.position;
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        m_Body.transform.rotation = Quaternion.AngleAxis(angle - m_aOffsetBody, Vector3.forward);
    }

    // Generate planet name, population count, name, class...
    public void GenerateBackstory ()
    {
        // Generate Planet name
        m_Identifier = m_PlanetNames1[Random.Range(0, m_PlanetNames1.Count)]
            + m_PlanetNames2[Random.Range(0, m_PlanetNames2.Count)]
            + m_PlanetNames3[Random.Range(0, m_PlanetNames3.Count)];

        // Generate Planet population
        float chance = Random.Range(0f, 1f);
        if (m_Type == PlanetType.Atmospheric)
            m_Population = Mathf.Round(Random.Range(5f, 15f) * 10f) / 10f + " billion";
        else if ((m_Type == PlanetType.Barren || m_Type == PlanetType.Moon) && chance <= m_barrenPopChance)
            m_Population = Mathf.Round(Random.Range(0f, 2000f) * 10f) / 10f + " million";
        else
            m_Population = "0";

        // Generate Planet resources
        m_Resources = m_ResourceAmt[Random.Range(0, m_ResourceAmt.Count)];

        // Select random System Faction
        List<Faction> factions = SystemGenerator.GetInstance().GetSystemFactions();
        m_Faction = factions[Random.Range(0, factions.Count)];
        m_OrbitRenderer.startColor = m_Faction.color;
        m_OrbitRenderer.endColor = m_Faction.color;
        m_OrbitRenderer.startWidth = 15f;
        m_OrbitRenderer.endWidth = 15f;
    }

    private void OnMouseEnter ()
    {
        Debug.Log("testing: entered");
    }

    private void OnMouseExit ()
    {
        Debug.Log("testing: exited");
    }

    // GETTERS AND SETTERS
    public float GetDistanceFromGrav () => m_distanceFromGrav;
}
