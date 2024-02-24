namespace Shop
{
    /// <summary>
    /// Отправка письма
    /// </summary>
    /// <exception cref="ConnectionException">Должен быть выброшен в случае ошибки подключения</exception>
    public interface IEmailSender
    {
        Task SendEmailAsync(string recipient, string subject, string message, CancellationToken cancellationtoken);
    }

    public class ConnectionException : Exception
    {
        public ConnectionException(Exception innerException) : base("ConnectionException", innerException)
        {

        }
    }
   
}
