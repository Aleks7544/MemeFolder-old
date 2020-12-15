namespace MemeFolder.Web.ViewModels.ValidationAttributes
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    using MemeFolder.Data.Models;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class TextOrFileRequired : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string text = (string)validationContext.ObjectType.GetProperty("Text").GetValue(validationContext.ObjectInstance, null);

            IEnumerable<MediaFile> mediaFiles = (IEnumerable<MediaFile>)validationContext.ObjectType.GetProperty("MediaFiles").GetValue(validationContext.ObjectInstance, null);

            if (string.IsNullOrEmpty(text) && !mediaFiles.Any())
                return new ValidationResult("You should upload at least 1 file or at least 1 character in the text field!");

            return ValidationResult.Success;
        }
    }
}
