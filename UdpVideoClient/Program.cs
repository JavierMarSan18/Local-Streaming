var ipAddress = "127.0.0.1";  // Cambia la dirección IP por la del cliente
var videoPort = 8080;
// var audioPort = 8081;

var videoStreamUrl = $"udp://{ipAddress}:{videoPort}";
// var audioStreamUrl = $"udp://{ipAddress}:{audioPort}";

VideoClient.Play(videoStreamUrl);

// Iniciar la transmisión de video
// var videoTask = VideoClient.ReceiveVideo(ipAddress, videoPort);
// var audioTask = VideoClient.ReceiveAudio(ipAddress, audioPort);

// // Esperar
// await Task.WhenAll(videoTask, audioTask);

// VideoClient.CombineAudioAndVideo(
//     "/Users/macbook/Projects/Streaming/UdpVideoClient/static/received_video.ts", 
//     "/Users/macbook/Projects/Streaming/UdpVideoClient/static/received_audio.ts", 
//     "/Users/macbook/Projects/Streaming/UdpVideoClient/static/output.mp4");
// Console.WriteLine("Transmisión combinada y guardada como 'output.mp4'.");