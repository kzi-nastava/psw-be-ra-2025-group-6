using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Administration
{
    public interface IAdminMapService
    {
        List<AdminMapDto> GetAllMapItems();
        AdminMapDto UpdateLocation(string type, long id, AdminUpdateLocationDto dto);
    }
}
