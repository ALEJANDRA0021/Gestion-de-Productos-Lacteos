using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Gestion_de_Productos_Lacteos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaInventarioLacteos.Services
{
    public class VerificadorVencimientoService : BackgroundService
    {
        private readonly ILogger<VerificadorVencimientoService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly EmailSettings _emailSettings;

        public VerificadorVencimientoService(
            ILogger<VerificadorVencimientoService> logger,
            IServiceProvider serviceProvider,
            IOptions<EmailSettings> emailSettings)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _emailSettings = emailSettings.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("🚀 Servicio de verificación de vencimientos iniciado.");

            // Ejecutar inmediatamente al iniciar
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<SistemaInventarioLacteosContext>();
                    _logger.LogInformation("🔍 Ejecutando verificación inicial de vencimientos...");
                    await VerificarLotesProximosAVencer(dbContext);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error en verificación inicial.");
            }

            // Luego ejecutar cada 24 horas
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<SistemaInventarioLacteosContext>();
                        await VerificarLotesProximosAVencer(dbContext);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error al verificar lotes próximos a vencer.");
                }

                // Esperar 24 horas antes de la próxima verificación
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }

        private async Task VerificarLotesProximosAVencer(SistemaInventarioLacteosContext dbContext)
        {
            var fechaLimite = DateOnly.FromDateTime(DateTime.Today.AddDays(14));
            var hoy = DateOnly.FromDateTime(DateTime.Today);

            _logger.LogInformation($"📅 Verificando lotes con vencimiento entre {hoy:dd/MM/yyyy} y {fechaLimite:dd/MM/yyyy}");

            // CORREGIDO: Usar LoteProximoAVencer en lugar de tipo anónimo
            var lotesPorVencer = dbContext.Lotes
                .Where(l => l.FechaVencimiento.HasValue &&
                           l.FechaVencimiento.Value <= fechaLimite &&
                           l.FechaVencimiento.Value >= hoy &&
                           l.Cantidad > 0)
                .Join(dbContext.Productos,
                      lote => lote.IdProducto,
                      producto => producto.IdProducto,
                      (lote, producto) => new LoteProximoAVencer { Lote = lote, Producto = producto })
                .OrderBy(l => l.Lote.FechaVencimiento)
                .ToList();

            if (!lotesPorVencer.Any())
            {
                _logger.LogInformation("✅ No se encontraron lotes próximos a vencer.");
                return;
            }

            _logger.LogInformation($"📦 Se encontraron {lotesPorVencer.Count} lote(s) próximo(s) a vencer.");

            // Obtener usuarios activos con correo
            var usuarios = dbContext.Usuarios
                .Where(u => u.Estado == true && !string.IsNullOrEmpty(u.Correo))
                .ToList();

            if (!usuarios.Any())
            {
                _logger.LogWarning("⚠️ No hay usuarios con correo registrado para enviar alertas.");
                return;
            }

            // Verificar si YA se envió alerta HOY (evitar duplicados)
            var alertaHoy = dbContext.AlertasVencimientos.Any(a => a.FechaAlerta == hoy);
            if (alertaHoy)
            {
                _logger.LogInformation("⏭️ Ya se enviaron alertas hoy. Se omite para no duplicar.");
                return;
            }

            // Registrar alertas para todos los lotes en la BD
            foreach (var item in lotesPorVencer)
            {
                dbContext.AlertasVencimientos.Add(new AlertasVencimiento
                {
                    IdLote = item.Lote.IdLote,
                    FechaAlerta = hoy,
                    Estado = "Pendiente"
                });
            }
            await dbContext.SaveChangesAsync();
            _logger.LogInformation($"📝 {lotesPorVencer.Count} alertas registradas en la base de datos.");

            // ENVIAR UN SOLO CORREO RESUMEN A CADA USUARIO
            foreach (var usuario in usuarios)
            {
                await EnviarResumenDiario(usuario.Correo, usuario.Nombre, lotesPorVencer);
            }

            _logger.LogInformation($"✅ Proceso de alertas completado. Se enviaron resúmenes a {usuarios.Count} usuario(s).");
        }

        // CORREGIDO: Cambiar List<dynamic> por List<LoteProximoAVencer>
        private async Task EnviarResumenDiario(string correoDestino, string nombreUsuario,
            List<LoteProximoAVencer> lotesPorVencer)
        {
            try
            {
                // Contar por nivel de urgencia y construir filas de la tabla
                int urgentes = 0, atencion = 0, aviso = 0;
                var filasTabla = new StringBuilder();

                foreach (var item in lotesPorVencer)
                {
                    var dias = item.DiasRestantes; // Usar la propiedad calculada
                    string colorFila, emoji;

                    if (dias <= 3)
                    {
                        colorFila = "#ffcccc";
                        emoji = "🔴";
                        urgentes++;
                    }
                    else if (dias <= 7)
                    {
                        colorFila = "#fff3cd";
                        emoji = "🟠";
                        atencion++;
                    }
                    else
                    {
                        colorFila = "#d4edda";
                        emoji = "🟢";
                        aviso++;
                    }

                    filasTabla.Append($@"
                        <tr style='background-color: {colorFila};'>
                            <td style='padding: 8px; border: 1px solid #ddd; text-align: center;'>{emoji}</td>
                            <td style='padding: 8px; border: 1px solid #ddd;'>{item.Producto.NombreProducto}</td>
                            <td style='padding: 8px; border: 1px solid #ddd; text-align: center;'>{item.Lote.NumeroLote}</td>
                            <td style='padding: 8px; border: 1px solid #ddd; text-align: center;'>{item.Lote.Cantidad}</td>
                            <td style='padding: 8px; border: 1px solid #ddd; text-align: center;'>{item.Lote.FechaVencimiento.Value:dd/MM/yyyy}</td>
                            <td style='padding: 8px; border: 1px solid #ddd; text-align: center; font-weight: bold;'>{dias} días</td>
                        </tr>");
                }

                using (var client = new SmtpClient(_emailSettings.Host, _emailSettings.Port))
                {
                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(_emailSettings.From, _emailSettings.Password);

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_emailSettings.From, "Sistema Gestión Lácteos"),
                        Subject = $"📋 RESUMEN DIARIO: {lotesPorVencer.Count} productos próximos a vencer (🔴{urgentes} 🟠{atencion} 🟢{aviso})",
                        Body = $@"
                            <!DOCTYPE html>
                            <html>
                            <head><meta charset='UTF-8'></head>
                            <body style='font-family: Arial, sans-serif; background-color: #f5f5f5; padding: 20px;'>
                                <div style='max-width: 750px; margin: auto; background: white; border-radius: 10px; padding: 25px; box-shadow: 0 2px 10px rgba(0,0,0,0.1);'>
                                    <h2 style='color: #2c3e50; margin-top: 0;'>📋 Resumen Diario de Vencimientos</h2>
                                    <p>Hola <strong>{nombreUsuario}</strong>,</p>
                                    <p>Este es el resumen de productos próximos a vencer generado el <strong>{DateTime.Today:dd/MM/yyyy}</strong>:</p>

                                    <div style='margin: 20px 0; display: flex; gap: 10px; flex-wrap: wrap;'>
                                        <span style='background-color: #ffcccc; padding: 8px 15px; border-radius: 20px; border: 1px solid #ff6666;'>
                                            🔴 <strong>Urgentes</strong> (1-3 días): {urgentes}
                                        </span>
                                        <span style='background-color: #fff3cd; padding: 8px 15px; border-radius: 20px; border: 1px solid #ffc107;'>
                                            🟠 <strong>Atención</strong> (4-7 días): {atencion}
                                        </span>
                                        <span style='background-color: #d4edda; padding: 8px 15px; border-radius: 20px; border: 1px solid #28a745;'>
                                            🟢 <strong>Aviso</strong> (8-14 días): {aviso}
                                        </span>
                                    </div>

                                    <table style='border-collapse: collapse; width: 100%; font-size: 13px; margin: 20px 0;'>
                                        <thead>
                                            <tr style='background-color: #2c3e50; color: white;'>
                                                <th style='padding: 10px; border: 1px solid #ddd; width: 40px;'>Nivel</th>
                                                <th style='padding: 10px; border: 1px solid #ddd;'>Producto</th>
                                                <th style='padding: 10px; border: 1px solid #ddd;'>Lote</th>
                                                <th style='padding: 10px; border: 1px solid #ddd; width: 60px;'>Stock</th>
                                                <th style='padding: 10px; border: 1px solid #ddd; width: 110px;'>Vencimiento</th>
                                                <th style='padding: 10px; border: 1px solid #ddd; width: 70px;'>Días</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            {filasTabla}
                                        </tbody>
                                    </table>

                                    <div style='background-color: #e8f4fd; border-left: 4px solid #2196F3; padding: 12px; margin: 15px 0; border-radius: 4px;'>
                                        <strong>📋 Acción recomendada:</strong> Revise el inventario físico y priorice la rotación o venta de los productos marcados como urgentes (🔴).
                                    </div>

                                    <p style='text-align: right; font-weight: bold;'>Total de productos a revisar: {lotesPorVencer.Count}</p>

                                    <hr style='border: none; border-top: 1px solid #eee; margin: 20px 0;'>
                                    <p style='color: #95a5a6; font-size: 11px; text-align: center;'>
                                        Este resumen se genera automáticamente una vez al día.<br>
                                        Si no hay cambios en el inventario, recibirá la misma alerta mañana.<br>
                                        Sistema de Gestión de Productos Lácteos © {DateTime.Now.Year}
                                    </p>
                                </div>
                            </body>
                            </html>",
                        IsBodyHtml = true,
                    };

                    mailMessage.To.Add(correoDestino);

                    await client.SendMailAsync(mailMessage);
                    _logger.LogInformation($"✅ Resumen diario enviado exitosamente a {correoDestino} ({lotesPorVencer.Count} productos)");
                }
            }
            catch (SmtpException smtpEx)
            {
                _logger.LogError(smtpEx, $"❌ Error SMTP al enviar resumen a {correoDestino}: {smtpEx.Message}");
                if (smtpEx.InnerException != null)
                {
                    _logger.LogError($"   Detalle: {smtpEx.InnerException.Message}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error general al enviar resumen a {correoDestino}: {ex.Message}");
            }
        }
    }
}