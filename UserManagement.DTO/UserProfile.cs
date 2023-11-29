
namespace UserManagement.DTO;

public class UserProfile : IEntity
{
    public int Id { get; set; }
    public string Firstname { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string PersonalNumber { get; set; } = null!;
}
