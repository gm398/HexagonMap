using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PathFinding
{
    
    //the A* search algorithm
    public bool AStarSearch(HexGrid hexGrid, Hex start, Hex goal, out List<Hex> rout, float heightStep, GameObject unit, out Dictionary<Hex, Hex> visited, LayerMask enemyLayers, int range)
    {
        
        Dictionary<Hex, Hex> VisitedHexs = new Dictionary<Hex, Hex>();
        Dictionary<Hex, Hex> cameFrom = new Dictionary<Hex, Hex>();
        Dictionary<Hex, float> costSoFar = new Dictionary<Hex, float>();
        HexQueue hexQueue = new HexQueue();

        UnitController controller = unit.GetComponentInParent<UnitController>();
        bool canFly = controller.CanFly();

        VisitedHexs.Add(start, start);
        cameFrom.Add(start, start);
        costSoFar.Add(start, 0);
        hexQueue.Add(start, 0);
        int loopCount = 0;
        bool doable = canFly;
        if (!canFly)
        {
            doable = ChooseBetterGoal(hexGrid, start, out goal, goal, unit, 5);
        }
        while (!hexQueue.IsEmpty() && doable)
        {
            loopCount++;
            PriorityHex currentPrioHex = hexQueue.GetNext();
            Hex currentHex = currentPrioHex.hex;
            
            if (currentHex == goal || goal.DistanceFromHex(currentHex) <= range) {
                // Debug.Log("reached goal");
                goal = currentHex;
                break; }

            foreach (Hex nextHex in hexGrid.GetNeighbours(currentHex))
            {
                float heightDiff = Mathf.Abs(nextHex.GetHexCoordinates().GetHeight() - currentHex.GetHexCoordinates().GetHeight());
                if ((nextHex.IsTraversable() || canFly) 
                    && heightDiff <= heightStep 
                    && (unit == null 
                        || nextHex.GetOccupant() == null 
                        || nextHex.GetOccupant().Equals(unit)
                        || (enemyLayers == (enemyLayers & (1 << nextHex.GetOccupant().layer)))
                        )
                    ) 
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
        //Debug.Log("a* looped: " + loopCount + " times");
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
    


    // if the goal is not traversable try to find a traversable hex in its neighbours to reduce hexs checked
    private bool ChooseBetterGoal(HexGrid hexGrid, Hex start, out Hex newGoal, Hex goal, GameObject unit, int attempts)
    {
        if (start == goal) { newGoal = goal; return true; }
        float distance = start.DistanceFromHex(goal);
        bool newHexFound = false;
        bool sucess = false;
        List<Hex> neighbours = hexGrid.GetNeighbours(goal);
        if (!goal.IsTraversable())
        {
            float dis;
            foreach (Hex h in neighbours)
            {
                dis = start.DistanceFromHex(h);
                if (!newHexFound) { if (h.IsTraversable() && !unit.Equals(h.GetOccupant())) { goal = h; newHexFound = true; } }
                else if (h.IsTraversable() && dis < distance && !unit.Equals(h.GetOccupant()))
                {
                    distance = dis;
                    goal = h;
                }
            }
            sucess = newHexFound;
            if (!newHexFound && attempts > 0)
            {
                foreach (Hex h in neighbours)
                {
                    dis = start.DistanceFromHex(h);
                    if (dis < distance) { goal = h; newHexFound = true; }
                }
                if (newHexFound)
                {
                    sucess = ChooseBetterGoal(hexGrid, start, out goal, goal, unit, attempts - 1);
                }
            }
        }
        else { sucess = true; }
        newGoal = goal;
        return sucess;
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
        public float GetPriority() { return priority; }
    }

    private class HexQueue
    {
        private List<PriorityHex> hexQueue = new List<PriorityHex>();

        public void Add(Hex hex, float priority)
        {
            PriorityHex pHex = new PriorityHex(hex, priority);
            hexQueue.Add(pHex);
            //Debug.Log("hex added to the hexQueue");
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
