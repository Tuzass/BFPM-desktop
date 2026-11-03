using System.ComponentModel.DataAnnotations;

namespace API.Requests;

public class CreateTaskTagLinkRequest
{
    [Required]
    public int TaskID {get; set;}

    [Required]
    public int TagID {get; set;}
}
