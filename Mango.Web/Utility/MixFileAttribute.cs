using System.ComponentModel.DataAnnotations;

namespace Mango.Web.Utility
{
    public class MixFileAttribute : ValidationAttribute
    {
        private readonly int _MixFile;
        public MixFileAttribute ( int  _mixFile )
        {
            _MixFile = _mixFile;
        }
        protected override ValidationResult? IsValid ( object? value, ValidationContext validationContext )
        {
            var file =value as IFormFile;
            if ( file != null )
            {
                var extension=Path.GetExtension(file.FileName);
                if ( file.Length> (1024 * 1024 * _MixFile ) )
                {
                    return new ValidationResult ($"Max Allow file size {_MixFile}");
                }
            }
            return ValidationResult.Success;
        }
    }
}
