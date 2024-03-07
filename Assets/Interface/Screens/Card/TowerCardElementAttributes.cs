using UnityEngine;
using UnityEngine.UIElements;

namespace Interface.Elements
{
    public partial class TowerCardElement : CustomElement
    {
        /* --- values --- */

        private string towerName;
        private string towerDescription;
        private int towerCost;
        private int towerType;

        /* --- Properties --- */

        public string TowerName
        {
            get => this.towerName;
            set
            {
                this.towerName = value;
                this.nameLabel?.SetText(value);
            }
        }

        public string TowerDescription
        {
            get => this.towerDescription;
            set
            {
                this.towerDescription = value;
                this.descriptionLabel?.SetText(value);
            }
        }

        public int TowerCost
        {
            get => this.towerCost;
            set
            {
                this.towerCost = value;
                this.costLabel?.SetText(value.ToString());
            }
        }

        public int TowerType
        {
            get => this.towerType;
            set
            {
                this.towerType = value;
            }
        }
    }
}