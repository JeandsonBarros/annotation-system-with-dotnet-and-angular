using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AnnotationsAPI.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AnnotationsAPI.DTOs
{
    /// <summary> 
    /// Object model for when fields need to be validated.
    /// </summary>
    public class UserDto
    {
        [Required(ErrorMessage = "Name is required!")]
        public string Name { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required!")]
        public string Password { get; set; }
    }

    /// <summary> 
    /// Object model for when fields don't need to be validated. 
    /// </summary>
    [ValidateNever]
    public class UserDtoNotValidate
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
    }

    /// <summary> 
    /// Object model for when data for one or more users needs to be returned. 
    /// </summary>
    public class UserDtoResponse : UserDtoNotValidate
    {
        public string Id { get; set; }
        public IList<string> Roles { get; set; }
    }
}