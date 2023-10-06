using System.ComponentModel.DataAnnotations;

namespace API;

public class Group
{
    [Key]
    public string Name { get; set; }
    public ICollection<Connection> Connections { get; set; } = new List<Connection>();

    public Group(string Name)
    {
        this.Name = Name;
    }
    public Group()
    {
        
    }
}
