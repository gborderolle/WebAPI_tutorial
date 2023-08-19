using WebAPI_tutorial.Validations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI_tutorial.Models.Dto
{
    /// <summary>
    /// Entidad:
    /// FK tiene el Id externo y el objeto con un datanotation: fk del mismo Id externo
    /// Error de relaciones entre lista y objeto (1..n y viceversa): corrección en NOTAS
    /// 
    /// Validaciones personalizadas (Data Annotations):
    /// Carpeta: Validations y 1 clase por validación, ej: FirstCharCapitalValidation.cs
    /// clase: https://www.udemy.com/course/construyendo-web-apis-restful-con-aspnet-core/learn/lecture/13815782#notes
    /// 
    /// Validaciones generales (no de un atributo particular, sino del modelo (entidad)):
    /// heredar interfaz: "Author : IValidatableObject"
    /// usar yield para acumular respuestas de las validaciones
    /// clase: https://www.udemy.com/course/construyendo-web-apis-restful-con-aspnet-core/learn/lecture/26839696#notes
    /// 
    /// DTOs:
    /// La idea es que sean los que se devuelven al front (devuelve los endpoints); no devolver la entidad misma
    /// Los DTOs contienen todas las validaciones
    /// No tienen los Datetime corporativos (create y update)
    /// CreateDTO: no lleva Id; UpdateDTO sí lleva Id (requerido)
    /// 
    /// Recordar implementar el AutoMap para cada relación entidad-DTO
    /// </summary>
    public class AuthorCreateDto : IValidatableObject
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 100, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
        [FirstCharCapitalValidation]
        public string Name { get; set; }

        public DateTime Creation { get; set; }

        public DateTime Update { get; set; }


        // propiedades de prueba
        /*
        [NotMapped]
        public int TestingNumber { get; set; } // Menor < x < Mayor
        [NotMapped]
        public int Menor { get; set; } = 0;
        [NotMapped]
        public int Mayor { get; set; } = 18;
        */

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            /*
            if (TestingNumber < Menor)
            {
                yield return new ValidationResult($"Este valor no puede ser menor a Menor: {Menor}",
                    new string[] { nameof(Menor)});
            }
            if (TestingNumber > Mayor)
            {
                yield return new ValidationResult($"Este valor no puede ser mayor a Mayor: {Mayor}",
                    new string[] { nameof(Mayor) });
            }
            */
            yield return ValidationResult.Success;
        }

    }
}
