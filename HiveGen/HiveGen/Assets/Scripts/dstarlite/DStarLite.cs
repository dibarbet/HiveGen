using AStar;
using Eppy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.Scripts;

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
        //Dictionary<State, List<State>> Pred;
        //Dictionary<State, List<State>> Succ;
        double C1 = 1;
        public List<State> Path { get; private set; }
        

        //other
        int maxIterations = 80000;
        private State[,] m_SearchSpace;

        public GameManager.SpecialPathNode[,] SearchSpace { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }


        public List<GameManager.SpecialPathNode> GetPath()
        {
            List<GameManager.SpecialPathNode> m_path = new List<GameManager.SpecialPathNode>();
            if (Path == null || Path.Count == 0)
            {
                Debug.Log("Empty dstar path");
                return m_path;
            }
            foreach(State s in Path)
            {
                m_path.Add(SearchSpace[s.X, s.Y]);
            }
            return m_path;
        }

        public class StateComparer : IComparer<State>, IEqualityComparer<State>
        {

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

            public bool Equals(State x, State y)
            {
                //Debug.Log("Comparing states with 2 equals");
                return ((x.X == y.X) && (x.Y == y.Y));
            }

            public int GetHashCode(State obj)
            {
                //Debug.Log("hashing state");
                return obj.X.GetHashCode() + obj.Y.GetHashCode() * 34245;
            }

            int IComparer<State>.Compare(State s1, State s2)
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
        }

        //replaces pathnode
        public class State : IIndexedObject, IComparable<State>
        {
            public int X;
            public int Y;
            public Tuple<double, double> k;

            //other
            public static readonly State Comparer = new State(0, 0);

            public override bool Equals(object obj)
            {
                //Debug.Log("Calling Equals");
                if (obj == null) return false;
                State s2 = obj as State;
                if ((System.Object)s2 == null) return false;
                return ((this.X == s2.X) && (this.Y == s2.Y));
            }

            public static bool operator ==(State s1, State s2)
            {
                if (System.Object.ReferenceEquals(s1, s2))
                {
                    return true;
                }
                if ((object)s1 == null || (object)s2 == null)
                {
                    return false;
                }
                //Debug.Log("State == Equals");
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
                //Debug.Log("s1: " + s1 + ";s2: " + s2);
                //Debug.Log("S1.k: " + s1.k + "; S2.k: " + s2.k);
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
            public bool IsWall { get; set; }
            public bool IsWalkable()
            {
                return !IsWall;
            }

            public State(int x, int y)
            {
                X = x;
                Y = y;
                k = new Tuple<double, double>(0, 0);
            }

            public State(int x, int y, Tuple<double, double> n_k)
            {
                X = x;
                Y = y;
                k = n_k;
            }

            //Create dummy state for comparisons.
            public State(Tuple<double, double> n_k)
            {
                X = -1;
                Y = -1;
                k = n_k;
            }

            public int CompareTo(State other)
            {
                return new StateComparer().Compare(this, other);
            }
        }

        public DstarLite(GameManager.SpecialPathNode[,] inGrid)
        {
            SearchSpace = inGrid;
            rhs = new Dictionary<State, double>(new StateComparer());
            g = new Dictionary<State, double>(new StateComparer());
            U = new PriorityQueue<State>(new StateComparer());
            Path = new List<State>();
            Width = inGrid.GetLength(0);
            Height = inGrid.GetLength(1);
            m_SearchSpace = new State[Width, Height];
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (inGrid[x, y] == null)
                        throw new ArgumentNullException("Null grid cell");

                    m_SearchSpace[x, y] = new State(x, y);
                    m_SearchSpace[x, y].IsWall = inGrid[x, y].IsWall;
                }
            }
            /*
            string matrix = "";
            for(int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    matrix += m_SearchSpace[i, j] + ", ";
                }
            }
            //Debug.Log("MATRIX: " + matrix);*/
        }

        public void InitializeGoals(int s_x, int s_y, int g_x, int g_y)
        {
            //Not in pseudo code
            s_start = m_SearchSpace[s_x, s_y];
            s_goal = m_SearchSpace[g_x, g_y];
        }

        public void UpdateStart(int s_x, int s_y)
        {
            s_start = new State(s_x, s_y, s_start.k);

            k_m += h(s_last, s_start);
            //Debug.Log("Start_s: " + s_start.X + ", " + s_start.Y);
            //Debug.Log("in dict: " + g[s_start] + ", " + rhs[s_start]);
            s_start.k = CalculateKey(s_start);
            s_last = s_start;
        }

        public void UpdateGoal(int g_x, int g_y)
        {
            
            //list< pair<ipoint2, double> > toAdd;
            List<State> toAdd = new List<State>();
            //pair<ipoint2, double> tp;
  
            //ds_ch::iterator i;
            //list< pair<ipoint2, double> >::iterator kk;
            //for(i=cellHash.begin(); i!=cellHash.end(); i++) {
            for (int i = 0; i < Width; i++)
            {
                for(int j = 0; j < Height; j++)
                {
                    if (!m_SearchSpace[i,j].IsWalkable() )
                    {
                        toAdd.Add(m_SearchSpace[i,j]);
                    }
                }
            }
            /*
              if (!close(i->second.cost, C1)) {
                  tp.first.x = i->first.x;
                  tp.first.y = i->first.y;
                  tp.second = i->second.cost;
                  toAdd.push_back(tp);
              }*/
            /*
            cellHash.clear();
            openHash.clear();

            while(!openList.empty())
                openList.pop();*/
            g.Clear();
            rhs.Clear();
            U.Clear();
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    rhs[m_SearchSpace[i, j]] = double.PositiveInfinity;
                    g[m_SearchSpace[i, j]] = double.PositiveInfinity;
                }
            }
  
            k_m = 0;
            s_goal = new State(g_x, g_y);
            g[s_goal] = 0;
            rhs[s_goal] = 0;
            //cellInfo tmp;
            //tmp.g = tmp.rhs =  0;
            
            //tmp.cost = C1;

            //cellHash[s_goal] = tmp;

            //tmp.g = tmp.rhs = heuristic(s_start,s_goal);
            //tmp.cost = C1;
            //cellHash[s_start] = tmp;
            g[s_start] = h(s_start, s_goal);
            rhs[s_start] = h(s_start, s_goal);

            s_start.k = CalculateKey(s_start);

            s_last = s_start;    

            //for (kk=toAdd.begin(); kk != toAdd.end(); kk++) {
            //    updateCell(kk->first.x, kk->first.y, kk->second);
            //}
            for(int i = 0; i < toAdd.Count; i++)
            {
                UpdateVertex(toAdd[i]);
            }
        }

        public void Replan()
        {
            Path.Clear();
            int iterations = 0;
            //Begin psuedo code here
            //s_last = s_start
            s_last = s_start;
            Initialize();
            //ComputeShortestPath()
            ComputeShortestPath();
            //while s_start != s_goal
            while (s_start != s_goal)
            {
                if (iterations > maxIterations)
                {
                    Debug.Log("Infinitely Looping");
                    return;
                }
                iterations++;
                //if g(s_start) = infinity, then there is no known path.
                if (double.IsPositiveInfinity(g[s_start]))
                {
                    Debug.Log("No Known Path");
                    return;
                }
                Path.Add(s_start);
                //s_start = argmin s' element of succ(s_start) (c(s_start, s') + g(s'))
                List<State> succesors = Succ(s_start);
                double min = double.PositiveInfinity;
                State minState = null;
                foreach (State s in succesors)
                {
                    double val = Cost(s_start, s) + g[s];
                    if (minState == null)
                    {
                        minState = s;
                        min = val;
                    }
                    else
                    {
                        if (val < min)
                        {
                            min = val;
                            minState = s;
                        }
                    }
                }
                s_start = minState;
                //Move to start; scan graph for any changed edge costs
            }
        }

        public List<State> Succ(State u)
        {
            List<State> s = new List<State>();
            if (!u.IsWalkable()) return s;
            int x = u.X;
            int y = u.Y;
            x += 1;
            if (x >= 0 && y >= 0 && x < Width && y < Height) s.Insert(0, m_SearchSpace[x, y]);
            y += 1;
            if (x >= 0 && y >= 0 && x < Width && y < Height) s.Insert(0, m_SearchSpace[x, y]);
            x -= 1;
            if (x >= 0 && y >= 0 && x < Width && y < Height) s.Insert(0, m_SearchSpace[x, y]);
            x -= 1;
            if (x >= 0 && y >= 0 && x < Width && y < Height) s.Insert(0, m_SearchSpace[x, y]);
            y -= 1;
            if (x >= 0 && y >= 0 && x < Width && y < Height) s.Insert(0, m_SearchSpace[x, y]);
            y -= 1;
            if (x >= 0 && y >= 0 && x < Width && y < Height) s.Insert(0, m_SearchSpace[x, y]);
            x += 1;
            if (x >= 0 && y >= 0 && x < Width && y < Height) s.Insert(0, m_SearchSpace[x, y]);
            x += 1;
            if (x >= 0 && y >= 0 && x < Width && y < Height) s.Insert(0, m_SearchSpace[x, y]);
            /*
            string s2 = "Suc of (" + u.X + ", " + u.Y + "): [";
            foreach (State s_i in s)
            {
                s2 += "(" + s_i.X + ", " + s_i.Y + "), ";
            }
            Debug.Log(s2);*/
            return s;
        }

        public List<State> Pred(State u)
        {
            List<State> s = new List<State>();
            int x = u.X;
            int y = u.Y;
            x += 1;
            if (x >= 0 && y >= 0 && u.IsWalkable() && x < Width && y < Height) s.Insert(0, m_SearchSpace[x, y]);
            y += 1;
            if (x >= 0 && y >= 0 && u.IsWalkable() && x < Width && y < Height) s.Insert(0, m_SearchSpace[x, y]);
            x -= 1;
            if (x >= 0 && y >= 0 && u.IsWalkable() && x < Width && y < Height) s.Insert(0, m_SearchSpace[x, y]);
            x -= 1;
            if (x >= 0 && y >= 0 && u.IsWalkable() && x < Width && y < Height) s.Insert(0, m_SearchSpace[x, y]);
            y -= 1;
            if (x >= 0 && y >= 0 && u.IsWalkable() && x < Width && y < Height) s.Insert(0, m_SearchSpace[x, y]);
            y -= 1;
            if (x >= 0 && y >= 0 && u.IsWalkable() && x < Width && y < Height) s.Insert(0, m_SearchSpace[x, y]);
            x += 1;
            if (x >= 0 && y >= 0 && u.IsWalkable() && x < Width && y < Height) s.Insert(0, m_SearchSpace[x, y]);
            x += 1;
            if (x >= 0 && y >= 0 && u.IsWalkable() && x < Width && y < Height) s.Insert(0, m_SearchSpace[x, y]);
            /*
            string s2 = "Pred of (" + u.X + ", " + u.Y + "): [";
            foreach(State s_i in s)
            {
                s2 += "(" + s_i.X + ", " + s_i.Y + "), ";
            }
            //Debug.Log(s2);*/
            return s;
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
                    rhs[m_SearchSpace[i, j]] = double.PositiveInfinity;
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
            State dummy = new State(CalculateKey(s_start));
            State top = U.Peek();
            int iterations = 0;
            while ((U.Count > 0) && (top < dummy || rhs[s_start] != g[s_start]))
            {
                if (iterations > maxIterations)
                {
                    return;
                }
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
                    foreach(State s in Pred(u))
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
                    List<State> u_Pred = Pred(u);
                    u_Pred.Add(u);
                    foreach(State s in u_Pred)
                    {
                        UpdateVertex(s);
                    }
                }
                top = U.Peek();
            }
        }

        public void UpdateVertex(State u)
        {
            //if (u != s_goal)
            if (u != s_goal)
            {
                //rhs(u) = min of s' element of succ(u) according to c(u, s') + g(s') is min
                List<State> succesors = Succ(u);
                double min = double.PositiveInfinity;
                foreach(State s in succesors)
                {
                    /*
                    Debug.Log("Update vertex s suc: " + s.X + ", " + s.Y);
                    string dict_s = "Dictionary: ";
                    foreach(State s2 in g.Keys)
                    {
                        dict_s += "(" + s2.X + ", " + s2.Y + "), ";
                    }
                    Debug.Log(dict_s);*/
                    double val = Cost(u, s) + g[s];
                    if (val < min)
                    {
                        min = val;
                    }
                }
                rhs[u] = min;
            }
            //if u in U
            if (U.InQueue(u))
            {
                //U.Remove(u)
                U.Remove(u);
            }
            //if (g(u) != rhs(u))
            if (g[u] != rhs[u])
            {
                //U.Insert(u, CalculateKey(u))
                u.k = CalculateKey(u);
                //Debug.Log("u in updatevertex: " + u.GetType());
                U.Push(u);
            }
        }
        //From assuming all edge costs are 1
        public double Cost(State a, State b)
        {
            /*
            int xd = Math.Abs(a.X - b.X);
            int yd = Math.Abs(a.Y - b.Y);
            double scale = 1;

            if (xd + yd > 1) scale = Math.Sqrt(2);

            if (!rhs.ContainsKey(a) && !g.ContainsKey(a)) return scale * C1;
            return scale * cellHash[a].cost;*/
            return 1;
        }

        public Tuple<double, double> CalculateKey(State s)
        {
            //return [ min( g(s), rhs(s) ) + h(s_start, s) + k_m ; min( g(s), rhs(s) ) ]
            Tuple<double, double> key = new Tuple<double,double>( (Math.Min(g[s], rhs[s]) + h(s_start, s) + k_m), Math.Min(g[s], rhs[s]) );
            //Debug.Log("key: " + key.Item1);
            return key;
        }

        //Todo
        //Euclid distance for now
        public double h(State s, State goal)
        {
            return Math.Sqrt((s.X - goal.X) * (s.X - goal.X) + (s.Y - goal.Y) * (s.Y - goal.Y));
        }
    }
}

    
    
