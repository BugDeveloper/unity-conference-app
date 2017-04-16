using System;

public static class InfoStorage
{
    public static float OpenedPanelPosY;
    public static float ClosedPanelPosY;

	public static readonly string Server = "http://small.sns.gdforge.fvds.ru";
	public static readonly string Connect = "/connect";
	public static readonly string NewsApi = "/news/";
	public static readonly string NewsImages = "/upload/news/";
	public static readonly string EventsApi = "/event/";
	public static readonly string GoogleStaticMapsApi = "https://maps.googleapis.com/maps/api/staticmap";
	public static readonly string GoogleMapsApi = "https://www.google.ru/maps";
	public static readonly string ScoreApi = "/rating/rate";
	public static readonly string QuestionApi = "/question/send";
    public static readonly string GoogleAndroidApi = "AIzaSyAa7GsD8GutWrMa0V3XTVYKvRg_6uAJRrM";
    public static readonly string GoogleMapApiKey = "AIzaSyAa7GsD8GutWrMa0V3XTVYKvRg_6uAJRrM";
    public static readonly string GoogleIosApi = "AIzaSyC0gQESjHnp5LzW-aStZmU6HRl2fgEZiXE";
    public static readonly float MaxDefaultTextSize = 160f;

    private static DateTime[] _validDates;

    public static string ProcessTags(string text)
    {
        text = text.Replace("<p>", "");
        text = text.Replace("</p>", "");
        text = text.Replace("<strong>", "<b>");
        text = text.Replace("</strong>", "</b>");
        text = text.Replace("<em>", "<i>");
        text = text.Replace("</em>", "</i>");
        text = text.Replace("&nbsp;", " ");

        return text;
    }

    public static bool IsDateValid(long time)
    {
        var dt = UnixTimeStampToDateTime(time);
        return dt.Month == 4 && dt.Year == 2017 && (dt.Day == 12 || dt.Day == 13 || dt.Day == 14 || dt.Day == 15);
    }

    public static bool IsDateValid(DateTime dt)
    {
        return dt.Month == 4 && dt.Year == 2017 && dt.Day > 11 && dt.Day < 16;
    }

	public static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
	{
		var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified);
		dt = dt.AddSeconds(unixTimeStamp);
		return dt;
	}

    public static int DateTimeToUnixTimeStamp(DateTime dt)
    {
        return (Int32)dt.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
    }

/*    public static string[] ShitToJson(string shit)
    {
        string[] json = shit.Split(new string[] { "}," }, StringSplitOptions.None);

        for (int i = 0; i < json.Length - 1; i++)
        {
            json[i] += "}";
        }

        json[0] = json[0].Substring(1);
        json[json.Length - 1] = json[json.Length - 1].Substring(0, json[json.Length - 1].Length - 1);

        return json;
    }*/

/*	public static void InitiateTestData()
	{
		var randomText = "";

		for (var i = 0; i < 50; i++)
		{
			randomText += "ЕНТА ТЕКСТ ";
		}

		var pdt = DateTime.Today;

		for (var i = 0; i < 50; i++)
		{
			Programme.Add(new ProgrammeItem(pdt, pdt.AddHours(1), "ОТКРЫТИЕ КОНФЕРЕНЦИИ", "ул Пушкина", randomText));
			pdt = pdt.AddHours(1);
		}

		var tmpNewsImages = new List<Sprite>
		{
			Resources.Load<Sprite>("Sprites/Temp/feed_photo_01"),
			Resources.Load<Sprite>("Sprites/Temp/feed_photo_02"),
			Resources.Load<Sprite>("Sprites/Temp/feed_photo_03")
		};

		var ndt = DateTime.Today;

		for (var i = 0; i < 10; i++)
		{
			News.Add(new NewsItem(ndt, "ФЛДОАЫФАЛОЫАФАЛО", randomText, tmpNewsImages));

			if (i % 4 == 0) ndt = ndt.AddDays(1);
		}

		Programme.Sort((x, y) => x.StartTime.CompareTo(y.StartTime));
		News.Sort((x, y) => x.Date.CompareTo(y.Date));
	}*/

}
