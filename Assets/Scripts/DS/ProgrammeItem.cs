using System;
using UnityEngine;

[Serializable]
public class ProgrammeItem
{
	public ProgrammeItem(long start, long end, string title, string adress, string info)
	{
		Start = start;
		End = end;
		Title = title;
		Address = adress;
		Info = info;
	}

	public static ProgrammeItem CreateFromJSON(string jsonString)
	{
		return JsonUtility.FromJson<ProgrammeItem>(jsonString);
	}

    public long Start { get; set; }
    public long End { get; set; }
    public string Title { get; set; }
    public string Address { get; set; }
    public string Info { get; set; }
}
