using WebAPI_tutorial.Validations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI_tutorial.Models
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
    /// clase: https://www.udemy.com/course/construyendo-web-apis-restful-con-aspnet-core/learn/lecture/26839696#notes
    /// 
    /// DTOs:
    /// La idea es que sean los que se devuelven al front (devuelve los endpoints); no devolver la entidad misma
    /// Mantienen los Required y los MaxLenth
    /// No tienen los Datetime corporativos (create y update)
    /// CreateDTO: no lleva Id; UpdateDTO sí lleva Id (requerido)
    /// 
    /// Recordar implementar el AutoMap para cada relación entidad-DTO
    /// </summary>
    public class Book
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Title { get; set; }

        public int AuthorId { get; set; }

        [ForeignKey("AuthorId")]
        public Author Author { get; set; }//n..1 Clase: https://www.udemy.com/course/construyendo-web-apis-restful-con-aspnet-core/learn/lecture/13815698#notes

        public DateTime Creation { get; set; }

        public DateTime Update { get; set; }
    }
}
