namespace MetuTrade.Business.Results;

using System.Net;
using System.Net.Http;

public class ResultBase
{
    public HttpStatusCode StatusCode { get; set; }
}