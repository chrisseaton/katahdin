using System;
using System.Collections.Generic;

using Katahdin.Collections;

namespace Katahdin.Debugger.ObjectViewer
{
    public interface IObjectViewer
    {
        bool Views(object obj);
        ObjectViewHeader GetHeader(object obj);
        List<Tupple<string, object>> GetMembers(object obj);
    }
}
