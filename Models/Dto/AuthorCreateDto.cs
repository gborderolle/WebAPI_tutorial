using System.ComponentModel.DataAnnotations;

namespace API_testing3.Models.Dto
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
    public class AuthorCreateDto
    {
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }

        public DateTime Creation { get; set; }

        public DateTime Update { get; set; }
    }
}
