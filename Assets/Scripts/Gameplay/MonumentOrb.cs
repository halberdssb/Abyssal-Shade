using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

[RequireComponent(typeof(MeshRenderer))]
public class MonumentOrb : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera cutsceneCam;
    [SerializeField]
    public float visualTweenTime;
    [SerializeField]
    private float emissiveIntensity;

    private MeshRenderer mesh;

    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshRenderer>();

        SetOrbEmissiveness(0f);
    }

    public void AreaRestoredVisuals()
    {
        DOVirtual.Float(0, emissiveIntensity, visualTweenTime, (emissivenessVal) =>
        {
            SetOrbEmissiveness(emissivenessVal);
        }).SetEase(Ease.InExpo);
    }

    public void SetCameraPriority(int priority)
    {
        cutsceneCam.Priority = priority;
    }

    private void SetOrbEmissiveness(float emissiveness)
    {
        HDMaterial.SetEmissiveIntensity(mesh.material, emissiveness, UnityEditor.Rendering.HighDefinition.EmissiveIntensityUnit.Nits);
    }
}
