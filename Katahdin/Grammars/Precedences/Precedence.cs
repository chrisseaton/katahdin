using System;
using System.Collections.Generic;

namespace Katahdin.Grammars.Precedences
{
    public class Precedence
    {
        private string name;
        private Group group;
        
        internal int position;
        
        public Precedence(string name)
        {
            this.name = name;
        }
        
        public static void SetPrecedence(Precedence a, Precedence b,
            Relation relation)
        {    
            if (b.Group == null)
            {
                b.Group = new Group();
                b.Group.Add(b);
            }
            
            if (a.Group != null)
                throw new Exception("Cannot set the precedence of "
                    + b.Name + " because it already belongs to a precedence "
                    + "group");
            
            a.Group = b.Group;
            b.Group.Insert(a, b, relation);
        }
        
        public bool IsLowerThan(Precedence b, bool canBeEqualTo)
        {
            if (group != b.group)
                throw new Exception("Cannot compare two precedences, "
                    + this.Name + " and " + b.Name + ", which are not "
                    + "in the same group");
            
            if (group == null)
                throw new Exception("Cannot compare two precedences, "
                    + this.Name + " and " + b.Name + ", which have not "
                    + "been added to a group");
            
            if (canBeEqualTo)
                return position <= b.position;
            else
                return position < b.position;
        }
        
        public bool Overwrites(Precedence b)
        {
            if (b == null)
                return true;
            
            if (group == null)
                return false;
            
            if (group != b.Group)
                return true;
            
            return b.IsLowerThan(this, true);
        }
        
        public string Name
        {
            get
            {
                return name;
            }
        }
        
        public Group Group
        {
            get
            {
                return group;
            }
            
            set
            {
                if (group != null)
                    throw new Exception("The precedence " + Name + " already"
                        + " belongs to a precedence group");
                
                this.group = value;
            }
        }
        
        public override string ToString()
        {
            return name;
        }
    }
}
