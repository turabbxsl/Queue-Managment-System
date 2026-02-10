namespace QMS.Application.Dtos
{
    public class DashboardDto
    {
        public DateTime LastUpdated { get; set; }
        public int TotalWaiting { get; set; }
        public List<DeskStatusDto> Desks { get; set; }
    }
}
