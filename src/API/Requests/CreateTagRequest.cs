using System.ComponentModel.DataAnnotations;

namespace API.Requests;

public class CreateTagRequest
{
    [Required]
    public int ProjectID {get; set;}
    
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string Name {get; set;} = string.Empty;
}
