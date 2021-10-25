using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleScript : MonoBehaviour
{
    public Rigidbody2D hingeRB;

    public List<Vector2> Points;
    Vector2 mainPoint;
    public Vector2 hingePoint;
    Vector2 prevHingePoint;

    public Transform Player;
    Vector2 playerVector1, playerVector2;

    RaycastHit2D hit1, hit2;

    public float distance, totalDistance;

    LineRenderer lr;

    void Start()
    {
        mainPoint = hingePoint = prevHingePoint = hingeRB.position = transform.position;
        Points.Add(mainPoint);
        lr = GetComponent<LineRenderer>();
        totalDistance = Vector2.Distance(hingeRB.position, Player.transform.position);
    }

    void Update()
    {
        GetInfo();
        
        if(hit1)
        {
            if (hit1.transform.CompareTag("Player"))
            {
                if(Points.Count > 2)
                {
                    hingePoint = Points[Points.Count - 2];
                    hingeRB.position = hingePoint;
                    prevHingePoint = Points[Points.Count - 3];
                    Points.RemoveAt(Points.Count - 1);
                    UpdatePlayerDistance();
                }
                else if(Points.Count == 2)
                {
                    hingePoint = prevHingePoint = mainPoint;
                    hingeRB.position = hingePoint;
                    Points.RemoveRange(0, Points.Count);
                    Points.Add(mainPoint);
                    UpdatePlayerDistance();
                }
            }
        }
        if (hit2.transform.CompareTag("Player") == false)
        {
            if(Points.Count == 1)
            {
                hingePoint = FindClosestCorner(hit2.transform.gameObject, hit2.point);
                hingeRB.position = hingePoint;
                if (!Points.Contains(hingePoint))
                {
                    Points.Add(hingePoint);
                }
                UpdatePlayerDistance();
            }
            else
            {
                if (!Points.Contains(hit2.point + hit2.normal * 0.1f))
                {
                    prevHingePoint = hingePoint;
                    hingePoint = FindClosestCorner(hit2.transform.gameObject, hit2.point);
                    hingeRB.position = hingePoint;
                    Points.Add(hingePoint);
                    UpdatePlayerDistance();
                }
            }
        }
        UpdateLineRenderer();
    }

    void GetInfo()
    {
        totalDistance = 0;
        for (int i = 0; i < Points.Count - 1; i++)
            totalDistance += Vector2.Distance(Points[i], Points[i + 1]);
        totalDistance += Vector2.Distance(Points[Points.Count - 1], (Vector2)Player.position);

        playerVector2 = (Vector2)Player.position - hingePoint;
        playerVector1 = (Vector2)Player.position - prevHingePoint;

        hit1 = Physics2D.Raycast(prevHingePoint, playerVector1);
        hit2 = Physics2D.Raycast(hingePoint, playerVector2);
    }

    void UpdateLineRenderer()
    {
        lr.positionCount = Points.Count + 1;
        for (int i = 0; i < Points.Count; i++)
        {
            lr.SetPosition(i, Points[i]);
        }
        lr.SetPosition(Points.Count, Player.position);
    }

    void CalculateDistanceToPlayer()
    {
        distance = totalDistance;
        for (int i = 0; i < Points.Count - 1; i++)
        {
            distance -= Vector2.Distance(Points[i], Points[i + 1]);
        }
    }

    void UpdatePlayerDistance()
    {
        CalculateDistanceToPlayer();
        hingeRB.gameObject.GetComponent<HingeJoint2D>().connectedAnchor = 
            Points[Points.Count - 1] + (Points[Points.Count - 1] - (Vector2)Player.position).normalized * distance;
    }

    Vector2 FindClosestCorner(GameObject box, Vector2 point)
    {
        Vector2 topLeft = (Vector2)box.transform.position + Vector2.left * box.transform.localScale.x / 2 + Vector2.up * box.transform.localScale.y / 2;
        Vector2 topRight = (Vector2)box.transform.position + Vector2.right * box.transform.localScale.x / 2 + Vector2.up * box.transform.localScale.y / 2;
        Vector2 bottomLeft = (Vector2)box.transform.position + Vector2.left * box.transform.localScale.x / 2 + Vector2.down * box.transform.localScale.y / 2;
        Vector2 bottomRight = (Vector2)box.transform.position + Vector2.right * box.transform.localScale.x / 2 + Vector2.down * box.transform.localScale.y / 2;

        float smallestDist = 10f;
        Vector2 smallestPoint = Vector2.zero;

        if (Vector2.Distance(point, topLeft) < smallestDist)
        {
            smallestDist = Vector2.Distance(point, topLeft);
            smallestPoint = topLeft;
            smallestPoint += (Vector2.up + Vector2.left).normalized * 0.025f;
        }
        if (Vector2.Distance(point, topRight) < smallestDist)
        {
            smallestDist = Vector2.Distance(point, topRight);
            smallestPoint = topRight;
            smallestPoint += (Vector2.up + Vector2.right).normalized * 0.025f;
        }
        if (Vector2.Distance(point, bottomLeft) < smallestDist)
        {
            smallestDist = Vector2.Distance(point, bottomLeft);
            smallestPoint = bottomLeft;
            smallestPoint += (Vector2.down + Vector2.left).normalized * 0.025f;
        }
        if (Vector2.Distance(point, bottomRight) < smallestDist)
        {
            smallestDist = Vector2.Distance(point, bottomRight);
            smallestPoint = bottomRight;
            smallestPoint += (Vector2.down + Vector2.right).normalized * 0.025f;
        }

        return smallestPoint;
    }


}
