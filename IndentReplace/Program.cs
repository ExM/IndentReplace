using NDesk.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace IndentReplace
{
	class Program
	{
		static int Main(string[] args)
		{
			bool showHelp = false;
			string indent = "";
			int tabSize = 0;

			var p = new OptionSet()
			{
				{ "s=|tabSize=", "tab size", v => tabSize = int.Parse(v) },
				{ "i=|indent=", "required character indentation (tab or space)", v => indent = v },
				{ "h|?|help", "show this message and exit", v => showHelp = v != null },
			};

			try
			{
				if (args.Length == 0)
					throw new ArgumentException("required parameters in command line");

				p.Parse(args);
				if (showHelp)
				{
					ShowHelp(p);
					return 0;
				}

				if (tabSize <= 0)
					throw new ArgumentException("indent size should be 1 or more characters");

				Action<Stream, Stream> action;

				if (indent == "tab")
				{
					action = (src, dst) => (new SpaceToTabConverter(src, dst, tabSize)).Convert();
				}
				else if(indent == "space")
				{
					action = (src, dst) => (new TabToSpaceConverter(src, dst, tabSize)).Convert();
				}
				else if(indent == "echo")
				{
					action = (src, dst) => src.CopyTo(dst);
				}
				else
					throw new ArgumentException("character indentation should be 'tab' or 'space'");

				StandardIO(action);
				return 0;
			}
			catch (Exception e)
			{
				Console.Error.WriteLine("IndentReplace: {0}", BuildMessage(e));
				Console.Error.WriteLine("Try `IndentReplace --help' for more information.");
				return 1;
			}
		}
		
		static void StandardIO(Action<Stream, Stream> action)
		{
			using (var stdin = Console.OpenStandardInput())
			using (var bufin = new BufferedStream(stdin, 1024 * 512))
			using (var stdout = Console.OpenStandardOutput())
			using (var bufout = new BufferedStream(stdout, 1024 * 512))
			{
				action(bufin, bufout);
				bufout.Flush();
			}
		}

		static string BuildMessage(Exception ex)
		{
			var sb = new StringBuilder(ex.Message);
			ex = ex.InnerException;

			while (ex != null)
			{
				sb.AppendFormat(" Reason: {0}", ex.Message);
				ex = ex.InnerException;
			}
			return sb.ToString();
		}

		static void ShowHelp(OptionSet p)
		{
			Console.WriteLine("Usage: IndentReplace [OPTIONS]");
			Console.WriteLine("Options:");
			p.WriteOptionDescriptions(Console.Out);
		}
	}
}
