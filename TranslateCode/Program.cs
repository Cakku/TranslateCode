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
		static string auth = "8df239f8-2b6a-3b81-fef0-15baca8d68fb";
		static JsonSerializerOptions options = new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase};
		static string responseRegex = "\"text\":\"(.*)\"";

		static string log = "";

		public static void Main(string[] args)
		{
			string texti = "buff = lang(\"この…鈴の音は…ランカータ…？なぜ…ここ…に。いや、違う…！\", \"Sorry, this is untranslated sentence.\") \nbuff = lang(\"は…ランカータ…？なぜ\", \"Sorry, this is untranslated sentence.\")";

			string regex = "\"(.*)\",.\"(Sorry, this is untranslated sentence.)\"";
			string path = "C:/Users/Cakku/Documents/Elona/elonaplus2.01/elonaplus/";


			if (File.Exists(path + "start1.hsp"))
			{
				texti = File.ReadAllText(path + "start1.hsp");
				//File.WriteAllText(path, createText, Encoding.UTF8);
			}

			//Console.WriteLine(texti);

			Console.WriteLine("Start translating..");
			string toTranslate = Regex.Replace(texti, regex, new MatchEvaluator(TranslateRegex));

			File.WriteAllText(path + "translated.hsp", toTranslate, Encoding.UTF8);
			File.WriteAllText(path + "log.txt", log, Encoding.UTF8);
			Console.WriteLine("*Translated:*");
			Console.WriteLine("*End Translation*");
		}

		public static string TranslateRegex(Match m)
		{
			string text1 = m.Groups[1].ToString();
			string text2 = m.Groups[2].ToString();
			//return string.Format("\"{0}\", \"{1}\"", text1, "This is not untranslated text!");
			string value = string.Format("\"{0}\", \"{1}\"", text1, Translate(text1).Result);
			log += "\n" + value;
			return value;
		}

		public static async Task<string> Translate(string text)
		{
			string uri = $"https://api.deepl.com/v1/translate?auth_key={auth}&source_lang=JA&target_lang=EN-US&text={text}";

			try
			{
				//TranslateResponse translation;
				string responseBody = await client.GetStringAsync(uri);
				Console.WriteLine($"translation response: " + responseBody);
				//Dunno why I can't get json parsing to work, eff it.
				//translation = JsonSerializer.Deserialize<TranslateResponse>(responseBody, options);
				//Console.WriteLine($"*text*: {translation.translations != null}" + translation?.translations?.FirstOrDefault()?.text);
				//regex for resque
				string translatedStr = Regex.Match(responseBody, responseRegex).Groups[1].Value;
				Console.WriteLine("translated text: "+ translatedStr);
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
