using System;


namespace Katahdin.CodeTree
{
    public class ObjectNode : CodeTreeNode
    {
        private object obj;
        
        public ObjectNode(Source source, object obj)
			: base(source)
        {
            this.obj = obj;
        }
        
        public override object Run(RuntimeState state)
        {
            object runMethod = MemberNode.GetMember(obj, "Run", true);
            return CallNode.Call(state, runMethod, null);
        }
        
        public override object Get(RuntimeState state, object[] parametersHint)
        {
            // when you get a user defined expression, goes here and then into expression.get which builds the code tree then and there, which passes a null method
            
            object getMethod = MemberNode.GetMember(obj, "Get", true);
            return CallNode.Call(state, getMethod, null);
        }
        
        public override void Set(RuntimeState state, object v)
        {
            object getMethod = MemberNode.GetMember(obj, "Set", true);
            CallNode.Call(state, getMethod, new object[]{v});
        }
    }
}


