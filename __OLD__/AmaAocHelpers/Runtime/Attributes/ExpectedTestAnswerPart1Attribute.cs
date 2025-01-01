namespace AmaAocHelpers;

[AttributeUsage(AttributeTargets.Class)]
public class ExpectedTestAnswerPart1Attribute : Attribute
{
    public ExpectedTestAnswerPart1Attribute(long answer) { Answer = answer; }
    public long Answer { get; set; }
}
