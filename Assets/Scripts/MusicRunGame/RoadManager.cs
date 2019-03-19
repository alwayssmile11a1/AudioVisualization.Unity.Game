using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit2D;

public class RoadManager : MonoBehaviour
{
    public enum ManagerMode { Record, Spawn }
    public enum Difficulty { Easy, Normal, Hard, SuperHard }

    public ManagerMode managerMode;

    [Header("Player Related Variables")]
    public float verticalSpeed = 10f;
    public float horizontalSpeed = 8f;

    [Header("Data")]
    public RoadData roadData;
    public RoadDataManifest roadDataManifest;

    [Header("Record Mode")]
    public float straighCheckingDistance = 5f;
    public float zigzagCheckingMaxDistance = 2f;
    public int zigzagCheckingMinCount = 5;

    [Header("Spawn Mode")]
    public GameObject obstacle;
    public float zigzagIntensity = 2f;
    public float spawnDelayHeight = 1f;

    [Space(10)]
    public Difficulty difficulty;
    public DifficultyManager[] difficultyManagers = new DifficultyManager[System.Enum.GetNames(typeof(Difficulty)).Length];


    private Vector2 m_MovementVector;
    private Rigidbody2D m_Rigidbody2D;
    //private List<Vector2> m_RecordedPositions = new List<Vector2>();

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();

        if (managerMode == ManagerMode.Spawn)
        {
            SpawnObstacles();
        }
        else
        {
            roadData.Clear();
        }


    }

    // Update is called once per frame
    private void Update()
    {
        GetMovement();
    }

    private void FixedUpdate()
    {
        Move();
        RecordRoad();
    }

    private void GetMovement()
    {
        float horizontalMovement = Input.GetAxisRaw("Horizontal");
        m_MovementVector.Set(horizontalMovement * horizontalSpeed, verticalSpeed);

    }

    private void Move()
    {
        m_Rigidbody2D.velocity = m_MovementVector;
    }

    private void RecordRoad()
    {
        if (managerMode != ManagerMode.Record) return;

        if (roadData.positions.Count < 2)
        {
            roadData.positions.Add(transform.position);
        }
        else
        {
            Vector2 previousPosition1 = roadData.positions[roadData.positions.Count - 1];
            Vector2 previousPosition2 = roadData.positions[roadData.positions.Count - 2];

            //If we are moving to left or to the right continuously
            if ((transform.position.x == previousPosition1.x && previousPosition1.x == previousPosition2.x) ||
                (transform.position.x > previousPosition1.x && previousPosition1.x > previousPosition2.x) ||
                (transform.position.x < previousPosition1.x && previousPosition1.x < previousPosition2.x) ||
                (transform.position.x > previousPosition1.x && previousPosition1.x == previousPosition2.x && previousPosition1.y - previousPosition2.y < straighCheckingDistance) ||
                (transform.position.x < previousPosition1.x && previousPosition1.x == previousPosition2.x && previousPosition1.y - previousPosition2.y < straighCheckingDistance))
            {
                //Modify the last value
                roadData.positions[roadData.positions.Count - 1] = transform.position;
            }
            else
            {
                //Add new value
                roadData.positions.Add(transform.position);
            }
        }

    }

    private void SpawnObstacles()
    {
        if (roadDataManifest == null) return;

        //half size of path
        float initialPathExtent = difficultyManagers[(int)difficulty].initialPathExtent;
        float minPathExtent = difficultyManagers[(int)difficulty].minPathExtent;

        //decrease rate
        float decreaseRange = initialPathExtent >= minPathExtent ? initialPathExtent - minPathExtent : 0;
        float decreaseRate = decreaseRange / (roadDataManifest.positions.Length - 1);
        float currentPathExtent = initialPathExtent;

        //Spawn obstacles
        for (int i = 0; i < roadDataManifest.positions.Length - 1; i++)
        {
            //Spawn zigzag if needed
            int endZigzagIndex = CheckZigZagPattern(i);
            if (endZigzagIndex != i)
            {
                float averageDistance = (roadDataManifest.positions[endZigzagIndex].y - roadDataManifest.positions[i].y) / (endZigzagIndex - i);
                float averageXOffset = (roadDataManifest.positions[endZigzagIndex].x - roadDataManifest.positions[i].x) / (endZigzagIndex - i);
                Vector2 endPosition = roadDataManifest.positions[i];
                Vector2 startPosition = endPosition;
                for (int j = i; j < endZigzagIndex; j++)
                {
                    //to the left or to the right
                    float sign = Mathf.Sign(roadDataManifest.positions[j + 1].x - roadDataManifest.positions[j].x);

                    //current obstacles start from previous endPosition
                    startPosition = endPosition;

                    //just half of zigzag intensity for the two first obstacles
                    if (j == i || j == endZigzagIndex - 1)
                    {
                        endPosition = startPosition + Vector2.up * averageDistance + Vector2.right * (zigzagIntensity / 2 * sign + averageXOffset);
                    }
                    else
                    {
                        endPosition = startPosition + Vector2.up * averageDistance + Vector2.right * (zigzagIntensity * sign + averageXOffset);
                    }

                    SpawnParallelObstacles(startPosition, endPosition, currentPathExtent);

                    currentPathExtent -= decreaseRate;
                }

                i = endZigzagIndex - 1;

            }
            else
            {
                SpawnParallelObstacles(roadDataManifest.positions[i], roadDataManifest.positions[i + 1], currentPathExtent);
                currentPathExtent -= decreaseRate;
            }
        }

    }

    public void SpawnParallelObstacles(Vector2 startPosition, Vector2 endPosition, float extent)
    {
        //Left obstacle
        Vector2 endLeftPoint = endPosition + Vector2.left * extent;
        Vector2 startLeftPoint = startPosition + Vector2.left * extent;
        SpawnObstacle(startLeftPoint, endLeftPoint);

        //Right obstacle
        Vector2 endRightPoint = endPosition + Vector2.right * extent;
        Vector2 startRightPoint = startPosition + Vector2.right * extent;
        SpawnObstacle(startRightPoint, endRightPoint);

    }

    public void SpawnObstacle(Vector2 startPosition, Vector2 endPosition)
    {
        Vector2 spawnPosition = (startPosition + endPosition) / 2;
        Vector2 direction = endPosition - startPosition;
        float length = direction.magnitude;

        Transform newObstacle = Instantiate(obstacle, spawnPosition, QuaternionExtension.RotateToDirection(direction, -90)).transform;
        newObstacle.localScale = new Vector3(newObstacle.localScale.x, length, newObstacle.localScale.z);
    }

    //check zigzag pattern from an index and return the other end index of the zigzag 
    public int CheckZigZagPattern(int index)
    {
        //int zigzagPatternCount = 0;
        //int totalZigZagPointCount = 0;

        int endIndex = index;

        if (index + zigzagCheckingMinCount < roadDataManifest.positions.Length)
        {
            int zigzagPointCount = 0;
            int previousMovingDirection = 0;
            int i = index;

            while (i < roadDataManifest.positions.Length - 1)
            {
                // check distance between 2 points && check if current moving to left and previous move to the right or the opposite
                if ((roadDataManifest.positions[i + 1].y - roadDataManifest.positions[i].y <= zigzagCheckingMaxDistance) &&
                    ((roadDataManifest.positions[i].x != roadDataManifest.positions[i + 1].x && previousMovingDirection == 0) ||
                    ((roadDataManifest.positions[i].x > roadDataManifest.positions[i + 1].x) && previousMovingDirection == 1) ||
                    ((roadDataManifest.positions[i].x < roadDataManifest.positions[i + 1].x) && previousMovingDirection == -1)))
                {
                    if (roadDataManifest.positions[i].x < roadDataManifest.positions[i + 1].x)
                    {
                        previousMovingDirection = 1;
                    }
                    else
                    {
                        previousMovingDirection = -1;
                    }

                    zigzagPointCount++;
                    //totalZigZagPointCount++;
                }
                else
                {
                    //zigzag
                    if (zigzagPointCount >= zigzagCheckingMinCount)
                    {
                        endIndex = i;
                    }
                    break;
                }

                i++;

                //zigzag
                if (i == roadDataManifest.positions.Length - 1)
                {
                    endIndex = i;
                }
            }

        }

        return endIndex;
    }

}

[System.Serializable]
public class DifficultyManager
{
    public float initialPathExtent = 3.5f;
    public float minPathExtent = 2.5f;
}