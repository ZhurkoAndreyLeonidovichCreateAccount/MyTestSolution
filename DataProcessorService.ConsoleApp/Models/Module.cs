namespace DataProcessorService.ConsoleApp.Models
{
    public class Module
    {
        public int Id { get; set; } // Primary key
        public string ModuleCategoryID { get; set; } = string.Empty;
        public ModuleState State { get; set; }
    }
}
