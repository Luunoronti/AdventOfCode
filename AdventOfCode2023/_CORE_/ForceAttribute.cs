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

    [AttributeUsage(AttributeTargets.Class)]
    class ExpectedTestAnswerPart1Attribute : Attribute
    {
        public ExpectedTestAnswerPart1Attribute(long answer) { Answer = answer; }
        public long Answer { get; set; }
    }
    [AttributeUsage(AttributeTargets.Class)]
    class ExpectedTestAnswerPart2Attribute : Attribute
    {
        public ExpectedTestAnswerPart2Attribute(long answer) { Answer = answer; }
        public long Answer { get; set; }
    }

[AttributeUsage(AttributeTargets.Method)]
    public class RemoveNewLinesFromInputAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Method)]
    public class RemoveSpacesFromInputAttribute : Attribute { }

}