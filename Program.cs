using System;
using System.IO;
using System.Text.RegularExpressions;

namespace FileNameToModifyDate
{
  class Program
  {
    static void Main()
    {
      //var details = FileDetails.ListAllDetailsForFile(@"D:\Downloads\VID_20190407_095244.mp4");
      //Console.WriteLine(string.Join("\n", details));

      //var mediaDate = FileDetails.GetMediaDateFromFileName(@"D:\Downloads\VID_20190407_095244.mp4");

      const string folderName = @"D:\Downloads";
      const string fileMask = @"*.mp4";

      var filePaths = Directory.GetFiles(folderName, fileMask);
      Console.WriteLine($"{filePaths.Length} files were detected.");

      foreach (var filePath in filePaths)
        SetFileDateFromFileName(filePath);

      Console.WriteLine("Done!");
      Console.ReadLine();
    }

    private static void SetFileDateFromFileName(string filePath)
    {
      var date = GetDateFromFileName(filePath);

      Console.WriteLine($"Setting date {date} to {filePath}...");

      File.SetLastWriteTime(filePath, date);
      //File.SetCreationTime(filePath, date);
      //File.SetLastAccessTime(filePath, date);
    }

    private static DateTime GetDateFromFileName(string filePath)
    {
      var dateMask1 = @"(?<year>\d{2})\.(?<month>\d{2})\.(?<day>\d{2}).+";
      var dateMask2 = @"(?<year>\d{2})\.(?<month>\d{2}).+";

      var fileName = Path.GetFileName(filePath) ?? throw new ArgumentOutOfRangeException(nameof(filePath), "Invalid value: " + filePath);

      var m = new Regex(dateMask1).Match(fileName);
      if (m.Success)
      {
        var year = GetYear(m.Groups["year"].Value);

        var month = m.Groups["month"].Value;
        var day = m.Groups["day"].Value;

        var date = new DateTime(year, int.Parse(month), int.Parse(day), 10, 0, 0);
        return date;
      }

      m = new Regex(dateMask2).Match(fileName);
      if (m.Success)
      {
        var year = GetYear(m.Groups["year"].Value);

        var month = m.Groups["month"].Value;
        var day = "1";

        var date = new DateTime(year, int.Parse(month), int.Parse(day), 10, 0, 0);
        return date;
      }
      throw new ArgumentOutOfRangeException(nameof(filePath), "Invalid value: " + filePath);
    }

    private static int GetYear(string yearS)
    {
      var year = int.Parse(yearS);
      if (year < 100)
        year += year > 40 ? 1900 : 2000;
      return year;
    }
  }
}
