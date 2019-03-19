using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour {

    public AudioAnalysis audioAnalysis;

    public GameObject bullet;
    public float shootingRate = 1f;
    public float rotationSpeed = 2f;
    public float timeToStartShooting = 2f;

    [Header("Laser")]
    //public LineRenderer leftLaser;
    //public LineRenderer rightLaser;
    //public LineRenderer upLaser;
    //public LineRenderer downLaser;
    public LineRenderer[] lasers;
    public float laserThreshhold = 0.3f;


    private float m_CurrentAngle;
    private float m_Timer;

    private void Awake()
    {
        m_Timer = timeToStartShooting;
        m_CurrentAngle = transform.rotation.eulerAngles.z;
    }

    private void Update () {

        if (m_Timer > 0)
        {
            m_Timer -= Time.deltaTime;
            if (m_Timer <= 0)
            {
                Shoot();
                m_Timer = shootingRate;
            }
        }

        //if (m_Timer<=0)
        //{
        //    Shoot();
        //}
        //else
        //{
        //    m_Timer -= Time.deltaTime;
        //}


        Rotate();

	}

    private void Rotate()
    {

        transform.rotation = Quaternion.AngleAxis(m_CurrentAngle, Vector3.forward);
        m_CurrentAngle += rotationSpeed * audioAnalysis.RMSValue;
        Debug.Log(audioAnalysis.RMSValue);
    }

    private void Shoot()
    {
        Rigidbody2D newBullet = Instantiate(bullet, transform.position + transform.up * 0.7f, transform.rotation).GetComponent<Rigidbody2D>();
        Rigidbody2D newBullet1 = Instantiate(bullet, transform.position - transform.up * 0.7f, transform.rotation * Quaternion.AngleAxis(180, Vector3.forward)).GetComponent<Rigidbody2D>();
        Rigidbody2D newBullet2 = Instantiate(bullet, transform.position + transform.right * 0.7f, transform.rotation * Quaternion.AngleAxis(-90, Vector3.forward)).GetComponent<Rigidbody2D>();
        Rigidbody2D newBullet3 = Instantiate(bullet, transform.position - transform.right * 0.7f, transform.rotation * Quaternion.AngleAxis(90, Vector3.forward)).GetComponent<Rigidbody2D>();
        Destroy(newBullet.gameObject, 2f);
        Destroy(newBullet1.gameObject, 2f);
        Destroy(newBullet2.gameObject, 2f);
        Destroy(newBullet3.gameObject, 2f);

        //for (int i = 0; i < lasers.Length; i++)
        //{
        //    Vector3 originalScale = lasers[i].transform.localScale;
        //    originalScale.x = AudioAnalysis.Instance.RMSBufferValue * 10f + 1;
        //    lasers[i].transform.localScale = originalScale;


        //    if (AudioAnalysis.Instance.RMSValue > laserThreshhold)
        //    {
        //        lasers[i].gameObject.SetActive(true);
        //    }
        //    else
        //    {
        //        lasers[i].gameObject.SetActive(false);
        //    }

        //}

    }

}
