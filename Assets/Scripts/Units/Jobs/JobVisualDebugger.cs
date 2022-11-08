using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

#if DEBUG

namespace Jobs
{
    public class JobVisualDebugger : MonoBehaviour
    {


        void Update()
        {
            // Purposefully updating every tick to make sure this debugger is never wrong (due to event issues or whatever)
            foreach (var job in JobManager.Instance.GetTakenJobs())
            {
                var unit = job.GetAssignedUnit();
                if(unit != null)
                {
                    var text = unit.gameObject.GetComponentInChildren<Text>();
                    text.text = job.GetCurrentTaskDebugDescription();
                }    
            }
        }
    }
}
#endif

