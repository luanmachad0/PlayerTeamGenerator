// /////////////////////////////////////////////////////////////////////////////
// TESTING AREA
// THIS IS AN AREA WHERE YOU CAN TEST YOUR WORK AND WRITE YOUR TESTS
// /////////////////////////////////////////////////////////////////////////////

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WebApi.Entities;
using WebApiTest.Base;

namespace WebApiTest
{
    public class ProcessTeamTest : BaseTestWrapper
    {
        [Test]
        public async Task TestSample()
        {
            List<TeamProcessViewModel> requestData = new List<TeamProcessViewModel>()
            {
                new TeamProcessViewModel()
                {
                    Position = "defender",
                    MainSkill = "speed",
                    NumberOfPlayers = "1"
                }
            };

            var response = await client.PostAsJsonAsync("/api/team/process", requestData);
            try
            {
                var responseObject = await response.Content.ReadAsStringAsync();
                Assert.That(responseObject, Is.Not.Null);
            }
            catch
            {
                Assert.Fail("Invalid response object");
            }

        }

        [Test]
        public async Task ProcessReturnRightPositionTest()
        {
            List<TeamProcessViewModel> requestData = new List<TeamProcessViewModel>()
            {
                new TeamProcessViewModel()
                {
                    Position = "defender",
                    MainSkill = "speed",
                    NumberOfPlayers = "1"
                }
            };

            await base.CreatePlayer();
            var response = await client.PostAsJsonAsync("/api/team/process", requestData);

            try
            {
                var responseObject = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<List<Player>>(responseObject);

                foreach (var player in result)
                {
                    Assert.AreEqual(player.Position, "defender");
                }
            }
            catch
            {
                Assert.Fail("Invalid response object");
            }

        }

        [Test]
        public async Task ProcessReturnPlayerWithHighestSkillValueTest()
        {
            List<TeamProcessViewModel> requestData = new List<TeamProcessViewModel>()
            {
                new TeamProcessViewModel()
                {
                    Position = "defender",
                    MainSkill = "speed",
                    NumberOfPlayers = "1"
                }
            };

            await base.CreatePlayer();
            var response = await client.PostAsJsonAsync("/api/team/process", requestData);

            try
            {
                var responseObject = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<List<Player>>(responseObject);

                foreach (var player in result)
                {
                    Assert.AreEqual(player.PlayerSkills.Select(ps => ps.Value).Max(), 80);
                }
            }
            catch
            {
                Assert.Fail("Invalid response object");
            }

        }
    }
}
