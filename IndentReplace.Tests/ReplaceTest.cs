using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndentReplace
{
	[TestFixture]
	public class ReplaceTest
	{
		[TestCase("    AB CD  EFG", 2, "		AB CD  EFG")]
		[TestCase("  ", 2, "	")]
		[TestCase("  AB CD\n  EFG", 2, "	AB CD\n	EFG")]
		[TestCase("	EFG\n", 2, "	EFG\n")]

		[TestCase(
@"        public object Deserialize(
           BsonReader bsonReader,
           Type nominalType,
           Type actualType,
           IBsonSerializationOptions options)
        {", 4,
@"		public object Deserialize(
		   BsonReader bsonReader,
		   Type nominalType,
		   Type actualType,
		   IBsonSerializationOptions options)
		{")]
		public void SpaceToTab(string srcText, int tabSize, string expText)
		{
			var src = GenerateStreamFromString(srcText);
			var dst = new MemoryStream();

			(new SpaceToTabConverter(src, dst, tabSize)).Convert();

			Assert.That(Encoding.UTF8.GetString(dst.ToArray()), Is.EqualTo(expText));
		}

		[TestCase("		AB CD  EF	G", 2, "    AB CD  EF	G")]
		[TestCase("	", 2, "  ")]
		[TestCase("	AB CD\n	EFG", 2, "  AB CD\n  EFG")]
		[TestCase("	EFG\n", 2, "  EFG\n")]
		[TestCase(
@"		public object Deserialize(
		   BsonReader bsonReader,
		   Type nominalType,
		   Type actualType,
		   IBsonSerializationOptions options)
		{", 4,
@"        public object Deserialize(
           BsonReader bsonReader,
           Type nominalType,
           Type actualType,
           IBsonSerializationOptions options)
        {")]
		public void TabToSpace(string srcText, int tabSize, string expText)
		{
			var src = GenerateStreamFromString(srcText);
			var dst = new MemoryStream();

			(new TabToSpaceConverter(src, dst, tabSize)).Convert();

			Assert.That(Encoding.UTF8.GetString(dst.ToArray()), Is.EqualTo(expText));
		}


		public Stream GenerateStreamFromString(string s)
		{
			MemoryStream stream = new MemoryStream();
			StreamWriter writer = new StreamWriter(stream);
			writer.Write(s);
			writer.Flush();
			stream.Position = 0;
			return stream;
		}
	}
}
