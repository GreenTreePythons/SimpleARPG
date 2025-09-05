using System;
using System.Collections.Generic;
using UnityEngine;

public class GrassController : MonoBehaviour
{
    private Material m_GrassMaterial;
    private MeshRenderer m_MeshRenderer;
    
    private bool m_IsEmptyList;
    private float m_Pos;
    private List<Transform> m_GrassPoints = new();

    private void Awake()
    {
        m_MeshRenderer = GetComponent<MeshRenderer>();
        m_GrassMaterial = m_MeshRenderer.material;
    }

    private void FixedUpdate()
    {
        if (m_GrassPoints.Count <= 0)
        {
            if (!m_IsEmptyList)
            {
                Vector3 currentGrassMatPos = m_GrassMaterial.GetVector("_Pos");
                m_GrassMaterial.SetVector("_Pos", currentGrassMatPos + new Vector3(0f,m_Pos, 0f));
                m_Pos += 0.0025f;
            }

            return;
        }

        if (m_GrassPoints[m_GrassPoints.Count - 1] == null) return;
        m_GrassMaterial.SetVector("_Pos", m_GrassPoints[m_GrassPoints.Count - 1].position);
    }

    private void OnTriggerEnter(Collider other)
    {
        m_GrassPoints.Add(other.transform);
        m_IsEmptyList = false;
        m_Pos = 0f;
    }

    private void OnTriggerExit(Collider other)
    {
        m_GrassPoints.Remove(other.transform);
    }
}