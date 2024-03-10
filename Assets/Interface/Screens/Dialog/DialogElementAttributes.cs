
namespace Interface.Elements
{
    public partial class DialogElement : CustomElement
    {
        /* --- values --- */

        private string dialogText;
        private string buttonText;

        /* --- Properties --- */

        public string DialogText
        {
            get => this.dialogText;
            set
            {
                this.dialogText = value;
                this.dialogLabel.SetText(value);
            }
        }

        public string ButtonText
        {
            get => this.buttonText;
            set
            {
                this.buttonText = value;
                this.dialogButton.Text = value;
            }
        }
    }
}