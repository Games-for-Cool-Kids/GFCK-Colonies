namespace PathFinding
{
    public class Heuristics
    {
        //Node's costs for pathfinding purposes
        public float hCost = 0;
        public float gCost = 0;
        
        public float fCost
        {
            get //the fCost is the gCost+hCost so we can get it directly this way
            {
                return gCost + hCost;
            }
        }
    }
}
