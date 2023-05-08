namespace Utilitar.Models
{
    public class PCA: BaseModel
    {
        public string? Name { get; set; }
        public int PtId { get; set; }
        public PTribunal? Tribunal { get; set; }
    }
}
