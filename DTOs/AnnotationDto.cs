using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AnnotationsAPI.DTOs
{
    public class AnnotationDto
    {
        [Required(ErrorMessage = "Title is required!")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required!")]
        public string Description { get; set; }
    }

    [ValidateNever]
    public class AnnotationDtoViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }
}