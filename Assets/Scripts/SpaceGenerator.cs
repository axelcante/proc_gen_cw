using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceGenerator : MonoBehaviour
{
    public int m_layers;
    public int m_nbPerLayerFactor;
    public float m_xMin;
    public float m_yMin;
    public GameObject m_Star;

    // Start is called before the first frame update
    void Start()
    {
        GenerateStarryBackground();
    }

    // Generate a number of stars (i.e., white points)
    private void GenerateStarryBackground ()
    {
        for (int i = 1; i <= m_layers; i++) {
            
            // Double the amount of stars per layer (meaning they become more numerous but smaller through the layers)
            m_nbPerLayerFactor *= 2 * i;

            // Each star has a scale and an opacity factor which is determined by its distance, i.e., layer
            for (int j = 0; j < m_nbPerLayerFactor; j++) {
                // Determine a random position in a pre-determined square
                Vector2 pos = new Vector2(Random.Range(m_xMin, -m_xMin), Random.Range(m_yMin, -m_yMin));
                GameObject star = Instantiate(m_Star, pos, Quaternion.identity, transform);

                // Compute a factor which will change the scale and material alpha of the star depending on its layer
                float factor = 1f / i;
                star.transform.localScale = new Vector3(factor, factor, factor);
                SpriteRenderer spr = star.GetComponentInChildren<SpriteRenderer>();
                Color color = spr.color;
                color.a = factor;
                spr.color = color;
            }
        }
    }
}
