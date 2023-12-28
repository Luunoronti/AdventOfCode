namespace AmaAocHelpers;

[AttributeUsage(AttributeTargets.Class)]
public class ExpectedTestAnswerPart2Attribute : Attribute
{
    public ExpectedTestAnswerPart2Attribute(long answer) { Answer = answer; }
    public long Answer { get; set; }
}
