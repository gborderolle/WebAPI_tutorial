using WebAPI_tutorial.Validations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI_tutorial.Models.Dto
{
    /// <summary>
    /// Entidad:
    /// FK tiene el Id externo y el objeto con un datanotation: fk del mismo Id externo
    /// 
    /// DTOs:
    /// La idea es que sean los que se devuelven al front (devuelve los endpoints); no devolver la entidad misma
    /// Mantienen los Required y los MaxLenth
    /// No tienen los Datetime corporativos (create y update)
    /// CreateDTO: no lleva Id; UpdateDTO sí lleva Id (requerido)
    /// 
    /// Recordar implementar el AutoMap para cada relación entidad-DTO
    /// </summary>
    public class BookCreateDto
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 100, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
        [FirstCharCapitalValidation]
        public string Title { get; set; }

        public int AuthorId { get; set; }

        [ForeignKey("AuthorId")]
        public Author Author { get; set; }//n..1

        public DateTime Creation { get; set; }

        public DateTime Update { get; set; }
    }
}
