using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Valid.Teste.Domain.Entities
{
 
    public class Profile
    {
        [Column]
        public int Id { get; set; }

        [Column]
        public string ProfileName { get; set; } = string.Empty;

        [Column]
        public string Parameters { get; set; } = string.Empty;
    }
  
}
