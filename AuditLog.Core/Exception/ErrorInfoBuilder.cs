namespace AuditLog.Core
{
    public class ErrorInfoBuilder : IErrorInfoBuilder
    {
        private IExceptionToErrorInfoConverter Converter { get; set; }

        
        public ErrorInfoBuilder()
        {
            Converter = new DefaultErrorInfoConverter();
        }

        
        public ErrorInfo BuildForException(Exception exception)
        {
            return Converter.Convert(exception);
        }

       
        public void AddExceptionConverter(IExceptionToErrorInfoConverter converter)
        {
            converter.Next = Converter;
            Converter = converter;
        }
    }
}
