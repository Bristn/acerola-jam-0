
namespace Pathfinding.Algorithm
{
    public struct PathNode
    {
        public int X;
        public int Y;
        public int Index;
        public bool IsWalkable;
        public int PreviousNodeIndex;

        public int GCost; // Move cust from start node to this node
        public int HCost; // Estimated cost from here to finish
        public int FCost; // Sum of G + H

        public void UpdateFCost()
        {
            this.FCost = this.GCost + this.HCost;
        }
    }
}