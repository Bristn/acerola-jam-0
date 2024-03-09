
namespace Interface.Elements
{
    public partial class StatElement : CustomElement
    {
        /* --- values --- */

        private string value;
        private string icon;

        /* --- Properties --- */

        public string Value
        {
            get => this.value;
            set
            {
                this.value = value;
                this.valueLabel?.SetText(value);
            }
        }

        public string Icon
        {
            get => this.icon;
            set
            {
                this.icon = value;
                this.statIcon.Icon = value;
            }
        }
    }
}