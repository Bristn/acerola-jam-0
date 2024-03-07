using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UIElements;

namespace Interface.Elements
{
    /// <summary>
    /// A ScriptableObject singleton to lookup VisualTreeAssets from within the custom ui toolkit elements.
    /// Custom elements use the static references to hookup the cs logic with the uxml file.
    /// The CustomElementLookup asset has to be in the root of the Resource folder.
    /// </summary>
    [CreateAssetMenu(fileName = "CustomElementLookup", menuName = "Utility/Scriptable Objects/CustomElementLookup")]
    public class CustomElementLookup : ScriptableObject
    {
        /* --- Singleton --- */

        private static CustomElementLookup _instance;
        public static CustomElementLookup GetInstance()
        {
            if (_instance == null)
            {
                _instance = (CustomElementLookup)Resources.Load(typeof(CustomElementLookup).Name);
            }

            return _instance;
        }

        /* --- Spacers --- */

        [BoxGroup("Spacers")] public VisualTreeAsset VerticalSpacerElement;
        [BoxGroup("Spacers")] public VisualTreeAsset HorizontalSpacerElement;

        /* --- Controls --- */

        [BoxGroup("Controls")] public VisualTreeAsset ButtonElement;
        [BoxGroup("Controls")] public VisualTreeAsset IconElement;
        [BoxGroup("Controls")] public VisualTreeAsset LinearProgressBarElement;

        /* --- Misc --- */

        [BoxGroup("Misc")] public VisualTreeAsset TowerCardElement;
    }
}