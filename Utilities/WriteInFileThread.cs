namespace WebAPI_tutorial.Utilities
{

    /// <summary>
    /// Thread: Escribe en un archivo cada 5 segundos
    /// Clase: https://www.udemy.com/course/construyendo-web-apis-restful-con-aspnet-core/learn/lecture/13816118#notes
    /// </summary>
    public class WriteInFileThread : IHostedService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly string _fileName = "Archivo 1.txt";
        private Timer _timer;

        public WriteInFileThread(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        /// <summary>
        /// Se ejecuta al iniciar el WebAPI
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            Write("Proceso iniciado.");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Se ejecuta al finalizar el WebAPI
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer.Dispose();
            Write("Proceso finalizado.");
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            Write("Proceso en ejecución: " + GlobalServices.GetDatetimeUruguayString());
        }

        private void Write(string message)
        {
            var route = $@"{_environment.ContentRootPath}\wwwroot\{_fileName}";
            using (StreamWriter writer = new(route, append: true))
            {
                writer.WriteLine(message);
            }
        }

    }
}