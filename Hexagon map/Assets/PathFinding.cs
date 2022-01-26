using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PathFinding
{
    

    //the A* search algorithm
    public bool AStarSearch(HexGrid hexGrid, Hex start, Hex goal, out List<Hex> rout, float heightStep, out Dictionary<Hex, Hex> visited)
    {
        Dictionary<Hex, Hex> VisitedHexs = new Dictionary<Hex, Hex>();
        Dictionary<Hex, Hex> cameFrom = new Dictionary<Hex, Hex>();
        Dictionary<Hex, float> costSoFar = new Dictionary<Hex, float>();

        HexQueue hexQueue = new HexQueue();

        VisitedHexs.Add(start, start);
        cameFrom.Add(start, start);
        costSoFar.Add(start, 0);
        hexQueue.Add(start, 0);
        int loopCount = 0;
        while (!hexQueue.IsEmpty())
        {
            loopCount++;
            PriorityHex currentPrioHex = hexQueue.GetNext();
            Hex currentHex = currentPrioHex.hex;
            
            if (currentHex == goal) { Debug.Log("reached goal"); break; }

            foreach (Hex nextHex in hexGrid.GetNeighbours(currentHex))
            {
                float heightDiff = Mathf.Abs(nextHex.GetHexCoordinates().GetHeight() - currentHex.GetHexCoordinates().GetHeight());
                if (nextHex.IsTraversable() && heightDiff <= heightStep)
                {
                    float newCost = costSoFar[currentHex] + nextHex.GetMoveDificulty();

                    float costOfNext = 0;
                    if (costSoFar.ContainsKey(nextHex)) { costOfNext = costSoFar[nextHex]; }
                    if (!VisitedHexs.ContainsKey(nextHex) || newCost < costOfNext)
                    {
                        
                        costSoFar[nextHex] = newCost;
                        float priority = newCost + nextHex.DistanceFromHex(goal);
                        hexQueue.Add(nextHex, priority);

                        cameFrom[nextHex] = currentHex;
                        if (!VisitedHexs.ContainsKey(nextHex)) { VisitedHexs.Add(nextHex, nextHex); }
                        
                    }
                }
            }
            

        }
        Debug.Log("a* looped: " + loopCount + " times");
        visited = VisitedHexs;
        //if the goal is unreachable the rout will be null and false will be returned 
        if (!VisitedHexs.ContainsKey(goal)) { Debug.Log("No path available"); rout = null; return false; }
        rout = ReconstructPath(start, goal, cameFrom);
        return true;
    }

    private List<Hex> ReconstructPath(Hex start, Hex goal, Dictionary<Hex, Hex> cameFrom)
    {
        List<Hex> path = new List<Hex>();
        Hex current = goal;
        while(current != start)
        {
            path.Add(current);
            current = cameFrom[current];
        }
        path.Reverse();
        return path;
    }


    private class PriorityHex
    {
        public Hex hex;
        public float priority;

        public PriorityHex(Hex hex, float priority)
        {
            this.hex = hex;
            this.priority = priority;
        }
        public int CompareTo(PriorityHex compareHex)
        {
            if (compareHex == null) { return 1; }
            else { return this.priority.CompareTo(compareHex.priority); }
        }
    }

    private class HexQueue
    {
        private List<PriorityHex> hexQueue = new List<PriorityHex>();

        public void Add(Hex hex, float priority)
        {
            PriorityHex pHex = new PriorityHex(hex, priority);
            hexQueue.Add(pHex);
            Debug.Log("hex added to the hexQueue");
        }
      
        void SortList()
        {
            hexQueue.Sort();
        }

        public PriorityHex GetNext()
        {

            float currentPrio = -1;
            PriorityHex hexReturn = null;
            foreach (PriorityHex ph in hexQueue)
            {
                if(currentPrio < 0)
                {
                    hexReturn = ph;
                    currentPrio = ph.priority;
                }
                else
                {
                    if(currentPrio >= ph.priority)
                    {
                        hexReturn = ph;
                        currentPrio = ph.priority;
                    }
                }
            }

            hexQueue.Remove(hexReturn);
            return hexReturn;
        }

        public bool IsEmpty()
        {
            return hexQueue.Count <= 0;
        }

    }
}
