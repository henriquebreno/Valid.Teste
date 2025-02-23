using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace Valid.Teste.API.Models
{
    public class ProfileParameter
    {         
        public string ProfileName { get; set; } = string.Empty;        
        public Dictionary<string, string> Parameters { get; set; } = new();

    }
}
