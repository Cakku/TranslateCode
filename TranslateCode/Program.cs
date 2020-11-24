using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace TranslateCode
{
	class Program
	{
		static readonly HttpClient client = new HttpClient();
		static string auth = "8df239f8-2b6a-3b81-fef0-15baca8d68fb";
		static JsonSerializerOptions options = new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase};

		public static void Main(string[] args)
		{
			string texti = "buff = lang(\"この…鈴の音は…ランカータ…？なぜ…ここ…に。いや、違う…！\", \"Sorry, this is untranslated sentence.\") \nbuff = lang(\"Toinen rivi\", \"Sorry, this is untranslated sentence.\")";

			string regex = "(\".*\"),.(\"Sorry, this is untranslated sentence.\")";

			Console.WriteLine(texti);


			Console.WriteLine("hai");
			string toTranslate = Regex.Replace(texti, regex, new MatchEvaluator(TranslateRegex));

			Console.WriteLine("*Translated:*");
			Console.WriteLine(toTranslate);
			Console.WriteLine("*End Translaion*");
		}

		public static string TranslateRegex(Match m)
		{
			string text1 = m.Groups[1].ToString();
			string text2 = m.Groups[2].ToString();
			return string.Format("{0}, {1}", text1, Translate(text2).Result);
		}

		public static async Task<string> Translate(string text)
		{
			string uri = $"https://api.deepl.com/v1/translate?auth_key={auth}&source_lang=JA&target_lang=EN-US&text={text}";

			try
			{
				TranslateResponse translation;
				/*
				HttpResponseMessage response = await client.GetAsync(uri);
				if (response.IsSuccessStatusCode)
				{
					translation = await response.Content.ReadFromJsonAsync<TranslateResponse>();
				}
				*/
				string responseBody = await client.GetStringAsync(uri);
				translation = JsonSerializer.Deserialize<TranslateResponse>(responseBody, options);
				Console.WriteLine($"translated resp: " + responseBody);
				Console.WriteLine($"*text*: {translation.translations != null}"
					+ translation?.translations?.FirstOrDefault()?.text);
				return responseBody;

			}
			catch (HttpRequestException e)
			{
				Console.WriteLine("\nException Caught!");
				Console.WriteLine("Message :{0} ", e.Message);
				string translated = "\"TODO get google translate\"";
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
