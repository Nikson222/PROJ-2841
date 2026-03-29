using System;
using System.Collections.Generic;
using UnityEngine;
using _Scripts._Infrastructure.MyEditorCustoms;
using _Scripts._Infrastructure.UI.Base;

namespace _Scripts._Infrastructure.Configs
{
    [CreateAssetMenu(fileName = "UIPanelConfig", menuName = "UI/UIPanelConfig")]
    public class UIPanelConfig : ScriptableObject
    {
        [Serializable]
        public class PanelEntry
        {
            public PanelType PanelType;
            public GameObject PanelPrefab;
            [Scene] public string SceneName;
            public bool IsInitiallyOpen;
        }

        public List<PanelEntry> PanelEntries = new();
    }
}