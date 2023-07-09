using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AnnotationsAPI.Models;

namespace AnnotationsAPI.Context
{
    public class ApplicationContext : IdentityDbContext<UserAplication>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }

        /* protected override void OnModelCreating(ModelBuilder builder)
        //https://learn.microsoft.com/en-us/ef/core/saving/cascade-delete
        {
            base.OnModelCreating(builder);
        } */

        public DbSet<Annotation> Annotation { get; set; }
        public DbSet<AuthorizationCode> AuthorizationCode { get; set; }

    }
}