namespace WebApiINMO.DTOs
{
    public class DataHATEOAS
    {

        public string Url { get; set; }
        public string Description { get; set; }
        public string Method { get; set; }


        public DataHATEOAS(string Url, string Description, String Method)
        {
            this.Url = Url;
            this.Description = Description;
            this.Method = Method;
        }


    }
}
