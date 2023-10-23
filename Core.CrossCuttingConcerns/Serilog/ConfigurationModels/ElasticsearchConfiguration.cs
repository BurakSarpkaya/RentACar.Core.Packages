namespace Core.CrossCuttingConcerns.Serilog.ConfigurationModels
{
    public class ElasticsearchConfiguration
    {
        public string Url { get; set; }
        public string AppName { get; set; }

        public ElasticsearchConfiguration()
        {
            Url = string.Empty;
            AppName = string.Empty;
        }

        public ElasticsearchConfiguration(string url, string appName)
        {
            Url = url;
            AppName = appName;
        }
    }
}
