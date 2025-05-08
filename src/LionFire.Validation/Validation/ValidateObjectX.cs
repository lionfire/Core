using LionFire.Validation;


namespace LionFire.ExtensionMethods.Validation;

/// <summary>
/// Pollute the object extension methods with validate, allowing any object (including non-validatable ones) to serve as the anchor/root for the validation context.
/// </summary>
public static class ValidateObjectX
{
    /// <summary>
    /// Create a ValidationContext for the object.
    /// If the object is IValidatable, Validate will be invoked.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="validationKind"></param>
    /// <returns></returns>
    public static ValidationContext Validate(this object obj, object validationKind = null)
    {
        var context = new ValidationContext() { Object = obj, ValidationKind = validationKind };

        if (obj is IValidatable validatable)
        {
            context = validatable.ValidateThis(context);
        }

        return context;
    }
}