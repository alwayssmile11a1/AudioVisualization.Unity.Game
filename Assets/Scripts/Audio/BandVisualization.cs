using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BandVisualization : MonoBehaviour {

    public AudioAnalysis audioAnalysis;
    public GameObject sampleDataVisualizePrefab;
    public GameObject spectrumBandVisualizePrefab;
    public GameObject bufferBandVisualizePrefab;

    //list of visualizeObject
    protected List<GameObject> m_SampleDataVisualizeObjects = new List<GameObject>();
    protected List<GameObject> m_SpectrumBandVisualizeObjects = new List<GameObject>();
    protected List<GameObject> m_BufferBandVisualizeObjects = new List<GameObject>();


    // Use this for initialization
    void Start () {
        for (int i = 0; i < audioAnalysis.spectrumSampleSize; i++)
        {
            GameObject visualizeObject = Instantiate(sampleDataVisualizePrefab, transform.position, Quaternion.identity);
            visualizeObject.transform.parent = transform;
            visualizeObject.name = "SampleDataVisualizer" + i;
            visualizeObject.transform.eulerAngles = new Vector3(0, 360.0f / audioAnalysis.spectrumSampleSize * i, 0);
            visualizeObject.transform.Translate(visualizeObject.transform.forward * 100);
            m_SampleDataVisualizeObjects.Add(visualizeObject);
        }

        for (int i = 0; i < audioAnalysis.BandFrequencies.Length; i++)
        {
            GameObject visualizeObject = Instantiate(spectrumBandVisualizePrefab, transform.position - Vector3.forward * 20
                              + Vector3.right * (audioAnalysis.BandFrequencies.Length / 2 - i - (audioAnalysis.BandFrequencies.Length % 2 == 0 ? 0.5f : 0f)) * 15f,
                              Quaternion.identity);
            visualizeObject.transform.parent = transform;
            visualizeObject.name = "BandVisualizer" + i;
            m_SpectrumBandVisualizeObjects.Add(visualizeObject);
        }

        for (int i = 0; i < audioAnalysis.BandFrequencyBuffers.Length; i++)
        {
            GameObject visualizeObject = Instantiate(bufferBandVisualizePrefab, transform.position - Vector3.forward * 50
                              + Vector3.right * (audioAnalysis.BandFrequencyBuffers.Length / 2 - i - (audioAnalysis.BandFrequencyBuffers.Length % 2 == 0 ? 0.5f : 0f)) * 15f,
                              Quaternion.identity);
            visualizeObject.transform.parent = transform;
            visualizeObject.name = "BandVisualizer" + i;
            m_BufferBandVisualizeObjects.Add(visualizeObject);
        }
    }
	
	// Update is called once per frame
	void Update () {
        Visualize();
	}


    private void Visualize()
    {
        for (int i = 0; i < audioAnalysis.spectrumSampleSize; i++)
        {
            m_SampleDataVisualizeObjects[i].transform.localScale = new Vector3(1, audioAnalysis.SpectrumSamples[i] * 1000 + 2, 1);
        }

        for (int i = 0; i < audioAnalysis.BandFrequencies.Length; i++)
        {
            m_SpectrumBandVisualizeObjects[i].transform.localScale = new Vector3(10, audioAnalysis.BandFrequencies[i] * 100 + 10, 10);
        }

        for (int i = 0; i < audioAnalysis.BandFrequencies.Length; i++)
        {
            m_BufferBandVisualizeObjects[i].transform.localScale = new Vector3(10, audioAnalysis.BandFrequencyBuffers01[i] * 50 + 10, 10);
        }

    }
}
