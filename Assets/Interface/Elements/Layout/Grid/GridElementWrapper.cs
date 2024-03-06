

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Interface.Elements
{
    public class GridElementWrapper<TElement, TData> where TElement : IGridEntryElement<TData> where TData : GridEntryData
    {
        /* --- References --- */

        private GridElement grid;
        private GridElementSelection<TElement, TData> selection;

        /// <summary>
        /// A separate map to allow for O(1) access of any element.
        /// This collection is not used to display the elements.
        /// </summary>
        private Dictionary<string, GridEntry<TElement, TData>> entryMapping = new();

        /// <summary>
        /// Contains all visible entries as a list.
        /// The entries are going to be rendered in the order of this list.
        /// </summary>
        private List<GridEntry<TElement, TData>> visibleEntries = new();

        /// <summary>
        /// A list containing all invisible, but still loaded entries.
        /// Used to add new elements without having to instantiate new objects
        /// </summary>
        private List<GridEntry<TElement, TData>> pooledEntries = new();

        private Func<GridEntry<TElement, TData>> getEmptyElement;

        /* --- Properties --- */

        public event Action<GridEntry<TElement, TData>> AfterAddCallback;
        public event Action<GridEntry<TElement, TData>> AfterRemoveCallback;
        public List<GridEntry<TElement, TData>> Entries => this.visibleEntries;

        /* --- Methods --- */

        public void SetReferences(GridElement grid, GridElementSelection<TElement, TData> selection, Func<GridEntry<TElement, TData>> getEmptyElement)
        {
            this.grid = grid;
            this.selection = selection;
            this.getEmptyElement = getEmptyElement;
        }

        /// <summary>
        /// Adds the data to the grid. If the entry is already present, the existing element is updated instead.
        /// </summary>
        /// <param name="data"></param>
        public void AddEntry(TData data)
        {
            GridEntry<TElement, TData> entry;

            // If this entry is already present, only update the visuals
            if (this.entryMapping.ContainsKey(data.Key))
            {
                entry = this.UpdatEntry(data);
                this.AfterAddCallback?.Invoke(entry);
                return;
            }

            // Otherwise check if a pooled entry may be used
            // TODO: Pooling does not work properly. If 2 entries are added with only one pooled element, both entries use the same element --> First element is not displayed
            if (this.pooledEntries.Count != 0 && false)
            {
                entry = this.AddEntryUsingPool(data);
                this.AfterAddCallback?.Invoke(entry);
                return;
            }

            // Otherwise create a new visualiser
            entry = this.AddNewEntry(data);
            this.AfterAddCallback?.Invoke(entry);
        }

        /// <summary>
        /// Updates the entry related to the given data 
        /// </summary>
        /// <param name="data">The entry data</param>
        private GridEntry<TElement, TData> UpdatEntry(TData data)
        {
            Debug.Log("GridElementWrapper: Update entry for key " + data.Key);
            this.entryMapping.TryGetValue(data.Key, out GridEntry<TElement, TData> mapping);
            mapping.Data = data;
            mapping.Element.SetVisualsFromData(data);
            return mapping;
        }

        /// <summary>
        /// Uses a pooled entry to display the new data
        /// </summary>
        /// <param name="data">The entry data</param>
        private GridEntry<TElement, TData> AddEntryUsingPool(TData data)
        {
            Debug.Log("GridElementWrapper: Use pooled entry for key " + data.Key);
            GridEntry<TElement, TData> pooled = this.pooledEntries[0];
            this.pooledEntries.RemoveAt(0);

            // Update visuals & add it to the hierachy
            pooled.Data = data;
            pooled.Element.SetVisualsFromData(data);
            this.grid.Add(pooled.Element.VisualElement);

            // Update the collections
            this.visibleEntries.Add(pooled);
            this.entryMapping.Add(data.Key, pooled);
            return pooled;
        }

        /// <summary>
        /// Creates a new element using the given data
        /// </summary>
        /// <param name="data">The entry data</param>
        private GridEntry<TElement, TData> AddNewEntry(TData data)
        {
            Debug.Log("GridElementWrapper: Instantiate new entry for key " + data.Key);
            GridEntry<TElement, TData> entry = this.getEmptyElement();
            entry.Data = data;
            entry.Element.SetVisualsFromData(data);
            this.grid.Add(entry.Element.VisualElement);

            // Update the collections
            this.visibleEntries.Add(entry);
            this.entryMapping.Add(data.Key, entry);
            return entry;
        }

        /// <summary>
        /// Removes all entries. The elements are pooled to be used again later
        /// </summary>
        public void RemoveAllEntries()
        {
            List<string> keys = this.entryMapping.Keys.ToList();
            for (int i = 0; i < keys.Count; i++)
            {
                this.RemoveEntry(keys[i]);
            }
        }

        /// <summary>
        /// Removes the entry from the ui. The elements are pooled to be used again later
        /// </summary>
        /// <param name="key"></param>
        public void RemoveEntry(string key)
        {
            if (!this.entryMapping.ContainsKey(key))
            {
                Debug.Log("GridElementWrapper: Key " + key + " cannot be removed, as there is no entry");
                return;
            }

            // Hide the entry 
            this.entryMapping.TryGetValue(key, out GridEntry<TElement, TData> entry);
            this.grid.Remove(entry.Element.VisualElement);

            // Update the lists
            this.entryMapping.Remove(key);
            this.visibleEntries.Remove(entry);
            this.pooledEntries.Add(entry);
            this.AfterRemoveCallback?.Invoke(entry);
        }

        /// <summary>
        /// Gets the entry with the given key. If there is no entry, null is returned
        /// </summary>
        /// <param name="key">The unique data key</param>
        /// <returns>The GridEntry object</returns>
        public GridEntry<TElement, TData> GetEntry(string key)
        {
            if (!this.entryMapping.ContainsKey(key))
            {
                return null;
            }

            this.entryMapping.TryGetValue(key, out GridEntry<TElement, TData> entry);
            return entry;
        }
    }
}