using System;

namespace Jobs
{
    [Flags]
    public enum TaskFlag : short
    {
        None = 0,
        OneTime = 1,
    }
}