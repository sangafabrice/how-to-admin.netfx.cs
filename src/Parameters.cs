/// <summary>The parsed parameters.</summary>
/// <version>0.0.1.0</version>

using System;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace cvmd2html
{
  static class Parameters
  {
    /// <summary>The parameter object.</summary>
    static readonly dynamic _param = ParseCommandLine(Environment.GetCommandLineArgs().Skip(1).ToArray());

    /// <summary>The input markdown path.</summary>
    internal static readonly string Markdown = _param.Markdown;

    /// <summary>Specify to configure the shortcut in the registry.</summary>
    internal static readonly bool Set = _param.Set;

    /// <summary>Specify to remove the shortcut menu.</summary>
    internal static readonly bool Unset = _param.Unset;

    /// <summary>Specify to configure the shortcut without the icon.</summary>
    internal static readonly bool? NoIcon = _param.NoIcon;

    static dynamic ParseCommandLine(string[] args)
    {
      dynamic paramExpando = new ExpandoObject();
      paramExpando.Markdown = null;
      paramExpando.Set = false;
      paramExpando.NoIcon = null;
      paramExpando.Unset = false;
      if (args.Length == 1)
      {
        string arg = args[0].Trim();
        string[] paramNameValue = arg.Split(new char[]{':'}, 2);
        string paramMarkdown;
        if (paramNameValue.Length == 2 && paramNameValue[0].Equals("/Markdown", StringComparison.OrdinalIgnoreCase) && (paramMarkdown = paramNameValue[1].Trim()).Length > 0)
        {
          paramExpando.Markdown = paramMarkdown;
          return paramExpando;
        }
        switch (arg.ToLower())
        {
          case "/set":
            paramExpando.Set = true;
            paramExpando.NoIcon = false;
            return paramExpando;
          case "/set:noicon":
            paramExpando.Set = true;
            paramExpando.NoIcon = true;
            return paramExpando;
          case "/unset":
            paramExpando.Unset = true;
            return paramExpando;
          default:
            paramExpando.Markdown = arg;
            return paramExpando;
        }
      }
      else if (args.Length == 0)
      {
        paramExpando.Set = true;
        paramExpando.NoIcon = false;
        return paramExpando;
      }
      ShowHelp();
      return null;
    }

    static void ShowHelp()
    {
      Util.Popup(
        new StringBuilder()
        .AppendLine("The MarkdownToHtml shortcut launcher.")
        .AppendLine("It starts the shortcut menu target script in a hidden window.")
        .AppendLine()
        .AppendLine("Syntax:")
        .AppendLine("  Convert-MarkdownToHtml [/Markdown:]<markdown file path>")
        .AppendLine("  Convert-MarkdownToHtml [/Set[:NoIcon]]")
        .AppendLine("  Convert-MarkdownToHtml /Unset")
        .AppendLine("  Convert-MarkdownToHtml /Help")
        .AppendLine()
        .AppendLine("<markdown file path>  The selected markdown's file path.")
        .AppendLine("                 Set  Configure the shortcut menu in the registry.")
        .AppendLine("              NoIcon  Specifies that the icon is not configured.")
        .AppendLine("               Unset  Removes the shortcut menu.")
        .AppendLine("                Help  Show the help doc.")
        .ToString()
      );
      Program.Quit(1);
    }
  }
}