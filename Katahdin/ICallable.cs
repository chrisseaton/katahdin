namespace Katahdin
{
    public interface ICallable
    {
        bool[] RefParams
        {
            get;
        }
        
        object Call(RuntimeState state, object[] parameters,
            bool wantRefParams, out bool[] refParams);
    }
}