using AStar;
using Eppy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AStar;

namespace Assets.Scripts.dstarlite
{
    class DstarLite<TPathNode, TUserContext> where TPathNode : IPathNode<TUserContext>
    {
        State s_start;
        State s_goal;
        State s_last;
        PriorityQueue<State> U;
        double k_m;
        Dictionary<State, double> rhs;
        Dictionary<State, double> g;
        //Dictionary of a state to its predecessors or successors
        Dictionary<State, List<State>> Pred;
        Dictionary<State, List<State>> Succ;


        //other
        private State[,] m_SearchSpace;

        public TPathNode[,] SearchSpace { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        //replaces pathnode
        public class State : IIndexedObject, IComparer<State>
        {
            public int X;
            public int Y;
            public Tuple<double, double> k;

            //other
            public static readonly State Comparer = new State(0, 0);

            public static bool operator ==(State s1, State s2)
            {
                return ((s1.X == s2.X) && (s1.Y == s2.Y));
            }

            public static bool operator !=(State s1, State s2)
            {
                return ((s1.X != s2.X) || (s1.Y != s2.Y));
            }

            public static bool operator >(State s1, State s2)
            {
                if (s1.k.Item1 - 0.00001 > s2.k.Item1) return true;
                else if (s1.k.Item1 < s2.k.Item1 - 0.00001) return false;
                return s1.k.Item2 > s2.k.Item2;
            }

            public static bool operator <=(State s1, State s2)
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


            public static bool operator <(State s1, State s2)
            {
                if (s1.k.Item1 + 0.000001 < s2.k.Item1) return true;
                else if (s1.k.Item1 - 0.000001 > s2.k.Item1) return false;
                return s1.k.Item2 < s2.k.Item2;
            }

            public int Index { get; set; }

            public int Compare(State s1, State s2)
            {
                if (s1 < s2)
                {
                    return -1;
                }
                else if (s1 > s2)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }

            public State(int x, int y)
            {
                X = x;
                Y = y;
            }

            //Create dummy state for comparisons.
            public State(Tuple<double, double> n_k)
            {
                X = -1;
                Y = -1;
                k = n_k;
            }
        }

        public DstarLite(TPathNode[,] inGrid)
        {
            Width = inGrid.GetLength(0);
            Height = inGrid.GetLength(1);
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (inGrid[x, y] == null)
                        throw new ArgumentNullException();

                    m_SearchSpace[x, y] = new State(x, y);
                }
            }


        }

        public void RunDstarLite(State start, State goal)
        {
            //Not in pseudo code
            s_start = start;
            s_goal = goal;
            //Begin psuedo code here
            //s_last = s_start
            s_last = s_start;
            Initialize();
            //ComputeShortestPath()
            ComputeShortestPath();
        }
        
        public void Initialize()
        {
            //U = empty
            U = new PriorityQueue<State>();
            //km = 0
            k_m = 0;
            //for all s element S rhs(s) = g(s) = infinity
            for(int i = 0; i < Width; i++)
            {
                for(int j = 0; j < Height; j++)
                {
                    rhs[m_SearchSpace[i,j]] = double.PositiveInfinity;
                    g[m_SearchSpace[i, j]] = double.PositiveInfinity;
                }
            }
            //rhs(s_goal) = 0
            rhs[s_goal] = 0;
            //U.Insert(s_goal, CalculateKey(s_goal)
            s_goal.k = CalculateKey(s_goal);
            U.Push(s_goal);
        }

        public void ComputeShortestPath()
        {
            Tuple<double, double> k_old = new Tuple<double,double>(0,0);
            //while (U.TopKey() < CalculateKey(s_start) || rhs(s_start) != g(s_start))
            //I create a dummy state to compare keys, since compararer is implemented there.
            while (U.Peek() < new State(CalculateKey(s_start)) || rhs[s_start] != g[s_start])
            {
                //k_old = U.TopKey()
                k_old = U.Peek().k;
                //u = U.Pop()
                State u = U.Pop();
                //if (k_old < CalculateKey(u)
                //Create dummy state to compare keys
                if (new State(k_old) < new State(CalculateKey(u)))
                {
                    //U.Insert(u, CalculateKey(u))
                    //Have to insert state u with key value of CalculateKey(u), so update u's k value
                    u.k = CalculateKey(u);
                    U.Push(u);
                }
                //else if (g(u) > rhs(u))
                else if (g[u] > rhs[u])
                {
                    //g(u) = rhs(u)
                    g[u] = rhs[u];
                    //for all s element Pred(u) UpdateVertex(s)
                    foreach(State s in Pred[u])
                    {
                        UpdateVertex(s);
                    }
                }
                //else
                else
                {
                    //g(u) = infinity
                    g[u] = double.PositiveInfinity;
                    //For all s element of Pred(u) Union {u} UpdateVertex(s)
                    List<State> u_Pred = Pred[u];
                    u_Pred.Add(u);
                    foreach(State s in u_Pred)
                    {
                        UpdateVertex(s);
                    }
                }
            }
        }

        public void UpdateVertex(State s)
        {

        }

        public Tuple<double, double> CalculateKey(State s)
        {
            //return [ min( g(s), rhs(s) ) + h(s_start, s) + k_m ; min( g(s), rhs(s) ) ]
            return new Tuple<double,double>( (Math.Min(g[s], rhs[s]) + h(s_start, s) + k_m), Math.Min(g[s], rhs[s]) );
        }

        //Todo
        //Euclid distance for now
        public double h(State s, State goal)
        {
            return Math.Sqrt((s.X - goal.X) * (s.X - goal.X) + (s.Y - goal.Y) * (s.Y - goal.Y));
        }
    }


    /*
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
    }*/
}

    
    
