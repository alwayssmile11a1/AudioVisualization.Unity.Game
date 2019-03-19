using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit2D;
using UnityEngine.SceneManagement;


public class PlayerController : MonoBehaviour
{
    public LayerMask wallPlayer;
    public float raycastDistance = 0.3f;
    public float moveSpeed = 5f;


    private Vector2 m_MovementVector;
    private Rigidbody2D m_Rigidbody2D;
    private Vector2 m_CollisionDirection = Vector2.up;
    private Vector2 m_PreviousCollisionDirection = Vector2.up;
    private RaycastHit2D[] m_Results = new RaycastHit2D[2];
    private ContactFilter2D m_ContactFilter = new ContactFilter2D();

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_ContactFilter.useTriggers = false;
        Physics2D.queriesStartInColliders = false;
        m_ContactFilter.layerMask = wallPlayer;

    }

    // Update is called once per frame
    private void Update()
    {
        GetMovement();
        //CheckForWall();

    }

    private void FixedUpdate()
    {
        //Rotate();
        Move();

    }

    private void GetMovement()
    {
        float m_HorizontalMovement = Input.GetAxisRaw("Horizontal");
        m_MovementVector.Set(m_HorizontalMovement, 0);
    }

    private void CheckForWall()
    {
        if (Mathf.Approximately(m_MovementVector.x, 0)) return;
        int hitCount = Physics2D.Raycast(transform.position, transform.right * m_MovementVector.x, m_ContactFilter, m_Results, raycastDistance);
        //Debug.DrawLine(transform.position, transform.position + transform.right * raycastDistance * m_MovementVector.x);
        if (hitCount > 0)
        {
            m_CollisionDirection = m_Results[0].normal;
        }
    }

    private void Rotate()
    {
        if (m_PreviousCollisionDirection != m_CollisionDirection)
        {
            transform.RotateToDirection(Quaternion.AngleAxis(-90f, Vector3.forward) * m_CollisionDirection);
            m_PreviousCollisionDirection = m_CollisionDirection;
        }
    }

    private void Move()
    {
        m_Rigidbody2D.velocity = transform.right * m_MovementVector.x * moveSpeed;
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

}
