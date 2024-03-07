
using System.ComponentModel.DataAnnotations;
namespace Domain.Entities;

public class TokenSupply
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = "BLP";

    public decimal CirculatingSupply { get; set; }
    public decimal TotalSupply { get; set; }

    [DataType(DataType.DateTime)]
    [Display(Name = "Created At")]
    public DateTime CreatedAt { get; set; }
}
