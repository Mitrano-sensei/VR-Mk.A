using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MechRocketLauncher))]
public class MechRocketLauncherEditor : Editor
{
    // Adds a button that calls a method on the target object
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Fire"))
        {
            var mechRocketLauncher = (MechRocketLauncher)target;
            mechRocketLauncher.Fire();
        }
    }

}
