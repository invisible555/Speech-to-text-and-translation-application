namespace ReactProject.Server.DTO
{
    public class TranslateRequest
    {        public string SourceLang { get; set; }
        public string TargetLang { get; set; }
        public string? OriginalFilepath { get; set; }  // opcjonalne
    }
}
