namespace MetuTrade.Core;

using System.Net.Http.Headers;
using Newtonsoft.Json;

public static class Tools
{
    public static string CreateQuery(params (string, object)[] keyValuePairs)
    {
        string query = "";
        for (int i = 0; i < keyValuePairs.Length; i++)
        {
            string key;
            object value;
            (key, value) = keyValuePairs[i];
            query += key;
            query += "=";
            query += value;

            if (i != keyValuePairs.Length - 1) query += "&";
        }
        return query;
    }

    public static long GetTimestamp(string date)
    {
        string[] split = date.Split('-');

        if (split.Length != 3)
        {
            throw new ArgumentException("Argument is invalid");
        }

        try
        {
            int year = int.Parse(split[0]);
            int month = int.Parse(split[1]);
            int day = int.Parse(split[2]);

            DateTimeOffset offset = new DateTimeOffset(year, month, day, 0, 0, 0, TimeSpan.Zero);

            return offset.ToUnixTimeMilliseconds();
        }
        catch
        {
            throw new ArgumentException("Argument is invalid");
        }
    }

    public static ByteArrayContent GenerateByteContent<T>(T model)
    {
        string content = JsonConvert.SerializeObject(model);
        var buffer = System.Text.Encoding.UTF8.GetBytes(content);
        var byteContent = new ByteArrayContent(buffer);
        byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        return byteContent;
    }
}