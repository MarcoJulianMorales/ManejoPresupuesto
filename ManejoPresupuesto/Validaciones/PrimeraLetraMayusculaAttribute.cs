using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Validaciones
{
    public class PrimeraLetraMayusculaAttribute: ValidationAttribute
    {
        protected override ValidationResult IsValid(Object value, ValidationContext validationContext)
        {
             if(value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success;
            }

             var primeraLetra = value.ToString()[0].ToString();
            
            if(primeraLetra != primeraLetra.ToUpper())
            {
                return new ValidationResult("la primera letra debe ser mayúscula");
            }

            return ValidationResult.Success;
        }
    }
}
