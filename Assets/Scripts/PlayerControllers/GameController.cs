using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class GameController : MonoBehaviour
{
    [SerializeField] private ARPlaneManager m_arPlaneManager;
    [SerializeField] private PlayerController m_playerPrefab;

    private PlayerController m_spawnedPlayer;

    private void Start()
    {
        m_arPlaneManager.planesChanged += HandlePlanesChanged;
    }

    private void HandlePlanesChanged(ARPlanesChangedEventArgs args)
    {
        if (m_spawnedPlayer == null)
        {
            foreach (var plane in args.updated)
            {
                Vector3 position = plane.center;
                m_spawnedPlayer = Instantiate<PlayerController>(m_playerPrefab, position, Quaternion.identity);
                m_arPlaneManager.planesChanged -= HandlePlanesChanged;
            }
        }
    }
}
