namespace AdventOfCode2023
{
    [AttributeUsage(AttributeTargets.Class)]
    class ForceAttribute : Attribute
    {
    }
    [AttributeUsage(AttributeTargets.Class)]
    class AlwaysUseTestDataAttribute : Attribute
    {
    }
    [AttributeUsage(AttributeTargets.Class)]
    class UseLiveDataInDeugAttribute : Attribute
    {
    }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    class DisableLogInDebugAttribute : Attribute
    {
    }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    class AlwaysEnableLogAttribute : Attribute
    {
    }
}