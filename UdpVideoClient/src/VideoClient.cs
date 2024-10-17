using System.Diagnostics;

namespace Streaming.UdpVideoClient;

public class VideoClient
{

    private VideoClient() { }

    public static void Play(string videoStreamUrl)
    {
        // Iniciar FFplay para reproducir el video y audio
        var ffPlayPath = "ffplay"; // Asegúrate de tener ffplay en tu PATH o usa la ruta completa
        var ffPlayArguments = $"{videoStreamUrl}?fifo_size=100000&overrun_nonfatal=1";

        var ffPlayProcess = new Process();
        ffPlayProcess.StartInfo.FileName = ffPlayPath;
        ffPlayProcess.StartInfo.Arguments = ffPlayArguments;
        ffPlayProcess.StartInfo.UseShellExecute = false;
        ffPlayProcess.StartInfo.RedirectStandardOutput = false;
        ffPlayProcess.StartInfo.CreateNoWindow = true;

        ffPlayProcess.Start();
        ffPlayProcess.WaitForExit();

        Console.WriteLine("Reproducción finalizada.");
    }

    public async static Task ReceiveVideo(string ipAddress, int port)
    {
        var udpClient = new UdpClient(port); // Puerto en el que escucharemos
        var ipAddressOb = IPAddress.Parse(ipAddress);
        var remoteEP = new IPEndPoint(ipAddressOb, port);
        var receiveTimeout = 5000; // Establecer un tiempo de espera para la recepción de datos

        Console.WriteLine("Esperando transmisión de video...");

        // Crear archivo para almacenar los datos de video recibidos
        using FileStream videoFile = new("./static/received_video.ts", FileMode.Create, FileAccess.Write);
        while (true)
        {
            // Crear la tarea de recepción de datos
            var receiveTask = udpClient.ReceiveAsync();

            // Crear una tarea de timeout
            var timeoutTask = Task.Delay(receiveTimeout);

            // Esperar a que se complete la recepción o el timeout
            var completedTask = await Task.WhenAny(receiveTask, timeoutTask);

            if (completedTask == timeoutTask)
            {
                Console.WriteLine($"Timeout: No se recibieron datos en los últimos {receiveTimeout / 1000} segundos. Asumiendo fin de la transmisión.");
                break; // Salir del bucle si ocurre el timeout
            }

            // Si la tarea de recepción se completa antes del timeout
            var result = await receiveTask;
            var videoData = result.Buffer;

            // Obtener información del remitente
            Console.WriteLine($"Recibidos {videoData.Length} bytes de video desde {remoteEP}.");

            videoFile.Write(videoData, 0, videoData.Length);
        }
    }

     // Método para recibir y reproducir audio
    public async static Task ReceiveAudio(string ipAddress, int port)
    {
        var udpClient = new UdpClient(port);  // Puerto para audio
        var ipAddressOb = IPAddress.Parse(ipAddress);
        var remoteEP = new IPEndPoint(ipAddressOb, port);
        var receiveTimeout = 5000; // Establecer un tiempo de espera para la recepción de datos

        Console.WriteLine("Esperando transmisión de audio...");

        using FileStream audioFile = new("./static/received_audio.ts", FileMode.Create, FileAccess.Write);
        while (true)
        {
            // Crear la tarea de recepción de datos
            var receiveTask = udpClient.ReceiveAsync();

            // Crear una tarea de timeout
            var timeoutTask = Task.Delay(receiveTimeout);

            // Esperar a que se complete la recepción o el timeout
            var completedTask = await Task.WhenAny(receiveTask, timeoutTask);

            if (completedTask == timeoutTask)
            {
                Console.WriteLine($"Timeout: No se recibieron datos en los últimos {receiveTimeout / 1000} segundos. Asumiendo fin de la transmisión.");
                break; // Salir del bucle si ocurre el timeout
            }

            // Si la tarea de recepción se completa antes del timeout
            var result = await receiveTask;
            var audioData = result.Buffer;

            // Obtener información del remitente
            Console.WriteLine($"Recibidos {audioData.Length} bytes de audio desde {remoteEP}.");

            audioFile.Write(audioData, 0, audioData.Length);
        }
    }

    // Método para combinar audio y video utilizando FFmpeg
    public static void CombineAudioAndVideo(string videoFilePath, string audioFilePath, string outputFilePath)
    {
        // Verifica que los archivos de entrada existan
        if (!File.Exists(videoFilePath) || !File.Exists(audioFilePath))
        {
            Console.WriteLine("Error: Los archivos de audio o video no existen.");
            return;
        }

        var ffmpegProcess = new Process();

        // Comando FFmpeg para combinar audio y video con recodificación de audio
        string ffmpegArguments = $"-i \"{videoFilePath}\" -i \"{audioFilePath}\" -map 0:v -map 1:a -c:v copy -c:a aac -b:a 192k -shortest \"{outputFilePath}\"";

        ffmpegProcess.StartInfo.FileName = "ffmpeg";
        ffmpegProcess.StartInfo.Arguments = ffmpegArguments;
        ffmpegProcess.StartInfo.UseShellExecute = false;
        ffmpegProcess.StartInfo.RedirectStandardOutput = true;
        ffmpegProcess.StartInfo.RedirectStandardError = true;
        ffmpegProcess.StartInfo.CreateNoWindow = true;

        // Iniciar el proceso FFmpeg
        ffmpegProcess.Start();

        // Capturar la salida y los errores de FFmpeg
        string output = ffmpegProcess.StandardOutput.ReadToEnd();
        string error = ffmpegProcess.StandardError.ReadToEnd();

        ffmpegProcess.WaitForExit();

        // Mostrar la salida de FFmpeg
        Console.WriteLine("FFmpeg Output: " + output);
        Console.WriteLine("FFmpeg Errors: " + error);

        if (ffmpegProcess.ExitCode == 0)
        {
            Console.WriteLine("Combinación completada exitosamente.");
        }
        else
        {
            Console.WriteLine("Error durante la combinación.");
        }
    }
}