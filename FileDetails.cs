using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Shell32;

namespace FileNameToModifyDate
{
  public static class FileDetails
  {
    public static Task<T> StartSTATask<T>(Func<T> func)
    {
      var tcs = new TaskCompletionSource<T>();
      var thread = new Thread(() =>
      {
        try
        {
          tcs.SetResult(func());
        }
        catch (Exception e)
        {
          tcs.SetException(e);
        }
      });
      thread.SetApartmentState(ApartmentState.STA);
      thread.Start();
      return tcs.Task;
    }

    public static string[] ListAllDetailsForFile(string filePath)
    {
      // Work with shell should be in method with [STAThread] attribute or in the Thread with ApartmentState.STA
      return StartSTATask(() =>
      {
        var shell = new ShellClass();
        var folder = shell.NameSpace(Path.GetDirectoryName(filePath));
        var folderItem = folder.ParseName(Path.GetFileName(filePath));

        var lines = new List<string>();
        for (var i = 1; i < 500; i++)
        {
          var detailName = folder.GetDetailsOf(null, i).Trim();
          var detailValue = folder.GetDetailsOf(folderItem, i).Trim();
          if (!string.IsNullOrWhiteSpace(detailName))
            lines.Add($"({i}) {detailName}: {detailValue}");
        }
        return lines.ToArray();
      }).Result;
    }

    public static DateTime GetMediaDateFromFileName(string filePath)
    {
      // Work with shell should be in method with [STAThread] attribute or in the Thread with ApartmentState.STA
      var value = StartSTATask(() =>
      {
        var shell = new ShellClass();
        var folder = shell.NameSpace(Path.GetDirectoryName(filePath));
        var folderItem = folder.ParseName(Path.GetFileName(filePath));

        const int mediaCreatedValue = 208;
        return folder.GetDetailsOf(folderItem, mediaCreatedValue).Trim();
      }).Result;

      value = RemoveInvalidCharacters(value);
      // If the value string is empty, return DateTime.MinValue, otherwise return the "Media Created" date
      return value == string.Empty ? DateTime.MinValue : DateTime.Parse(value);
    }

    private static string RemoveInvalidCharacters(string value)
    {
      // These are the characters that are not allowing me to parse into a DateTime
      var charactersToRemove = new[] {(char) 8206, (char) 8207};
      // Removing the suspect characters
      foreach (var c in charactersToRemove)
        value = value.Replace(c.ToString(), "").Trim();
      return value;
    }
  }
}
