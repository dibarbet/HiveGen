using AStar;
using Eppy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.dstarlite
{
    class DStarLite
    {
        static PriorityQueue<State> ds_pq;
        static Dictionary<State, cellInfo> ds_ch;
        static DStarLite()
        {
            state_hash state_compare = new state_hash();
            ds_pq = new PriorityQueue<State>();
            ds_ch = new Dictionary<State, cellInfo>(state_compare);
        }

        

        int m_MaxSteps = 80000;

    }

    public class State : IIndexedObject
    {
        public int x;
        public int y;
        Tuple<double, double> k;

        public static bool operator == (State s1, State s2)
        {
            return ((s1.x == s2.x) && (s1.y == s2.y));
        }
  
        public static bool operator != (State s1, State s2)
        {
            return ((s1.x != s2.x) || (s1.y != s2.y));
        }
  
        public static bool operator > (State s1, State s2)
        {
            if (s1.k.Item1-0.00001 > s2.k.Item1) return true;
            else if (s1.k.Item1 < s2.k.Item1-0.00001) return false;
            return s1.k.Item2 > s2.k.Item2;
        }

        public static bool operator <= (State s1, State s2)
        {
            if (s1.k.Item1 < s2.k.Item1) return true;
            else if (s1.k.Item1 > s2.k.Item1) return false;
            return s1.k.Item2 < s2.k.Item2 + 0.00001;
        }
        //Should be unused
        public static bool operator >=(State s1, State s2)
        {
            if (s1.k.Item1 > s2.k.Item1) return true;
            else if (s1.k.Item1 < s2.k.Item1) return false;
            return s1.k.Item2 > s2.k.Item2 + 0.00001;
        }


        public static bool operator < (State s1, State s2)
        {
            if (s1.k.Item1 + 0.000001 < s2.k.Item1) return true;
            else if (s1.k.Item1 - 0.000001 > s2.k.Item1) return false;
            return s1.k.Item2 < s2.k.Item2;
        }
    
        public int Index
        {
	          get 
	        { 
		        throw new NotImplementedException(); 
	        }
	          set 
	        { 
		        throw new NotImplementedException(); 
	        }
        }
    }

    struct ipoint2
    {
        int x, y;
    }

    struct cellInfo
    {
        double g;
        double rhs;
        double cost;
    }

    class state_hash : IEqualityComparer<State>
    {
        public int GetHashCode(State s)
        {
 	         return s.x + 34245*s.y;
        }

        public bool Equals(State s1, State s2)
        {
            return ((s1.x == s2.x) && (s1.y == s2.y));
        }
    }
}

    
    
