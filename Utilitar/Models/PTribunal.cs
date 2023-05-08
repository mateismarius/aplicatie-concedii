namespace Utilitar.Models
{
    public class PTribunal : BaseModel
    {
        public string? Name { get; set; }
        public int PjId { get; set; }
        public PJudecatorie? Judecatorie { get; set; }
    }
}
