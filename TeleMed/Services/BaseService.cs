using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Xml;
using Formatting = Newtonsoft.Json.Formatting;

namespace TeleMed.Services
{

    /// <summary>
    /// Base service which contains logging implementation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseService<T>
    {
        private readonly ILogger<T> _logger;
        protected IServiceProvider serviceProvider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        protected BaseService(ILogger<T> logger, IServiceProvider serviceProvider = null!)
        {
            _logger = logger;
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Helps with logging information
        /// </summary>
        /// <param name="message"></param>
        /// <param name="data"></param>
        protected void LogInformation(string message, object data)
        {
            _logger.LogInformation($"{message}: \n{JsonConvert.SerializeObject(data, Formatting.Indented)}\n");
        }

        /// <summary>
        /// Helps with logging error 
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        protected void LogError(Exception ex, string message)
        {
            _logger.LogError(ex, $"\n {message}");
        }

        

    }
}
