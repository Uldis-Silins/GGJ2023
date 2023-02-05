using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ComicStripController : MonoBehaviour
{
    [System.Serializable]
    public class Waypoint
    {
        public Vector3 position;
        public AnimationCurve curve;
        public float time;
    }

    [SerializeField] private RectTransform m_comicStripTransform;
    [SerializeField] private Waypoint[] m_waypoints;

    private int m_targetWaypointIndex, m_currentWaypointIndex;
    private float m_waypointTimer;

    private void Update()
    {
        if (m_waypointTimer >= m_waypoints[m_currentWaypointIndex].time)
        {
            SetNextWapoint();
        }

        m_comicStripTransform.position = Vector3.Lerp(m_waypoints[m_currentWaypointIndex].position, m_waypoints[m_targetWaypointIndex].position, m_waypoints[m_currentWaypointIndex].curve.Evaluate(m_waypointTimer / m_waypoints[m_currentWaypointIndex].time));

        m_waypointTimer += Time.deltaTime;
    }

    private void SetNextWapoint()
    {
        m_currentWaypointIndex = m_targetWaypointIndex;
        m_targetWaypointIndex++;
        m_waypointTimer = 0f;
    }
}