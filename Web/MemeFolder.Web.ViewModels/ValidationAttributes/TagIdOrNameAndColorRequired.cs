namespace MemeFolder.Web.ViewModels.ValidationAttributes
{
    using System;
    using System.ComponentModel.DataAnnotations;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class TagIdOrNameAndColorRequired : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string id = (string)validationContext.ObjectType.GetProperty("Id").GetValue(validationContext.ObjectInstance, null);

            string name = (string)validationContext.ObjectType.GetProperty("Name").GetValue(validationContext.ObjectInstance, null);

            string color = (string)validationContext.ObjectType.GetProperty("Color").GetValue(validationContext.ObjectInstance, null);

            if (!string.IsNullOrEmpty(id))
            {
                return ValidationResult.Success;
            }

            if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(color))
            {
                return new ValidationResult("Tag name and color are both required!");
            }

            return ValidationResult.Success;
        }
    }
}
