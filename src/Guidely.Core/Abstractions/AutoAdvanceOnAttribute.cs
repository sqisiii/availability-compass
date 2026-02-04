namespace Guidely.Core.Abstractions;

/// <summary>
/// Specifies triggers that can cause this step to auto-advance.
/// Can be applied multiple times for multiple triggers.
/// </summary>
/// <param name="trigger">The trigger enum value (e.g., MyTrigger.ButtonClicked).</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class AutoAdvanceOnAttribute(object trigger) : Attribute
{
    /// <summary>
    /// The trigger that can cause auto-advance.
    /// </summary>
    public object Trigger { get; } = trigger;
}