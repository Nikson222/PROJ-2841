#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace _Scripts._Infrastructure.MyEditorCustoms
{
    [CustomPropertyDrawer(typeof(SceneAttribute))]
    public class SceneDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                // Получаем список сцен, добавленных в билд
                var scenes = EditorBuildSettings.scenes;
                string[] sceneNames = new string[scenes.Length];

                for (int i = 0; i < scenes.Length; i++)
                {
                    sceneNames[i] = System.IO.Path.GetFileNameWithoutExtension(scenes[i].path);
                }

                // Поиск индекса текущей выбранной сцены
                int selectedIndex = Array.IndexOf(sceneNames, property.stringValue);
                if (selectedIndex == -1)
                {
                    selectedIndex = 0; // Если сцена не найдена, выбираем первую
                }

                // Рисуем выпадающий список в инспекторе
                selectedIndex = EditorGUI.Popup(position, label.text, selectedIndex, sceneNames);
                property.stringValue = sceneNames[selectedIndex]; // Присваиваем выбранное значение
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Use [Scene] with string.");
            }
        }
    }
}
#endif