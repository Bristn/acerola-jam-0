using UnityEngine.UIElements;

namespace Interface.Elements
{
    public interface IGridEntryElement<T> where T : GridEntryData
    {
        public CustomElement VisualElement { get; }
        public bool AllowSelection { get; }
        public bool IsSelected { get; }

        public void SetVisualsFromData(T data);
        public void SetSelected(bool selected);
    }
}