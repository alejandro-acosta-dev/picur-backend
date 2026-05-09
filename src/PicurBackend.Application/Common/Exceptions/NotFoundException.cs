namespace PicurBackend.Application.Common.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string entity, object key)
            : base($"{entity} con identificador '{key}' no fue encontrado.") { }

        public NotFoundException(string message) : base(message) { }
    }
}
