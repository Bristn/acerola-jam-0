
using System.Collections.Generic;

namespace Interface.Elements
{
    /// <summary>
    /// Defines custom styles using enums and USS class names.
    /// Allows only a few method to manage all custom style implementation inheriting form this schema
    /// </summary>
    /// <typeparam name="T">The main enum type. Usually a enum storing style or color identifiers</typeparam>
    public abstract class CustomStyle<T> where T : System.Enum
    {
        /// <summary>
        /// Gets a mapping of style enum value to their respective USS style classes.
        /// </summary>
        public abstract Dictionary<T, string> GetStyleClasses();
    }
}