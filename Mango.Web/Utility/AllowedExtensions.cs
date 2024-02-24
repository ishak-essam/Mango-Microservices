using System.ComponentModel.DataAnnotations;

namespace Mango.Web.Utility
{
    public class AllowedExtensions:ValidationAttribute
    {
        private readonly string [ ] _extensions;

        public AllowedExtensions ( string [ ] extensions)
        {
            _extensions = extensions;
        }
        protected override ValidationResult? IsValid ( object? value, ValidationContext validationContext )
        {
            var file =value as IFormFile;
             if ( file != null ) {
                var extension=Path.GetExtension(file.FileName);
                if (!_extensions.Contains(extension.ToLower())) {
                    return new ValidationResult ("this photo extension isn't allow");
                }
            }
            return ValidationResult.Success;
        }
    }
}
