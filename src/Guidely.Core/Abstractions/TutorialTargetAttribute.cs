namespace Guidely.Core.Abstractions;

/// <summary>
/// Specifies a target element for the tutorial step.
/// Can be applied multiple times to target multiple elements.
/// </summary>
/// <param name="elementName">The name of the target element (matches TutorialTarget.Element attached property).</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class TutorialTargetAttribute(string elementName) : Attribute
{
    /// <summary>
    /// The name of the target element.
    /// </summary>
    public string ElementName { get; } = elementName;
}