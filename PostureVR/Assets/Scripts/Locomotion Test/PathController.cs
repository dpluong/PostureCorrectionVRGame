using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PathController : MonoBehaviour
{
    [SerializeField] List<Transform> points;

    public float radius = 0.5f;
    public LayerMask layerCollided;
    public DataCollection dataCollection;
    public GameObject scorePanel;
    public GameObject spheres;
    public float distance = 1.5f;

    private int score = 0;
    //private int i = 0;

    Color level1 = new Color(165f/255f, 15f/255f, 21f/255f, 1f);
    Color level2 = new Color(222f/255f, 45f/255f, 38f/255f, 1f);
    Color level3 = new Color(251f/255f, 106f/255f, 74f/255f, 1f);
    Color level4 = new Color(252/255f, 174f/255f, 145f/255f, 1f);
    Color level5 = new Color(254f/255f, 229f/255f, 217f/255f, 1f);

    int targetSphere = -1;

    List<Color> colors = new List<Color>();
    private bool isNormalized = false;

    void NormalizeRGBColor()
    {
        if (!isNormalized)
        {
            for (int i = 0; i < colors.Count; ++i)
            {
                colors[i] = new Color(colors[i].r / 255f, colors[i].g / 255f, colors[i].b / 255f, 1f); 
                Debug.Log(colors[i]);
            }
            isNormalized = true;
        }
    }

    void ShuffleList<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    void SphereColorGeneration()
    {
        if (colors.Count == 0)
        {
            colors.Add(level1);
            colors.Add(level2);
            colors.Add(level3);
            colors.Add(level4);
            colors.Add(level5);
            ShuffleList(colors);
            //NormalizeRGBColor();
        }

        for (int i = 0; i < points.Count; ++i)
        {
            int randomIndex = Random.Range(0, colors.Count);
            Color randomValueColor = colors[randomIndex];
            points[i].gameObject.GetComponent<Renderer>().material.SetColor("_Color", randomValueColor);
            if (colors[randomIndex] == level1)
            {
                targetSphere = i;
                points[i].gameObject.GetComponent<TeleportationAnchor>().enabled = true;
            }
            colors.RemoveAt(randomIndex);
        }
    }

    void SpherePositionGeneration()
    {
        transform.position = Camera.main.transform.position + (-Vector3.forward * distance);
        spheres.SetActive(true);
    }
    
    void PathGeneration()
    {
        if (targetSphere == -1)
        {
            SpherePositionGeneration();
            SphereColorGeneration();
        }
        else
        {
            if (Physics.CheckSphere(points[targetSphere].position, radius, layerCollided))
            {
                if (dataCollection.startCollectingData == false)
                {
                    dataCollection.startCollectingData = true;
                }

                score += 1;
                // reset to original sphere state
                
                points[targetSphere].gameObject.GetComponent<TeleportationAnchor>().enabled = false;
                targetSphere = -1;
                spheres.SetActive(false);
            }
        }
    }

    void DisplayFinalScore()
    {
        scorePanel.GetComponent<TMPro.TextMeshProUGUI>().text = score.ToString();
        scorePanel.transform.parent.gameObject.SetActive(true);
    }   

    void Update()
    {
        if (!dataCollection.endCollectingData)
        {
            PathGeneration();
        }
        else
        {
            DisplayFinalScore();
        }
    }
}
