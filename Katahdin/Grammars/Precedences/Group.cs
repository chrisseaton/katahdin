using System;
using System.Collections.Generic;

namespace Katahdin.Grammars.Precedences
{
    public class Group
    {
        private List<List<Precedence>> order = new List<List<Precedence>>();
        
        public void Add(Precedence precedence)
        {
            List<Precedence> entry = new List<Precedence>();
            entry.Add(precedence);
            
            order.Add(entry);
            
            UpdatePositions();
        }
        
        public void Insert(Precedence a, Precedence b, Relation relation)
        {
            for (int n = 0; n < order.Count; n++)
            {
                if (order[n].Contains(b))
                {
                    switch (relation)
                    {
                        case Relation.Lower:
                        {
                            List<Precedence> entry = new List<Precedence>();
                            entry.Add(a);
                            
                            order.Insert(n, entry);
                        } break;
                        
                        case Relation.Equal:
                            order[n].Insert(0, a);
                            break;
                        
                        case Relation.Higher:
                        {
                            List<Precedence> entry = new List<Precedence>();
                            entry.Add(a);
                            
                            order.Insert(n + 1, entry);
                        } break;
                    }
                    
                    UpdatePositions();
                    
                    return;
                }
            }
            
            throw new Exception("Group does not contain " + b.Name);
        }
        
        private void UpdatePositions()
        {
            for (int n = 0; n < order.Count; n++)
            {
                foreach (Precedence entry in order[n])
                    entry.position = n;
            }
        }
        
        /*public bool IsLowerThan(Precedence a, Precedence b, bool canBeEqualTo)
        {
            foreach (List<Precedence> entry in order)
            {
                if (entry.Contains(a))
                {
                    if (canBeEqualTo)
                        return true;
                    else
                        return !entry.Contains(b);
                }
                else if (entry.Contains(b))
                {
                    return false;
                }
            }
            
            throw new Exception("Group does not contain " + a.Name);
        }*/
        
        public List<List<Precedence>> Order
        {
            get
            {
                return order;
            }
        }
    }
}
