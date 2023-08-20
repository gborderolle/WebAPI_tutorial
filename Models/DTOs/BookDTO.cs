﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI_tutorial.Models.DTOs
{
    /// <summary>
    /// Los DataAnnotations van en las entidades (crea los campos en la BD con dichas restricciones) y en los DTOs (valida los inputs del usuario)
    /// 
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
    public class BookDTO
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 100, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
        public string Title { get; set; }

        public int AuthorId { get; set; }

        [ForeignKey("AuthorId")]
        public Author Author { get; set; }//n..1
    }
}
