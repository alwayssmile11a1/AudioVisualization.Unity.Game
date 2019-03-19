using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhyllotaxisVisualization : MonoBehaviour {

    public AudioAnalysis audioAnalysis;

    [Header("Phyllotaxis related variables")]
    //137.508 is the golden angle
    public float degree = 137.508f;
    public float scalingFactor = 1;
    public int startIndex = 0;
    public int step = 1;
    public int maxIndex = 1000;


    [Header("Trail related variables")]
    public float minSpeed = 10;
    public float maxSpeed = 20;


    //[Header("Band")]
    //public int band;

    private int m_CurrentIndex;
    //private TrailRenderer m_LineRenderer;
    //private List<Vector3> m_PhyllotaxisPositions = new List<Vector3>();

    ////private Vector3 m_StartPosition;
    private Vector3 m_EndPosition;
    

    private void Awake()
    {
        m_CurrentIndex = startIndex;
        //m_LineRenderer = GetComponent<TrailRenderer>();

        //Setup start and end position
        //m_StartPosition = transform.position;
        m_EndPosition = GetPhyllotaxisCoordinate(degree, scalingFactor, m_CurrentIndex);
        m_CurrentIndex += step;
    }


    // Update is called once per frame
    private void Update()
    {

        if (m_CurrentIndex < maxIndex)
        {
            //Debug.Log(Time.deltaTime * Mathf.Lerp(minSpeed, maxSpeed, AudioAnalysis.Instance.BandFrequecies01[1]));
            transform.position = Vector3.MoveTowards(transform.position, m_EndPosition, Time.deltaTime * Mathf.Lerp(minSpeed, maxSpeed, audioAnalysis.AverageAmplitudeBuffer01));
            
            if (transform.position == m_EndPosition)
            {
                m_EndPosition = GetPhyllotaxisCoordinate(degree, scalingFactor, m_CurrentIndex);
                m_CurrentIndex += step;
            }

        }
    }

    private Vector3 GetPhyllotaxisCoordinate(float deg, float scale, int index)
    {
        float r = scale * Mathf.Sqrt(index);
        float a = index * deg * Mathf.Deg2Rad;
        float x = r * Mathf.Cos(a);
        float y = r * Mathf.Sin(a);

        return new Vector3(x, y, 0);
    }



}
