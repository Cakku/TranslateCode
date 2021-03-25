using System.Text.RegularExpressions;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TranslateCode
{
	class Program
	{
		static readonly HttpClient client = new HttpClient();
		static string auth = "f9d692bb-8887-9d07-6f3f-60be515fd81c";
		static JsonSerializerOptions options = new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase};
		static string responseRegex = "\"text\":\"(.*)\"";

		static string script = "";
		static string log = "";

		public static void Main(string[] args)
		{
			string regex = "lang\\((.*),.\"(Sorry, this is untranslated sentence.)\"";
			string path = "C:/Users/Cakku/Documents/Elona/elonaplus2.03/elonaplus/";

			string texti = "buff = lang(\"この…鈴の音は…ランカータ…？なぜ…ここ…に。いや、違う…！\", \"Sorry, this is untranslated sentence.\") \nbuff = lang(\"は…ランカータ…？なぜ\", \"Sorry, this is untranslated sentence.\")";
			string filename = "start1.hsp";

			if (File.Exists(path + filename))
			{
				texti = File.ReadAllText(path + filename);
			}

			Console.WriteLine("Start translating..");
			string toTranslate = Regex.Replace(texti, regex, new MatchEvaluator(TranslateRegex));

			File.WriteAllText(path + "translated_" + filename, toTranslate, Encoding.UTF8);
			File.WriteAllText(path + "script.txt", script, Encoding.UTF8);
			File.WriteAllText(path + "log.txt", log, Encoding.UTF8);
			Console.WriteLine("*Translated:* \n");
			Console.WriteLine(log);
		}

		public static string TranslateRegex(Match m)
		{
			string text1 = m.Groups[1].ToString();
			//return string.Format("\"{0}\", \"{1}\"", text1, "This is not untranslated text!");
			string value = Regex.Unescape(string.Format("lang({0}, {1}", text1, Translate(text1).Result));
			script += "\n" + value;
			Console.WriteLine("translated text: " + value);
			return value;
		}

		public static async Task<string> Translate(string text)
		{
			string uri = $"https://api.deepl.com/v1/translate?auth_key={auth}&source_lang=JA&target_lang=EN-US&text={text}";

			try
			{
				//TranslateResponse translation;
				string responseBody = await client.GetStringAsync(uri);
				//Console.WriteLine($"translation response: " + responseBody);
				//Dunno why I can't get json parsing to work, eff it.
				//translation = JsonSerializer.Deserialize<TranslateResponse>(responseBody, options);
				//Console.WriteLine($"*text*: {translation.translations != null}" + translation?.translations?.FirstOrDefault()?.text);
				//regex for resque
				string translatedStr = Regex.Match(responseBody, responseRegex).Groups[1].Value;
				if (translatedStr.Last() != '"')
				{
					translatedStr += "\"";
					Console.WriteLine("added missing quotes");
					log += "added missing quotes: " + translatedStr + "\n";
				}
				//Console.WriteLine("translated text: "+ translatedStr);
				return translatedStr;

			}
			catch (HttpRequestException e)
			{
				Console.WriteLine("\nException Caught!");
				Console.WriteLine("Message :{0} ", e.Message);
				string translated = "TODO get google translate";
				return translated;
			}

		}


		public class TranslateResponse
		{ 
			public List<Translation> translations;
		}
		public class Translation
		{
			public string detected_source_language;
			public string text;
		}

	}
}
