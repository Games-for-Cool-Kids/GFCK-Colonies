using Jobs;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DebugGUI
{
    internal class DebugGUIJobsWindowContent : DebugGUIWindowContent
    {
        private bool _availableJobsFoldout = false;
        private List<bool> _availableJobsTasksFoldouts = new List<bool>();
        private bool _takenJobsFoldout = false;
        private List<bool> _takenJobsTasksFoldouts = new List<bool>();

        public override void DrawGUI(int windowId)
        {
            GUILayout.BeginVertical();
            // ------------------------

            var availableJobs = JobManager.Instance.GetAvailableJobs();
            _availableJobsTasksFoldouts.Resize(availableJobs.Count);
            if (_availableJobsFoldout = EditorGUILayout.Foldout(_availableJobsFoldout && availableJobs.Count > 0, "Available - " + availableJobs.Count))
            {
                for(int i = 0; i < availableJobs.Count; i++)
                {
                    bool foldout = _availableJobsTasksFoldouts[i];
                    DrawJobGUI(availableJobs[i], ref foldout);
                    _availableJobsTasksFoldouts[i] = foldout;
                }
            }

            var takenJobs = JobManager.Instance.GetTakenJobs();
            _takenJobsTasksFoldouts.Resize(takenJobs.Count);
            if (_takenJobsFoldout = EditorGUILayout.Foldout(_takenJobsFoldout && takenJobs.Count > 0, "Taken - " + takenJobs.Count))
            {
                for (int i = 0; i < takenJobs.Count; i++)
                {
                    bool foldout = _takenJobsTasksFoldouts[i];
                    DrawJobGUI(takenJobs[i], ref foldout);
                    _takenJobsTasksFoldouts[i] = foldout;
                }
            }

            // ------------------------
            GUILayout.EndVertical();
        }

        private static void DrawJobGUI(Job job, ref bool tasksFoldout)
        {
            GUILayout.BeginHorizontal();
            // ------------------------

            GUILayout.Label(job.type.ToString(), GUILayout.MinWidth(80));

            DrawTaskList(job, ref tasksFoldout);

            GUILayout.FlexibleSpace();

            GUILayout.Button(job.building.gameObject.name, GUILayout.MinWidth(85));

            // ------------------------
            GUILayout.EndHorizontal();
        }

        private static void DrawTaskList(Job job, ref bool foldout)
        {
            GUILayout.BeginVertical();
            // ------------------------

            string foldout_label; 

            if (job.currentTask == null)
            {
                foldout_label = "Inactive";
            }
            else 
            {
                foldout_label = string.Format("{0} - {1}/{2}", CleanString(job.currentTask), job.tasks.IndexOf(job.currentTask) + 1, job.tasks.Count);
            }

            if (foldout = EditorGUILayout.Foldout(foldout, foldout_label))
            {
                foreach (var task in job.tasks)
                {
                    if (task == job.currentTask)
                        GUI.skin.label.normal.textColor = Color.yellow;

                    GUILayout.Label(CleanString(task));

                    if (task == job.currentTask)
                        GUI.skin.label.normal.textColor = Color.white;
                }
            }

            // ------------------------
            GUILayout.EndVertical();
        }

        private static string CleanString(Task task)
        {
            string s = task.ToString();
            s = s.Replace("Jobs.", "");
            s = s.Replace("Task", "");
            return s;
        }
    }
}
