using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace AnnotationsAPI.DTOs
{
    public class ChangeForgottenPasswordDto : AuthorizationCodeDto
    {

        [Required(ErrorMessage = "New password is required!")]
        public string NewPassword { get; set; }

    }
}