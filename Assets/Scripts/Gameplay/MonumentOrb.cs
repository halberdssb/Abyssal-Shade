using Cinemachine;
using DG.Tweening;
using System;
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
    private Material ghostMaterial;
    [SerializeField]
    public float visualTweenTime;
    [SerializeField]
    private float emissiveIntensity;
    [SerializeField]
    private AudioSource restoredSound;

    private MeshRenderer mesh;
    private Material restoredMat;

    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        restoredMat = mesh.material;
        mesh.material = ghostMaterial;

        //SetOrbEmissiveness(0f);
    }

    public void AreaRestoredEffect()
    {
        restoredSound.Play();

        DOVirtual.Float(0, 2, visualTweenTime, (innerWeight) =>
        {
            mesh.material.SetFloat("_Inner_Spread", innerWeight);
        });
        DOVirtual.Color(Color.white, restoredMat.color, visualTweenTime, (color) =>
        {
            mesh.material.SetColor("_Inner_Color", color);
        }).onComplete = () =>
        {
            mesh.material = restoredMat;
            DOVirtual.Float(0, emissiveIntensity, visualTweenTime, (emissivenessVal) =>
            {
                SetOrbEmissiveness(emissivenessVal);
            }).SetEase(Ease.InExpo);
        };
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
