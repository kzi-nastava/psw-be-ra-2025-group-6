namespace Explorer.Stakeholders.API.Dtos;

public class AuthenticationTokensDto
{
    public long Id { get; set; }
    public string AccessToken { get; set; }
    public long PersonId { get; set; }
}
