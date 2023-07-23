using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AnnotationsAPI.DTOs
{
    [ValidateNever]
    public class AnnotationDto
    {
        public string? Title { get; set; }

        public string? Description { get; set; }

        public bool? IsImportant { get; set; }
    }

}