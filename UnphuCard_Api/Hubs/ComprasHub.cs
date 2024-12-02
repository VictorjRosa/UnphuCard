using Microsoft.AspNetCore.SignalR;

public class CompraHub : Hub
{
    // Método para unirse a un grupo basado en el equipo
    public async Task UnirseGrupo(string equipo)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, equipo);
    }

    // Método para notificar a los clientes sobre el escaneo del QR
    public async Task NotificarEscaneo(string equipo, string mensaje)
    {
        await Clients.Group(equipo).SendAsync("EscaneoCompletado", mensaje);
    }
}
