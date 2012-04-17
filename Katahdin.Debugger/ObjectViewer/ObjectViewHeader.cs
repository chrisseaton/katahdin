namespace Katahdin.Debugger.ObjectViewer
{
    public class ObjectViewHeader
    {
        private string description;
        private bool hasMembers;
        private bool allowRawView;
        
        public ObjectViewHeader(string description, bool hasMembers, bool allowRawView)
        {
            this.description = description;
            this.hasMembers = hasMembers;
            this.allowRawView = allowRawView;
        }
        
        public string Description
        {
            get
            {
                return description;
            }
        }
        
        public bool HasMembers
        {
            get
            {
                return hasMembers;
            }
        }
        
        public bool AllowRawView
        {
            get
            {
                return allowRawView;
            }
        }
    }
}
