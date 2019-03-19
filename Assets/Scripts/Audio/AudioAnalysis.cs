using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioAnalysis : MonoBehaviour, ISerializationCallbackReceiver
{

    //#region Singleton

    //public static AudioAnalysis Instance
    //{
    //    get
    //    {
    //        if (m_Instance != null)
    //        {
    //            return m_Instance;
    //        }
    //        else
    //        {
    //            m_Instance = FindObjectOfType<AudioAnalysis>();
    //            if (m_Instance == null)
    //            {
    //                GameObject gameObject = new GameObject("AudioAnalysis");
    //                gameObject.AddComponent<AudioAnalysis>();
    //            }

    //            return m_Instance;
    //        }
    //    }
    //}

    //protected static AudioAnalysis m_Instance = null;

    //#endregion

    public AudioClip audioClip;
    [Header("Sound Sample")]
    public int soundSampleSize = 512;
    //RMS value for 0 dB
    public float refValue = 0.1f;
    //Initial decreate rate
    public float SoundBufferDecreasingRate = 0.005f;
    //Decreate rate multiplier
    public float SoundBufferDecreasingRateMultiplier = 1.2f;

    [Header("Spectrum Sample")]
    public int spectrumSampleSize = 512;
    //Initial decreate rate
    public float bandBufferDecreasingRate = 0.005f;
    //Decreate rate multiplier
    public float bandBufferDecreasingRateMultiplier = 1.2f;

    [HideInInspector]
    public AudioSource Audio;

    //public enum MaxFrequencyBand { SubBass = 60, Bass = 250, LowMidRange = 500, MidRange = 2000, UpperMidRange = 4000, Presence = 6000, Brilliance = 20000 }
    //Usually: SubBass = 0-60, Bass = 60-250, LowMidRange = 250-500, MidRange = 500-2000, UpperMidRange = 2000-4000, Presence = 4000-6000, Brilliance = 6000-20000, NoName = MaxSpectrumValue (usually 24000)
    public readonly int[] BandLimiters = new int[8] { 60, 250, 500, 2000, 4000, 6000, 20000, 24000 };
    //Audio samples
    public float[] SoundSamples { get; private set; }
    //RMS value
    public float RMSValue { get; private set; }
    ////RMS value
    //public float RMSValue01 { get; private set; }
    //dB value
    public float DBValue { get; private set; }
    //Like RMS value but this value decreases more smoothly
    public float RMSBufferValue { get; private set; }
    //public float RMSBufferValue01 { get; private set; }

    //Spectrum samples
    public float[] SpectrumSamples { get; private set; }
    //Frequencies of bands (usually just 7 but we make it to 8 because the highest frequency value in spectrum often is 20000 but outputsamplerate/2 of computer often is 24000)  
    public float[] BandFrequencies { get; protected set;}
    //Similiar to bandfrequencies but the elements in bandbuffers decrease more smoothly
    public float[] BandFrequencyBuffers { get; protected set; }
    //Frequecies of bands represented from 0 to 1 
    public float[] BandFrequecies01 { get; protected set; }
    //Band buffers represented from 0 to 1
    public float[] BandFrequencyBuffers01 { get; protected set; }
    //Average amplitude of all bands
    public float AverageAmplitude01 { get; protected set; }
    //Average amplitude buffer
    public float AverageAmplitudeBuffer01 { get; protected set; }

    //Current sound buffer decrease rate
    protected float m_SoundBufferDecreateRate;
    //Spectrum resolution is the average different frequency value between the previous and the next element in sample data array (for example: element 0 is 24 and element 1 is 48 if the resolution is 24)
    protected float m_SpectrumResolution;
    //Max frequency value in spectrum (ussualy 24000)
    protected int m_MaxFrequencyValue;
    //the number of bands
    protected int m_BandCount = 8;
    //Decrease rate of each element in bandbuffers
    protected float[] m_BandBufferDecreaseRates = new float[8];
    //Current highest frequency of each band
    protected float[] m_CurrentHighestBandFrequecies = new float[8];
    //Current highest amplitude
    protected float m_CurrentHighestAmplitude;
    //protected float m_CurrentHighestRMSValue;


    // Use this for initialization
    protected void Awake()
    {

        //if (Instance != this)
        //{
        //    Destroy(this);
        //    return;
        //}

        Audio = GetComponent<AudioSource>();
        Audio.clip = audioClip;

        if (Audio.playOnAwake)
        {
            Audio.Play();
        }

        m_MaxFrequencyValue = AudioSettings.outputSampleRate / 2;
        m_SpectrumResolution = m_MaxFrequencyValue / spectrumSampleSize;

        SoundSamples = new float[soundSampleSize];
        SpectrumSamples = new float[spectrumSampleSize];
        BandFrequencies = new float[m_BandCount];
        BandFrequencyBuffers = new float[m_BandCount];
        BandFrequecies01 = new float[m_BandCount];
        BandFrequencyBuffers01 = new float[m_BandCount];
        m_BandBufferDecreaseRates = new float[m_BandCount];
        m_CurrentHighestBandFrequecies = new float[m_BandCount];
        


    }
	
	// Update is called once per frame
	protected void Update () {
        AnalyzeSound();
        AnalyzeSpectrum();
	}

    private void AnalyzeSound()
    {
        Audio.GetOutputData(SoundSamples, 0);

        float sum = 0;
        for (int i = 0; i < soundSampleSize; i++)
        {
            sum += SoundSamples[i] * SoundSamples[i];
        }

        RMSValue = Mathf.Sqrt(sum / soundSampleSize);
        DBValue = 20 * Mathf.Log10(RMSValue / refValue);
        if (DBValue < -160) DBValue = -160; // clamp it to -160dB min


        if(RMSBufferValue < RMSValue)
        {
            RMSBufferValue = RMSValue;
            m_SoundBufferDecreateRate = SoundBufferDecreasingRate;
        }
        else
        {
            RMSBufferValue -= m_SoundBufferDecreateRate;
            if (RMSBufferValue < 0) RMSBufferValue = 0;
            m_SoundBufferDecreateRate *= SoundBufferDecreasingRateMultiplier;
        }

    }


    private void AnalyzeSpectrum()
    {
        //Get samples
        Audio.GetSpectrumData(SpectrumSamples, 0, FFTWindow.BlackmanHarris);

        //Divide up into 8 frequency bands(Sub-bass > Bass > Low midrange > Midrange > Upper midrange > Presence > Brilliance > NoName)
        for (int i = 0; i < BandLimiters.Length; i++)
        {
            //Calculate band range
            int minBandValue = i == 0 ? 0 : BandLimiters[i - 1];
            int maxBandValue = BandLimiters[i];

            //Calculate sample index
            int minSampleIndex = Mathf.FloorToInt(minBandValue / m_SpectrumResolution);
            minSampleIndex = (spectrumSampleSize > minSampleIndex) ? minSampleIndex : (spectrumSampleSize - 1);
            int maxSampleIndex = Mathf.FloorToInt(maxBandValue / m_SpectrumResolution);
            maxSampleIndex = (spectrumSampleSize >= maxSampleIndex) ? maxSampleIndex : spectrumSampleSize;
            

            //Calculate average frequency
            float bandAverageFrequency = 0;
            for (int index = minSampleIndex; index < maxSampleIndex; index++)   
            {
                bandAverageFrequency += SpectrumSamples[index];
            }
            bandAverageFrequency /= (maxSampleIndex == minSampleIndex) ? 1 : (maxSampleIndex - minSampleIndex);

            //Set average frequency
            BandFrequencies[i] = bandAverageFrequency;

        }

        //Set band buffer
        for (int i = 0; i < BandLimiters.Length; i++)
        {
            //Set to current frequency value
            if(BandFrequencyBuffers[i]< BandFrequencies[i])
            {
                BandFrequencyBuffers[i] = BandFrequencies[i];
                m_BandBufferDecreaseRates[i] = bandBufferDecreasingRate;
            }
            else //Decrease gradually
            {
                BandFrequencyBuffers[i] -= BandFrequencyBuffers[i] * m_BandBufferDecreaseRates[i];
                if (BandFrequencyBuffers[i] < 0) BandFrequencyBuffers[i] = 0;
                m_BandBufferDecreaseRates[i] *= bandBufferDecreasingRateMultiplier;
            }
        }

        //0 to 1 representation
        for (int i = 0; i < BandLimiters.Length; i++)
        {
            if(m_CurrentHighestBandFrequecies[i] < BandFrequencies[i])
            {
                m_CurrentHighestBandFrequecies[i] = BandFrequencies[i];
            }

            BandFrequecies01[i] = m_CurrentHighestBandFrequecies[i] == 0 ? 0 : BandFrequencies[i] / m_CurrentHighestBandFrequecies[i];
            BandFrequencyBuffers01[i] = m_CurrentHighestBandFrequecies[i] == 0 ? 0 : BandFrequencyBuffers[i] / m_CurrentHighestBandFrequecies[i];
            
        }

        //Get average amplitude
        float currentAmplitude = 0;
        float currentAmplitudeBuffer = 0;
        for (int i = 0; i < BandLimiters.Length; i++)
        {
            currentAmplitude += BandFrequencies[i];
            currentAmplitudeBuffer += BandFrequencyBuffers[i];
        }
        if (m_CurrentHighestAmplitude < currentAmplitude) m_CurrentHighestAmplitude = currentAmplitude;
        AverageAmplitude01 = m_CurrentHighestAmplitude == 0 ? 0 : currentAmplitude / m_CurrentHighestAmplitude;
        AverageAmplitudeBuffer01 = m_CurrentHighestAmplitude == 0 ? 0 : currentAmplitude / m_CurrentHighestAmplitude;
        

    }

    public void OnAfterDeserialize()
    {
        
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
#if UNITY_EDITOR
        // Prevent saving if this is a prefab
        if (UnityEditor.PrefabUtility.GetPrefabType(this) == UnityEditor.PrefabType.Prefab)
        {
            audioClip = null;
        }
#endif
    }
}
