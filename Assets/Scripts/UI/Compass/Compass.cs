using System;
using System.Collections.Generic;
using UnityEngine;

public class Compass : MonoBehaviour
{
    [SerializeField] private float angleViewCompass;
    [SerializeField] private Point[] cardinalPoints = new Point[4];

    private readonly float maxValue = 180f;

    private Player player;

    private readonly List<Point> pointOnCompass = new List<Point>();

    [SerializeField] private string suffixePoint;

    public float AngleViewCompass
    {
        get { return angleViewCompass; }
    }

    public float MaxValue
    {
        get { return maxValue; }
    }

    public void Init(Player _player)
    {
        player = _player;
    }

    private void LateUpdate()
    {
        ComputeCardinalPoints();

        foreach (Point point in pointOnCompass)
        {
            point.UpdatePosition(CalculateAngle(point.objTransform.position - player.transform.position));

            if (point.IsInsideCompass())
                point.gameObject.SetActive(true);
            else
                point.gameObject.SetActive(false);
        }
    }

    public void AddPoint(GameObject obj)
    {
        if (!pointOnCompass.Exists(point => point.instanceID == obj.GetInstanceID()))
        {
            Point prefabPoint = Resources.Load<Point>("Prefabs/Compass/" + obj.name + suffixePoint);
            Point newPoint = Instantiate(prefabPoint, transform.position, Quaternion.identity);

            newPoint.transform.SetParent(transform);
            newPoint.instanceID = obj.GetInstanceID();
            newPoint.objTransform = obj.transform;

            pointOnCompass.Add(newPoint);
        }
    }

    public void RemovePoint(GameObject obj)
    {
        foreach (Point point in pointOnCompass)
            if (point.instanceID == obj.GetInstanceID())
            {
                pointOnCompass.Remove(point);
                Destroy(point.gameObject);
                return;
            }
    }

    public bool EnemyPointOnCompass()
    {
        foreach (Point point in pointOnCompass)
            if (point.objTransform.tag == "Enemy")
                return true;

        return false;
    }

    private float CalculateAngle(Vector3 directionPlayerPoint)
    {
        directionPlayerPoint.y = 0f;
        float angle = Vector3.Angle(player.transform.forward, directionPlayerPoint);

        Vector3 cross = Vector3.Cross(player.transform.forward, directionPlayerPoint);
        if (cross.y < 0)
            angle = -angle;

        return angle;
    }

    private void ComputeCardinalPoints()
    {
        cardinalPoints[0].UpdatePosition(CalculateAngle(Vector3.forward));
        cardinalPoints[1].UpdatePosition(CalculateAngle(Vector3.right));
        cardinalPoints[2].UpdatePosition(CalculateAngle(Vector3.back));
        cardinalPoints[3].UpdatePosition(CalculateAngle(Vector3.left));

        foreach (Point point in cardinalPoints)
            if (point.IsInsideCompass())
                point.gameObject.SetActive(true);
            else
                point.gameObject.SetActive(false);
    }

    private void OnValidate()
    {
        if (cardinalPoints.Length != 4)
            Array.Resize(ref cardinalPoints, 4);
    }
}