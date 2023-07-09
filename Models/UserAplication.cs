using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace AnnotationsAPI.Models
{
    public class UserAplication : IdentityUser
    {
        public string Name { get; set;}
        public IList<Annotation> Annotations { get; } = new List<Annotation>();
    }
}