using System.ComponentModel.DataAnnotations.Schema;
using API.Entities;

namespace API;

[Table("Photo")]
public class Photo
{
    public int Id { get; set; }
    public string Url { get; set; }
    public bool IsMain { get; set; }
    public string PublicID { get; set; }
    public int AppUserId { get; set; }
    public AppUser AppUser { get; set; }
}
