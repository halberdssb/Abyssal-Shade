using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.HighDefinition;

public class RestorationManager : MonoBehaviour
{
    [System.Serializable]
    public class AreaData
    {
        public RestorationObject[] restorationObjects;
        public MonumentOrb monumentOrb;
        [HideInInspector]
        public delegate void AreaRestoredDel(AreaData area);
        public AreaRestoredDel areaRestoredDel;

        private int numObjectsRestored;

        public void OnObjectRestored()
        {
            Debug.Log("object restored");
            numObjectsRestored++;

            if (numObjectsRestored >= restorationObjects.Length)
            {
                Debug.Log("invoke area restored");
                areaRestoredDel?.Invoke(this);
            }
            else
            {
                Debug.Log("numObjects restored: " +  numObjectsRestored + "\narray length: " + restorationObjects.Length);
            }
        }
    }

    private const float CAM_BLEND_TIME = 2f;

    public AreaData[] areasData;

    private PlayerStateController player;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < areasData.Length; i++)
        {
            foreach(RestorationObject restoreObj in areasData[i].restorationObjects)
            {
                restoreObj.RestoredDelegate += areasData[i].OnObjectRestored;
            }
            areasData[i].areaRestoredDel += OnAreaRestored;
        }

        player = FindObjectOfType<PlayerStateController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnAreaRestored(AreaData area)
    {
        StartCoroutine(AreaRestoredRoutine(area));
    }

    private IEnumerator AreaRestoredRoutine(AreaData area)
    {
        player.blockInput = true;
        area.monumentOrb.SetCameraPriority(20);

        yield return new WaitForSeconds(CAM_BLEND_TIME);

        area.monumentOrb.AreaRestoredEffect();

        yield return new WaitForSeconds(area.monumentOrb.visualTweenTime + 1f);

        area.monumentOrb.SetCameraPriority(-1);

        yield return new WaitForSeconds(CAM_BLEND_TIME);

        player.blockInput = false;
    }
}
