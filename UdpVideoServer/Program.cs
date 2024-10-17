// Ruta del archivo de video que deseas transmitir
var videoFilePath = "/Users/macbook/Projects/Streaming/UdpVideoServer/static/dragonball.mp4";  // Cambia esta ruta por la ubicación de tu video
var ipAddress = "127.0.0.1";  // Cambia la dirección IP por la del cliente
var videoPort = 8080;
var audioPort = 8081;

// Iniciar la transmisión de video
var videoTask = VideoServer.SendVideo(videoFilePath, ipAddress, videoPort);
// var audioTask = VideoServer.SendAudio(videoFilePath, ipAddress, audioPort);

// Esperar
// await Task.WhenAll(videoTask, audioTask);
await Task.WhenAll(videoTask);
// await Task.WhenAll(audioTask);

Console.WriteLine("Transmisión de video y audio finalizada.");