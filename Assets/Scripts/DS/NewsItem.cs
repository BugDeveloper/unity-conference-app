using System;
using UnityEngine;

[Serializable]
public class NewsItem
{
	public NewsItem(string id, long date, string info, string image)
	{
		Date = date;
		Info = info;
		Image = image;
	    Id = id;
	}

	public static NewsItem CreateFromJSON(string jsonString)
	{
		return JsonUtility.FromJson<NewsItem>(jsonString);
	}

    public long Date { get; private set; }
    public string Id { get; private set; }
    public string Info { get; private set; }
    public string Image { get; private set; }

}
