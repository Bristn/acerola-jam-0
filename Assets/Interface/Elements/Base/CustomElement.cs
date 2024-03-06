using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Interface.Elements
{
    public abstract class CustomElement : VisualElement, IRoundedCornersTarget
    {
        /// <summary>
        /// Override this property to define which visualelement should recieve rounded corners. 
        /// Based on the grid settings, all child elements of a grid will have rounded corners based on their neighbors.
        /// By default the actual CustomElement is returned, which does not have any background, leading to no rounded corners.
        /// </summary>
        public virtual VisualElement RoundedCornersTarget => this;

        /// <summary>
        /// Stores the custom pseudo states which are currently applied to this element
        /// </summary>
        public List<CustomPseudoStates.States> PseudoStates = new();

        /// <summary>
        /// Loads the root element of the VisualAssetTree and adds a clone to the hierachy
        /// </summary>
        /// <param name="source"></param>
        public void BuildVisuals(VisualTreeAsset source)
        {
            if (source == null)
            {
                return;
            }

            // Clear hierachy
            hierarchy.Clear();

            // Add to hierachy
            TemplateContainer asset = source.CloneTree();
            VisualElement root = asset.Children().First();
            hierarchy.Add(root);

            this.UpdateReferences();
        }

        /// <summary>
        /// Called once the hierachy contains all VisualElements.
        /// This is the earliset time queries can be executed.
        /// </summary>
        public abstract void UpdateReferences();

        /// <summary>
        /// Checks if the element has the given pseudo state
        /// </summary>
        /// <param name="state">The state to check for</param>
        /// <returns>True, if the state is present</returns>
        public bool HasPseudoState(CustomPseudoStates.States state) => this.PseudoStates.Contains(state);

        /// <summary>
        /// Add/Remove the pseudo state based on the boolean value.
        /// As this implementation is inside the CustomElement, the pseudo state is appended to the variable
        /// </summary>
        /// <param name="state">The state to modify</param>
        /// <param name="active">True if the state should be added. False to remove the state</param>
        public void SetPseudoState(CustomPseudoStates.States state, bool active)
        {
            ((VisualElement)this).SetPseudoState(state, active);

            if (active)
            {
                this.PseudoStates.Add(state);
            }
            else
            {
                this.PseudoStates.Remove(state);
            }
        }

        /// <summary>
        /// Overrides the default remove behaviour. Removes the style class by calling the base implementation,
        /// but also removes all pseudo states of the given class. Only possible for CustomElements, thus does
        /// not work for regular VisualElements
        /// </summary>
        /// <param name="style">The class to remove</param>
        public new void RemoveFromClassList(string style)
        {
            // Debug.Log("CustomElement: Remove " + style + " from class list");
            base.RemoveFromClassList(style);

            // Additionally remove all custom pseudo classes using thie given class as a base
            foreach (CustomPseudoStates.States state in this.PseudoStates)
            {
                bool hasState = CustomPseudoStates.ClassSuffix.TryGetValue(state, out string suffix);
                if (!hasState)
                {
                    continue;
                }

                if (this.ClassListContains(style + suffix))
                {
                    // Debug.Log("CustomElement: Remove " + style + suffix + " from class list");
                    base.RemoveFromClassList(style + suffix);
                }
            }
        }

        /// <summary>
        /// Overrides the default add behaviour. Adds the style class by calling the base implementation,
        /// but also add all current pseudo state variations of this class. Only works for CustomElements
        /// </summary>
        /// <param name="style">The class to add</param>
        public new void AddToClassList(string style)
        {
            // Debug.Log("CustomElement: Add " + style + " to class list");
            base.AddToClassList(style);

            // Additionally add all currently active pseudo states
            foreach (CustomPseudoStates.States state in this.PseudoStates)
            {
                bool hasState = CustomPseudoStates.ClassSuffix.TryGetValue(state, out string suffix);
                if (!hasState)
                {
                    continue;
                }

                if (!this.ClassListContains(style + suffix))
                {
                    // Debug.Log("CustomElement: Add " + style + suffix + " to class list");
                    base.AddToClassList(style + suffix);
                }
            }
        }
    }
}