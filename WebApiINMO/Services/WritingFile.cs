namespace WebApiINMO.Services
{
    public class WritingFile : IHostedService
    {

        private readonly IWebHostEnvironment Env;
        private readonly string FileName = "File 1.txt";
        private Timer Timer;

        public WritingFile(IWebHostEnvironment env)
        {
            Env = env;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            WriteFile("START");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Timer.Dispose();
            WriteFile("STOP");
            return Task.CompletedTask;
        }



        public void DoWork(object state)
        {
            WriteFile("Proceso en ejecucion: " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
        }


        private void WriteFile(string message)
        {
            var url = $@"{ Env.ContentRootPath }\wwwroot\{FileName}";

            using (StreamWriter writer = new StreamWriter(url, append: true))
            {
                writer.WriteLine(message);
            }
        }


    }
}
