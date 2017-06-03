using UnityEngine;
using UnityEditor;

namespace Demonixis.Toolbox.VR.Editor
{
    [InitializeOnLoad]
    public static class VRInputLoader
    {
        private static bool AllowOverride { get; set; }

        static VRInputLoader()
        {
            BindAxisAndButtons();
        }

        private static void BindAxisAndButtons()
        {
            try
            {
                // Buttons 1, 3 and 7 are only used but Oculus.
                BindAxis("Axis 1", 0);
                BindAxis("Axis 2", 1, true);
                BindAxis("Axis 4", 3);
                BindAxis("Axis 5", 4, true);
                BindAxis("Axis 9", 8);
                BindAxis("Axis 10", 9);
                BindAxis("Axis 11", 10);
                BindAxis("Axis 12", 11);

                BindButton("Button 0", "joystick button 0");
                BindButton("Button 1", "joystick button 1");
                BindButton("Button 2", "joystick button 2");
                BindButton("Button 3", "joystick button 3");
                BindButton("Button 7", "joystick button 7");
                BindButton("Button 8", "joystick button 8");
                BindButton("Button 9", "joystick button 9");
                BindButton("Button 16", "joystick button 16");
                BindButton("Button 17", "joystick button 17");
            }
            catch
            {
                Debug.LogError("Failed to apply VR Input manager bindings");
            }
        }

        [MenuItem("Demonixis/VR/Reset Input Binding", false, 100000)]
        static void ResetInputBinding()
        {
            AllowOverride = true;
            BindAxisAndButtons();
            AllowOverride = false;
        }

        public static void BindButton(string buttonName, string buttonID)
        {
            BindRawAxis(new Axis() { name = buttonName, positiveButton = buttonID, type = 0 });
        }

        public static void BindAxis(string axisName, int axisID, bool invertAxis = false)
        {
            BindRawAxis(new Axis() { name = axisName, axis = axisID, invert = invertAxis });
        }

        private static void BindRawAxis(Axis axis)
        {
            var serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
            var axesProperty = serializedObject.FindProperty("m_Axes");
            var axisIter = axesProperty.Copy();

            axisIter.Next(true);
            axisIter.Next(true);

            while (axisIter.Next(false))
                if (axisIter.FindPropertyRelative("m_Name").stringValue == axis.name && !AllowOverride)
                    return;

            axesProperty.arraySize++;
            serializedObject.ApplyModifiedProperties();

            var axisProperty = axesProperty.GetArrayElementAtIndex(axesProperty.arraySize - 1);
            axisProperty.FindPropertyRelative("m_Name").stringValue = axis.name;
            axisProperty.FindPropertyRelative("descriptiveName").stringValue = axis.descriptiveName;
            axisProperty.FindPropertyRelative("descriptiveNegativeName").stringValue = axis.descriptiveNegativeName;
            axisProperty.FindPropertyRelative("negativeButton").stringValue = axis.negativeButton;
            axisProperty.FindPropertyRelative("positiveButton").stringValue = axis.positiveButton;
            axisProperty.FindPropertyRelative("altNegativeButton").stringValue = axis.altNegativeButton;
            axisProperty.FindPropertyRelative("altPositiveButton").stringValue = axis.altPositiveButton;
            axisProperty.FindPropertyRelative("gravity").floatValue = axis.gravity;
            axisProperty.FindPropertyRelative("dead").floatValue = axis.dead;
            axisProperty.FindPropertyRelative("sensitivity").floatValue = axis.sensitivity;
            axisProperty.FindPropertyRelative("snap").boolValue = axis.snap;
            axisProperty.FindPropertyRelative("invert").boolValue = axis.invert;
            axisProperty.FindPropertyRelative("type").intValue = axis.type;
            axisProperty.FindPropertyRelative("axis").intValue = axis.axis;
            axisProperty.FindPropertyRelative("joyNum").intValue = axis.joyNum;
            serializedObject.ApplyModifiedProperties();
        }

        private class Axis
        {
            public string name = string.Empty;
            public string descriptiveName = string.Empty;
            public string descriptiveNegativeName = string.Empty;
            public string negativeButton = string.Empty;
            public string positiveButton = string.Empty;
            public string altNegativeButton = string.Empty;
            public string altPositiveButton = string.Empty;
            public float gravity = 0.0f;
            public float dead = 0.001f;
            public float sensitivity = 1.0f;
            public bool snap = false;
            public bool invert = false;
            public int type = 2;
            public int axis = 0;
            public int joyNum = 0;
        }
    }
}