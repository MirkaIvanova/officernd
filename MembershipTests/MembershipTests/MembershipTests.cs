using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RestSharp;

namespace MembershipTests
{
    [TestFixture]
    public class MembershipTests
    {
        private static readonly string BaseUrl = "https://app.officernd.com/api/v1/organizations/miroslava-qa-assignment-demo/";
        private static readonly RestClient Client = new RestClient(BaseUrl);
        private static readonly string ClientId = "YJE6VCYCyjYNtBYm";
        private static readonly string ClientSecret = "nQXcdXovfzwWhx4FjHYbweyg8aHlPb52";

        [TestCase("Private office", "Miroslava QA Assignment Demo", "London")]
        public void CreateMembership(string planName, string teamName, string officeName)
        {
            var request = new RestRequest("memberships", Method.POST);
            request.AddHeader("Authorization", "Bearer " + GetAccessToken());

            var membership = new
            {
                name = "Membership 1",
                plan = GetPlanId(planName),
                team = GetTeamId(teamName),
                office = GetOfficeId(officeName),
                startDate = "2020-02-21T00:00:00.000Z"
            };

            request.AddJsonBody(membership);

            var response = Client.Execute(request);
            var newMemberships = JArray.Parse(response.Content);

            Assert.AreEqual(1, newMemberships.Count);
            Assert.AreEqual(membership.name, newMemberships[0]["name"].ToString());
            Assert.AreEqual(membership.plan, newMemberships[0]["plan"].ToString());
            Assert.AreEqual(membership.team, newMemberships[0]["team"].ToString());
            Assert.AreEqual(membership.office, newMemberships[0]["office"].ToString());
        }

        private string GetPlanId (string planName)
        {
            var request = new RestRequest("plans", Method.GET);
            request.AddHeader("Authorization", "Bearer " + GetAccessToken());

            var response = Client.Execute(request);
            var plans = JArray.Parse(response.Content);
            var planId = plans.SelectToken($"$.[?(@.name=='{planName}')]")["_id"];

            return planId.ToString();
        }

        private string GetTeamId(string teamName)
        {
            var request = new RestRequest("teams", Method.GET);
            request.AddHeader("Authorization", "Bearer " + GetAccessToken());

            var response = Client.Execute(request);
            var teams = JArray.Parse(response.Content);
            var teamId = teams.SelectToken($"$.[?(@.name=='{teamName}')]")["_id"];

            return teamId.ToString();
        }

        private string GetOfficeId(string officeName)
        {
            var request = new RestRequest("offices", Method.GET);
            request.AddHeader("Authorization", "Bearer " + GetAccessToken());

            var response = Client.Execute(request);
            var offices = JArray.Parse(response.Content);
            var officeId = offices.SelectToken($"$.[?(@.name=='{officeName}')]")["_id"];

            return officeId.ToString();
        }
        
        private static string GetAccessToken()
        {
            var client = new RestClient("https://identity.officernd.com/oauth/token");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("client_id", ClientId);
            request.AddParameter("client_secret", ClientSecret);
            request.AddParameter("grant_type", "client_credentials");
            request.AddParameter("scope", "officernd.api.read officernd.api.write");
            var responseStr = client.Execute(request);

            JObject responseJson = JObject.Parse(responseStr.Content);

            return responseJson["access_token"].ToString();
        }
    }
}
