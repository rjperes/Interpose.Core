using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Interpose.Core.Tests
{
    public interface IValidatable
    {
        void Try();
    }

    public class Validatable : IValidatable, IValidatableObject
    {
        public void Try()
        {

        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield return new ValidationResult("Error");
        }
    }
}
