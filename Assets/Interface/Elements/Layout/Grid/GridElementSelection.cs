using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Interface.Elements
{
    public class GridElementSelection<TElement, TData> where TElement : IGridEntryElement<TData> where TData : GridEntryData
    {
        /* --- References --- */

        private GridElement grid;
        private GridElementWrapper<TElement, TData> wrapper;

        /* --- Values --- */

        private List<string> selection = new();

        /* --- Properties --- */

        /// <summary>
        /// Gets called whenever a entry is selected. The first parameter contains the relevant entry, 
        /// the second one whether the entry has been selected or deselected
        /// </summary>
        public event Action<GridEntry<TElement, TData>, bool> EntrySelectedCallback;

        /* --- Methods --- */

        public void SetReferences(GridElement grid, GridElementWrapper<TElement, TData> wrapper)
        {
            this.grid = grid;
            this.wrapper = wrapper;

            // Setup callbacks
            this.wrapper.AfterAddCallback += AfterEntryAdded;
            this.wrapper.AfterAddCallback += AfterEntryAdded;
        }

        private void AfterEntryAdded(GridEntry<TElement, TData> entry)
        {
            entry.Element.VisualElement.UnregisterCallback<ClickEvent, GridEntry<TElement, TData>>(OnEntryClicked);
            entry.Element.VisualElement.RegisterCallback<ClickEvent, GridEntry<TElement, TData>>(OnEntryClicked, entry);
        }

        private void AfterEntryRemoved(GridEntry<TElement, TData> entry)
        {
            entry.Element.VisualElement.UnregisterCallback<ClickEvent, GridEntry<TElement, TData>>(OnEntryClicked);
        }

        private void OnEntryClicked(ClickEvent clickEvent, GridEntry<TElement, TData> entry)
        {
            // If the entry is not selectable, return
            if (!entry.Element.AllowSelection)
            {
                return;
            }

            // If this entry is selected, deselect it
            if (entry.Element.IsSelected)
            {
                if (!this.grid.AllowDeselection)
                {
                    return;
                }

                entry.Element.SetSelected(false);
                this.selection.Remove(entry.Data.Key);
                this.EntrySelectedCallback?.Invoke(entry, false);
                return;
            }

            // Check if any additional entries can be selected, if not deselct a entry if necessary
            if (this.selection.Count >= this.grid.MaxSelectedElements)
            {
                if (this.grid.SelectionOverflowType == SelectionOverflowType.NONE)
                {
                    return;
                }

                if (this.grid.SelectionOverflowType == SelectionOverflowType.DESELECT_FIRST)
                {
                    GridEntry<TElement, TData> firstEntry = this.wrapper.GetEntry(this.selection[0]);
                    firstEntry.Element.SetSelected(false);
                    this.selection.RemoveAt(0);
                    this.EntrySelectedCallback?.Invoke(firstEntry, false);
                }
            }

            // If this entry is not selected, select it
            if (!entry.Element.IsSelected)
            {
                entry.Element.SetSelected(true);
                this.selection.Add(entry.Data.Key);
                this.EntrySelectedCallback?.Invoke(entry, true);
            }
        }

        /// <summary>
        /// Gets the currently selected entries. If none are selected an empty list is returned
        /// </summary>
        /// <returns>The selected entries if there are any</returns>
        public List<GridEntry<TElement, TData>> GetSelection()
        {
            List<GridEntry<TElement, TData>> result = new();
            foreach (string key in this.selection)
            {
                result.Add(this.wrapper.GetEntry(key));
            }

            return result;
        }
    }
}