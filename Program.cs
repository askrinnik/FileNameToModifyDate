using System;
using System.IO;
using System.Text.RegularExpressions;

namespace FileNameToModifyDate
{
  class Program
  {
    static void Main(string[] args)
    {
      var folderName = @"E:\Disk_G\DiskReserv\Video\1998";
      var fileMask = @"*.avi";
      var dateMask = @"(?<year>\d{2})\.(?<month>\d{2})\.(?<day>\d{2}).+";

      var filePathes = Directory.GetFiles(folderName, fileMask);
      foreach (var filePath in filePathes)
        ProcessFile(filePath, dateMask);
      Console.WriteLine("Done!");
    }

    private static void ProcessFile(string filePath, string dateMask)
    {
      var fileName = Path.GetFileName(filePath);
      var date = GetDateFromText(fileName, dateMask);
      Console.WriteLine($"Setting date {date} to {fileName}...");
      File.SetLastWriteTime(filePath, date);
    }

    private static DateTime GetDateFromText(string fileName, string dateMask)
    {
      var regex = new Regex(dateMask);
      var m = regex.Match(fileName);

      var yearS = m.Groups["year"].Value;
      var month = m.Groups["month"].Value;
      var day = m.Groups["day"].Value;
      var year = int.Parse(yearS);
      year += year > 40 ? 1900 : 2000;
      var date = new DateTime(year, int.Parse(month), int.Parse(day));
      return date;
    }
  }
}
