using System.Diagnostics;

namespace Streaming.UdpVideoServer;

public class VideoServer
{
    private VideoServer() { }

    public async static Task SendVideo(string videoPath, string ipAddress, int port)
    {
        // Crear un nuevo proceso de FFmpeg
        Process videoProcess = new Process();

        videoProcess.StartInfo.FileName = "ffmpeg"; // Asegúrate de tener FFmpeg en tu PATH o usa la ruta completa
        videoProcess.StartInfo.Arguments = $"-re -i \"{videoPath}\" -c:v libx264 -f mpegts udp://{ipAddress}:{port}";
        videoProcess.StartInfo.UseShellExecute = false;
        videoProcess.StartInfo.RedirectStandardOutput = false; // No redirige la salida, solo envía el stream UDP
        videoProcess.StartInfo.CreateNoWindow = true; // No muestra la ventana del proceso FFmpeg

        Console.WriteLine("Iniciando transmisión de video...");

        // Iniciar el proceso de FFmpeg para video
        videoProcess.Start();

        // Esperar a que finalice el proceso
        await videoProcess.WaitForExitAsync();

        Console.WriteLine("Transmisión de video finalizada.");
    }

    public async static Task SendAudio(string audioPath, string ipAddress, int port)
    {
         // Crear un nuevo proceso de FFmpeg
        Process audioProcess = new Process();
        audioProcess.StartInfo.FileName = "ffmpeg"; // Asegúrate de tener FFmpeg en tu PATH o usa la ruta completa
        audioProcess.StartInfo.Arguments = $"-re -i \"{audioPath}\" -vn -c:a aac -f mpegts udp://{ipAddress}:{port}";
        audioProcess.StartInfo.UseShellExecute = false;
        audioProcess.StartInfo.RedirectStandardOutput = false; // No redirige la salida, solo envía el stream UDP
        audioProcess.StartInfo.CreateNoWindow = true; // No muestra la ventana del proceso FFmpeg

        Console.WriteLine("Iniciando transmisión de audio...");

        // Iniciar el proceso de FFmpeg para audio
        audioProcess.Start();

        // Esperar a que finalice el proceso
        await audioProcess.WaitForExitAsync();

        Console.WriteLine("Transmisión de audio finalizada.");
    }
}