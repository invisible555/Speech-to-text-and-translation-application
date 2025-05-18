namespace ReactProject.Server.DTO
{
    public class FileWithStreamDTO
    {
            public FileStream Stream { get; set; }
            public string ContentType { get; set; }
            public string FileName { get; set; }
    }
}
