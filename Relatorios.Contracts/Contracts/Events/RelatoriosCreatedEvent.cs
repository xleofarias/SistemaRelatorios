namespace Contracts.Events
{
    public record RelatoriosCreatedEvent(Guid Id, string Tipo, string EmailSolicitante, DateTime CreatedAt);
}
