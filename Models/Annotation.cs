using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AnnotationsAPI.Models
{
    public class Annotation
    {
        public long Id { get; set; }
        public string? Title { get; set; }
        public string Description { get; set; }
        public bool IsImportant { get; set; }
        public DateTime Date { get; } = DateTime.Now;

/*         [JsonIgnore] */
        public string UserAplicationId { get; set; }
        [JsonIgnore]
        public UserAplication User { get; set; }
    }
}