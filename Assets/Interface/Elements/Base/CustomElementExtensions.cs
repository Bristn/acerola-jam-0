using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace Interface.Elements
{
    public static class CustomElementExtensions
    {
        // ----- ----- ----- ----- ----- Labels

        /// <summary>
        /// Sets the text of a label. Using this method allows the usage of null tenary operatiror (label?.SetText)
        /// </summary>
        /// <param name="element">The label element</param>
        /// <param name="text">The text to set</param>
        public static void SetText(this Label element, string text)
        {
            if (element != null)
            {
                element.text = text;
            }
        }

        /// <summary>
        /// Sets the display of a element. Using this method allows the usage of null tenary operatiror (label?.SetText)
        /// </summary>
        /// <param name="element">The element</param>
        /// <param name="style">The text to set</param>
        public static void SetDisplayStyle(this VisualElement element, DisplayStyle style)
        {
            if (element != null)
            {
                element.style.display = style;
            }
        }

        // ----- ----- ----- ----- ----- Misc

        /// <summary>
        /// Sets all padding values for the given element.
        /// The order of values is: Top, Right, Bottom, Left
        /// </summary>
        /// <param name="element">The element</param>
        /// <param name="margin">The new padding value</param>
        public static void SetPadding(this VisualElement element, Vector4 padding)
        {
            if (element == null)
            {
                return;
            }

            element.style.paddingTop = padding.x;
            element.style.paddingRight = padding.y;
            element.style.paddingBottom = padding.z;
            element.style.paddingLeft = padding.w;
        }

        /// <summary>
        /// Sets all margin values for the given element.
        /// The order of values is: Top, Right, Bottom, Left
        /// </summary>
        /// <param name="element">The element</param>
        /// <param name="margin">The new margin value</param>
        public static void SetMargin(this VisualElement element, Vector4 margin)
        {
            if (element == null)
            {
                return;
            }

            element.style.marginTop = margin.x;
            element.style.marginRight = margin.y;
            element.style.marginBottom = margin.z;
            element.style.marginLeft = margin.w;
        }

        /// <summary>
        /// Gets the root element of the whole visual tree by calling this method recursively
        /// </summary>
        /// <param name="element">The origin element to start with</param>
        /// <returns>The root VisualElement</returns>
        public static VisualElement GetRootElement(this VisualElement element)
        {
            if (element.parent == null)
            {
                return element;
            }

            return element.parent.GetRootElement();
        }

        /// <summary>
        /// Gets the inspector name of the given enum value. Is used for drawing buttons of custom enum dropdown
        /// Source: https://forum.unity.com/threads/how-to-display-an-enum-in-ui-with-spaces.462961/
        /// </summary>
        /// <param name="element">The enum value</param>
        /// <returns>The inspector name. If no inspector name is present, the default enum value is used.</returns>
        public static string GetInspectorName(this Enum element)
        {
            string name = element.ToString();
            Type type = element.GetType();
            FieldInfo field = type.GetField(name);
            InspectorNameAttribute attribute = Attribute.GetCustomAttribute(field, typeof(InspectorNameAttribute)) as InspectorNameAttribute;

            if (attribute != null)
            {
                return attribute.displayName;
            }

            return name;
        }

        // ----- ----- ----- ----- ----- Style classes

        /// <summary>
        /// Sets the text color style based on the background of the element
        /// </summary>
        /// <param name="element">The element to add the class to</param>
        /// <param name="background">The background color</param>
        public static void SetTextColorOnBackground(this VisualElement element, BackgroundColor.Color background)
        {
            TextColor.OnBackgroundClasses.TryGetValue(background, out TextColor.Color textColor);
            element.SetCustomStyle(TextColor.Instance, textColor);
        }

        /// <summary>
        /// Removes all style classes from the given enum. Ensures only one of the enum values is present on the element
        /// </summary>
        /// <param name="element">The element to add the class to</param>
        /// <param name="style">The custom style defining the actual enum to class mappings</param>
        /// <param name="value">The class to add to the element</param>
        public static void SetCustomStyle<T>(this VisualElement element, CustomStyle<T> style, T value) where T : System.Enum
        {
            element.RemoveAllCustomStyles(style);
            element.AddCustomStyle(style, value);
        }

        /// <summary>
        /// Adds a additional style class using the given enum.
        /// </summary>
        /// <param name="element">The element to add the class to</param>
        /// <param name="style">The custom style defining the actual enum to class mappings</param>
        /// <param name="value">The class to add to the element</param>
        public static void AddCustomStyle<T>(this VisualElement element, CustomStyle<T> style, T value) where T : System.Enum
        {
            style.GetStyleClasses().TryGetValue(value, out string styleClass);

            if (element is CustomElement customElement)
            {
                customElement.AddToClassList(styleClass);
            }
            else
            {
                element.AddToClassList(styleClass);
            }
        }

        /// <summary>
        /// Removes a style class using the given enum.
        /// </summary>
        /// <param name="element">The element to add the class to</param>
        /// <param name="style">The custom style defining the actual enum to class mappings</param>
        /// <param name="value">The class to remove from the element</param>
        public static void RemoveCustomStyle<T>(this VisualElement element, CustomStyle<T> style, T value) where T : System.Enum
        {
            style.GetStyleClasses().TryGetValue(value, out string styleClass);

            if (element is CustomElement customElement)
            {
                customElement.RemoveFromClassList(styleClass);
            }
            else
            {
                element.RemoveFromClassList(styleClass);
            }
        }

        /// <summary>
        /// Removes all style classes associated with teh given enum
        /// </summary>
        /// <param name="element">The element to remove the classes from</param>
        /// <param name="style">The custom style defining the actual enum to class mappings</param>
        public static void RemoveAllCustomStyles<T>(this VisualElement element, CustomStyle<T> style) where T : System.Enum
        {
            bool isCustomElement = element is CustomElement;
            CustomElement customElement = isCustomElement ? element as CustomElement : null;

            System.Array values = System.Enum.GetValues(typeof(T));
            foreach (var value in values)
            {
                style.GetStyleClasses().TryGetValue((T)value, out string styleClass);

                if (isCustomElement)
                {
                    customElement.RemoveFromClassList(styleClass);
                }
                else
                {
                    element.RemoveFromClassList(styleClass);
                }
            }
        }

        /// <summary>
        /// Add/Remove the pseudo state based on the boolean value. This is a more generic and limited
        /// version of the CustomElement counterpart. The pseudo states of regular VisualElements
        /// is not stored in a variable & newly added classes don't add the current state variatios automatically
        /// </summary>
        /// <param name="element">The current VisualElement</param>
        /// <param name="state">The state to modify</param>
        /// <param name="active">True if the state should be added. False to remove the state</param>
        public static void SetPseudoState(this VisualElement element, CustomPseudoStates.States state, bool active)
        {
            bool hasState = CustomPseudoStates.ClassSuffix.TryGetValue(state, out string suffix);
            if (!hasState)
            {
                return;
            }

            bool isCustomElement = element is CustomElement;
            CustomElement customElement = isCustomElement ? element as CustomElement : null;

            List<string> styleClasses = element.GetClasses().ToList();
            foreach (string styleClass in styleClasses)
            {
                // Handles removing the style class
                if (!active && styleClass.EndsWith(suffix))
                {
                    if (isCustomElement)
                    {
                        customElement.RemoveFromClassList(styleClass);
                    }
                    else
                    {
                        element.RemoveFromClassList(styleClass);
                    }
                }

                // Handles adding the style class
                if (active && !styleClass.EndsWith(suffix) && !element.ClassListContains(styleClass + suffix))
                {
                    if (isCustomElement)
                    {
                        customElement.AddToClassList(styleClass + suffix);
                    }
                    else
                    {
                        element.AddToClassList(styleClass + suffix);
                    }
                }
            }
        }
    }
}