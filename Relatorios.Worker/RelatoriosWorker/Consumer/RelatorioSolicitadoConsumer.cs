using MassTransit;
using Contracts.Events;
using RelatoriosWorker.Data;

namespace RelatoriosWorker.Consumer
{
    public class RelatorioSolicitadoConsumer : IConsumer<RelatoriosCreatedEvent>
    {
        private readonly WorkerDbContext _db;
        private readonly ILogger<RelatorioSolicitadoConsumer> _logger;
    
        public RelatorioSolicitadoConsumer(WorkerDbContext db, ILogger<RelatorioSolicitadoConsumer> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<RelatoriosCreatedEvent> context)
        {
            var pedido = context.Message;
            _logger.LogInformation($"[COZINHA] Recebido pedido de relatório {pedido.Id}");

            var relatorio = await _db.Relatorios.FindAsync(pedido.Id);
            if (relatorio == null) return;


            relatorio.Status = "Processando";
            await _db.SaveChangesAsync();
            _logger.LogWarning($"[COZINHA] O relatório {pedido.Id} mudou para PROCESSANDO. A gerar ficheiro pesado...");

            // Apenas para simular um SELECT pesado
            await Task.Delay(5000);

            relatorio.Status = "Concluído";
            await _db.SaveChangesAsync();
            _logger.LogInformation($"[COZINHA] Relatório {pedido.Id} CONCLUÍDO com sucesso!");
        }
    }
}
